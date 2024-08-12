using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class SubModule : BaseModel
    {
        public string Name { get; set; }
        public ICollection<RoleSubModulePermission> RoleModulePermissions { get; set; }

        [ForeignKey("Module")]
        public Guid ModuleId { get; set; }
        public virtual Module Module { get; set; }
    }
}
