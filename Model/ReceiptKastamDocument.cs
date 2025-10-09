using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class ReceiptKastamDocument : BaseModel
    {
        [ForeignKey("Registration")]
        public Guid RegistrationId { get; set; }
        public virtual Registration Registration { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
