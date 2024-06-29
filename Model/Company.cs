using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Company : BaseModel
    {
        public string Name { get; set; }
        public string RegNo { get; set; }
        public string TagLine { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string LogoUrl { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNo { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }

    }
}
