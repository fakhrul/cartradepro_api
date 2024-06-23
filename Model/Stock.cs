using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Stock : BaseModel
    {
        public string StockNo { get; set; }

        public virtual ICollection<Remarks> RemarksList { get; set; }

        [NotMapped]
        public StockStatusHistory LatestStockStatus
        {
            get
            {
                if (StockStatusHistories == null)
                    return null;
                if (StockStatusHistories.Count == 0)
                    return null;
                return StockStatusHistories.OrderByDescending(c=> c.DateTime).First();
            }
        }
        public virtual ICollection<StockStatusHistory> StockStatusHistories { get; set; }

        [ForeignKey("Vehicle")]
        public Guid VehicleId { get; set; }
        public virtual Vehicle Vehicle { get; set; }

        [ForeignKey("Purchase")]
        public Guid PurchaseId { get; set; }
        public virtual Purchase Purchase { get; set; }

        [ForeignKey("Import")]
        public Guid ImportId { get; set; }
        public virtual Import Import { get; set; }

        [ForeignKey("Clearance")]
        public Guid ClearanceId { get; set; }
        public virtual Clearance Clearance { get; set; }


    }
}
