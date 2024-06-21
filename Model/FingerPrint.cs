using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    /// <summary>
    /// SPOT Tag and Mobile App are Listed Here
    /// </summary>
    public class FingerPrint : BaseModel
    {
        [ForeignKey("Tenant")]
        public Guid TenantId { get; set; }

        public virtual Tenant Tenant { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public virtual ICollection<FingerPrintDetail> FingerPrintDetailList { get; set; }

        [ForeignKey("DocumentCollectionSchedule")]
        public Guid? DocumentCollectionScheduleId { get; set; }

        public virtual Document DocumentCollectionSchedule { get; set; }

        [ForeignKey("DocumentTrainData")]
        public Guid? DocumentTrainDataId { get; set; }

        public virtual Document DocumentTrainData { get; set; }

        public bool IsTrainDataGenerated { get; set; }
        //public virtual Area Area { get; set; }

        //[ForeignKey("Device")]
        //public Guid DeviceId { get; set; }
        //public Device Device { get; set; }

        //public bool IsGoodAccuracy { get; set; }

        //public bool IsMLCompleted { get; set; }

        //public string RSSI { get; set; }

    }
}
