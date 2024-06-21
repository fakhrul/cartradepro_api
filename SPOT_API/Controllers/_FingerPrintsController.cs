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
    public class FingerPrintsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public FingerPrintsController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/FingerPrints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FingerPrint>>> GetFingerPrint()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            return await _context.FingerPrint.Where(c => c.Device.TenantId == user.TenantId)
                .Include(o => o.Device)
                .Include(o => o.Device.DeviceType)
                .Include(o => o.Area)
                .ToListAsync();

            //return await _context.FingerPrint.ToListAsync();
        }

        // GET: api/FingerPrints/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FingerPrint>> GetFingerPrint(Guid id)
        {
            var fingerPrint = await _context.FingerPrint.FindAsync(id);

            if (fingerPrint == null)
            {
                return NotFound();
            }

            return fingerPrint;
        }

        // PUT: api/FingerPrints/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFingerPrint(Guid id, FingerPrint fingerPrint)
        {
            if (id != fingerPrint.Id)
            {
                return BadRequest();
            }

            _context.Entry(fingerPrint).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FingerPrintExists(id))
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

        // POST: api/FingerPrints
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FingerPrint>> PostFingerPrint(FingerPrint fingerPrint)
        {
            try
            {
                fingerPrint.Area = null;
                fingerPrint.Device = null;

                _context.FingerPrint.Add(fingerPrint);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return CreatedAtAction("GetFingerPrint", new { id = fingerPrint.Id }, fingerPrint);
        }

        // DELETE: api/FingerPrints/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFingerPrint(Guid id)
        {
            var fingerPrint = await _context.FingerPrint.FindAsync(id);
            if (fingerPrint == null)
            {
                return NotFound();
            }

            _context.FingerPrint.Remove(fingerPrint);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FingerPrintExists(Guid id)
        {
            return _context.FingerPrint.Any(e => e.Id == id);
        }
    }
}
