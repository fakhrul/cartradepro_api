using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    /// <summary>
    /// SPOT Tag and Mobile App are Listed Here
    /// </summary>
    public class Device : BaseModel
    {
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }

        [ForeignKey("DeviceType")]
        public Guid DeviceTypeId { get; set; }
        public DeviceType DeviceType { get; set; }

        public string Code{ get; set; }
        public string Name { get; set; }

        public DateTime LastDetected { get; set; }

        public bool IsEnable { get; set; }

        public string MacAddress { get; set; }

        //[ForeignKey("Profile")]
        public Guid? ProfileId { get; set; }
        //public virtual Device Profile { get; set; }

        public virtual LocationLog LocationLog { get; set; }
    }
}
