using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    /// <summary>
    /// Service for handling user authorization and permission checks
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Check if user has permission to perform action on a module
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="moduleCode">Module code (e.g., "STOCKS", "CUSTOMER")</param>
        /// <param name="permissionType">Permission type (View, Add, Update, Delete)</param>
        /// <returns>True if user has permission</returns>
        Task<bool> HasModulePermissionAsync(string userId, string moduleCode, PermissionType permissionType);

        /// <summary>
        /// Check if user has permission to perform action on a submodule
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="subModuleCode">Submodule code (e.g., "STOCK_INFO", "VEHICLE_INFO")</param>
        /// <param name="permissionType">Permission type (View, Add, Update, Delete)</param>
        /// <returns>True if user has permission</returns>
        Task<bool> HasSubModulePermissionAsync(string userId, string subModuleCode, PermissionType permissionType);

        /// <summary>
        /// Get all modules accessible by user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of modules with permissions</returns>
        Task<List<ModulePermissionDto>> GetUserModulesAsync(string userId);

        /// <summary>
        /// Get all submodules accessible by user for a specific module
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="moduleCode">Module code</param>
        /// <returns>List of submodules with permissions</returns>
        Task<List<SubModulePermissionDto>> GetUserSubModulesAsync(string userId, string moduleCode);

        /// <summary>
        /// Check if user has any active roles
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>True if user has at least one active role</returns>
        Task<bool> HasActiveRolesAsync(string userId);

        /// <summary>
        /// Get all active roles for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of active roles</returns>
        Task<List<Role>> GetUserActiveRolesAsync(string userId);

        /// <summary>
        /// Check if user is Super Admin
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>True if user is Super Admin</returns>
        Task<bool> IsSuperAdminAsync(string userId);
    }

    /// <summary>
    /// DTO for module permissions
    /// </summary>
    public class ModulePermissionDto
    {
        public Guid ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleCode { get; set; }
        public int DisplayOrder { get; set; }
        public string Icon { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    /// <summary>
    /// DTO for submodule permissions
    /// </summary>
    public class SubModulePermissionDto
    {
        public Guid SubModuleId { get; set; }
        public string SubModuleName { get; set; }
        public string SubModuleCode { get; set; }
        public int DisplayOrder { get; set; }
        public string Icon { get; set; }
        public bool CanView { get; set; }
        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}
