using System.Collections.Generic;

namespace SPOT_API.Models
{
    public class AdminitrativeCost : BaseModel
    {
        public virtual ICollection<AdminitrativeCostItem> AdminitrativeCostItems { get; set; }

    }


}
