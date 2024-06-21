using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class MovementHistory : BaseModel
    {

        [ForeignKey("Profile")]
        public Guid ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        [ForeignKey("Device")]
        public Guid DeviceId { get; set; }

        public virtual Device Device { get; set; }

        [ForeignKey("Area")]
        public Guid? AreaId { get; set; }

        public virtual Area Area { get; set; }


        public string Location { get; set; }
        /// <summary>
        /// either GPS, or Room
        /// </summary>
        public string LocationType { get; set; }

        public DateTime DateTime { get; set; }

        public string Alarm { get; set; }

        public string Battery { get; set; }

        [NotMapped]
        public double Latitude
        {
            get;set;
        }

        [NotMapped]
        public double Longitude
        {
            get;set;
        }
        //public string FirmwareVersion { get; set; }

        //public string HardwareVersion { get; set; }

        //public string Latitude { get; set; }
        //public string Longitude { get; set; }
        //public string WiFiRssi { get; set; }
        //public string BleRssi { get; set; }

    }
}
