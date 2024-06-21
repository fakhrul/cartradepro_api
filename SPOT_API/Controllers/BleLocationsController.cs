using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BleLocationsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public BleLocationsController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/BleLocations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BleLocation>>> GetBleLocations()
        {

            //throw new ApiException(new Exception("asdas"));

            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            return await _context
                .BleLocations
                .Where(c=> c.TenantId == user.TenantId)
                .ToListAsync();
        }

        // GET: api/BleLocations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BleLocation>> GetBleLocation(Guid id)
        {
            var bleLocation = await _context.BleLocations.FindAsync(id);

            if (bleLocation == null)
            {
                return NotFound();
            }

            return bleLocation;
        }

        // PUT: api/BleLocations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBleLocation(Guid id, BleLocation bleLocation)
        {
            if (id != bleLocation.Id)
            {
                return BadRequest();
            }

            _context.Entry(bleLocation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BleLocationExists(id))
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

        // POST: api/BleLocations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BleLocation>> PostBleLocation(BleLocation bleLocation)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();
                if (user.TenantId == Guid.Empty)
                    return Unauthorized();

                if (bleLocation.TenantId == Guid.Empty)
                    bleLocation.TenantId = user.TenantId.Value;

                _context.BleLocations.Add(bleLocation);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBleLocation", new { id = bleLocation.Id }, bleLocation);
            }
            catch (Exception ex)
            {
                throw new ApiException(ex.InnerException);
            }
        }

        // DELETE: api/BleLocations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBleLocation(Guid id)
        {
            var bleLocation = await _context.BleLocations.FindAsync(id);
            if (bleLocation == null)
            {
                return NotFound();
            }

            _context.BleLocations.Remove(bleLocation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BleLocationExists(Guid id)
        {
            return _context.BleLocations.Any(e => e.Id == id);
        }
    }
}
