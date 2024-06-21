using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Performance: BaseModel
    {
        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        [ForeignKey("Schedule")]
        public Guid ScheduleId { get; set; }
        public virtual Schedule Schedule { get; set; }

        //public DateTime StartDateTime { get; set; }
        //public DateTime EndDateTime { get; set; }

        public double ActivityTimeInMinutes { get; set; }
        public double ScheduledTimeInMinutes { get; set; }

        public DateTime Date { get; set; }
        public double OverTimeBeginInMinutes { get;  set; }
        public double OverTimeEndInMinutes { get;  set; }
        public double UnderTimeBeginInMunites { get;  set; }
        public double UnderTimeEndInMinutes { get;  set; }
        public double NormalTimeInMinutes { get;  set; }
        public double UnderTimeMiddleInMinutes { get;  set; }
    }
}
