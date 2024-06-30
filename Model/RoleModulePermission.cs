using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class RoleModulePermission : BaseModel
    {
        [ForeignKey("Role")]
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; }

        [ForeignKey("Module")]
        public Guid ModuleId { get; set; }
        public virtual Module Module { get; set; }

        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }

    }
}
