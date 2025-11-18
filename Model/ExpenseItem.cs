using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class ExpenseItem : BaseModel
    {
        [ForeignKey("Expense")]
        public Guid ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }

        public DateTime ExpenseDate { get; set; } = DateTime.Now.ToUniversalTime();
        public string Category { get; set; }
        public string Name { get; set; }
        public string BillOrInvoiceNo { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }

        [ForeignKey("Document")]
        public Guid? DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
