using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Pricing : BaseModel
    {
        public decimal RecommendedSalePrice { get; set; }
        public decimal MinimumSalePrice { get; set; }

    }



}
