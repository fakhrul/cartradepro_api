using System.Collections.Generic;

namespace SPOT_API.Models
{
    public class Module : BaseModel
    {
        public string Name { get; set; }

        /// <summary>
        /// Unique code for the module (e.g., "STOCKS", "CUSTOMER", "DASHBOARD")
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Display order in navigation menu
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Icon class (e.g., CoreUI/FontAwesome icons)
        /// </summary>
        public string Icon { get; set; }

        public ICollection<RoleModulePermission> RoleModulePermissions { get; set; }
        public ICollection<SubModule> SubModules { get; set; }

    }
}
