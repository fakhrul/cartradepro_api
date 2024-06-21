using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class SellingPricing : BaseModel
    {
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public virtual Stock Stock { get; set; }

        public string EstimatedCostPrice { get; set; }
        public string MinimumSalePrice { get; set; }
        public string RecommendedSalePrice { get; set; }
        //public string Description { get; set; }

    }



}
