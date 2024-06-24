using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class AdminitrativeCostItem : BaseModel
    {
        [ForeignKey("AdminitrativeCost")]
        public Guid AdminitrativeCostId { get; set; }
        public virtual AdminitrativeCost AdminitrativeCost { get; set; }

        public string Name { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }


    }


}
