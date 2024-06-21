using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Loan : BaseModel
    {
        [ForeignKey("Sale")]
        public Guid SaleId { get; set; }
        public virtual Sale Sale { get; set; }

        public string BankName { get; set; }
        public string Deposit { get; set; }

        public string UnpaidBalance { get; set; }

        public string LoanAmount { get; set; }

        [ForeignKey("LetterOfUndertakingDocument")]
        public Guid? LetterOfUndertakingDocumentId { get; set; }
        public virtual LetterOfUndertakingDocument LetterOfUndertakingDocument { get; set; }

    }



}
