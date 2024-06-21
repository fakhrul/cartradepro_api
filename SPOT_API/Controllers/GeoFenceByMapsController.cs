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
    public class GeoFenceByMapsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public GeoFenceByMapsController(SpotDBContext context, IUserAccessor userAccessor)
        {

            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/GeoFenceByMaps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoFenceByMap>>> GetGeoFences()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();
            return await _context.GeoFenceByMaps.Where(c => c.TenantId == user.TenantId)
                .ToListAsync();

            //return await _context.GeoFenceByMaps.ToListAsync();
        }

        // GET: api/GeoFenceByMaps/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoFenceByMap>> GetGeoFenceByMap(Guid id)
        {
            var geoFenceByMap = await _context.GeoFenceByMaps.Include(c => c.GeoFenceCoordItemList).FirstOrDefaultAsync(c => c.Id == id);

            if (geoFenceByMap == null)
            {
                return NotFound();
            }

            foreach (var area in geoFenceByMap.GeoFenceCoordItemList)
            {
                area.GeoFenceByMap = null;
            }

            return geoFenceByMap;
        }

        // PUT: api/GeoFenceByMaps/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGeoFenceByMap(Guid id, GeoFenceByMap geoFenceByMap)
        {
            if (id != geoFenceByMap.Id)
            {
                return BadRequest();
            }

            if (geoFenceByMap.Tenant != null)
                geoFenceByMap.Tenant = null;

            RemoveOldItem(id);

            //foreach (var area in geoFenceByMap.GeoFenceCoordItemList)
            //{
            //    if (area. != null)
            //        area.Area = null;
            //}

            _context.GeoFenceCoordItems.AddRange(geoFenceByMap.GeoFenceCoordItemList);

            _context.Entry(geoFenceByMap).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeoFenceByMapExists(id))
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

        private void RemoveOldItem(Guid id)
        {
            var itemList = _context.GeoFenceCoordItems.Where(c => c.GeoFenceByMapId == id);

            foreach (var inDataBase in itemList.ToList())
            {
                _context.GeoFenceCoordItems.Remove(inDataBase);
            }
            _context.SaveChanges();
        }


        // POST: api/GeoFenceByMaps
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GeoFenceByMap>> PostGeoFenceByMap(GeoFenceByMap geoFenceByMap)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            if (geoFenceByMap.TenantId == Guid.Empty)
            {
                geoFenceByMap.TenantId = user.TenantId.Value;
            }

            if (geoFenceByMap.Tenant != null)
                geoFenceByMap.Tenant = null;

            foreach (var area in geoFenceByMap.GeoFenceCoordItemList)
            {
                if (area.GeoFenceByMap != null)
                    area.GeoFenceByMap = null;
                if (area.GeoFenceByMapId == Guid.Empty)
                {
                    area.GeoFenceByMapId = geoFenceByMap.Id;
                }
            }

            _context.GeoFenceCoordItems.AddRange(geoFenceByMap.GeoFenceCoordItemList);
            _context.GeoFenceByMaps.Add(geoFenceByMap);
            await _context.SaveChangesAsync();

            foreach (var area in geoFenceByMap.GeoFenceCoordItemList)
            {
                area.GeoFenceByMap = null;
            }

            //
            //_context.GeoFenceByMaps.Add(geoFenceByMap);
            //await _context.SaveChangesAsync();

            return CreatedAtAction("GetGeoFenceByMap", new { id = geoFenceByMap.Id }, geoFenceByMap);
        }

        // DELETE: api/GeoFenceByMaps/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeoFenceByMap(Guid id)
        {
            var geoFenceByMap = await _context.GeoFenceByMaps.FindAsync(id);
            if (geoFenceByMap == null)
            {
                return NotFound();
            }

            _context.GeoFenceByMaps.Remove(geoFenceByMap);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GeoFenceByMapExists(Guid id)
        {
            return _context.GeoFenceByMaps.Any(e => e.Id == id);
        }
    }
}
