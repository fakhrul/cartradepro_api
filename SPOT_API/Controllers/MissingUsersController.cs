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
    public class MissingUsersController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public MissingUsersController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        // GET: api/MissingUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MissingUser>>> GetMissingUser()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();
            return await _context.MissingUsers
                .Where(c => c.TenantId == user.TenantId)
                .Include(c => c.Profile)
                .Include(c => c.Profile.Department)
                .Include(c => c.Schedule)
                .OrderByDescending(c => c.DateTime)
                .ToListAsync();
        }

        // GET: api/MissingUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MissingUser>> GetMissingUser(Guid id)
        {
            var missingUser = await _context.MissingUsers
                .Include(c=> c.Profile)
                .Include(c => c.Schedule)
                .FirstOrDefaultAsync(c => c.Id ==  id);

            if (missingUser == null)
            {
                return NotFound();
            }

            return missingUser;
        }

        // PUT: api/MissingUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMissingUser(Guid id, MissingUser missingUser)
        {
            if (id != missingUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(missingUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MissingUserExists(id))
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

        // POST: api/MissingUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MissingUser>> PostMissingUser(MissingUser missingUser)
        {
            _context.MissingUsers.Add(missingUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMissingUser", new { id = missingUser.Id }, missingUser);
        }

        // DELETE: api/MissingUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMissingUser(Guid id)
        {
            var missingUser = await _context.MissingUsers.FindAsync(id);
            if (missingUser == null)
            {
                return NotFound();
            }

            _context.MissingUsers.Remove(missingUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MissingUserExists(Guid id)
        {
            return _context.MissingUsers.Any(e => e.Id == id);
        }
    }
}
