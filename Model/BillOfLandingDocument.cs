using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class BillOfLandingDocument : BaseModel
    {
        [ForeignKey("Import")]
        public Guid ImportId { get; set; }
        public virtual Import Import { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
