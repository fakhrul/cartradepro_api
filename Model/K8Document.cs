using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class K8Document : BaseModel
    {
        [ForeignKey("Clearance")]
        public Guid ClearanceId { get; set; }
        public virtual Clearance Clearance { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
