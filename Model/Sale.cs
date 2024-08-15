using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Sale : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }

        [ForeignKey("Customer")]
        public Guid? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("Loan")]
        public Guid? LoanId { get; set; }
        public virtual Loan Loan { get; set; }

        public DateTime SaleDateTime { get; set; } = DateTime.Now.ToUniversalTime();

        public float SaleAmount { get; set; }
        public float DepositAmount { get; set; }
        public float TradeInAmount { get; set; }

        public float EoeAmount { get; set; }
        
        public bool IsUseLoan { get; set; }


    }



}
