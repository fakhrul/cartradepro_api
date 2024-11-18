using System.Collections.Generic;

namespace SPOT_API.Models
{
    public class SubCompany : BaseModel
    {
        public string Name { get; set; }
        public string RegNo { get; set; }
        public string TagLine { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string LogoUrl { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }

    }
}
