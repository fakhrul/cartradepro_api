using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Emergency : BaseModel
    {
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("Area")]
        public Guid? AreaId { get; set; }
        public virtual Area Area { get; set; }

        [ForeignKey("GeoFenceByArea")]
        public Guid? GeoFenceByAreaId { get; set; }
        public virtual GeoFenceByArea GeoFenceByArea { get; set; }

        [ForeignKey("GeoFenceByMap")]
        public Guid? GeoFenceByMapId { get; set; }
        public virtual GeoFenceByMap GeoFenceByMap { get; set; }
        
        public bool IsGeoFenceByArea { get; set; }
        public virtual ICollection<EmergencyUser> EmergencyUserList { get; set; }
    }
}
