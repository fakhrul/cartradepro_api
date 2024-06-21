using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class RainFall : BaseModel
    {
        [ForeignKey("Station")]
        public Guid StationId { get; set; }

        public virtual Station Station { get; set; }

        public DateTime LogDateTime { get; set; }
        public float Today { get; set; }
        public float PrevDay { get; set; }
        public float Prev2Day { get; set; }
        public float Prev3Day { get; set; }
        public float Prev4Day { get; set; }
        public float Prev5Day { get; set; }
        public float Prev6Day { get; set; }
        public float Prev7Day { get; set; }
        public float PrevHour { get; set; }
        public float FromMidNight { get; set; }
        

    }
}
