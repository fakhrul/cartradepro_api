using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SPOT_API.Persistence;
using Application.Interfaces;
using SPOT_API.Models;
using System;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesControllerProb : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesControllerProb(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        //public RolesController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _userAccessor = userAccessor;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            return await _context.Roles
                .Include(r => r.RoleModulePermissions)
                .ThenInclude(rmp => rmp.Module)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(Guid id)
        {
            var role = await _context.Roles
                .Include(r => r.RoleModulePermissions)
                .ThenInclude(rmp => rmp.Module)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return role;
        }

        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRole", new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(Guid id, Role role)
        {
            if (id != role.Id)
            {
                return BadRequest();
            }

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!IsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("AddUserRole/{userEmail}/{roleName}")]
        public async Task<IActionResult> AddUserRole(string userEmail, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound($"User with Email = {userEmail} cannot be found");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return NotFound($"Role {roleName} does not exist");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok($"User {userEmail} added to role {roleName}");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("RemoveUserRole/{userEmail}/{roleName}")]
        public async Task<IActionResult> RemoveUserRole(string userEmail, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound($"User with Email = {userEmail} cannot be found");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok($"User {userEmail} removed from role {roleName}");
            }

            return BadRequest(result.Errors);
        }

        private bool IsExists(Guid id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}