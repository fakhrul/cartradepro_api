using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Role : BaseModel
    {
        public string Name { get; set; }
        public ICollection<RoleModulePermission> RoleModulePermissions { get; set; }
        public ICollection<RoleSubModulePermission> RoleSubModulePermissions { get; set; }

    }
}
