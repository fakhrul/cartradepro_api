using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class ServiceProvider : BaseModel
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Category> PackageCategories { get; set; }

    }
}
