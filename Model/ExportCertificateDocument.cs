using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class ExportCertificateDocument : BaseModel
    {
        [ForeignKey("Clearance")]
        public Guid ClearanceId { get; set; }
        public virtual Clearance Clearance { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
