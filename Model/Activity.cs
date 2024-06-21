using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Activity : BaseModel
    {
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        [ForeignKey("Area")]
        public Guid AreaId { get; set; }

        public virtual Area Area { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public bool IsEvaluated { get; set; }


    }
}
