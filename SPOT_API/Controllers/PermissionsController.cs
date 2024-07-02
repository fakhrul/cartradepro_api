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
    public class PermissionsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;

        public PermissionsController(SpotDBContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleModulePermission>>> GetPermissions()
        {
            return await _context.RoleModulePermissions.Include(rmp => rmp.Role).Include(rmp => rmp.Module).ToListAsync();
        }


        //[HttpGet("GetRolesAndPermissions")]
        //public async Task<IActionResult> GetRolesAndPermissions()
        //{
        //    try
        //    {
        //        //var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        //        //var roles = await _userManager.GetRolesAsync(user.Profile.AppUser);
        //        // Assume you have a method to get permissions based on roles

        //        var roles = _context.Roles.ToListAsync();
        //        var permissions = GetPermissionsForRoles(roles);

        //        return Ok(new { roles, permissions });

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //private Dictionary<string, bool> GetPermissionsForRoles(IList<string> roles)
        //{
        //    // Implement your logic to get permissions based on roles
        //    // This is a placeholder
        //    return new Dictionary<string, bool>
        //{
        //    { "canAdd", roles.Contains("Admin") || roles.Contains("Stock Manager") },
        //    { "canEdit", roles.Contains("Admin") || roles.Contains("Stock Manager") },
        //    { "canDelete", roles.Contains("Admin") },
        //    { "canView", true }
        //};
        //}


        [HttpGet("{roleId}/{moduleId}")]
        public async Task<ActionResult<RoleModulePermission>> GetPermission(Guid roleId, Guid moduleId)
        {
            var permission = await _context.RoleModulePermissions.FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.ModuleId == moduleId);

            if (permission == null)
            {
                return NotFound();
            }

            return permission;
        }

        [HttpPost]
        public async Task<ActionResult<RoleModulePermission>> PostPermission(RoleModulePermission permission)
        {
            _context.RoleModulePermissions.Add(permission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPermission", new { roleId = permission.RoleId, moduleId = permission.ModuleId }, permission);
        }

        [HttpPut("{roleId}/{moduleId}")]
        public async Task<IActionResult> PutPermission(Guid roleId, Guid moduleId, RoleModulePermission permission)
        {
            if (roleId != permission.RoleId || moduleId != permission.ModuleId)
            {
                return BadRequest();
            }

            _context.Entry(permission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermissionExists(roleId, moduleId))
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

        [HttpDelete("{roleId}/{moduleId}")]
        public async Task<IActionResult> DeletePermission(Guid roleId, Guid moduleId)
        {
            var permission = await _context.RoleModulePermissions.FirstOrDefaultAsync(rmp => rmp.RoleId == roleId && rmp.ModuleId == moduleId);
            if (permission == null)
            {
                return NotFound();
            }

            _context.RoleModulePermissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PermissionExists(Guid roleId, Guid moduleId)
        {
            return _context.RoleModulePermissions.Any(e => e.RoleId == roleId && e.ModuleId == moduleId);
        }
    }

}
