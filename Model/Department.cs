using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Department : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
