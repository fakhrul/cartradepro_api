using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Pricing : BaseModel
    {
        public float RecommendedSalePrice { get; set; }
        public float MinimumSalePrice { get; set; }

    }



}
