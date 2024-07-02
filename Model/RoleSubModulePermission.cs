using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class RoleSubModulePermission : BaseModel
    {
        [ForeignKey("Role")]
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; }

        [ForeignKey("SubModule")]
        public Guid SubModuleId { get; set; }
        public virtual SubModule SubModule { get; set; }

        public bool CanAdd { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }

    }
}
