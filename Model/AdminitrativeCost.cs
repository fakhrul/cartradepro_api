using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class AdminitrativeCost : BaseModel
    {
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public virtual Stock Stock { get; set; }
        public string AdminitrativeCostType { get; set; }
        public string Remark { get; set; }
        public decimal Amount { get; set; }

    }



}
