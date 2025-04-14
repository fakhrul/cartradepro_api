using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class StockToBuy : BaseModel
    {
        public string StockNo { get; set; }
        public string Description { get; set; }

        public virtual ICollection<StockToBuyPhoto> VehiclePhotoList { get; set; }

        public string Color { get; set; }

        //[ForeignKey("ApCompany")]
        //public Guid? ApCompanyId { get; set; }
        //public virtual ApCompany ApCompany { get; set; }

        [ForeignKey("SubCompany")]
        public Guid? SubCompanyId { get; set; }
        public virtual SubCompany SubCompany { get; set; }

        [ForeignKey("Supplier")]
        public Guid? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string SupplierCurrency { get; set; }
        public decimal VehiclePriceSupplierCurrency { get; set; }
        public decimal VehiclePriceLocalCurrency { get; set; }

        public decimal BodyPriceLocalCurrency { get; set; }

        public string DefaultImageUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }


}
