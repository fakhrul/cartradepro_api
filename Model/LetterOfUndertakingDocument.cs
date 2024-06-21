using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class LetterOfUndertakingDocument : BaseModel
    {
        [ForeignKey("Loan")]
        public Guid LoanId { get; set; }
        public virtual Loan Loan { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
