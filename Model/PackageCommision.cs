using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class PackageCommision : BaseModel
    {
        [ForeignKey("Package")]
        public Guid PackageId { get; set; }
        public virtual Package Package { get; set; }
        public float Amount { get; set; }
        public DateTime StartTime { get; set; }
        public int Level { get; set; }

        public bool IsEnable { get; set; }
    }
}
