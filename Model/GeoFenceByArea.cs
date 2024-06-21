using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class GeoFenceByArea : BaseModel
    {
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }

        public string Name { get; set; }

        public virtual ICollection<GeoFenceAreaItem> GeoFenceAreaItemList { get; set; }

        //public GeoFenceByArea()
        //{
        //    //if (GeoFenceAreaItemList == null)
        //    //    GeoFenceAreaItemList = new List<GeoFenceAreaItem>();
        //}
    }
}
