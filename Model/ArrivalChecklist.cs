using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class ArrivalChecklist : BaseModel
    {
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public virtual Stock Stock { get; set; }
        public string Name { get; set; }

        public bool IsAvailable { get; set; }

        public string Remarks { get; set; }


    }



}
