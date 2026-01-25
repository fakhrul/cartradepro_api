using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class SubModule : BaseModel
    {
        public string Name { get; set; }

        /// <summary>
        /// Unique code for the submodule (e.g., "STOCK_INFO", "VEHICLE_INFO", "PURCHASE")
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Display order within the module
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Icon class (optional)
        /// </summary>
        public string Icon { get; set; }

        public ICollection<RoleSubModulePermission> RoleSubModulePermissions { get; set; }

        [ForeignKey("Module")]
        public Guid ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
