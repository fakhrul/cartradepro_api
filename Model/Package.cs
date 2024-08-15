using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Package : BaseModel
    {
        //[ForeignKey("ServiceProvider")]
        //public Guid? ServiceProviderId { get; set; }
        //public virtual ServiceProvider ServiceProvider { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }

        public float CommisionAmount { get; set; }

        public string ConsumerType { get; set; }

        public string ApplicationType { get; set; }

        [ForeignKey("Category")]
        public Guid? CategoryId { get; set; }
        public virtual Category Category { get; set; }

    }
}
