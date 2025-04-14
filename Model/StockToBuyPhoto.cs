using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class StockToBuyPhoto : BaseModel
    {
        [ForeignKey("StockToBuy")]
        public Guid StockToBuyId { get; set; }
        public virtual StockToBuy StockToBuy { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }

        public int Position { get; set; }
    }
}
