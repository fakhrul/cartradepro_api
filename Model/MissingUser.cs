using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class MissingUser : BaseModel
    {
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        [ForeignKey("Schedule")]
        public Guid ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; }

        public DateTime DateTime { get; set; }

    }
}
