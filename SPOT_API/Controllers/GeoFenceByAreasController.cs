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
    public class GeoFenceByAreasController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public GeoFenceByAreasController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/GeoFenceByAreas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoFenceByArea>>> GetGeoFenceByArea()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var areas = await _context.GeoFenceByAreas
                .Where(c => c.TenantId == user.TenantId)
                .Include(c => c.GeoFenceAreaItemList)
                .ToListAsync();
            foreach(var area in areas)
            {
                foreach(var item in area.GeoFenceAreaItemList)
                {
                    item.GeoFenceByArea = null;
                    item.Area = _context.Areas.FirstOrDefault(c => c.Id == item.AreaId);
                }
            }
            return areas;
        }

        // GET: api/GeoFenceByAreas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoFenceByArea>> GetGeoFenceByArea(Guid id)
        {
            var geoFenceByArea = await _context.GeoFenceByAreas
                .Include(c => c.GeoFenceAreaItemList)
                .FirstOrDefaultAsync(c => c.Id == id);

            //geoFenceByArea.GeoFenceAreaItemList.Add(new GeoFenceAreaItem
            //{
            //    Area = _context.Areas.FirstOrDefault(),
            //    AreaId = _context.Areas.FirstOrDefault().Id
            //});
            if (geoFenceByArea == null)
            {
                return NotFound();
            }

            foreach (var area in geoFenceByArea.GeoFenceAreaItemList)
            {
                area.GeoFenceByArea = null;
            }
            return geoFenceByArea;
        }

        // PUT: api/GeoFenceByAreas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGeoFenceByArea(Guid id, GeoFenceByArea geoFenceByArea)
        {
            if (id != geoFenceByArea.Id)
            {
                return BadRequest();
            }

            if (geoFenceByArea.Tenant != null)
                geoFenceByArea.Tenant = null;

            RemoveOldAreaItem(id);

            if (geoFenceByArea.GeoFenceAreaItemList == null)
                geoFenceByArea.GeoFenceAreaItemList = new List<GeoFenceAreaItem>();

            foreach (var area in geoFenceByArea.GeoFenceAreaItemList)
            {
                if (area.Area != null)
                    area.Area = null;
            }

            _context.GeoFenceAreaItems.AddRange(geoFenceByArea.GeoFenceAreaItemList);
            _context.Entry(geoFenceByArea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeoFenceByAreaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {

            }


            return NoContent();
        }

        private void RemoveOldAreaItem(Guid id)
        {
            var areaItemList = _context.GeoFenceAreaItems.Where(c => c.GeoFenceByAreaId == id);

            foreach (var inDataBase in areaItemList.ToList())
            {
                _context.GeoFenceAreaItems.Remove(inDataBase);
            }

            _context.SaveChanges();
        }

        // POST: api/GeoFenceByAreas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GeoFenceByArea>> PostGeoFenceByArea(GeoFenceByArea geoFenceByArea)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                if (geoFenceByArea.TenantId == Guid.Empty)
                {
                    geoFenceByArea.TenantId = user.TenantId.Value;
                }

                if (geoFenceByArea.Tenant != null)
                    geoFenceByArea.Tenant = null;

                foreach (var area in geoFenceByArea.GeoFenceAreaItemList)
                {
                    if (area.Area != null)
                        area.Area = null;
                    if (area.GeoFenceByArea != null)
                        area.GeoFenceByArea = null;
                    if(area.GeoFenceByAreaId == Guid.Empty)
                    {
                        area.GeoFenceByAreaId = geoFenceByArea.Id;
                    }

                }
                _context.GeoFenceAreaItems.AddRange(geoFenceByArea.GeoFenceAreaItemList);
                _context.GeoFenceByAreas.Add(geoFenceByArea);
                await _context.SaveChangesAsync();

                foreach (var area in geoFenceByArea.GeoFenceAreaItemList)
                {
                    area.GeoFenceByArea = null;
                }

                return CreatedAtAction("GetGeoFenceByArea", new { id = geoFenceByArea.Id }, geoFenceByArea);
            }
            catch (Exception ex)
            {
            }
            return BadRequest();
        }

        // DELETE: api/GeoFenceByAreas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeoFenceByArea(Guid id)
        {
            var geoFenceByArea = await _context.GeoFenceByAreas.FindAsync(id);
            if (geoFenceByArea == null)
            {
                return NotFound();
            }

            _context.GeoFenceByAreas.Remove(geoFenceByArea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GeoFenceByAreaExists(Guid id)
        {
            return _context.GeoFenceByAreas.Any(e => e.Id == id);
        }
    }
}
