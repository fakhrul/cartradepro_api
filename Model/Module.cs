using System.Collections.Generic;

namespace SPOT_API.Models
{
    public class Module : BaseModel
    {
        public string Name { get; set; }
        //public ICollection<RoleModulePermission> RoleModulePermissions { get; set; }

        public ICollection<SubModule> SubModules { get; set; }

    }
}
