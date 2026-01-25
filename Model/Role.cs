using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public ICollection<RoleModulePermission> RoleModulePermissions { get; set; }
        public ICollection<RoleSubModulePermission> RoleSubModulePermissions { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

    }
}
