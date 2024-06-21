using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Sale : BaseModel
    {
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public virtual Stock Stock { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("Loan")]
        public Guid? LoanId { get; set; }
        public virtual Loan Loan { get; set; }

        public string SaleAmount { get; set; }
        public string LoanTenure { get; set; }
        public string RequestedLoanAmount { get; set; }
        public string DepositAmount { get; set; }

    }



}
