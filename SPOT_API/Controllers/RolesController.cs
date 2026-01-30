using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Services;
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

            // Simply return roles with Permissions JSON column
            var roles = await _context.Roles.ToListAsync();
            return roles;
        }


        // GET: api/Roles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleResponseDto>> Get(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            // Get the hardcoded modules template
            var modules = ModulesProvider.GetModules();

            // Convert the role with JSONB permissions to the old format
            var response = RoleResponseDto.FromRole(role, modules);

            return response;
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
        public async Task<IActionResult> Put(Guid id, RoleDto dto)
        {
            if (dto.Id.HasValue && id != dto.Id.Value)
            {
                return BadRequest("The ID in the URL does not match the ID in the object.");
            }

            var existingRole = await _context.Roles.FindAsync(id);

            if (existingRole == null)
            {
                return NotFound("The role you are trying to update does not exist.");
            }

            // Get hardcoded modules template to build mapping
            var modules = ModulesProvider.GetModules();
            var moduleIdToName = modules.ToDictionary(m => m.Id, m => m.Name);
            var subModuleIdToName = modules
                .SelectMany(m => m.SubModules)
                .ToDictionary(sm => sm.Id, sm => sm.Name);

            // Convert DTO to Role entity with JSONB Permissions
            dto.Id = id; // Ensure ID is set
            var updatedRole = dto.ToRole(moduleIdToName, subModuleIdToName);

            // Update role properties
            existingRole.Name = updatedRole.Name;
            existingRole.DisplayName = updatedRole.DisplayName;
            existingRole.Description = updatedRole.Description;
            existingRole.Permissions = updatedRole.Permissions; // Update JSON permissions

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



        // POST: api/Roles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Role>> Post(RoleDto dto)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                // Get hardcoded modules template to build mapping
                var modules = ModulesProvider.GetModules();
                var moduleIdToName = modules.ToDictionary(m => m.Id, m => m.Name);
                var subModuleIdToName = modules
                    .SelectMany(m => m.SubModules)
                    .ToDictionary(sm => sm.Id, sm => sm.Name);

                // Convert DTO to Role entity with JSONB Permissions
                var role = dto.ToRole(moduleIdToName, subModuleIdToName);

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                return CreatedAtAction("Get", new { id = role.Id }, role);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while creating the role: {ex.Message}");
            }
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

        // GET: api/Roles/modules-template
        // Returns completely hardcoded modules with database IDs for creating new roles
        [HttpGet("modules-template")]
        public ActionResult<IEnumerable<Module>> GetModulesTemplate()
        {
            return ModulesProvider.GetModules();
        }

        /// <summary>
        /// Fix module and submodule structure according to requirements
        /// GET: api/Roles/fix-module-structure
        /// </summary>
        [HttpGet("fix-module-structure")]
        public async Task<ActionResult> FixModuleStructure()
        {
            try
            {
                var changes = new List<string>();

                // 1. Rename "Vehicle" to "Pricelist"
                var vehicleModule = await _context.Modules.FirstOrDefaultAsync(m => m.Code == "VEHICLE");
                if (vehicleModule != null)
                {
                    vehicleModule.Name = "Pricelist";
                    vehicleModule.DisplayOrder = 5;
                    changes.Add($"✓ Renamed 'Vehicle' to 'Pricelist' (DisplayOrder: {vehicleModule.DisplayOrder})");
                }

                // 2. Ensure "Stock To Buy" module exists
                var stockToBuyModule = await _context.Modules.FirstOrDefaultAsync(m => m.Code == "STOCK_TO_BUY");
                if (stockToBuyModule == null)
                {
                    stockToBuyModule = new Module
                    {
                        Name = "Stock To Buy",
                        Code = "STOCK_TO_BUY",
                        DisplayOrder = 2
                    };
                    _context.Modules.Add(stockToBuyModule);
                    changes.Add("✓ Added 'Stock To Buy' module (DisplayOrder: 2)");
                }
                else if (stockToBuyModule.DisplayOrder != 2)
                {
                    stockToBuyModule.DisplayOrder = 2;
                    changes.Add($"✓ Updated 'Stock To Buy' DisplayOrder to 2");
                }

                // 3. Remove "System" module and its submodules
                var systemModule = await _context.Modules
                    .Include(m => m.SubModules)
                    .Include(m => m.RoleModulePermissions)
                    .FirstOrDefaultAsync(m => m.Code == "SYSTEM");
                if (systemModule != null)
                {
                    // Remove related permissions first
                    if (systemModule.RoleModulePermissions != null && systemModule.RoleModulePermissions.Any())
                    {
                        _context.RoleModulePermissions.RemoveRange(systemModule.RoleModulePermissions);
                        changes.Add($"✓ Removed {systemModule.RoleModulePermissions.Count} permissions for 'System' module");
                    }

                    // Remove submodules
                    if (systemModule.SubModules != null && systemModule.SubModules.Any())
                    {
                        foreach (var subModule in systemModule.SubModules)
                        {
                            var subModulePermissions = await _context.RoleSubModulePermissions
                                .Where(p => p.SubModuleId == subModule.Id)
                                .ToListAsync();
                            _context.RoleSubModulePermissions.RemoveRange(subModulePermissions);
                        }
                        _context.SubModules.RemoveRange(systemModule.SubModules);
                    }

                    _context.Modules.Remove(systemModule);
                    changes.Add("✓ Removed 'System' module");
                }

                // 4. Remove "Letters" module and its submodules
                var lettersModule = await _context.Modules
                    .Include(m => m.SubModules)
                    .Include(m => m.RoleModulePermissions)
                    .FirstOrDefaultAsync(m => m.Code == "LETTERS");
                if (lettersModule != null)
                {
                    // Remove related permissions first
                    if (lettersModule.RoleModulePermissions != null && lettersModule.RoleModulePermissions.Any())
                    {
                        _context.RoleModulePermissions.RemoveRange(lettersModule.RoleModulePermissions);
                        changes.Add($"✓ Removed {lettersModule.RoleModulePermissions.Count} permissions for 'Letters' module");
                    }

                    // Remove submodules
                    if (lettersModule.SubModules != null && lettersModule.SubModules.Any())
                    {
                        foreach (var subModule in lettersModule.SubModules)
                        {
                            var subModulePermissions = await _context.RoleSubModulePermissions
                                .Where(p => p.SubModuleId == subModule.Id)
                                .ToListAsync();
                            _context.RoleSubModulePermissions.RemoveRange(subModulePermissions);
                        }
                        _context.SubModules.RemoveRange(lettersModule.SubModules);
                    }

                    _context.Modules.Remove(lettersModule);
                    changes.Add("✓ Removed 'Letters' module");
                }

                // 5. Add "Sub Companies" to Master Data
                var masterDataModule = await _context.Modules
                    .Include(m => m.SubModules)
                    .FirstOrDefaultAsync(m => m.Code == "MASTER_DATA");
                if (masterDataModule != null)
                {
                    var subCompaniesExists = masterDataModule.SubModules?.Any(s => s.Code == "SUB_COMPANIES") ?? false;
                    if (!subCompaniesExists)
                    {
                        var subCompaniesSubModule = new SubModule
                        {
                            Name = "Sub Companies",
                            Code = "SUB_COMPANIES",
                            ModuleId = masterDataModule.Id,
                            DisplayOrder = 5
                        };
                        _context.SubModules.Add(subCompaniesSubModule);
                        changes.Add("✓ Added 'Sub Companies' submodule to Master Data");
                    }

                    // Remove "Models" submodule
                    var modelsSubModule = masterDataModule.SubModules?.FirstOrDefault(s => s.Code == "MODELS");
                    if (modelsSubModule != null)
                    {
                        var modelsPermissions = await _context.RoleSubModulePermissions
                            .Where(p => p.SubModuleId == modelsSubModule.Id)
                            .ToListAsync();
                        _context.RoleSubModulePermissions.RemoveRange(modelsPermissions);
                        _context.SubModules.Remove(modelsSubModule);
                        changes.Add("✓ Removed 'Models' submodule from Master Data");
                    }

                    // Remove "Showroom" submodule
                    var showroomSubModule = masterDataModule.SubModules?.FirstOrDefault(s => s.Code == "SHOWROOM" || s.Code == "SHOWROOMS");
                    if (showroomSubModule != null)
                    {
                        var showroomPermissions = await _context.RoleSubModulePermissions
                            .Where(p => p.SubModuleId == showroomSubModule.Id)
                            .ToListAsync();
                        _context.RoleSubModulePermissions.RemoveRange(showroomPermissions);
                        _context.SubModules.Remove(showroomSubModule);
                        changes.Add("✓ Removed 'Showroom' submodule from Master Data");
                    }
                }

                // 6. Add "Company" and "Audit Logs" to Administration
                var administrationModule = await _context.Modules
                    .Include(m => m.SubModules)
                    .FirstOrDefaultAsync(m => m.Code == "ADMINISTRATION");
                if (administrationModule != null)
                {
                    var companyExists = administrationModule.SubModules?.Any(s => s.Code == "COMPANY") ?? false;
                    if (!companyExists)
                    {
                        var companySubModule = new SubModule
                        {
                            Name = "Company",
                            Code = "COMPANY",
                            ModuleId = administrationModule.Id,
                            DisplayOrder = 4
                        };
                        _context.SubModules.Add(companySubModule);
                        changes.Add("✓ Added 'Company' submodule to Administration");
                    }

                    var auditLogsExists = administrationModule.SubModules?.Any(s => s.Code == "AUDIT_LOGS") ?? false;
                    if (!auditLogsExists)
                    {
                        var auditLogsSubModule = new SubModule
                        {
                            Name = "Audit Logs",
                            Code = "AUDIT_LOGS",
                            ModuleId = administrationModule.Id,
                            DisplayOrder = 3
                        };
                        _context.SubModules.Add(auditLogsSubModule);
                        changes.Add("✓ Added 'Audit Logs' submodule to Administration");
                    }
                }

                // 7. Fix Report submodules - keep only "Sales Report"
                var reportModule = await _context.Modules
                    .Include(m => m.SubModules)
                    .FirstOrDefaultAsync(m => m.Code == "REPORT");
                if (reportModule != null)
                {
                    // Remove "Sales Monthly" and "Sales Yearly" (search by name since code might be null)
                    var salesMonthly = reportModule.SubModules?.FirstOrDefault(s =>
                        s.Name == "SalesMonthly" || s.Code == "SALES_MONTHLY");
                    var salesYearly = reportModule.SubModules?.FirstOrDefault(s =>
                        s.Name == "SalesYearly" || s.Code == "SALES_YEARLY");

                    if (salesMonthly != null)
                    {
                        var permissions = await _context.RoleSubModulePermissions
                            .Where(p => p.SubModuleId == salesMonthly.Id)
                            .ToListAsync();
                        _context.RoleSubModulePermissions.RemoveRange(permissions);
                        _context.SubModules.Remove(salesMonthly);
                        changes.Add($"✓ Removed '{salesMonthly.Name}' submodule from Report");
                    }

                    if (salesYearly != null)
                    {
                        var permissions = await _context.RoleSubModulePermissions
                            .Where(p => p.SubModuleId == salesYearly.Id)
                            .ToListAsync();
                        _context.RoleSubModulePermissions.RemoveRange(permissions);
                        _context.SubModules.Remove(salesYearly);
                        changes.Add($"✓ Removed '{salesYearly.Name}' submodule from Report");
                    }

                    // Add "Sales Report" if it doesn't exist
                    var salesReportExists = reportModule.SubModules?.Any(s => s.Code == "SALES_REPORT") ?? false;
                    if (!salesReportExists)
                    {
                        var salesReportSubModule = new SubModule
                        {
                            Name = "Sales Report",
                            Code = "SALES_REPORT",
                            ModuleId = reportModule.Id,
                            DisplayOrder = 1
                        };
                        _context.SubModules.Add(salesReportSubModule);
                        changes.Add("✓ Added 'Sales Report' submodule to Report");
                    }
                }

                // 8. Update DisplayOrder for all modules according to requirements
                var allModules = await _context.Modules.ToListAsync();
                var moduleOrder = new Dictionary<string, int>
                {
                    { "DASHBOARD", 1 },
                    { "STOCK_TO_BUY", 2 },
                    { "STOCKS", 3 },
                    { "CUSTOMER", 4 },
                    { "VEHICLE", 5 },  // Now Pricelist
                    { "ADVERTISEMENT", 6 },
                    { "MASTER_DATA", 7 },
                    { "REPORT", 8 },
                    { "ADMINISTRATION", 9 }
                };

                foreach (var module in allModules)
                {
                    if (!string.IsNullOrEmpty(module.Code) && moduleOrder.ContainsKey(module.Code))
                    {
                        var newOrder = moduleOrder[module.Code];
                        if (module.DisplayOrder != newOrder)
                        {
                            module.DisplayOrder = newOrder;
                            changes.Add($"✓ Updated DisplayOrder for '{module.Name}' to {newOrder}");
                        }
                    }
                }

                // 9. Update DisplayOrder for all submodules according to requirements
                var allSubModules = await _context.SubModules.ToListAsync();
                var subModuleOrder = new Dictionary<string, int>
                {
                    // Stock submodules
                    { "STOCK_INFO", 1 },
                    { "VEHICLE_INFO", 2 },
                    { "PURCHASE", 3 },
                    { "IMPORT", 4 },
                    { "CLEARANCE", 5 },
                    { "SALE", 6 },
                    { "PRICING", 7 },
                    { "ARRIVAL_CHECKLIST", 8 },
                    { "REGISTRATION", 9 },
                    { "EXPENSES", 10 },
                    { "ADMINISTRATIVE_COST", 11 },
                    { "DISBURSEMENT", 12 },
                    { "ADVERTISEMENT", 13 },
                    // MasterData submodules
                    { "SUPPLIERS", 1 },
                    { "FORWARDERS", 2 },
                    { "BANKS", 3 },
                    { "BRANDS", 4 },
                    { "SUB_COMPANIES", 5 },
                    // Administration submodules
                    { "ROLES", 1 },
                    { "USER", 2 },
                    { "AUDIT_LOGS", 3 },
                    { "COMPANY", 4 },
                    // Reports submodules
                    { "SALES_REPORT", 1 }
                };

                foreach (var subModule in allSubModules)
                {
                    if (!string.IsNullOrEmpty(subModule.Code) && subModuleOrder.ContainsKey(subModule.Code))
                    {
                        var newOrder = subModuleOrder[subModule.Code];
                        if (subModule.DisplayOrder != newOrder)
                        {
                            subModule.DisplayOrder = newOrder;
                            changes.Add($"✓ Updated DisplayOrder for submodule '{subModule.Name}' to {newOrder}");
                        }
                    }
                }

                // Save all changes
                await _context.SaveChangesAsync();
                changes.Add("✓ All changes saved successfully");

                return Ok(new
                {
                    success = true,
                    message = "Module structure updated successfully",
                    changesCount = changes.Count,
                    changes = changes
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to update module structure",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

    }
}
