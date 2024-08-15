using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class ProfileCommision : BaseModel
    {
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        [ForeignKey("Package")]
        public Guid PackageId { get; set; }
        public virtual Package Package { get; set; }

        public float CommissionAmount { get; set; }

        public bool IsEnable { get; set; }

        //public string Code { get; set; }
        //public string Name { get; set; }


    }
}
