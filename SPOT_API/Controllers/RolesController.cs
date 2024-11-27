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
    public class RolesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/Roles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.Roles
                .Include(r => r.RoleModulePermissions)
                .ThenInclude(rmp => rmp.Module)
                .ToListAsync();

            foreach(var obj in objs)
                if (obj.RoleModulePermissions != null)
                    foreach (var v in obj.RoleModulePermissions)
                    {
                        v.Role = null;
                        //v.Module.RoleModulePermissions = null;
                    }
            return objs;
        }


        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> Get(Guid id)
        {
            var obj = await _context.Roles
                .Include(r => r.RoleModulePermissions)
                .ThenInclude(rmp => rmp.Module)
                .ThenInclude(c=> c.SubModules)
                .Include(c => c.RoleSubModulePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            if(obj.RoleModulePermissions != null)
            foreach(var v in obj.RoleModulePermissions)
            {
                    v.Role = null;
                    if(v.Module != null)
                    {
                        foreach(var subModule in v.Module.SubModules)
                        {
                            subModule.Module = null;
                        }
                    }
                    //v.Module.RoleModulePermissions = null;
            }
            return obj;
        }


        // PUT: api/Roles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put(Guid id, Role obj)
        //{
        //    if (id != obj.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(obj).State = EntityState.Modified;


        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!IsExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }





        //    return NoContent();
        //}

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Role obj)
        {
            if (id != obj.Id)
            {
                return BadRequest("The ID in the URL does not match the ID in the object.");
            }

            var existingRole = await _context.Roles
                .Include(r => r.RoleModulePermissions)
                .Include(r => r.RoleSubModulePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingRole == null)
            {
                return NotFound("The role you are trying to update does not exist.");
            }

            // Update the existing role's properties
            existingRole.Name = obj.Name;

            // Update RoleModulePermissions
            UpdateRoleModulePermissions(existingRole, obj);

            // Update RoleSubModulePermissions
            //UpdateRoleSubModulePermissions(existingRole, obj);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(id))
                {
                    return NotFound("The role was deleted by another user.");
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while updating the role: {ex.Message}");
            }

            return NoContent();
        }

        private void UpdateRoleModulePermissions(Role existingRole, Role updatedRole)
        {
            // Remove RoleModulePermissions that are no longer in the updatedRole
            var removedModulePermissions = existingRole.RoleModulePermissions
                .Where(p => !updatedRole.RoleModulePermissions.Any(up => up.Id == p.Id))
                .ToList();

            foreach (var permission in removedModulePermissions)
            {
                _context.RoleModulePermissions.Remove(permission);
            }

            // Add or update RoleModulePermissions
            foreach (var updatedPermission in updatedRole.RoleModulePermissions)
            {
                var existingPermission = existingRole.RoleModulePermissions
                    .FirstOrDefault(p => p.Id == updatedPermission.Id);

                if (existingPermission != null)
                {
                    // Update the existing permission
                    existingPermission.CanAdd = updatedPermission.CanAdd;
                    existingPermission.CanUpdate = updatedPermission.CanUpdate;
                    existingPermission.CanDelete = updatedPermission.CanDelete;
                    existingPermission.CanView = updatedPermission.CanView;
                    // Update other properties as necessary
                }
                else
                {
                    // Add the new permission
                    existingRole.RoleModulePermissions.Add(updatedPermission);
                }
            }
        }

        private void UpdateRoleSubModulePermissions(Role existingRole, Role updatedRole)
        {
            // Remove RoleSubModulePermissions that are no longer in the updatedRole
            var removedSubModulePermissions = existingRole.RoleSubModulePermissions
                .Where(p => !updatedRole.RoleSubModulePermissions.Any(up => up.Id == p.Id))
                .ToList();

            foreach (var permission in removedSubModulePermissions)
            {
                _context.RoleSubModulePermissions.Remove(permission);
            }

            // Add or update RoleSubModulePermissions
            foreach (var updatedPermission in updatedRole.RoleSubModulePermissions)
            {
                var existingPermission = existingRole.RoleSubModulePermissions
                    .FirstOrDefault(p => p.Id == updatedPermission.Id);

                if (existingPermission != null)
                {
                    // Update the existing permission
                    existingPermission.CanAdd = updatedPermission.CanAdd;
                    existingPermission.CanUpdate = updatedPermission.CanUpdate;
                    existingPermission.CanDelete = updatedPermission.CanDelete;
                    existingPermission.CanView = updatedPermission.CanView;
                    // Update other properties as necessary
                }
                else
                {
                    // Add the new permission
                    existingRole.RoleSubModulePermissions.Add(updatedPermission);
                }
            }
        }


        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Role>> Post(Role obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                _context.Roles.Add(obj);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var category = await _context.Roles
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(category);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool IsExists(Guid id)
        {
            return _context.Roles.Any(e => e.Id == id);
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

    }
}
