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
        public virtual ICollection<ExpenseItem> Expenses { get; set; }

    }
}
