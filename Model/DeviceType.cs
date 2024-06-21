using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    /// <summary>
    /// SPOT Tag and Mobile App are Listed Here
    /// </summary>
    public class DeviceType : BaseModel
    {
        [ForeignKey("Tenant")]
        public Guid? TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        public string Code{ get; set; }
        public string Name { get; set; }
    }
}
