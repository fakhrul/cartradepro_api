using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class FingerPrintDetail : BaseModel
    {
        [ForeignKey("FingerPrint")]
        public Guid FingerPrintId { get; set; }

        public virtual FingerPrint FingerPrint { get; set; }

        [ForeignKey("Area")]
        public Guid AreaId { get; set; }
        public virtual Area Area { get; set; }
        public string AreaCode { get; set; }
        [ForeignKey("Device")]
        public Guid DeviceId { get; set; }
        public virtual Device Device { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceMacAddress { get; set; }
        public string Code { get; set; }

        public int RSSI { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public bool IsGoodAccuracy { get; set; }

        public bool IsMLCompleted { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


        //public string RSSI { get; set; }

    }
}
