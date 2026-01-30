using System;
using System.Collections.Generic;
using System.Linq;
using SPOT_API.Models;

namespace SPOT_API.DTOs
{
    /// <summary>
    /// DTO for creating/updating roles with the old permission format
    /// </summary>
    public class RoleDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Old format: module-level permissions
        /// </summary>
        public List<RoleModulePermissionDto> RoleModulePermissions { get; set; }

        /// <summary>
        /// Old format: submodule-level permissions
        /// </summary>
        public List<RoleSubModulePermissionDto> RoleSubModulePermissions { get; set; }

        public RoleDto()
        {
            RoleModulePermissions = new List<RoleModulePermissionDto>();
            RoleSubModulePermissions = new List<RoleSubModulePermissionDto>();
        }

        /// <summary>
        /// Converts the DTO to a Role entity with JSONB Permissions
        /// </summary>
        public Role ToRole(Dictionary<Guid, string> moduleIdToName, Dictionary<Guid, string> subModuleIdToName)
        {
            var role = new Role
            {
                Id = Id ?? Guid.NewGuid(),
                Name = Name,
                DisplayName = DisplayName,
                Description = Description,
                IsSystemRole = IsSystemRole
            };

            // Build the new Permissions JSON structure
            var permissions = new RolePermissions();

            // Group module permissions by module ID
            foreach (var modulePerm in RoleModulePermissions ?? new List<RoleModulePermissionDto>())
            {
                if (moduleIdToName.TryGetValue(modulePerm.ModuleId, out var moduleName))
                {
                    var modulePermission = new ModulePermission
                    {
                        CanAdd = modulePerm.CanAdd,
                        CanUpdate = modulePerm.CanUpdate,
                        CanDelete = modulePerm.CanDelete,
                        CanView = modulePerm.CanView
                    };

                    // Add submodule permissions for this module
                    var subModulePerms = RoleSubModulePermissions?
                        .Where(sp => {
                            // Find the submodule name and check if it belongs to this module
                            if (subModuleIdToName.TryGetValue(sp.SubModuleId, out var subModName))
                            {
                                // We need to check if this submodule belongs to the current module
                                // For now, we'll add all submodules (will be filtered by module structure)
                                return true;
                            }
                            return false;
                        })
                        .ToList() ?? new List<RoleSubModulePermissionDto>();

                    foreach (var subModPerm in subModulePerms)
                    {
                        if (subModuleIdToName.TryGetValue(subModPerm.SubModuleId, out var subModuleName))
                        {
                            modulePermission.SubModules[subModuleName] = new SubModulePermission
                            {
                                CanAdd = subModPerm.CanAdd,
                                CanUpdate = subModPerm.CanUpdate,
                                CanDelete = subModPerm.CanDelete,
                                CanView = subModPerm.CanView
                            };
                        }
                    }

                    permissions.Modules[moduleName] = modulePermission;
                }
            }

            role.PermissionsObject = permissions;
            return role;
        }
    }

    public class RoleModulePermissionDto
    {
        public Guid ModuleId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
    }

    public class RoleSubModulePermissionDto
    {
        public Guid SubModuleId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
    }

    /// <summary>
    /// Response DTO for GET /api/Roles/{id} endpoint
    /// Includes nested module and submodule objects for frontend compatibility
    /// </summary>
    public class RoleResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsSystemRole { get; set; }

        public List<RoleModulePermissionResponseDto> RoleModulePermissions { get; set; }
        public List<RoleSubModulePermissionResponseDto> RoleSubModulePermissions { get; set; }

        public RoleResponseDto()
        {
            RoleModulePermissions = new List<RoleModulePermissionResponseDto>();
            RoleSubModulePermissions = new List<RoleSubModulePermissionResponseDto>();
        }

        /// <summary>
        /// Converts a Role entity with JSONB Permissions to RoleResponseDto with the old format
        /// </summary>
        public static RoleResponseDto FromRole(Role role, List<Module> modules)
        {
            var response = new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                DisplayName = role.DisplayName,
                Description = role.Description,
                IsSystemRole = role.IsSystemRole
            };

            // Parse the JSONB Permissions
            var permissions = role.PermissionsObject;
            if (permissions == null || permissions.Modules == null)
                return response;

            // Build module ID lookup
            var modulesByName = modules.ToDictionary(m => m.Name, m => m);

            // Convert module permissions
            foreach (var moduleName in permissions.Modules.Keys)
            {
                if (!modulesByName.TryGetValue(moduleName, out var module))
                    continue;

                var modulePerm = permissions.Modules[moduleName];

                // Create ModuleDto with all its submodules
                var moduleDto = new ModuleDto
                {
                    Id = module.Id,
                    Name = module.Name,
                    Code = module.Code,
                    DisplayOrder = module.DisplayOrder,
                    SubModules = module.SubModules?.Select(sm => new SubModuleDto
                    {
                        Id = sm.Id,
                        Name = sm.Name,
                        Code = sm.Code,
                        DisplayOrder = sm.DisplayOrder
                    }).ToList() ?? new List<SubModuleDto>()
                };

                // Add module permission
                response.RoleModulePermissions.Add(new RoleModulePermissionResponseDto
                {
                    ModuleId = module.Id,
                    CanAdd = modulePerm.CanAdd,
                    CanUpdate = modulePerm.CanUpdate,
                    CanDelete = modulePerm.CanDelete,
                    CanView = modulePerm.CanView,
                    Module = moduleDto
                });

                // Convert submodule permissions
                if (modulePerm.SubModules != null)
                {
                    foreach (var subModuleName in modulePerm.SubModules.Keys)
                    {
                        var subModule = module.SubModules?.FirstOrDefault(sm => sm.Name == subModuleName);
                        if (subModule == null)
                            continue;

                        var subModulePerm = modulePerm.SubModules[subModuleName];

                        response.RoleSubModulePermissions.Add(new RoleSubModulePermissionResponseDto
                        {
                            SubModuleId = subModule.Id,
                            CanAdd = subModulePerm.CanAdd,
                            CanUpdate = subModulePerm.CanUpdate,
                            CanDelete = subModulePerm.CanDelete,
                            CanView = subModulePerm.CanView,
                            SubModule = new SubModuleDto
                            {
                                Id = subModule.Id,
                                Name = subModule.Name,
                                Code = subModule.Code,
                                DisplayOrder = subModule.DisplayOrder
                            }
                        });
                    }
                }
            }

            return response;
        }
    }

    public class RoleModulePermissionResponseDto
    {
        public Guid ModuleId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
        public ModuleDto Module { get; set; }
    }

    public class RoleSubModulePermissionResponseDto
    {
        public Guid SubModuleId { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
        public SubModuleDto SubModule { get; set; }
    }

    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DisplayOrder { get; set; }
        public List<SubModuleDto> SubModules { get; set; }

        public ModuleDto()
        {
            SubModules = new List<SubModuleDto>();
        }
    }

    public class SubModuleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int DisplayOrder { get; set; }
    }
}
