using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Vehicle : BaseModel
    {
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public virtual Stock Stock { get; set; }

        /// <summary>
        /// Refer to Brand Object
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// Refer to Model Obj
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// Refer to Model Object
        /// </summary>
        public string ModelShortName { get; set; }
        public string ChasisNo { get; set; }
        public string EngineNo { get; set; }
        public string EngineCapacity { get; set; }
        /// <summary>
        /// Refer to VehicleType Object
        /// </summary>
        public string VehicleType { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Color { get; set; }

        public virtual ICollection<VehiclePhoto> VehiclePhotoList { get; set; }

    }

}
