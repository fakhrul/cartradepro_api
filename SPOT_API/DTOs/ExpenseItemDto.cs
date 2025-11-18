using System;

namespace SPOT_API.DTOs
{
    public class ExpenseItemDto
    {
        public DateTime ExpenseDate { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string BillOrInvoiceNo { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public Guid? DocumentId { get; set; }
    }
}
