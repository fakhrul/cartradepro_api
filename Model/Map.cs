using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Map : BaseModel
    {


        public string Name { get; set; }
        public string Code { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [ForeignKey("DocumentMap")]
        public Guid? DocumentMapId { get; set; }

        public virtual Document DocumentMap { get; set; }

        public int Count { get; set; }
        public DateTime CountLastUpdated { get; set; }

        //public virtual ICollection<Location> LocationList { get; set; }

    }
}
