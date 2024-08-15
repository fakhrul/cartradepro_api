using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class ExpenseItem : BaseModel
    {
        [ForeignKey("Expense")]
        public Guid ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }

        public string Name { get; set; }
        public string Remarks { get; set; }
        public float Amount { get; set; }


    }
}
