using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class GeoFenceAreaItem : BaseModel
    {
        [ForeignKey("GeoFenceByArea")]
        public Guid GeoFenceByAreaId { get; set; }

        public virtual GeoFenceByArea GeoFenceByArea { get; set; }

        [ForeignKey("Area")]
        public Guid AreaId { get; set; }

        public virtual Area Area { get; set; }
    }
}
