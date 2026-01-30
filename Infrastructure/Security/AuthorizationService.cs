using Application.Interfaces;
using Application.Services;
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
    /// Uses JSONB permissions stored in Role.Permissions column
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

            // Get the module from hardcoded list
            var module = ModulesProvider.GetModuleByCode(moduleCode);
            if (module == null)
                return false;

            // Get user's active roles with JSONB permissions
            var activeRoles = await GetUserActiveRolesWithPermissionsAsync(userId);
            if (!activeRoles.Any())
                return false;

            // Check if any role grants the permission (OR logic)
            foreach (var role in activeRoles)
            {
                var permissions = role.PermissionsObject;
                if (permissions == null || permissions.Modules == null)
                    continue;

                if (!permissions.Modules.ContainsKey(module.Name))
                    continue;

                var modulePerm = permissions.Modules[module.Name];

                bool hasPermission = permissionType switch
                {
                    PermissionType.View => modulePerm.CanView,
                    PermissionType.Add => modulePerm.CanAdd,
                    PermissionType.Update => modulePerm.CanUpdate,
                    PermissionType.Delete => modulePerm.CanDelete,
                    _ => false
                };

                if (hasPermission)
                    return true;
            }

            return false;
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

            // Get the submodule from hardcoded list
            var subModule = ModulesProvider.GetSubModuleByCode(subModuleCode);
            if (subModule == null)
                return false;

            // Find the parent module
            var allModules = ModulesProvider.GetModules();
            var parentModule = allModules.FirstOrDefault(m =>
                m.SubModules != null && m.SubModules.Any(sm => sm.Id == subModule.Id));

            if (parentModule == null)
                return false;

            // Get user's active roles with JSONB permissions
            var activeRoles = await GetUserActiveRolesWithPermissionsAsync(userId);
            if (!activeRoles.Any())
                return false;

            // Check if any role grants the permission (OR logic)
            foreach (var role in activeRoles)
            {
                var permissions = role.PermissionsObject;
                if (permissions == null || permissions.Modules == null)
                    continue;

                if (!permissions.Modules.ContainsKey(parentModule.Name))
                    continue;

                var modulePerm = permissions.Modules[parentModule.Name];
                if (modulePerm.SubModules == null || !modulePerm.SubModules.ContainsKey(subModule.Name))
                    continue;

                var subModulePerm = modulePerm.SubModules[subModule.Name];

                bool hasPermission = permissionType switch
                {
                    PermissionType.View => subModulePerm.CanView,
                    PermissionType.Add => subModulePerm.CanAdd,
                    PermissionType.Update => subModulePerm.CanUpdate,
                    PermissionType.Delete => subModulePerm.CanDelete,
                    _ => false
                };

                if (hasPermission)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get all modules accessible by user with aggregated permissions across all active roles
        /// </summary>
        public async Task<List<ModulePermissionDto>> GetUserModulesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new List<ModulePermissionDto>();

            // Get hardcoded modules
            var allModules = ModulesProvider.GetModules();

            // Check if user is Super Admin - grant all modules
            if (await IsSuperAdminAsync(userId))
            {
                return allModules
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
                    .ToList();
            }

            // Get user's active roles with JSONB permissions
            var activeRoles = await GetUserActiveRolesWithPermissionsAsync(userId);
            if (!activeRoles.Any())
                return new List<ModulePermissionDto>();

            // Aggregate permissions across all roles (OR logic)
            var modulePermissionsDict = new Dictionary<string, ModulePermissionDto>();

            foreach (var role in activeRoles)
            {
                var permissions = role.PermissionsObject;
                if (permissions == null || permissions.Modules == null)
                    continue;

                foreach (var moduleName in permissions.Modules.Keys)
                {
                    var modulePerm = permissions.Modules[moduleName];
                    var module = allModules.FirstOrDefault(m => m.Name == moduleName);

                    if (module == null)
                        continue;

                    if (!modulePermissionsDict.ContainsKey(moduleName))
                    {
                        modulePermissionsDict[moduleName] = new ModulePermissionDto
                        {
                            ModuleId = module.Id,
                            ModuleName = module.Name,
                            ModuleCode = module.Code,
                            DisplayOrder = module.DisplayOrder,
                            Icon = module.Icon,
                            CanView = modulePerm.CanView,
                            CanAdd = modulePerm.CanAdd,
                            CanUpdate = modulePerm.CanUpdate,
                            CanDelete = modulePerm.CanDelete
                        };
                    }
                    else
                    {
                        // OR logic: if any role grants permission, user has it
                        var existing = modulePermissionsDict[moduleName];
                        existing.CanView = existing.CanView || modulePerm.CanView;
                        existing.CanAdd = existing.CanAdd || modulePerm.CanAdd;
                        existing.CanUpdate = existing.CanUpdate || modulePerm.CanUpdate;
                        existing.CanDelete = existing.CanDelete || modulePerm.CanDelete;
                    }
                }
            }

            return modulePermissionsDict.Values
                .OrderBy(m => m.DisplayOrder)
                .ToList();
        }

        /// <summary>
        /// Get all submodules accessible by user for a specific module with aggregated permissions
        /// </summary>
        public async Task<List<SubModulePermissionDto>> GetUserSubModulesAsync(string userId, string moduleCode)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(moduleCode))
                return new List<SubModulePermissionDto>();

            // Get the module from hardcoded list
            var module = ModulesProvider.GetModuleByCode(moduleCode);
            if (module == null)
                return new List<SubModulePermissionDto>();

            // Check if user is Super Admin - grant all submodules
            if (await IsSuperAdminAsync(userId))
            {
                return (module.SubModules ?? new List<SubModule>())
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
                    .ToList();
            }

            // Get user's active roles with JSONB permissions
            var activeRoles = await GetUserActiveRolesWithPermissionsAsync(userId);
            if (!activeRoles.Any())
                return new List<SubModulePermissionDto>();

            // Aggregate submodule permissions across all roles (OR logic)
            var subModulePermissionsDict = new Dictionary<string, SubModulePermissionDto>();

            foreach (var role in activeRoles)
            {
                var permissions = role.PermissionsObject;
                if (permissions == null || permissions.Modules == null)
                    continue;

                if (!permissions.Modules.ContainsKey(module.Name))
                    continue;

                var modulePerm = permissions.Modules[module.Name];
                if (modulePerm.SubModules == null)
                    continue;

                foreach (var subModuleName in modulePerm.SubModules.Keys)
                {
                    var subModulePerm = modulePerm.SubModules[subModuleName];
                    var subModule = module.SubModules?.FirstOrDefault(sm => sm.Name == subModuleName);

                    if (subModule == null)
                        continue;

                    if (!subModulePermissionsDict.ContainsKey(subModuleName))
                    {
                        subModulePermissionsDict[subModuleName] = new SubModulePermissionDto
                        {
                            SubModuleId = subModule.Id,
                            SubModuleName = subModule.Name,
                            SubModuleCode = subModule.Code,
                            DisplayOrder = subModule.DisplayOrder,
                            Icon = subModule.Icon,
                            CanView = subModulePerm.CanView,
                            CanAdd = subModulePerm.CanAdd,
                            CanUpdate = subModulePerm.CanUpdate,
                            CanDelete = subModulePerm.CanDelete
                        };
                    }
                    else
                    {
                        // OR logic: if any role grants permission, user has it
                        var existing = subModulePermissionsDict[subModuleName];
                        existing.CanView = existing.CanView || subModulePerm.CanView;
                        existing.CanAdd = existing.CanAdd || subModulePerm.CanAdd;
                        existing.CanUpdate = existing.CanUpdate || subModulePerm.CanUpdate;
                        existing.CanDelete = existing.CanDelete || subModulePerm.CanDelete;
                    }
                }
            }

            return subModulePermissionsDict.Values
                .OrderBy(sm => sm.DisplayOrder)
                .ToList();
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
            return await GetUserActiveRolesWithPermissionsAsync(userId);
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

        /// <summary>
        /// Internal helper: Get user's active roles with permissions loaded
        /// </summary>
        private async Task<List<Role>> GetUserActiveRolesWithPermissionsAsync(string userId)
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
    }
}
