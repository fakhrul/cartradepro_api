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
    public class GeoFenceRuleByAreasController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public GeoFenceRuleByAreasController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/GeoFenceRuleByAreas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoFenceRuleByArea>>> GetGeoFenceRuleByArea()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var areas = await _context.GeoFenceRuleByAreas
                .Where(c => c.TenantId == user.TenantId)
                .Include(c=> c.GeoFenceByArea)
                //.Include(c => c.GeoFenceRuleByAreaProfileList)
                .ToListAsync();
            foreach (var area in areas)
            {
                if (area.GeoFenceRuleByAreaProfileList == null)
                    area.GeoFenceRuleByAreaProfileList = new List<GeoFenceRuleByAreaProfile>();

                foreach (var item in area.GeoFenceRuleByAreaProfileList)
                {
                    item.GeoFenceRuleByArea = null;
                    item.Profile= _context.Profiles.FirstOrDefault(c => c.Id == item.ProfileId);
                }
            }
            return areas;

            //return await _context.GeoFenceRuleByAreas.ToListAsync();
        }

        // GET: api/GeoFenceRuleByAreas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoFenceRuleByArea>> GetGeoFenceRuleByArea(Guid id)
        {
            var geoFenceRuleByArea = await _context.GeoFenceRuleByAreas
                .Include(c => c.GeoFenceByArea)
                .Include(c=> c.GeoFenceRuleByAreaProfileList)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (geoFenceRuleByArea == null)
            {
                return NotFound();
            }

            if (geoFenceRuleByArea.GeoFenceRuleByAreaProfileList == null)
                geoFenceRuleByArea.GeoFenceRuleByAreaProfileList = new List<GeoFenceRuleByAreaProfile>();

            foreach (var area in geoFenceRuleByArea.GeoFenceRuleByAreaProfileList)
            {
                area.GeoFenceRuleByArea = null;
            }

            return geoFenceRuleByArea;
        }

        // PUT: api/GeoFenceRuleByAreas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGeoFenceRuleByArea(Guid id, GeoFenceRuleByArea geoFenceRuleByArea)
        {
            if (id != geoFenceRuleByArea.Id)
            {
                return BadRequest();
            }

            if (geoFenceRuleByArea.Tenant != null)
                geoFenceRuleByArea.Tenant = null;

            RemoveOldChild(id);

            foreach (var area in geoFenceRuleByArea.GeoFenceRuleByAreaProfileList)
            {
                if (area.Profile != null)
                    area.Profile = null;
            }

            _context.GeoFenceRuleByAreaProfiles.AddRange(geoFenceRuleByArea.GeoFenceRuleByAreaProfileList);


            _context.Entry(geoFenceRuleByArea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeoFenceRuleByAreaExists(id))
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

        private void RemoveOldChild(Guid id)
        {
            var areaItemList = _context.GeoFenceRuleByAreaProfiles.Where(c => c.GeoFenceRuleByAreaId == id).ToList();

            foreach (var inDataBase in areaItemList)
            {
                _context.GeoFenceRuleByAreaProfiles.Remove(inDataBase);
            }

            _context.SaveChanges();
        }

        // POST: api/GeoFenceRuleByAreas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GeoFenceRuleByArea>> PostGeoFenceRuleByArea(GeoFenceRuleByArea geoFenceRuleByArea)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                if (geoFenceRuleByArea.TenantId == Guid.Empty)
                {
                    geoFenceRuleByArea.TenantId = user.TenantId.Value;
                }

                if (geoFenceRuleByArea.Tenant != null)
                    geoFenceRuleByArea.Tenant = null;

                if (geoFenceRuleByArea.GeoFenceRuleByAreaProfileList == null)
                    geoFenceRuleByArea.GeoFenceRuleByAreaProfileList = new List<GeoFenceRuleByAreaProfile>();

                foreach (var area in geoFenceRuleByArea.GeoFenceRuleByAreaProfileList)
                {
                    if (area.Profile != null)
                        area.Profile = null;
                    if (area.GeoFenceRuleByArea != null)
                        area.GeoFenceRuleByArea = null;
                    if (area.GeoFenceRuleByAreaId == Guid.Empty)
                    {
                        area.GeoFenceRuleByAreaId = geoFenceRuleByArea.Id;
                    }

                }
                _context.GeoFenceRuleByAreaProfiles.AddRange(geoFenceRuleByArea.GeoFenceRuleByAreaProfileList);


                _context.GeoFenceRuleByAreas.Add(geoFenceRuleByArea);
                await _context.SaveChangesAsync();

                foreach (var area in geoFenceRuleByArea.GeoFenceRuleByAreaProfileList)
                {
                    area.GeoFenceRuleByArea = null;

                }
                return CreatedAtAction("GetGeoFenceRuleByArea", new { id = geoFenceRuleByArea.Id }, geoFenceRuleByArea);
            }
            catch (Exception ex)
            {
            }

            return BadRequest();
        }

        // DELETE: api/GeoFenceRuleByAreas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeoFenceRuleByArea(Guid id)
        {
            var geoFenceRuleByArea = await _context.GeoFenceRuleByAreas.FindAsync(id);
            if (geoFenceRuleByArea == null)
            {
                return NotFound();
            }

            _context.GeoFenceRuleByAreas.Remove(geoFenceRuleByArea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GeoFenceRuleByAreaExists(Guid id)
        {
            return _context.GeoFenceRuleByAreas.Any(e => e.Id == id);
        }
    }
}
