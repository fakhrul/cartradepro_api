using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Category : BaseModel
    {
        //[ForeignKey("ServiceProvider")]
        //public Guid ServiceProviderId { get; set; }
        //public virtual ServiceProvider ServiceProvider { get; set; }
        public string Name { get; set; }
    }
}
