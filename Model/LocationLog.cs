using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class LocationLog : BaseModel
    {
        [ForeignKey("Device")]
        public Guid DeviceId { get; set; }

        public virtual Device Device { get; set; }

        public string PrevLocation { get; set; }
        /// <summary>
        /// either GPS, or Room
        /// </summary>
        public string PrevLocationType { get; set; }

        public DateTime PrevLocationUpdateOn { get; set; }

        public string LastKnownLocation { get; set; }
        /// <summary>
        /// either GPS, or Room
        /// </summary>
        public string LastKnownLocationType { get; set; }

        public DateTime LastKnownLocationUpdateOn { get; set; }

        public string Alarm { get; set; }

        public string Battery { get; set; }

        public string FirmwareVersion { get; set; }

        public string HardwareVersion { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string WiFiRssi { get; set; }
        public string BleRssi { get; set; }

    }
}
