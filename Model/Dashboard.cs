using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Dashboard : BaseModel
    {

        public DateTime Date { get; set; }
        [NotMapped]
        public int ThisYear { get; set; }
        [NotMapped]
        public int LastYear { get; set; }
        [NotMapped]
        public int TotalTotalStockSoldThisYear{ get; set; }
        [NotMapped]
        public int TotalTotalStockSoldLastYear { get; set; }
        [NotMapped]
        public int TotalStockAvailable { get; set; }
        [NotMapped]
        public int TotalStockInProcess { get; set; }
    }
}
