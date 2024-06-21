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
    public class EmergencyUsersController : ControllerBase
    {
        private readonly SpotDBContext _context;

        public EmergencyUsersController(SpotDBContext context)
        {
            _context = context;
        }

        // GET: api/EmergencyUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmergencyUser>>> GetEmergencyUsers()
        {
            return await _context.EmergencyUsers.ToListAsync();
        }

        // GET: api/EmergencyUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmergencyUser>> GetEmergencyUser(Guid id)
        {
            var emergencyUser = await _context.EmergencyUsers.FindAsync(id);

            if (emergencyUser == null)
            {
                return NotFound();
            }

            return emergencyUser;
        }

        // PUT: api/EmergencyUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmergencyUser(Guid id, EmergencyUser emergencyUser)
        {
            if (id != emergencyUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(emergencyUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmergencyUserExists(id))
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

        // POST: api/EmergencyUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmergencyUser>> PostEmergencyUser(EmergencyUser emergencyUser)
        {
            _context.EmergencyUsers.Add(emergencyUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmergencyUser", new { id = emergencyUser.Id }, emergencyUser);
        }

        // DELETE: api/EmergencyUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmergencyUser(Guid id)
        {
            var emergencyUser = await _context.EmergencyUsers.FindAsync(id);
            if (emergencyUser == null)
            {
                return NotFound();
            }

            _context.EmergencyUsers.Remove(emergencyUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmergencyUserExists(Guid id)
        {
            return _context.EmergencyUsers.Any(e => e.Id == id);
        }
    }
}
