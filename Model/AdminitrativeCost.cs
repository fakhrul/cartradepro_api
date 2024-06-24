using System.Collections.Generic;

namespace SPOT_API.Models
{
    public class AdminitrativeCost : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }
        //public string Name { get; set; }
        //public string Remark { get; set; }
        //public decimal Amount { get; set; }
        public virtual ICollection<AdminitrativeCostItem> AdminitrativeCostItems { get; set; }

    }


}
