using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Vehicle : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }

        /// <summary>
        /// Refer to Brand Object
        /// </summary>
        //public string Brand { get; set; }

        [ForeignKey("Brand")]
        public Guid? BrandId { get; set; }
        public virtual Brand Brand { get; set; }


        [ForeignKey("Model")]
        public Guid? ModelId { get; set; }
        public virtual Model Model { get; set; }

        /// <summary>
        /// Refer to Model Obj
        /// </summary>
        //public string Model { get; set; }
        /// <summary>
        /// Refer to Model Object
        /// </summary>
        //public string ModelShortName { get; set; }
        public string ChasisNo { get; set; }
        public string EngineNo { get; set; }
        public string EngineCapacity { get; set; }
        /// <summary>
        /// Refer to VehicleType Object
        /// </summary>
        //public string VehicleType { get; set; }
        [ForeignKey("VehicleType")]
        public Guid? VehicleTypeId { get; set; }
        public virtual VehicleType VehicleType { get; set; }

        public string Month { get; set; }
        public string Year { get; set; }
        public string Color { get; set; }

        public virtual ICollection<VehiclePhoto> VehiclePhotoList { get; set; }

        public string Description { get; set; }


    }

}
