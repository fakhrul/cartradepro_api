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

            foreach (var obj in objs)
                if (obj.RoleModulePermissions != null)
                    foreach (var v in obj.RoleModulePermissions)
                    {
                        v.Role = null;
                        if (v.Module != null)
                            v.Module.RoleModulePermissions = null;
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
                .Include(r => r.RoleSubModulePermissions)
                .ThenInclude(rsmp => rsmp.SubModule)
                .FirstOrDefaultAsync(r => r.Id == id);

            var modules = await _context.Modules
                .AsNoTracking()
                .ToListAsync();

            foreach (var module in modules)
            {
                bool moduleAlreadyAvailable = false;
                foreach (var roleModulePermission in obj.RoleModulePermissions)
                {
                    if(roleModulePermission.ModuleId == module.Id)
                    {
                        moduleAlreadyAvailable = true;
                        continue;
                    }
                }

                if(!moduleAlreadyAvailable)
                {
                    var newPermission = new RoleModulePermission
                    {
                         ModuleId = module.Id,
                         RoleId = obj.Id,
                         CanAdd = false,
                         CanUpdate = false,
                         CanDelete = false,
                         CanView = false
                    };
                    _context.RoleModulePermissions.Add(newPermission);
                }
            }

            // Create missing RoleSubModulePermissions for all SubModules
            var subModules = await _context.SubModules
                .AsNoTracking()
                .ToListAsync();

            foreach (var subModule in subModules)
            {
                bool subModuleAlreadyAvailable = false;
                if (obj.RoleSubModulePermissions != null)
                {
                    foreach (var roleSubModulePermission in obj.RoleSubModulePermissions)
                    {
                        if (roleSubModulePermission.SubModuleId == subModule.Id)
                        {
                            subModuleAlreadyAvailable = true;
                            continue;
                        }
                    }
                }

                if (!subModuleAlreadyAvailable)
                {
                    var newPermission = new RoleSubModulePermission
                    {
                        SubModuleId = subModule.Id,
                        RoleId = obj.Id,
                        CanAdd = false,
                        CanUpdate = false,
                        CanDelete = false,
                        CanView = false
                    };
                    _context.RoleSubModulePermissions.Add(newPermission);
                }
            }

            // Save all new permissions at once
            await _context.SaveChangesAsync();

            // Reload to ensure Module and SubModule navigation properties are loaded correctly after adding new permissions
            obj = await _context.Roles
                .Include(r => r.RoleModulePermissions)
                .ThenInclude(rmp => rmp.Module)
                .ThenInclude(m => m.SubModules)
                .Include(r => r.RoleSubModulePermissions)
                .ThenInclude(rsmp => rsmp.SubModule)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (obj == null)
            {
                return NotFound();
            }
            if (obj.RoleModulePermissions != null)
                foreach (var v in obj.RoleModulePermissions)
                {
                    v.Role = null;
                    if (v.Module != null)
                    {
                        v.Module.RoleModulePermissions = null;
                        if (v.Module.SubModules != null)
                            foreach (var subModule in v.Module.SubModules)
                            {
                                subModule.Module = null;
                                subModule.RoleSubModulePermissions = null;
                            }
                    }
                }



            if (obj.RoleSubModulePermissions != null)
                foreach (var v in obj.RoleSubModulePermissions)
                {
                    v.Role = null;
                    if (v.SubModule != null)
                    {
                        v.SubModule.RoleSubModulePermissions = null;
                        v.SubModule.Module = null;
                    }
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
            UpdateRoleSubModulePermissions(existingRole, obj);

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

        /// <summary>
        /// Fix Registration role permissions - sets submodule permissions to match seed data expectations
        /// GET: api/Roles/fix-registration-permissions
        /// </summary>
        [HttpGet("fix-registration-permissions")]
        public async Task<ActionResult> FixRegistrationPermissions()
        {
            try
            {
                var results = new List<string>();

                // Find the Registration role
                var registrationRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Name == "Registration");

                if (registrationRole == null)
                {
                    return BadRequest(new { success = false, message = "Registration role not found" });
                }

                results.Add($"Found Registration role with ID: {registrationRole.Id}");

                // Define which submodules should have full permissions for Registration role
                var subModuleCodes = new[]
                {
                    "STOCK_INFO", "VEHICLE_INFO", "CLEARANCE", "SALE",
                    "REGISTRATION", "EXPENSES", "ADMINISTRATIVE_COST", "DISBURSEMENT"
                };

                int updatedCount = 0;
                int createdCount = 0;

                foreach (var code in subModuleCodes)
                {
                    // Find the submodule
                    var subModule = await _context.SubModules
                        .FirstOrDefaultAsync(sm => sm.Code == code);

                    if (subModule == null)
                    {
                        results.Add($"Warning: SubModule '{code}' not found in database");
                        continue;
                    }

                    // Check if permission exists
                    var existing = await _context.RoleSubModulePermissions
                        .FirstOrDefaultAsync(rsmp =>
                            rsmp.RoleId == registrationRole.Id &&
                            rsmp.SubModuleId == subModule.Id);

                    if (existing != null)
                    {
                        // Update existing permission
                        existing.CanView = true;
                        existing.CanAdd = true;
                        existing.CanUpdate = true;
                        existing.CanDelete = true;
                        updatedCount++;
                        results.Add($"Updated permission for '{subModule.Name}' (Code: {code})");
                    }
                    else
                    {
                        // Create new permission
                        var newPermission = new RoleSubModulePermission
                        {
                            RoleId = registrationRole.Id,
                            SubModuleId = subModule.Id,
                            CanView = true,
                            CanAdd = true,
                            CanUpdate = true,
                            CanDelete = true,
                            CreatedBy = "System Fix"
                        };
                        _context.RoleSubModulePermissions.Add(newPermission);
                        createdCount++;
                        results.Add($"Created permission for '{subModule.Name}' (Code: {code})");
                    }
                }

                // Also ensure STOCKS module permission exists and is enabled
                var stocksModule = await _context.Modules
                    .FirstOrDefaultAsync(m => m.Code == "STOCKS");

                if (stocksModule != null)
                {
                    var modulePermission = await _context.RoleModulePermissions
                        .FirstOrDefaultAsync(rmp =>
                            rmp.RoleId == registrationRole.Id &&
                            rmp.ModuleId == stocksModule.Id);

                    if (modulePermission != null)
                    {
                        modulePermission.CanView = true;
                        results.Add($"Updated STOCKS module permission to CanView=true");
                    }
                }

                // Save all changes
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Registration role permissions fixed: {updatedCount} updated, {createdCount} created",
                    updated = updatedCount,
                    created = createdCount,
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to fix permissions",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Fix missing SubModules for STOCKS module
        /// GET: api/Roles/fix-missing-submodules
        /// </summary>
        [HttpGet("fix-missing-submodules")]
        public async Task<ActionResult> FixMissingSubModules()
        {
            try
            {
                var results = new List<string>();

                // Find the STOCKS module
                var stocksModule = await _context.Modules
                    .FirstOrDefaultAsync(m => m.Code == "STOCKS");

                if (stocksModule == null)
                {
                    return BadRequest(new { success = false, message = "STOCKS module not found" });
                }

                results.Add($"Found STOCKS module with ID: {stocksModule.Id}");

                // Define all SubModules that should exist (from Seed.cs)
                var expectedSubModules = new[]
                {
                    new { Name = "Stock Info", Code = "STOCK_INFO", DisplayOrder = 1 },
                    new { Name = "Vehicle Info", Code = "VEHICLE_INFO", DisplayOrder = 2 },
                    new { Name = "Purchase", Code = "PURCHASE", DisplayOrder = 3 },
                    new { Name = "Import", Code = "IMPORT", DisplayOrder = 4 },
                    new { Name = "Clearance", Code = "CLEARANCE", DisplayOrder = 5 },
                    new { Name = "Sale", Code = "SALE", DisplayOrder = 6 },
                    new { Name = "Pricing", Code = "PRICING", DisplayOrder = 7 },
                    new { Name = "Arrival Checklist", Code = "ARRIVAL_CHECKLIST", DisplayOrder = 8 },
                    new { Name = "Registration", Code = "REGISTRATION", DisplayOrder = 9 },
                    new { Name = "Expenses", Code = "EXPENSES", DisplayOrder = 10 },
                    new { Name = "Administrative Cost", Code = "ADMINISTRATIVE_COST", DisplayOrder = 11 },
                    new { Name = "Disbursement", Code = "DISBURSEMENT", DisplayOrder = 12 },
                    new { Name = "Advertisement", Code = "ADVERTISEMENT", DisplayOrder = 13 }
                };

                int createdCount = 0;
                int existingCount = 0;

                foreach (var expected in expectedSubModules)
                {
                    // Check if SubModule already exists
                    var existing = await _context.SubModules
                        .FirstOrDefaultAsync(sm => sm.Code == expected.Code && sm.ModuleId == stocksModule.Id);

                    if (existing != null)
                    {
                        existingCount++;
                        results.Add($"SubModule '{expected.Name}' (Code: {expected.Code}) already exists");
                    }
                    else
                    {
                        // Create the missing SubModule
                        var newSubModule = new SubModule
                        {
                            Name = expected.Name,
                            Code = expected.Code,
                            ModuleId = stocksModule.Id,
                            DisplayOrder = expected.DisplayOrder,
                            CreatedBy = "System Fix"
                        };

                        _context.SubModules.Add(newSubModule);
                        createdCount++;
                        results.Add($"Created SubModule '{expected.Name}' (Code: {expected.Code})");
                    }
                }

                // Save all changes
                if (createdCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    success = true,
                    message = $"SubModules check completed: {createdCount} created, {existingCount} already exist",
                    created = createdCount,
                    existing = existingCount,
                    total = expectedSubModules.Length,
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to fix SubModules",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

    }
}
