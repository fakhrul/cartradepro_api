using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Role : BaseModel
    {
        /// <summary>
        /// System name of the role (e.g., "StockManager", "Sales", "SuperAdmin")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name for UI (e.g., "Stock Manager", "Salesman")
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Role description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// System-managed role that cannot be deleted or modified
        /// </summary>
        public bool IsSystemRole { get; set; } = false;

        /// <summary>
        /// JSON column storing all module and submodule permissions
        /// Stored as JSONB in PostgreSQL
        /// </summary>
        public string Permissions { get; set; }

        /// <summary>
        /// OBSOLETE: Legacy navigation property, kept for backwards compatibility during migration
        /// Use Permissions property instead
        /// </summary>
        [Obsolete("Use Permissions property instead")]
        public ICollection<RoleModulePermission> RoleModulePermissions { get; set; }

        /// <summary>
        /// OBSOLETE: Legacy navigation property, kept for backwards compatibility during migration
        /// Use Permissions property instead
        /// </summary>
        [Obsolete("Use Permissions property instead")]
        public ICollection<RoleSubModulePermission> RoleSubModulePermissions { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Helper property to get/set permissions as strongly-typed object
        /// Not mapped to database - uses Permissions JSON string
        /// </summary>
        [NotMapped]
        public RolePermissions PermissionsObject
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Permissions))
                    return new RolePermissions();

                try
                {
                    return JsonSerializer.Deserialize<RolePermissions>(Permissions);
                }
                catch
                {
                    return new RolePermissions();
                }
            }
            set
            {
                Permissions = value == null
                    ? null
                    : JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = false });
            }
        }
    }
}
