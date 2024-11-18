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

        [ForeignKey("Sale")]
        public Guid SaleId { get; set; }
        public virtual Sale Sale { get; set; }

        [ForeignKey("Registration")]
        public Guid RegistrationId { get; set; }
        public virtual Registration Registration { get; set; }
        
        //[ForeignKey("SellingPricing")]
        //public Guid SellingPricingId { get; set; }
        //public virtual SellingPricing SellingPricing { get; set; }


        [ForeignKey("Pricing")]
        public Guid PricingId { get; set; }
        public virtual Pricing Pricing { get; set; }


        [ForeignKey("ArrivalChecklist")]
        public Guid ArrivalChecklistId { get; set; }
        public virtual ArrivalChecklist ArrivalChecklist { get; set; }

        [ForeignKey("Expense")]
        public Guid ExpenseId { get; set; }
        public virtual Expense Expense { get; set; }

        [ForeignKey("AdminitrativeCost")]
        public Guid AdminitrativeCostId { get; set; }
        public virtual AdminitrativeCost AdminitrativeCost { get; set; }


        [ForeignKey("ApCompany")]
        public Guid? ApCompanyId { get; set; }
        public virtual ApCompany ApCompany { get; set; }

        //public string Location { get; set; }
        public string LocationCode { get; set; }

        public ArrivalState ArrivalState { get; set; } = ArrivalState.Incoming;

        [NotMapped]
        public string ArrivalStateDescription
        {
            get { return ArrivalState.ToString(); }
        }

        public bool IsOpen { get; set; } = false;
        public bool IsBooked { get; set; } = false;
        public bool IsLou { get; set; } = false;
        public bool IsSold { get; set; } = false;
        public bool IsCancelled { get; set; } = false;

    }


    public enum ArrivalState
    {
        Incoming,
        Received
    }


}
