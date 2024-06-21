using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class GeoFenceRuleByAreaProfile : BaseModel
    {
        [ForeignKey("GeoFenceRuleByArea")]
        public Guid GeoFenceRuleByAreaId { get; set; }

        public virtual GeoFenceRuleByArea GeoFenceRuleByArea { get; set; }

        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }

        public virtual Profile Profile { get; set; }


    }
}
