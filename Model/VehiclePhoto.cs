using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class VehiclePhoto : BaseModel
    {
        [ForeignKey("Vehicle")]
        public Guid VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }

        public int Position { get; set; }
    }
}
