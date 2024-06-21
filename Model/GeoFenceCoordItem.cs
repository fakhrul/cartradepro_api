using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class GeoFenceCoordItem : BaseModel
    {
        [ForeignKey("GeoFenceByMap")]
        public Guid GeoFenceByMapId { get; set; }

        public virtual GeoFenceByMap GeoFenceByMap { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }

    }
}
