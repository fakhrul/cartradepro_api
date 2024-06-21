using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class EmergencyUser : BaseModel
    {
        [ForeignKey("Emergency")]
        public Guid EmergencyId { get; set; }

        public virtual Emergency Emergency { get; set; }

        [ForeignKey("Profile")]
        public Guid? ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public bool IsAvailable { get; set; }
    }
}
