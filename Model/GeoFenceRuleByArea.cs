using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class GeoFenceRuleByArea : BaseModel
    {
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }
        [ForeignKey("GeoFenceByArea")]
        public Guid GeoFenceByAreaId { get; set; }

        public virtual GeoFenceByArea GeoFenceByArea { get; set; }

        public string Name { get; set; }

        public GeoFencePolicy GeoFencePolicy { get; set; }

        [NotMapped]
        public string GeoFencePolicyInString { get { return GeoFencePolicy.ToString(); } }
        

        public virtual ICollection<GeoFenceRuleByAreaProfile> GeoFenceRuleByAreaProfileList { get; set; }

        public bool IsEnable { get; set; }
    }

    public enum GeoFencePolicy
    {
        Inside,
        Outside
    }
}
