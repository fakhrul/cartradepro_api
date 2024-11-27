using System.Collections.Generic;

namespace SPOT_API.Models
{
    public class Expense : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }
        //public string ExpenseType { get; set; }
        //public string Remark { get; set; }
        //public decimal Amount { get; set; }
        public decimal ServiceEzCareCostAmount { get; set; }
        public decimal InteriorCostAmount { get; set; }
        public decimal PaintCostAmount { get; set; }
        public decimal TyreCostAmount { get; set; }
        public decimal RentalCostAmount { get; set; }

        public virtual ICollection<ExpenseItem> Expenses { get; set; }

    }
}
