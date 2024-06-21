using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class BleLocation : BaseModel
    {
        /// <summary>
        /// Tenant Foreign Key
        /// </summary>
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        /// <summary>
        /// Tenant Relationship
        /// </summary>
        public virtual Tenant Tenant { get; set; }
        public string Signature { get; set; }
        public string FingerPrintCode { get; set; }

    }
}
