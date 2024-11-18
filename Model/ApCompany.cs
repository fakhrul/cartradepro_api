using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class ApCompany : BaseModel
    {
        [ForeignKey("SubCompany")]
        public Guid? SubCompanyId { get; set; }
        public virtual SubCompany SubCompany { get; set; }

        [ForeignKey("BankAccount")]
        public Guid? BankAccountId { get; set; }
        public virtual BankAccount BankAccount { get; set; }

    }


}
