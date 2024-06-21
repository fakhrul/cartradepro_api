using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class GeoFenceRuleByMapProfile : BaseModel
    {
        [ForeignKey("GeoFenceRuleByMap")]
        public Guid GeoFenceRuleByMapId { get; set; }

        public virtual GeoFenceRuleByMap GeoFenceRuleByMap { get; set; }

        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }

        public virtual Profile Profile { get; set; }


    }
}
