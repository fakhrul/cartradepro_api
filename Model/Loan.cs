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
        public float DepositAmount { get; set; }

        public float UnpaidBalanceAmount { get; set; }

        public float RequestedLoanAmount { get; set; }

        public float ApprovedLoanAmount { get; set; }

        public string LoanTenure { get; set; }

        public virtual ICollection<LetterOfUndertakingDocument> LetterOfUndertakingDocuments { get; set; }

    }



}
