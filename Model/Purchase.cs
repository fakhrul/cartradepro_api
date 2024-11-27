using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Purchase : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }

        [ForeignKey("Supplier")]
        public Guid? SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string SupplierCurrency { get; set; }
        public decimal VehiclePriceSupplierCurrency { get; set; }
        public decimal VehiclePriceLocalCurrency { get; set; }

        public decimal BodyPriceLocalCurrency { get; set; }

        //public float ApAmount { get; set; }

    }


}
