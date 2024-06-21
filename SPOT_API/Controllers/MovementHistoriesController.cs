using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovementHistoriesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public MovementHistoriesController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/MovementHistories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovementHistory>>> GetMovementHistories()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var movements = await _context.MovementHistories
                    //.Select(x => new MovementHistory
                    //{
                    //    DateTime = x.DateTime
                    //})
                    .Where(c => c.Profile.TenantId == user.TenantId)
                    .Include(o => o.Profile)
                    //.Include(o => o.Profile.Department)
                    .Include(o => o.Area)
                    .Include(o => o.Area.Level)
                    .Include(o => o.Area.Level.Location)
                    .Include(o => o.Device)
                    .Include(o => o.Device.Tenant)
                    .OrderByDescending(c => c.DateTime)
                    .Take(20000)
                    .ToListAsync();
                foreach (var movement in movements)
                {
                    try
                    {

                        if (movement.LocationType.ToLower() == "gps")
                        {
                            var gps = movement.Location.Split(',');
                            movement.Latitude = double.Parse(gps[0].Trim());
                            movement.Longitude = double.Parse(gps[1].Trim());
                        }
                        else
                        {
                            if (movement.Area != null)
                            {
                                movement.Latitude = movement.Area.Level.Location.Latitude;
                                movement.Longitude = movement.Area.Level.Location.Longitude;
                            }
                        }

                        if (movement.Profile != null && movement.Profile.Device != null)
                            movement.Profile.Device = null;

                        if (movement.Profile != null && movement.Profile.MovementHistoryList.Count > 0)
                            movement.Profile.MovementHistoryList.Clear();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                var movementCleans = movements.Where(c => c.Latitude != 0 && c.Longitude != 0).ToList();
                return movementCleans;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.InnerException);

            }

            return await _context.MovementHistories.ToListAsync();
        }

        // GET: api/MovementHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovementHistory>> GetMovementHistory(Guid id)
        {
            var movementHistory = await _context.MovementHistories.FindAsync(id);

            if (movementHistory == null)
            {
                return NotFound();
            }

            return movementHistory;
        }

        [HttpGet("ByProfileByDate/{profileId}/{date}")]
        public async Task<ActionResult<IEnumerable<MovementHistory>>> GetMovementHistoryByProfile(Guid profileId, string date)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();
            DateTime dtStart = DateTime.ParseExact(date + "000000", "yyyyMMddHHmmss", null);
            DateTime dtEnd = DateTime.ParseExact(date + "235959", "yyyyMMddHHmmss", null);
            try
            {
                var movements = await _context.MovementHistories.Where(c => c.Profile.Id == profileId)
                    //.Include(o => o.Profile)
                    //.Include(o => o.Profile.Department)
                    .Include(o => o.Area)
                    .Include(o => o.Area.Level)
                    .Include(o => o.Area.Level.Location)
                    .Include(o => o.Device)
                    .Include(o => o.Device.Tenant)
                    .Where(c => c.DateTime >= dtStart && c.DateTime <= dtEnd)
                    .OrderByDescending(c => c.DateTime)
                    .Take(1000)
                    .ToListAsync();

                foreach (var movement in movements)
                {
                    try
                    {

                        if (movement.LocationType.ToLower() == "gps")
                        {
                            var gps = movement.Location.Split(',');
                            movement.Latitude = double.Parse(gps[0].Trim());
                            movement.Longitude = double.Parse(gps[1].Trim());
                        }
                        else
                        {
                            if (movement.Area != null)
                            {
                                movement.Latitude = movement.Area.Level.Location.Latitude;
                                movement.Longitude = movement.Area.Level.Location.Longitude;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                var movementCleans = movements.Where(c => c.Latitude != 0 && c.Longitude != 0).ToList();
                return movementCleans;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.InnerException);

            }

            //return await _context.Activities.ToListAsync();
        }


        [HttpGet("ByProfileByDateRange/{profileId}/{start}/{end}")]
        public async Task<ActionResult<IEnumerable<MovementHistory>>> GetMovementHistoryByProfileByDateRange(Guid profileId, string start, string end)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();
            DateTime dtStart = DateTime.ParseExact(start, "yyyyMMddHHmm", null);
            DateTime dtEnd = DateTime.ParseExact(end, "yyyyMMddHHmm", null);
            try
            {
                var movements = await _context.MovementHistories.Where(c => c.Profile.Id == profileId)
                    //.Include(o => o.Profile)
                    //.Include(o => o.Profile.Department)
                    .Include(o => o.Area)
                    .Include(o => o.Area.Level)
                    .Include(o => o.Area.Level.Location)
                    .Include(o => o.Device)
                    .Include(o => o.Device.Tenant)
                    .Where(c => c.DateTime >= dtStart && c.DateTime <= dtEnd)
                    .OrderByDescending(c => c.DateTime)
                    .Take(1000)
                    .ToListAsync();

                foreach (var movement in movements)
                {
                    try
                    {

                        if (movement.LocationType.ToLower() == "gps")
                        {
                            var gps = movement.Location.Split(',');
                            movement.Latitude = double.Parse(gps[0].Trim());
                            movement.Longitude = double.Parse(gps[1].Trim());
                        }
                        else
                        {
                            if (movement.Area != null)
                            {
                                movement.Latitude = movement.Area.Level.Location.Latitude;
                                movement.Longitude = movement.Area.Level.Location.Longitude;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                var movementCleans = movements.Where(c => c.Latitude != 0 && c.Longitude != 0).ToList();
                return movementCleans;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.InnerException);

            }

            //return await _context.Activities.ToListAsync();
        }
        [HttpGet("ByProfile/{id}")]
        public async Task<ActionResult<IEnumerable<MovementHistory>>> GetMovementHistoryByProfile(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var movements = await _context.MovementHistories.Where(c => c.Profile.Id == id)
                    //.Include(o => o.Profile)
                    //.Include(o => o.Profile.Department)
                    .Include(o => o.Area)
                    .Include(o => o.Area.Level)
                    .Include(o => o.Area.Level.Location)
                    .Include(o => o.Device)
                    .Include(o => o.Device.Tenant)
                    .OrderByDescending(c => c.DateTime)
                    .Take(1000)
                    .ToListAsync();
                foreach (var movement in movements)
                {
                    try
                    {

                        if (movement.LocationType.ToLower() == "gps")
                        {
                            var gps = movement.Location.Split(',');
                            movement.Latitude = double.Parse(gps[0].Trim());
                            movement.Longitude = double.Parse(gps[1].Trim());
                        }
                        else
                        {
                            if (movement.Area != null)
                            {
                                movement.Latitude = movement.Area.Level.Location.Latitude;
                                movement.Longitude = movement.Area.Level.Location.Longitude;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                var movementCleans = movements.Where(c => c.Latitude != 0 && c.Longitude != 0).ToList();
                return movementCleans;
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.InnerException);

            }

            //return await _context.Activities.ToListAsync();
        }

        // PUT: api/MovementHistories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovementHistory(Guid id, MovementHistory movementHistory)
        {
            if (id != movementHistory.Id)
            {
                return BadRequest();
            }

            _context.Entry(movementHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovementHistoryExists(id))
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

        // POST: api/MovementHistories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MovementHistory>> PostMovementHistory(MovementHistory movementHistory)
        {
            _context.MovementHistories.Add(movementHistory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovementHistory", new { id = movementHistory.Id }, movementHistory);
        }

        // DELETE: api/MovementHistories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovementHistory(Guid id)
        {
            var movementHistory = await _context.MovementHistories.FindAsync(id);
            if (movementHistory == null)
            {
                return NotFound();
            }

            _context.MovementHistories.Remove(movementHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovementHistoryExists(Guid id)
        {
            return _context.MovementHistories.Any(e => e.Id == id);
        }
    }
}
