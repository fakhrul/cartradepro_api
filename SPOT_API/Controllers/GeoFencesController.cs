using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Persistence;
using SPOT_API.Models;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeoFencesController : ControllerBase
    {
        private readonly SpotDBContext _context;

        public GeoFencesController(SpotDBContext context)
        {
            _context = context;
        }

        // GET: api/GeoFences
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoFenceByMap>>> GetGeoFences()
        {
            return await _context.GeoFenceByMaps.ToListAsync();
        }

        // GET: api/GeoFences/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoFenceByMap>> GetGeoFence(Guid id)
        {
            var geoFence = await _context.GeoFenceByMaps.FindAsync(id);

            if (geoFence == null)
            {
                return NotFound();
            }

            return geoFence;
        }

        // PUT: api/GeoFences/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGeoFence(Guid id, GeoFenceByMap geoFence)
        {
            if (id != geoFence.Id)
            {
                return BadRequest();
            }

            _context.Entry(geoFence).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeoFenceExists(id))
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

        // POST: api/GeoFences
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GeoFenceByMap>> PostGeoFence(GeoFenceByMap geoFence)
        {
            _context.GeoFenceByMaps.Add(geoFence);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGeoFence", new { id = geoFence.Id }, geoFence);
        }

        // DELETE: api/GeoFences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeoFence(Guid id)
        {
            var geoFence = await _context.GeoFenceByMaps.FindAsync(id);
            if (geoFence == null)
            {
                return NotFound();
            }

            _context.GeoFenceByMaps.Remove(geoFence);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GeoFenceExists(Guid id)
        {
            return _context.GeoFenceByMaps.Any(e => e.Id == id);
        }
    }
}
