using Application.Interfaces;
using AppAuthService = Application.Interfaces.IAuthorizationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using SPOT_API.DTOs;
using SPOT_API.Models;
using SPOT_API.Persistence;
using SPOT_API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppAuthService _authorizationService;
        private readonly IAuditService _auditService;

        public AccountController(
            SpotDBContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            TokenService tokenService,
            AppAuthService authorizationService,
            IAuditService auditService)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
            _auditService = auditService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    await _auditService.LogAuthenticationAsync(
                        AuditEventType.LoginFailed,
                        null,
                        false,
                        $"Login failed - user not found: {loginDto.Email}",
                        "User not found");
                    return Unauthorized();
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (!result.Succeeded)
                {
                    await _auditService.LogAuthenticationAsync(
                        AuditEventType.LoginFailed,
                        user.Id,
                        false,
                        $"Login failed for user: {user.UserName}",
                        "Invalid password");
                    return Unauthorized();
                }

                // Update last login timestamp
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Get all active roles for the user
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id && ur.IsActive)
                    .Where(ur => !ur.EffectiveFrom.HasValue || ur.EffectiveFrom.Value <= DateTime.UtcNow)
                    .Where(ur => !ur.EffectiveUntil.HasValue || ur.EffectiveUntil.Value >= DateTime.UtcNow)
                    .Include(ur => ur.Role)
                    .ToListAsync();

                if (!userRoles.Any())
                {
                    await _auditService.LogAuthenticationAsync(
                        AuditEventType.LoginFailed,
                        user.Id,
                        false,
                        $"Login failed for user: {user.UserName}",
                        "User has no active roles");
                    return Unauthorized("User has no active roles");
                }

                // Get aggregated permissions from all active roles
                var modulePermissions = await _authorizationService.GetUserModulesAsync(user.Id);
                Console.WriteLine($"=== Login: User {user.UserName} ===");
                Console.WriteLine($"Module Permissions Count: {modulePermissions.Count}");
                foreach (var mp in modulePermissions)
                {
                    Console.WriteLine($"  Module: {mp.ModuleName} (Code: {mp.ModuleCode}) - CanView: {mp.CanView}");
                }

                var allSubModulePermissions = new List<SubModulePermissionDto>();

                // Get submodule permissions for each module
                foreach (var module in modulePermissions)
                {
                    var subModules = await _authorizationService.GetUserSubModulesAsync(user.Id, module.ModuleCode);
                    allSubModulePermissions.AddRange(subModules);
                }

                Console.WriteLine($"SubModule Permissions Count: {allSubModulePermissions.Count}");
                foreach (var smp in allSubModulePermissions)
                {
                    Console.WriteLine($"  SubModule: {smp.SubModuleName} (Code: {smp.SubModuleCode}) - CanView: {smp.CanView}");
                }

                // Convert to legacy format for compatibility with frontend
                var roleModulePermissions = modulePermissions.Select(mp => new RoleModulePermission
                {
                    ModuleId = mp.ModuleId,
                    CanView = mp.CanView,
                    CanAdd = mp.CanAdd,
                    CanUpdate = mp.CanUpdate,
                    CanDelete = mp.CanDelete,
                    Module = new Module
                    {
                        Id = mp.ModuleId,
                        Name = mp.ModuleName,
                        Code = mp.ModuleCode,
                        DisplayOrder = mp.DisplayOrder,
                        Icon = mp.Icon,
                        SubModules = null
                    }
                }).ToList();

                var roleSubModulePermissions = allSubModulePermissions.Select(smp => new RoleSubModulePermission
                {
                    SubModuleId = smp.SubModuleId,
                    CanView = smp.CanView,
                    CanAdd = smp.CanAdd,
                    CanUpdate = smp.CanUpdate,
                    CanDelete = smp.CanDelete,
                    SubModule = new SubModule
                    {
                        Id = smp.SubModuleId,
                        Name = smp.SubModuleName,
                        Code = smp.SubModuleCode,
                        DisplayOrder = smp.DisplayOrder,
                        Icon = smp.Icon,
                        RoleSubModulePermissions = null
                    }
                }).ToList();

                // Check if user is Super Admin
                var isSuperAdmin = await _authorizationService.IsSuperAdminAsync(user.Id);

                // Get primary role (first active role or legacy role field)
                var primaryRole = userRoles.FirstOrDefault()?.Role?.Name ?? user.Role;

                var userDto = new UserDto
                {
                    DisplayName = user.DisplayName,
                    Image = null,
                    Token = _tokenService.CreateToken(user),
                    UserName = user.UserName,
                    Role = primaryRole,
                    IsSuperAdmin = isSuperAdmin,
                    TenantCode = user.TenantCode,
                    TenantId = user.TenantId,
                    RoleModulePermissions = roleModulePermissions,
                    RoleSubModulePermissions = roleSubModulePermissions
                };

                Console.WriteLine($"=== Returning UserDto for {user.UserName} ===");
                Console.WriteLine($"Role: {primaryRole}");
                Console.WriteLine($"IsSuperAdmin: {isSuperAdmin}");
                Console.WriteLine($"RoleModulePermissions count: {roleModulePermissions.Count}");
                Console.WriteLine($"RoleSubModulePermissions count: {roleSubModulePermissions.Count}");

                // Log successful login
                await _auditService.LogAuthenticationAsync(
                    AuditEventType.Login,
                    user.Id,
                    true,
                    $"User logged in successfully: {user.UserName}");

                return userDto;
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "Login",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    "Error during login process",
                    null,
                    null,
                    AuditSeverity.High);
                throw;
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            if (user != null)
            {
                await _auditService.LogAuthenticationAsync(
                    AuditEventType.Logout,
                    user.Id,
                    true,
                    $"User logged out: {user.UserName}");
            }

            return Ok("Logout successful");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("email", "Email taken");
                return ValidationProblem();
            }

            Profile profile = new Profile
            {
                FullName = "Your Full Name",
                Email = registerDto.Email,
                Role = "TenantAdmin",
            };

            await _context.Profiles.AddAsync(profile);

            var user = new AppUser
            {
                DisplayName = profile.FullName,
                Email = profile.Email,
                UserName = profile.Email,
                ProfileId = profile.Id,
                IsEmailVerified = false
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.UserRegistrationFailed,
                    "Register",
                    string.Join(", ", result.Errors.Select(e => e.Description)),
                    null,
                    null,
                    $"Registration failed for email: {registerDto.Email}",
                    "AppUser",
                    null,
                    AuditSeverity.Medium);
                return BadRequest("Problem registering user");
            }

            await _auditService.LogAsync(
                AuditEventType.UserRegistered,
                "Register",
                $"New user registered: {user.UserName}",
                "AppUser",
                user.Id,
                user.DisplayName,
                null,
                new { Email = user.Email, UserName = user.UserName },
                AuditSeverity.Info);

            return Ok("Registration success");
        }

        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.EmailVerificationFailed,
                    "VerifyEmail",
                    "Email verification failed",
                    null,
                    null,
                    $"Email verification failed for: {email}",
                    "AppUser",
                    user.Id,
                    AuditSeverity.Medium);
                return BadRequest("Could not verify email address");
            }

            user.IsEmailVerified = true;
            await _userManager.UpdateAsync(user);

            await _auditService.LogAsync(
                AuditEventType.EmailVerified,
                "VerifyEmail",
                $"Email verified for user: {user.UserName}",
                "AppUser",
                user.Id,
                user.DisplayName,
                null,
                null,
                AuditSeverity.Info);

            return Ok("Email confirmed - you can now login");
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            if (user == null) return Unauthorized();

            await SetRefreshToken(user);
            return await CreateUserObjectAsync(user);
        }

        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var user = await _userManager.Users
                .Include(r => r.RefreshTokens)
                .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

            if (user == null) return Unauthorized();

            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

            if (oldToken != null && !oldToken.IsActive) return Unauthorized();

            return await CreateUserObjectAsync(user);
        }

        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        private async Task<UserDto> CreateUserObjectAsync(AppUser user)
        {
            // Get user permissions
            var modulePermissions = await _authorizationService.GetUserModulesAsync(user.Id);
            var allSubModulePermissions = new List<SubModulePermissionDto>();

            // Get submodule permissions for each module
            foreach (var module in modulePermissions)
            {
                var subModules = await _authorizationService.GetUserSubModulesAsync(user.Id, module.ModuleCode);
                allSubModulePermissions.AddRange(subModules);
            }

            var isSuperAdmin = await _authorizationService.IsSuperAdminAsync(user.Id);
            var activeRoles = await _authorizationService.GetUserActiveRolesAsync(user.Id);
            var primaryRole = activeRoles.FirstOrDefault()?.Name ?? user.Role;

            // Convert to legacy format for compatibility
            var roleModulePermissions = modulePermissions.Select(mp => new RoleModulePermission
            {
                ModuleId = mp.ModuleId,
                CanView = mp.CanView,
                CanAdd = mp.CanAdd,
                CanUpdate = mp.CanUpdate,
                CanDelete = mp.CanDelete,
                Module = new Module
                {
                    Id = mp.ModuleId,
                    Name = mp.ModuleName,
                    Code = mp.ModuleCode,
                    DisplayOrder = mp.DisplayOrder,
                    Icon = mp.Icon,
                    SubModules = null
                }
            }).ToList();

            var roleSubModulePermissions = allSubModulePermissions.Select(smp => new RoleSubModulePermission
            {
                SubModuleId = smp.SubModuleId,
                CanView = smp.CanView,
                CanAdd = smp.CanAdd,
                CanUpdate = smp.CanUpdate,
                CanDelete = smp.CanDelete,
                SubModule = new SubModule
                {
                    Id = smp.SubModuleId,
                    Name = smp.SubModuleName,
                    Code = smp.SubModuleCode,
                    DisplayOrder = smp.DisplayOrder,
                    Icon = smp.Icon,
                    RoleSubModulePermissions = null
                }
            }).ToList();

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                UserName = user.UserName,
                Role = primaryRole,
                IsSuperAdmin = isSuperAdmin,
                TenantCode = user.TenantCode,
                TenantId = user.TenantId,
                RoleModulePermissions = roleModulePermissions,
                RoleSubModulePermissions = roleSubModulePermissions
            };
        }

        /// <summary>
        /// Migration endpoint: Creates UserRole records for existing users
        /// Call this once to migrate existing users to the new UserRoles table
        /// GET: api/Account/migrate-user-roles
        /// </summary>
        [AllowAnonymous]
        [HttpGet("migrate-user-roles")]
        public async Task<ActionResult> MigrateUserRoles()
        {
            try
            {
                // Map old role names to new role names
                var roleMapping = new Dictionary<string, string>
                {
                    { "Admin", "SuperAdmin" },
                    { "Stock Manager", "StockManager" },
                    { "Finance Manager", "Account" },
                    { "Marketing", "Advertisement" },
                    { "Sales", "Sales" },
                    { "StockManager", "StockManager" },
                    { "Account", "Account" },
                    { "Advertisement", "Advertisement" },
                    { "SuperAdmin", "SuperAdmin" },
                    { "Registration", "Registration" }
                };

                // Get all users
                var allUsers = await _context.Users.ToListAsync();
                var results = new List<string>();
                var processedCount = 0;
                var skippedCount = 0;

                foreach (var user in allUsers)
                {
                    // Skip users without a role
                    if (string.IsNullOrEmpty(user.Role))
                    {
                        results.Add($"Skipped {user.Email} - no role assigned");
                        skippedCount++;
                        continue;
                    }

                    // Map old role name to new role name
                    var roleName = roleMapping.ContainsKey(user.Role) ? roleMapping[user.Role] : user.Role;

                    // Find the role in the custom Roles table
                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                    if (role == null)
                    {
                        results.Add($"Warning: Role '{roleName}' not found for user {user.Email}");
                        skippedCount++;
                        continue;
                    }

                    // Check if UserRole already exists
                    var existingUserRole = await _context.UserRoles
                        .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

                    if (existingUserRole != null)
                    {
                        results.Add($"UserRole already exists for {user.Email} with role {roleName}");
                        skippedCount++;
                        continue;
                    }

                    // Create new UserRole record
                    var userRole = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id,
                        IsActive = true,
                        AssignedBy = "System Migration",
                        AssignedAt = DateTime.UtcNow,
                        CreatedBy = "System"
                    };

                    await _context.UserRoles.AddAsync(userRole);
                    processedCount++;
                    results.Add($"Created UserRole for {user.Email} with role {roleName}");
                }

                // Save all changes
                if (processedCount > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    success = true,
                    message = $"Migration completed: {processedCount} created, {skippedCount} skipped",
                    processed = processedCount,
                    skipped = skippedCount,
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Migration failed",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Migration endpoint: Populate Code field for existing Module records
        /// GET: api/Account/migrate-module-codes
        /// </summary>
        [AllowAnonymous]
        [HttpGet("migrate-module-codes")]
        public async Task<ActionResult> MigrateModuleCodes()
        {
            try
            {
                var moduleCodes = new Dictionary<string, string>
                {
                    { "Dashboard", "DASHBOARD" },
                    { "Stock to Buy", "STOCK_TO_BUY" },
                    { "Stocks", "STOCKS" },
                    { "Customer", "CUSTOMER" },
                    { "Vehicle", "VEHICLE" },
                    { "Advertisement", "ADVERTISEMENT" },
                    { "Master Data", "MASTER_DATA" },
                    { "MasterData", "MASTER_DATA" },
                    { "Report", "REPORT" },
                    { "Reports", "REPORT" },
                    { "Administration", "ADMINISTRATION" },
                    { "System", "SYSTEM" },
                    { "Letters", "LETTERS" }
                };

                var subModuleCodes = new Dictionary<string, string>
                {
                    // Stocks submodules
                    { "Stock Info", "STOCK_INFO" },
                    { "Vehicle Info", "VEHICLE_INFO" },
                    { "Purchase", "PURCHASE" },
                    { "Import", "IMPORT" },
                    { "Clearance", "CLEARANCE" },
                    { "Sale", "SALE" },
                    { "Pricing", "PRICING" },
                    { "Arrival Checklist", "ARRIVAL_CHECKLIST" },
                    { "Registration", "REGISTRATION" },
                    { "Expenses", "EXPENSES" },
                    { "Administrative Cost", "ADMINISTRATIVE_COST" },
                    { "Disbursement", "DISBURSEMENT" },
                    // Master Data submodules
                    { "Suppliers", "SUPPLIERS" },
                    { "Forwarders", "FORWARDERS" },
                    { "Banks", "BANKS" },
                    { "Brands", "BRANDS" },
                    { "Models", "MODELS" },
                    { "Showroom", "SHOWROOM" },
                    // Report submodules
                    { "Sales Monthly", "SALES_MONTHLY" },
                    { "Sales Yearly", "SALES_YEARLY" },
                    { "Sale by Monthly", "SALE_BY_MONTHLY" },
                    // Administration submodules
                    { "Roles", "ROLES" },
                    { "User", "USER" },
                    { "Users", "USER" },
                    { "Company", "COMPANY" },
                    { "Sub Companies", "SUB_COMPANIES" }
                };

                var results = new List<string>();
                var moduleUpdated = 0;
                var moduleSkipped = 0;
                var subModuleUpdated = 0;
                var subModuleSkipped = 0;

                // Update Modules
                var modules = await _context.Modules.ToListAsync();
                foreach (var module in modules)
                {
                    if (string.IsNullOrEmpty(module.Code) && moduleCodes.ContainsKey(module.Name))
                    {
                        module.Code = moduleCodes[module.Name];
                        moduleUpdated++;
                        results.Add($"Updated Module '{module.Name}' with code '{module.Code}'");
                    }
                    else if (!string.IsNullOrEmpty(module.Code))
                    {
                        moduleSkipped++;
                        results.Add($"Skipped Module '{module.Name}' - already has code '{module.Code}'");
                    }
                    else
                    {
                        results.Add($"Warning: No code mapping found for Module '{module.Name}'");
                    }
                }

                // Update SubModules
                var subModules = await _context.SubModules.ToListAsync();
                foreach (var subModule in subModules)
                {
                    if (string.IsNullOrEmpty(subModule.Code) && subModuleCodes.ContainsKey(subModule.Name))
                    {
                        subModule.Code = subModuleCodes[subModule.Name];
                        subModuleUpdated++;
                        results.Add($"Updated SubModule '{subModule.Name}' with code '{subModule.Code}'");
                    }
                    else if (!string.IsNullOrEmpty(subModule.Code))
                    {
                        subModuleSkipped++;
                        results.Add($"Skipped SubModule '{subModule.Name}' - already has code '{subModule.Code}'");
                    }
                    else
                    {
                        results.Add($"Warning: No code mapping found for SubModule '{subModule.Name}'");
                    }
                }

                // Save changes
                if (moduleUpdated > 0 || subModuleUpdated > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    success = true,
                    message = $"Migration completed: {moduleUpdated} modules + {subModuleUpdated} submodules updated",
                    modulesUpdated = moduleUpdated,
                    modulesSkipped = moduleSkipped,
                    subModulesUpdated = subModuleUpdated,
                    subModulesSkipped = subModuleSkipped,
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Migration failed",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Migration endpoint: Add missing Advertisement submodule to STOCKS
        /// GET: api/Account/add-advertisement-submodule
        /// </summary>
        [AllowAnonymous]
        [HttpGet("add-advertisement-submodule")]
        public async Task<ActionResult> AddAdvertisementSubmodule()
        {
            try
            {
                var results = new List<string>();

                // Find STOCKS module
                var stocksModule = await _context.Modules.FirstOrDefaultAsync(m => m.Code == "STOCKS");
                if (stocksModule == null)
                {
                    return BadRequest(new { success = false, message = "STOCKS module not found" });
                }

                // Check if Advertisement submodule already exists
                var existingSubModule = await _context.SubModules
                    .FirstOrDefaultAsync(sm => sm.Code == "ADVERTISEMENT" && sm.ModuleId == stocksModule.Id);

                if (existingSubModule != null)
                {
                    results.Add($"Advertisement submodule already exists with ID: {existingSubModule.Id}");
                    return Ok(new { success = true, message = "Advertisement submodule already exists", details = results });
                }

                // Create Advertisement submodule
                var advertisementSubModule = new SubModule
                {
                    Name = "Advertisement",
                    Code = "ADVERTISEMENT",
                    ModuleId = stocksModule.Id,
                    DisplayOrder = 13,
                    Icon = "cil-bullhorn",
                    CreatedBy = "System Migration"
                };

                await _context.SubModules.AddAsync(advertisementSubModule);
                await _context.SaveChangesAsync();

                results.Add($"Created Advertisement submodule with ID: {advertisementSubModule.Id}");

                // Optionally grant Advertisement role permission to this submodule
                var advertisementRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Advertisement");
                if (advertisementRole != null)
                {
                    var existingPermission = await _context.RoleSubModulePermissions
                        .FirstOrDefaultAsync(p => p.RoleId == advertisementRole.Id && p.SubModuleId == advertisementSubModule.Id);

                    if (existingPermission == null)
                    {
                        var permission = new RoleSubModulePermission
                        {
                            RoleId = advertisementRole.Id,
                            SubModuleId = advertisementSubModule.Id,
                            CanView = true,
                            CanAdd = true,
                            CanUpdate = true,
                            CanDelete = true,
                            CreatedBy = "System Migration"
                        };

                        await _context.RoleSubModulePermissions.AddAsync(permission);
                        await _context.SaveChangesAsync();

                        results.Add($"Granted Advertisement role full permission to Advertisement submodule");
                    }
                    else
                    {
                        results.Add($"Advertisement role already has permission to Advertisement submodule");
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = "Advertisement submodule added successfully",
                    subModuleId = advertisementSubModule.Id,
                    details = results
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Migration failed",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
