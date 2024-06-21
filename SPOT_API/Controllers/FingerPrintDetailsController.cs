using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FingerPrintDetailsController : ControllerBase
    {
        private readonly SpotDBContext _context;

        public FingerPrintDetailsController(SpotDBContext context)
        {
            _context = context;
        }

        // GET: api/FingerPrintDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FingerPrintDetail>>> GetFingerPrintDetail()
        {
            return await _context.FingerPrintDetails.ToListAsync();
        }

        // GET: api/FingerPrintDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FingerPrintDetail>> GetFingerPrintDetail(Guid id)
        {
            var fingerPrintDetail = await _context.FingerPrintDetails
                .Include(c => c.Device)
                .Include(c => c.Area)
                .Include(c => c.Area.Level)
                .Include(c => c.Area.Level.Location)
                .FirstOrDefaultAsync(c => c.Id  == id);

            if (fingerPrintDetail == null)
            {
                return NotFound();
            }

            return fingerPrintDetail;
        }

        // GET: api/FingerPrintDetails
        [HttpGet("ByFpId/{id}")]
        public async Task<ActionResult<IEnumerable<FingerPrintDetail>>> GetFingerPrintDetailByFpId(Guid id)
        {
            return await _context.FingerPrintDetails
                .Where(c=> c.FingerPrintId == id)
                //.Include(c=> c.FingerPrint)
                .Include(c=> c.Device)
                .Include(c=> c.Area)
                .Include(c => c.Area.Level)
                .Include(c => c.Area.Level.Location)
                .ToListAsync();
        }

        // PUT: api/FingerPrintDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFingerPrintDetail(Guid id, FingerPrintDetail fingerPrintDetail)
        {
            if (id != fingerPrintDetail.Id)
            {
                return BadRequest();
            }

            _context.Entry(fingerPrintDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FingerPrintDetailExists(id))
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

        // POST: api/FingerPrintDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FingerPrintDetail>> PostFingerPrintDetail(FingerPrintDetail fingerPrintDetail)
        {
            _context.FingerPrintDetails.Add(fingerPrintDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFingerPrintDetail", new { id = fingerPrintDetail.Id }, fingerPrintDetail);
        }

        // DELETE: api/FingerPrintDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFingerPrintDetail(Guid id)
        {
            var fingerPrintDetail = await _context.FingerPrintDetails.FindAsync(id);
            if (fingerPrintDetail == null)
            {
                return NotFound();
            }

            _context.FingerPrintDetails.Remove(fingerPrintDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FingerPrintDetailExists(Guid id)
        {
            return _context.FingerPrintDetails.Any(e => e.Id == id);
        }
    }
}
