using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class ReceivedCheckList : BaseModel
    {
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }

    }



}
