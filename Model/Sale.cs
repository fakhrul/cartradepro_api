using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Sale : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }

        public string SalesmanName { get; set; }


        [ForeignKey("SalesManId")]
        public Guid? SalesManId { get; set; }
        public virtual Profile SalesMan { get; set; }


        [ForeignKey("Customer")]
        public Guid? CustomerId { get; set; }
        public virtual Customer Customer { get; set; }

        [ForeignKey("Loan")]
        public Guid? LoanId { get; set; }
        public virtual Loan Loan { get; set; }

        public DateTime SaleDateTime { get; set; } = DateTime.Now.ToUniversalTime();

        public decimal SaleAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal TradeInAmount { get; set; }

        public decimal EoeAmount { get; set; }
        public decimal HpAmount { get; set; }

        public bool IsUseLoan { get; set; }


        public decimal SalesmanCommisionAmount { get; set; }
        public decimal PromotionDiscountAmount{ get; set; }

    }



}
