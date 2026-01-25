using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    /// <summary>
    /// Implementation of authorization service for permission checks
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private readonly SpotDBContext _context;

        public AuthorizationService(SpotDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Check if user has permission to perform action on a module
        /// </summary>
        public async Task<bool> HasModulePermissionAsync(string userId, string moduleCode, PermissionType permissionType)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(moduleCode))
                return false;

            // Check if user is Super Admin
            if (await IsSuperAdminAsync(userId))
                return true;

            // Get the module
            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Code == moduleCode);

            if (module == null)
                return false;

            // Get user's active role IDs
            var activeRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Where(ur => !ur.EffectiveFrom.HasValue || ur.EffectiveFrom.Value <= DateTime.UtcNow)
                .Where(ur => !ur.EffectiveUntil.HasValue || ur.EffectiveUntil.Value >= DateTime.UtcNow)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!activeRoleIds.Any())
                return false;

            // Check module permissions for any of the user's active roles
            var permissions = await _context.RoleModulePermissions
                .Where(rmp => activeRoleIds.Contains(rmp.RoleId) && rmp.ModuleId == module.Id)
                .ToListAsync();

            bool hasPermission = false;
            if (permissionType == PermissionType.View)
                hasPermission = permissions.Any(rmp => rmp.CanView);
            else if (permissionType == PermissionType.Add)
                hasPermission = permissions.Any(rmp => rmp.CanAdd);
            else if (permissionType == PermissionType.Update)
                hasPermission = permissions.Any(rmp => rmp.CanUpdate);
            else if (permissionType == PermissionType.Delete)
                hasPermission = permissions.Any(rmp => rmp.CanDelete);

            return hasPermission;
        }

        /// <summary>
        /// Check if user has permission to perform action on a submodule
        /// </summary>
        public async Task<bool> HasSubModulePermissionAsync(string userId, string subModuleCode, PermissionType permissionType)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(subModuleCode))
                return false;

            // Check if user is Super Admin
            if (await IsSuperAdminAsync(userId))
                return true;

            // Get the submodule
            var subModule = await _context.SubModules
                .FirstOrDefaultAsync(sm => sm.Code == subModuleCode);

            if (subModule == null)
                return false;

            // Get user's active role IDs
            var activeRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Where(ur => !ur.EffectiveFrom.HasValue || ur.EffectiveFrom.Value <= DateTime.UtcNow)
                .Where(ur => !ur.EffectiveUntil.HasValue || ur.EffectiveUntil.Value >= DateTime.UtcNow)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!activeRoleIds.Any())
                return false;

            // Check submodule permissions for any of the user's active roles
            var permissions = await _context.RoleSubModulePermissions
                .Where(rsmp => activeRoleIds.Contains(rsmp.RoleId) && rsmp.SubModuleId == subModule.Id)
                .ToListAsync();

            bool hasPermission = false;
            if (permissionType == PermissionType.View)
                hasPermission = permissions.Any(rsmp => rsmp.CanView);
            else if (permissionType == PermissionType.Add)
                hasPermission = permissions.Any(rsmp => rsmp.CanAdd);
            else if (permissionType == PermissionType.Update)
                hasPermission = permissions.Any(rsmp => rsmp.CanUpdate);
            else if (permissionType == PermissionType.Delete)
                hasPermission = permissions.Any(rsmp => rsmp.CanDelete);

            return hasPermission;
        }

        /// <summary>
        /// Get all modules accessible by user
        /// </summary>
        public async Task<List<ModulePermissionDto>> GetUserModulesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<ModulePermissionDto>();

            // Check if user is Super Admin - grant all modules
            if (await IsSuperAdminAsync(userId))
            {
                return await _context.Modules
                    .OrderBy(m => m.DisplayOrder)
                    .Select(m => new ModulePermissionDto
                    {
                        ModuleId = m.Id,
                        ModuleName = m.Name,
                        ModuleCode = m.Code,
                        DisplayOrder = m.DisplayOrder,
                        Icon = m.Icon,
                        CanView = true,
                        CanAdd = true,
                        CanUpdate = true,
                        CanDelete = true
                    })
                    .ToListAsync();
            }

            // Get user's active role IDs
            var activeRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Where(ur => !ur.EffectiveFrom.HasValue || ur.EffectiveFrom.Value <= DateTime.UtcNow)
                .Where(ur => !ur.EffectiveUntil.HasValue || ur.EffectiveUntil.Value >= DateTime.UtcNow)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!activeRoleIds.Any())
                return new List<ModulePermissionDto>();

            // Get all module permissions for active roles, aggregated
            var modulePermissions = await _context.RoleModulePermissions
                .Where(rmp => activeRoleIds.Contains(rmp.RoleId))
                .Include(rmp => rmp.Module)
                .GroupBy(rmp => rmp.ModuleId)
                .Select(g => new ModulePermissionDto
                {
                    ModuleId = g.Key,
                    ModuleName = g.First().Module.Name,
                    ModuleCode = g.First().Module.Code,
                    DisplayOrder = g.First().Module.DisplayOrder,
                    Icon = g.First().Module.Icon,
                    CanView = g.Any(rmp => rmp.CanView),
                    CanAdd = g.Any(rmp => rmp.CanAdd),
                    CanUpdate = g.Any(rmp => rmp.CanUpdate),
                    CanDelete = g.Any(rmp => rmp.CanDelete)
                })
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();

            return modulePermissions;
        }

        /// <summary>
        /// Get all submodules accessible by user for a specific module
        /// </summary>
        public async Task<List<SubModulePermissionDto>> GetUserSubModulesAsync(string userId, string moduleCode)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(moduleCode))
                return new List<SubModulePermissionDto>();

            // Get the module
            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.Code == moduleCode);

            if (module == null)
                return new List<SubModulePermissionDto>();

            // Check if user is Super Admin - grant all submodules
            if (await IsSuperAdminAsync(userId))
            {
                return await _context.SubModules
                    .Where(sm => sm.ModuleId == module.Id)
                    .OrderBy(sm => sm.DisplayOrder)
                    .Select(sm => new SubModulePermissionDto
                    {
                        SubModuleId = sm.Id,
                        SubModuleName = sm.Name,
                        SubModuleCode = sm.Code,
                        DisplayOrder = sm.DisplayOrder,
                        Icon = sm.Icon,
                        CanView = true,
                        CanAdd = true,
                        CanUpdate = true,
                        CanDelete = true
                    })
                    .ToListAsync();
            }

            // Get user's active role IDs
            var activeRoleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Where(ur => !ur.EffectiveFrom.HasValue || ur.EffectiveFrom.Value <= DateTime.UtcNow)
                .Where(ur => !ur.EffectiveUntil.HasValue || ur.EffectiveUntil.Value >= DateTime.UtcNow)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!activeRoleIds.Any())
                return new List<SubModulePermissionDto>();

            // Get all submodule permissions for active roles and this module, aggregated
            var subModulePermissions = await _context.RoleSubModulePermissions
                .Where(rsmp => activeRoleIds.Contains(rsmp.RoleId))
                .Include(rsmp => rsmp.SubModule)
                .Where(rsmp => rsmp.SubModule.ModuleId == module.Id)
                .GroupBy(rsmp => rsmp.SubModuleId)
                .Select(g => new SubModulePermissionDto
                {
                    SubModuleId = g.Key,
                    SubModuleName = g.First().SubModule.Name,
                    SubModuleCode = g.First().SubModule.Code,
                    DisplayOrder = g.First().SubModule.DisplayOrder,
                    Icon = g.First().SubModule.Icon,
                    CanView = g.Any(rsmp => rsmp.CanView),
                    CanAdd = g.Any(rsmp => rsmp.CanAdd),
                    CanUpdate = g.Any(rsmp => rsmp.CanUpdate),
                    CanDelete = g.Any(rsmp => rsmp.CanDelete)
                })
                .OrderBy(sm => sm.DisplayOrder)
                .ToListAsync();

            return subModulePermissions;
        }

        /// <summary>
        /// Check if user has any active roles
        /// </summary>
        public async Task<bool> HasActiveRolesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Where(ur => !ur.EffectiveFrom.HasValue || ur.EffectiveFrom.Value <= DateTime.UtcNow)
                .Where(ur => !ur.EffectiveUntil.HasValue || ur.EffectiveUntil.Value >= DateTime.UtcNow)
                .AnyAsync();
        }

        /// <summary>
        /// Get all active roles for a user
        /// </summary>
        public async Task<List<Role>> GetUserActiveRolesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<Role>();

            var activeRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Where(ur => !ur.EffectiveFrom.HasValue || ur.EffectiveFrom.Value <= DateTime.UtcNow)
                .Where(ur => !ur.EffectiveUntil.HasValue || ur.EffectiveUntil.Value >= DateTime.UtcNow)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .Distinct()
                .ToListAsync();

            return activeRoles;
        }

        /// <summary>
        /// Check if user is Super Admin
        /// </summary>
        public async Task<bool> IsSuperAdminAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return false;

            // Check if user has SuperAdmin role and it's active
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .Where(ur => !ur.EffectiveFrom.HasValue || ur.EffectiveFrom.Value <= DateTime.UtcNow)
                .Where(ur => !ur.EffectiveUntil.HasValue || ur.EffectiveUntil.Value >= DateTime.UtcNow)
                .Include(ur => ur.Role)
                .AnyAsync(ur => ur.Role.Name == "SuperAdmin");
        }
    }
}
