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
    public class PackageCommisionsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public PackageCommisionsController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/PackageCommisions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackageCommision>>> GetPackages()
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var objs = await _context.PackageCommisions
                    .Include(c => c.Package)
                    .ToListAsync();

                return objs;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        // GET: api/PackageCommisions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PackageCommision>> GetPackage(Guid id)
        {
            var package = await _context.PackageCommisions
                .FirstOrDefaultAsync(c => c.Id == id);

            if (package == null)
            {
                return NotFound();
            }

            return package;
        }


        // PUT: api/PackageCommisions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPackage(Guid id, PackageCommision package)
        {
            if (id != package.Id)
            {
                return BadRequest();
            }

            _context.Entry(package).State = EntityState.Modified;



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

        // POST: api/PackageCommisions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PackageCommision>> PostPackage(PackageCommision package)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();


                _context.PackageCommisions.Add(package);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("GetPackage", new { id = package.Id }, package);
        }

        // DELETE: api/PackageCommisions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var package = await _context.PackageCommisions
                .FirstOrDefaultAsync(c => c.Id == id);
            if (package == null)
            {
                return NotFound();
            }

            _context.PackageCommisions.Remove(package);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool PackageExists(Guid id)
        {
            return _context.PackageCommisions.Any(e => e.Id == id);
        }
    }
}
