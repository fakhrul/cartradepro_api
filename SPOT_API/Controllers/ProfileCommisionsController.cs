using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.DTOs;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileCommisionsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ProfileCommisionsController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/ProfileCommisions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileCommision>>> GetProfileCommisions()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var objs = await _context.ProfileCommisions
                    .Include(c => c.Package)
                    .Include(c => c.Profile)
                    .ToListAsync();

                return objs;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        // GET: api/ProfileCommisions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileCommision>> GetProfileCommision(Guid id)
        {
            var profileCommision = await _context.ProfileCommisions
                .Include(c => c.Profile)
                .Include(c => c.Package)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (profileCommision == null)
            {
                return NotFound();
            }

            return profileCommision;
        }


        // PUT: api/ProfileCommisions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfileCommision(Guid id, ProfileCommision profileCommision)
        {
            if (id != profileCommision.Id)
            {
                return BadRequest();
            }

            _context.Entry(profileCommision).State = EntityState.Modified;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileCommisionExists(id))
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
                throw ex;

            }





            return NoContent();
        }

        // POST: api/ProfileCommisions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProfileCommision>> PostProfileCommision(ProfileCommision profileCommision)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();


                _context.ProfileCommisions.Add(profileCommision);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("GetProfileCommision", new { id = profileCommision.Id }, profileCommision);
        }

        // DELETE: api/ProfileCommisions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfileCommision(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var profileCommision = await _context.ProfileCommisions
                .FirstOrDefaultAsync(c => c.Id == id);
            if (profileCommision == null)
            {
                return NotFound();
            }

            _context.ProfileCommisions.Remove(profileCommision);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool ProfileCommisionExists(Guid id)
        {
            return _context.ProfileCommisions.Any(e => e.Id == id);
        }
    }
}
