using System;

namespace SPOT_API.Models
{
    public class BankAccount : BaseModel
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public string AccountName { get; set; }
        public string AccountNo { get; set; }
        public string AccountType { get; set; }

        // Foreign Key to SubCompany
        public Guid SubCompanyId { get; set; }
        public virtual SubCompany SubCompany { get; set; } // Navigation property


    }
}
