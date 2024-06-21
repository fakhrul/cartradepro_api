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
    public class ProfilePackagesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ProfilePackagesController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/ProfilePackages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfilePackage>>> GetProfilePackages()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var objs = await _context.ProfilePackages
                    .Include(c => c.Package)
                    .Include(c => c.Profile)
                    .Include(c => c.Profile.Leader)
                    .Where(c => c.ProfileId == user.ProfileId)
                    .ToListAsync();

                var obj2s = await _context.ProfilePackages
                    .Include(c => c.Package)
                    .Include(c => c.Profile)
                    .Include(c => c.Profile.Leader)
                    .Where(c => c.Profile.LeaderId == user.ProfileId)
                    .ToListAsync();

                objs.AddRange(obj2s);

                foreach (var obj in objs)
                {
                    ////obj.Profile = null;
                    //if (obj.Profile != null)
                    //    obj.Profile.ProfilePackages = null;
                    //if (obj.Profile != null)
                    //    obj.Profile.Leader = null;
                    if (obj.Profile.Leader != null)
                    {
                        obj.Profile = new Profile
                        {
                            FullName = obj.Profile.FullName,
                            Leader = new Profile
                            {
                                FullName = obj.Profile.Leader.FullName,

                            }
                        };
                    }
                    else
                    {
                        obj.Profile = new Profile
                        {
                            FullName = obj.Profile.FullName,
                        };

                    }

                    //if (obj.Profile != null && obj.Profile.Leader != null)
                    //    obj.Profile.Leader.ProfilePackages = null;
                }
                return objs;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet("ByCurrentProfile")]
        public async Task<ActionResult<IEnumerable<ProfilePackage>>> GetProfilePackagesByCurrentProfile()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();


                var objs = await _context.ProfilePackages
                    .Where(c => c.ProfileId == user.ProfileId)
                    .Include(c => c.Package)
                    .ToListAsync();

                return objs;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        // GET: api/ProfilePackages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfilePackage>> GetPackage(Guid id)
        {
            var profilePackage = await _context.ProfilePackages
                .FirstOrDefaultAsync(c => c.Id == id);

            if (profilePackage == null)
            {
                return NotFound();
            }

            return profilePackage;
        }


        // PUT: api/ProfilePackages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPackage(Guid id, ProfilePackage profilePackage)
        {
            if (id != profilePackage.Id)
            {
                return BadRequest();
            }

            _context.Entry(profilePackage).State = EntityState.Modified;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackageExists(id))
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

        // POST: api/ProfilePackages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProfilePackage>> PostPackage(ProfilePackage profilePackage)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();


                _context.ProfilePackages.Add(profilePackage);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("GetPackage", new { id = profilePackage.Id }, profilePackage);
        }

        // DELETE: api/ProfilePackages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var profilePackage = await _context.ProfilePackages
                .FirstOrDefaultAsync(c => c.Id == id);
            if (profilePackage == null)
            {
                return NotFound();
            }

            _context.ProfilePackages.Remove(profilePackage);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool PackageExists(Guid id)
        {
            return _context.ProfilePackages.Any(e => e.Id == id);
        }
    }
}
