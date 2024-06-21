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
    public class GeoFenceRuleByMapsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public GeoFenceRuleByMapsController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/GeoFenceRuleByMaps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoFenceRuleByMap>>> GetGeoFenceRuleByMap()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var areas = await _context.GeoFenceRuleByMaps
              .Where(c => c.TenantId == user.TenantId)
              .Include(c => c.GeoFenceByMap)
              .ToListAsync();
            foreach (var area in areas)
            {
                if (area.GeoFenceRuleByMapProfileList == null)
                    area.GeoFenceRuleByMapProfileList = new List<GeoFenceRuleByMapProfile>();

                foreach (var item in area.GeoFenceRuleByMapProfileList)
                {
                    item.GeoFenceRuleByMap = null;
                    item.Profile = _context.Profiles.FirstOrDefault(c => c.Id == item.ProfileId);
                }
            }
            return areas;

            //return await _context.GeoFenceRuleByMaps.ToListAsync();
        }

        // GET: api/GeoFenceRuleByMaps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoFenceRuleByMap>> GetGeoFenceRuleByMap(Guid id)
        {
            var geoFenceRuleByArea = await _context.GeoFenceRuleByMaps
               .Include(c => c.GeoFenceByMap)
               .Include(c => c.GeoFenceRuleByMapProfileList)
               .FirstOrDefaultAsync(c => c.Id == id);

            if (geoFenceRuleByArea == null)
            {
                return NotFound();
            }

            if (geoFenceRuleByArea.GeoFenceRuleByMapProfileList == null)
                geoFenceRuleByArea.GeoFenceRuleByMapProfileList = new List<GeoFenceRuleByMapProfile>();

            foreach (var area in geoFenceRuleByArea.GeoFenceRuleByMapProfileList)
            {
                area.GeoFenceRuleByMap = null;
            }

            return geoFenceRuleByArea;


            //var geoFenceRuleByMap = await _context.GeoFenceRuleByMaps.FindAsync(id);

            //if (geoFenceRuleByMap == null)
            //{
            //    return NotFound();
            //}

            //return geoFenceRuleByMap;
        }

        // PUT: api/GeoFenceRuleByMaps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGeoFenceRuleByMap(Guid id, GeoFenceRuleByMap geoFenceRuleByMap)
        {
            if (id != geoFenceRuleByMap.Id)
            {
                return BadRequest();
            }

            if (geoFenceRuleByMap.Tenant != null)
                geoFenceRuleByMap.Tenant = null;

            RemoveOldChild(id);

            foreach (var area in geoFenceRuleByMap.GeoFenceRuleByMapProfileList)
            {
                if (area.Profile != null)
                    area.Profile = null;
            }

            _context.GeoFenceRuleByMapProfiles.AddRange(geoFenceRuleByMap.GeoFenceRuleByMapProfileList);


            _context.Entry(geoFenceRuleByMap).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeoFenceRuleByMapExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();


            //if (id != geoFenceRuleByMap.Id)
            //{
            //    return BadRequest();
            //}

            //_context.Entry(geoFenceRuleByMap).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!GeoFenceRuleByMapExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return NoContent();
        }

        private void RemoveOldChild(Guid id)
        {
            var areaItemList = _context.GeoFenceRuleByMapProfiles.Where(c => c.GeoFenceRuleByMapId == id).ToList();

            foreach (var inDataBase in areaItemList)
            {
                _context.GeoFenceRuleByMapProfiles.Remove(inDataBase);
            }

            _context.SaveChanges();
        }

        // POST: api/GeoFenceRuleByMaps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GeoFenceRuleByMap>> PostGeoFenceRuleByMap(GeoFenceRuleByMap geoFenceRuleByMap)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                if (geoFenceRuleByMap.TenantId == Guid.Empty)
                {
                    geoFenceRuleByMap.TenantId = user.TenantId.Value;
                }

                if (geoFenceRuleByMap.Tenant != null)
                    geoFenceRuleByMap.Tenant = null;

                if (geoFenceRuleByMap.GeoFenceRuleByMapProfileList == null)
                    geoFenceRuleByMap.GeoFenceRuleByMapProfileList = new List<GeoFenceRuleByMapProfile>();

                foreach (var area in geoFenceRuleByMap.GeoFenceRuleByMapProfileList)
                {
                    if (area.Profile != null)
                        area.Profile = null;
                    if (area.GeoFenceRuleByMap != null)
                        area.GeoFenceRuleByMap = null;
                    if (area.GeoFenceRuleByMapId == Guid.Empty)
                    {
                        area.GeoFenceRuleByMapId = geoFenceRuleByMap.Id;
                    }

                }
                _context.GeoFenceRuleByMapProfiles.AddRange(geoFenceRuleByMap.GeoFenceRuleByMapProfileList);


                _context.GeoFenceRuleByMaps.Add(geoFenceRuleByMap);
                await _context.SaveChangesAsync();

                foreach (var area in geoFenceRuleByMap.GeoFenceRuleByMapProfileList)
                {
                    area.GeoFenceRuleByMap = null;

                }
                return CreatedAtAction("GetGeoFenceRuleByMap", new { id = geoFenceRuleByMap.Id }, geoFenceRuleByMap);
            }
            catch (Exception ex)
            {
            }

            return BadRequest();


            //_context.GeoFenceRuleByMaps.Add(geoFenceRuleByMap);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetGeoFenceRuleByMap", new { id = geoFenceRuleByMap.Id }, geoFenceRuleByMap);
        }

        // DELETE: api/GeoFenceRuleByMaps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeoFenceRuleByMap(Guid id)
        {
            var geoFenceRuleByMap = await _context.GeoFenceRuleByMaps.FindAsync(id);
            if (geoFenceRuleByMap == null)
            {
                return NotFound();
            }

            _context.GeoFenceRuleByMaps.Remove(geoFenceRuleByMap);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GeoFenceRuleByMapExists(Guid id)
        {
            return _context.GeoFenceRuleByMaps.Any(e => e.Id == id);
        }
    }
}
