using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Location : BaseModel
    {

        /// <summary>
        /// Tenant Foreign Key
        /// </summary>
        [ForeignKey("Map")]
        public Guid MapId { get; set; }

        /// <summary>
        /// Tenant Relationship
        /// </summary>
        public virtual Map Map { get; set; }

        public string Name { get; set; }
        public string Code { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [ForeignKey("DocumentMap")]
        public Guid? DocumentMapId { get; set; }

        public virtual Document DocumentMap { get; set; }

        public int Count { get; set; }
        public DateTime CountLastUpdated { get; set; }

        //public virtual ICollection<Level> LevelList { get; set; }

    }
}
