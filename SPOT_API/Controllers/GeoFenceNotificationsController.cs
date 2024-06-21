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
    public class GeoFenceNotificationsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public GeoFenceNotificationsController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/GeoFenceNotifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoFenceNotification>>> GetGeoFenceNotifications()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.GeoFenceNotifications
                .Where(c => c.Profile.TenantId == user.TenantId)
                .Include(c => c.Profile)
                .Include(c=> c.Profile.Department)
                .Include(c=> c.GeoFenceRuleByArea)
                .Include(c=> c.GeoFenceRuleByMap)
                .OrderByDescending(c=> c.DateTime)
                .ToListAsync();
            return objs;
        }

        // GET: api/GeoFenceNotifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoFenceNotification>> GetGeoFenceNotification(Guid id)
        {
            var geoFenceNotification = await _context.GeoFenceNotifications.FindAsync(id);

            if (geoFenceNotification == null)
            {
                return NotFound();
            }

            return geoFenceNotification;
        }

        // PUT: api/GeoFenceNotifications/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGeoFenceNotification(Guid id, GeoFenceNotification geoFenceNotification)
        {
            if (id != geoFenceNotification.Id)
            {
                return BadRequest();
            }

            _context.Entry(geoFenceNotification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeoFenceNotificationExists(id))
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

        // POST: api/GeoFenceNotifications
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GeoFenceNotification>> PostGeoFenceNotification(GeoFenceNotification geoFenceNotification)
        {
            _context.GeoFenceNotifications.Add(geoFenceNotification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGeoFenceNotification", new { id = geoFenceNotification.Id }, geoFenceNotification);
        }

        // DELETE: api/GeoFenceNotifications/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeoFenceNotification(Guid id)
        {
            var geoFenceNotification = await _context.GeoFenceNotifications.FindAsync(id);
            if (geoFenceNotification == null)
            {
                return NotFound();
            }

            _context.GeoFenceNotifications.Remove(geoFenceNotification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GeoFenceNotificationExists(Guid id)
        {
            return _context.GeoFenceNotifications.Any(e => e.Id == id);
        }
    }
}
