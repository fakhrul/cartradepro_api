using System.Collections.Generic;

namespace SPOT_API.Models
{
    /// <summary>
    /// Represents a module's permission settings
    /// </summary>
    public class ModulePermission
    {
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }

        /// <summary>
        /// Sub-module permissions (key: submodule name, value: permissions)
        /// </summary>
        public Dictionary<string, SubModulePermission> SubModules { get; set; }

        public ModulePermission()
        {
            SubModules = new Dictionary<string, SubModulePermission>();
        }
    }

    /// <summary>
    /// Represents a sub-module's permission settings
    /// </summary>
    public class SubModulePermission
    {
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
    }

    /// <summary>
    /// Root permissions object containing all module permissions for a role
    /// Key: Module name (e.g., "Dashboard", "Stocks", "Customer")
    /// Value: Module permission settings
    /// </summary>
    public class RolePermissions
    {
        public Dictionary<string, ModulePermission> Modules { get; set; }

        public RolePermissions()
        {
            Modules = new Dictionary<string, ModulePermission>();
        }
    }
}
