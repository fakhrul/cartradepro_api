using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class GeoFenceRuleByMap : BaseModel
    {
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }
        [ForeignKey("GeoFenceByMap")]
        public Guid GeoFenceByMapId { get; set; }

        public virtual GeoFenceByMap GeoFenceByMap { get; set; }

        public string Name { get; set; }

        public GeoFencePolicy GeoFencePolicy { get; set; }
        [NotMapped]
        public string GeoFencePolicyInString { get { return GeoFencePolicy.ToString(); } }

        public virtual ICollection<GeoFenceRuleByMapProfile> GeoFenceRuleByMapProfileList { get; set; }

        public bool IsEnable { get; set; }
    }

    //public enum GeoFencePolicy
    //{
    //    Inside,
    //    Outside
    //}
}
