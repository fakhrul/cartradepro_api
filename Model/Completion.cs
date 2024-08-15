using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Completion : BaseModel
    {
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public virtual Stock Stock { get; set; }

        public DateTime DeliveryDateTime { get; set; } = DateTime.Now.ToUniversalTime();

    }



}
