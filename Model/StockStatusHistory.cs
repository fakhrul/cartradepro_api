using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class StockStatusHistory : BaseModel
    {
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public virtual Stock Stock { get; set; }

        [ForeignKey("StockStatus")]
        public Guid StockStatusId { get; set; }
        public virtual StockStatus StockStatus { get; set; }

        //public string Name { get; set; }
        //public string Description { get; set; }

        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}
