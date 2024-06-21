using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class GeoFenceNotification : BaseModel
    {
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        [ForeignKey("GeoFenceRuleByArea")]
        public Guid? GeoFenceRuleByAreaId { get; set; }
        public virtual GeoFenceRuleByArea GeoFenceRuleByArea { get; set; }

        [ForeignKey("GeoFenceRuleByMap")]
        public Guid? GeoFenceRuleByMapId { get; set; }
        public virtual GeoFenceRuleByMap GeoFenceRuleByMap { get; set; }

        public GeoFenceRuleType GeoFenceRuleType { get; set; }

        [NotMapped]
        public string GeoFenceRuleTypeInString
        {
            get
            {
                return GeoFenceRuleType.ToString();
            }
        }

        public DateTime DateTime { get; set; }
        public AlarmCode AlarmCode { get; set; }
       
        [NotMapped]
        public string AlarmCodeInString
        {
            get
            {
                return AlarmCode.ToString();
            }
        }

        public string Description { get; set; }
    }

    public enum GeoFenceRuleType
    {
        Area,
        Map,
    }

    public enum AlarmCode { 
        DetectInsideArea,
        DetectOutsideArea,
        DetectInsideMap,
        DetectOutsideMap,

    }

}
