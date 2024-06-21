using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Level : BaseModel
    {

        /// <summary>
        /// Tenant Foreign Key
        /// </summary>
        [ForeignKey("Location")]
        public Guid LocationId { get; set; }

        /// <summary>
        /// Tenant Relationship
        /// </summary>
        public virtual Location Location { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [ForeignKey("DocumentMap")]
        public Guid? DocumentMapId { get; set; }

        public virtual Document DocumentMap { get; set; }
        //public virtual ICollection<Area> AreaList { get; set; }
        public int Count { get; set; }
        public DateTime CountLastUpdated { get; set; }

    }
}
