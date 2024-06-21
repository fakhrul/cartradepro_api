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
    public class GeoFenceRulesController : ControllerBase
    {
        private readonly SpotDBContext _context;

        public GeoFenceRulesController(SpotDBContext context)
        {
            _context = context;
        }

        // GET: api/GeoFenceRules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoFenceRule>>> GetGeoFenceRule()
        {
            return await _context.GeoFenceRule.ToListAsync();
        }

        // GET: api/GeoFenceRules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GeoFenceRule>> GetGeoFenceRule(Guid id)
        {
            var geoFenceRule = await _context.GeoFenceRule.FindAsync(id);

            if (geoFenceRule == null)
            {
                return NotFound();
            }

            return geoFenceRule;
        }

        // PUT: api/GeoFenceRules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGeoFenceRule(Guid id, GeoFenceRule geoFenceRule)
        {
            if (id != geoFenceRule.Id)
            {
                return BadRequest();
            }

            _context.Entry(geoFenceRule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GeoFenceRuleExists(id))
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

        // POST: api/GeoFenceRules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GeoFenceRule>> PostGeoFenceRule(GeoFenceRule geoFenceRule)
        {
            _context.GeoFenceRule.Add(geoFenceRule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGeoFenceRule", new { id = geoFenceRule.Id }, geoFenceRule);
        }

        // DELETE: api/GeoFenceRules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeoFenceRule(Guid id)
        {
            var geoFenceRule = await _context.GeoFenceRule.FindAsync(id);
            if (geoFenceRule == null)
            {
                return NotFound();
            }

            _context.GeoFenceRule.Remove(geoFenceRule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GeoFenceRuleExists(Guid id)
        {
            return _context.GeoFenceRule.Any(e => e.Id == id);
        }
    }
}
