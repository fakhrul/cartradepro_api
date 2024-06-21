using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformancesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public PerformancesController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/Performances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Performance>>> GetPerformances()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            return await _context.Performances.Where(c => c.Profile.Department.TenantId == user.TenantId)
                .Include(o => o.Profile)
                .Include(o => o.Profile.Department)
                .Include(o => o.Schedule)
                .Include(o => o.Schedule.Area)
                .ToListAsync();

            //return await _context.Performances.ToListAsync();
        }

        // GET: api/Performances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Performance>> GetPerformance(Guid id)
        {
            var performance = await _context.Performances.FindAsync(id);

            if (performance == null)
            {
                return NotFound();
            }

            return performance;
        }

        // PUT: api/Performances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerformance(Guid id, Performance performance)
        {
            if (id != performance.Id)
            {
                return BadRequest();
            }

            _context.Entry(performance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerformanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Performances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Performance>> PostPerformance(Performance performance)
        {
            _context.Performances.Add(performance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerformance", new { id = performance.Id }, performance);
        }

        // DELETE: api/Performances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerformance(Guid id)
        {
            var performance = await _context.Performances.FindAsync(id);
            if (performance == null)
            {
                return NotFound();
            }

            _context.Performances.Remove(performance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PerformanceExists(Guid id)
        {
            return _context.Performances.Any(e => e.Id == id);
        }

        [HttpGet("EvaluatePerformance")]
        public async Task<IActionResult> EvalutePerformance()
        {
            await CalculatePerformance();

            //var performance = await _context.Performances.FindAsync(id);

            //var performance = await _context.Performances.FindAsync(id);
            //if (performance == null)
            //{
            //    return NotFound();
            //}

            //_context.Performances.Remove(performance);
            //await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task CalculatePerformance()
        {
            try
            {
                var currentDateTime = DateTime.Now;

                var schedules = await _context.Schedules
                    .Where(c => c.IsEvaluated == false)
                    .Where(c => c.EndDateTime < currentDateTime)
                    .ToListAsync();

                foreach (var schedule in schedules)
                {
                    var activities = await _context.Activities
                        .Where(c => c.AreaId == schedule.AreaId)
                        .Where(c => c.ProfileId == schedule.ProfileId)
                        .Where(c => c.StartDateTime <= schedule.EndDateTime)
                        .Where(c => c.EndDateTime >= schedule.StartDateTime)
                        .ToListAsync();


                    var timeSpanOverTimeBegin = TimeSpan.Zero;
                    var timeSpanOverTimeEnd = TimeSpan.Zero;

                    var timeSpanUnderTimeBegin = TimeSpan.Zero;
                    var timeSpanUnderTimeEnd = TimeSpan.Zero;

                    var timeSpanNormal = TimeSpan.Zero;
                    var timeSpanUnderTimeMiddle = TimeSpan.Zero;

                    foreach (var activity in activities)
                    {
                        /*
                         S  |<----->|
                         A  |<----->|

                         S  |<----->|
                         A   |<--->|
                         */
                        if (activity.StartDateTime >= schedule.StartDateTime && activity.EndDateTime <= schedule.EndDateTime)
                        {
                            timeSpanUnderTimeBegin += activity.StartDateTime.Subtract(schedule.StartDateTime);
                            timeSpanNormal += activity.EndDateTime.Subtract(activity.StartDateTime);
                            timeSpanUnderTimeEnd += schedule.EndDateTime.Subtract(activity.EndDateTime);
                        }

                        /*
                         S    |<----->|
                         A  |<--------->|
                         */
                        if (activity.StartDateTime < schedule.StartDateTime && activity.EndDateTime > schedule.EndDateTime)
                        {
                            timeSpanOverTimeBegin += schedule.StartDateTime.Subtract(activity.StartDateTime);
                            timeSpanNormal += schedule.EndDateTime.Subtract(schedule.StartDateTime);
                            timeSpanOverTimeEnd += activity.EndDateTime.Subtract(schedule.EndDateTime);
                        }


                        /*
                         S    |<----->|
                         A  |<----->|
                         */
                        if (activity.StartDateTime < schedule.StartDateTime && activity.EndDateTime <= schedule.EndDateTime)
                        {
                            timeSpanOverTimeBegin += schedule.StartDateTime.Subtract(activity.StartDateTime);
                            timeSpanNormal += activity.EndDateTime.Subtract(schedule.StartDateTime);
                            timeSpanUnderTimeEnd += schedule.EndDateTime.Subtract(activity.EndDateTime);
                        }

                        /*
                         S  |<----->|
                         A    |<----->|
                         */
                        if (activity.StartDateTime >= schedule.StartDateTime && activity.EndDateTime >= schedule.EndDateTime)
                        {
                            timeSpanUnderTimeBegin += activity.StartDateTime.Subtract(schedule.StartDateTime);
                            timeSpanNormal += schedule.EndDateTime.Subtract(activity.StartDateTime);
                            timeSpanOverTimeEnd += activity.EndDateTime.Subtract(schedule.EndDateTime);
                        }

                        activity.IsEvaluated = true;
                    }

                    var scheduledTimeSpan = schedule.EndDateTime.Subtract(schedule.StartDateTime);
                    timeSpanUnderTimeMiddle = scheduledTimeSpan - timeSpanNormal - timeSpanUnderTimeBegin - timeSpanUnderTimeEnd;

                    schedule.IsEvaluated = true;

                    var perf = new Performance
                    {
                        ProfileId = schedule.ProfileId,
                        Date = schedule.StartDateTime.Date,
                        ScheduleId = schedule.Id,
                        ScheduledTimeInMinutes = scheduledTimeSpan.TotalMinutes,
                        OverTimeBeginInMinutes = timeSpanOverTimeBegin.TotalMinutes,
                        OverTimeEndInMinutes = timeSpanOverTimeEnd.TotalMinutes,
                        UnderTimeBeginInMunites = timeSpanUnderTimeBegin.TotalMinutes,
                        UnderTimeEndInMinutes = timeSpanUnderTimeEnd.TotalMinutes,
                        NormalTimeInMinutes = timeSpanNormal.TotalMinutes,
                        UnderTimeMiddleInMinutes = timeSpanUnderTimeMiddle.TotalMinutes,
                    };
                    _context.Performances.Add(perf);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
           
        }
    }
}
