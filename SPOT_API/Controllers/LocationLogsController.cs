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
    public class LocationLogsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public LocationLogsController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/LocationLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationLog>>> GetLocationLogs()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var locationLogs = await _context.LocationLogs
                .Where(c => c.Device.TenantId == user.TenantId)
                .Include(c => c.Device)
                .ToListAsync();

            foreach (var l in locationLogs)
                if (l.Device.LocationLog != null)
                    l.Device.LocationLog = null;

            return locationLogs;
            //return await _context.LocationLogs.ToListAsync();
        }

        // GET: api/LocationLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationLog>> GetLocationLog(Guid id)
        {
            var locationLog = await _context.LocationLogs.FindAsync(id);

            if (locationLog == null)
            {
                return NotFound();
            }

            return locationLog;
        }

        // PUT: api/LocationLogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocationLog(Guid id, LocationLog locationLog)
        {
            if (id != locationLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(locationLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationLogExists(id))
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

        // POST: api/LocationLogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LocationLog>> PostLocationLog(LocationLog locationLog)
        {
            _context.LocationLogs.Add(locationLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocationLog", new { id = locationLog.Id }, locationLog);
        }

        // DELETE: api/LocationLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationLog(Guid id)
        {
            var locationLog = await _context.LocationLogs.FindAsync(id);
            if (locationLog == null)
            {
                return NotFound();
            }

            _context.LocationLogs.Remove(locationLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationLogExists(Guid id)
        {
            return _context.LocationLogs.Any(e => e.Id == id);
        }
    }
}
