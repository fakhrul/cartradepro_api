using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Persistence;
using SPOT_API.Models;
using Application.Interfaces;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergenciesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public EmergenciesController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/Emergencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Emergency>>> GetEmergencies()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var emergencies = await _context.Emergencies
                .Where(c => c.TenantId == user.TenantId)
                .ToListAsync();

            foreach(var e in emergencies)
            {
                if (e.GeoFenceByAreaId != null && e.GeoFenceByAreaId != Guid.Empty)
                    e.GeoFenceByArea = _context.GeoFenceByAreas.FirstOrDefault(c => c.Id == e.GeoFenceByAreaId);
                if (e.GeoFenceByMapId != null && e.GeoFenceByMapId != Guid.Empty)
                    e.GeoFenceByMap = _context.GeoFenceByMaps.FirstOrDefault(c => c.Id == e.GeoFenceByMapId);
            }

            return emergencies;
        }

        // GET: api/Emergencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Emergency>> GetEmergency(Guid id)
        {
            var emergency = await _context.Emergencies
                .Include(c=> c.EmergencyUserList)
                .FirstOrDefaultAsync(c=> c.Id ==  id);

            if (emergency == null)
            {
                return NotFound();
            }

            foreach (var area in emergency.EmergencyUserList)
            {
                area.Emergency = null;
                area.Profile = null;
            }

            return emergency;
        }

        // PUT: api/Emergencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmergency(Guid id, Emergency emergency)
        {
            if (id != emergency.Id)
            {
                return BadRequest();
            }

            if (emergency.Tenant != null)
                emergency.Tenant = null;

            RemoveOldProfileItem(id);

            foreach (var area in emergency.EmergencyUserList)
            {
                if (area.Profile != null)
                    area.Profile = null;
                if (area.Emergency != null)
                    area.Emergency = null;
            }

            _context.EmergencyUsers.AddRange(emergency.EmergencyUserList);
            _context.Entry(emergency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmergencyExists(id))
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

        private void RemoveOldProfileItem(Guid id)
        {
            var areaItemList = _context.EmergencyUsers.Where(c => c.EmergencyId == id);

            foreach (var inDataBase in areaItemList.ToList())
            {
                _context.EmergencyUsers.Remove(inDataBase);
            }

          
            _context.SaveChanges();
        }

        // POST: api/Emergencies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Emergency>> PostEmergency(Emergency emergency)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();


            try
            {
                if (emergency.TenantId == Guid.Empty)
                {
                    emergency.TenantId = user.TenantId.Value;
                }

                if (emergency.Tenant != null)
                    emergency.Tenant = null;

                if(emergency.Area != null)
                    emergency.Area = null;

                foreach (var emergencyUser in emergency.EmergencyUserList)
                {
                    if (emergencyUser.Emergency != null)
                        emergencyUser.Emergency = null;
                    if (emergencyUser.Profile != null)
                        emergencyUser.Profile = null;
                    if (emergencyUser.EmergencyId == Guid.Empty)
                    {
                        emergencyUser.EmergencyId = emergency.Id;
                    }

                }
                _context.EmergencyUsers.AddRange(emergency.EmergencyUserList);
                _context.Emergencies.Add(emergency);
                await _context.SaveChangesAsync();

                foreach (var area in emergency.EmergencyUserList)
                {
                    area.Emergency = null;
                }

                return CreatedAtAction("GetEmergency", new { id = emergency.Id }, emergency);
            }
            catch (Exception ex)
            {
            }

            return BadRequest();
        }

        // DELETE: api/Emergencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmergency(Guid id)
        {
            var emergency = await _context.Emergencies.FindAsync(id);
            if (emergency == null)
            {
                return NotFound();
            }

            _context.Emergencies.Remove(emergency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmergencyExists(Guid id)
        {
            return _context.Emergencies.Any(e => e.Id == id);
        }
    }
}
