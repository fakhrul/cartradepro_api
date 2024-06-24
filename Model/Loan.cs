using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Loan : BaseModel
    {
        [ForeignKey("Bank")]
        public Guid? BankId { get; set; }
        public virtual Bank Bank { get; set; }


        //public string BankName { get; set; }
        public decimal DepositAmount { get; set; }

        public decimal UnpaidBalanceAmount { get; set; }

        public decimal RequestedLoanAmount { get; set; }

        public decimal ApprovedLoanAmount { get; set; }

        public string LoanTenure { get; set; }

        public virtual ICollection<LetterOfUndertakingDocument> LetterOfUndertakingDocuments { get; set; }

    }



}
