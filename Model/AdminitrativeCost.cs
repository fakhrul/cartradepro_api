using System.Collections.Generic;

namespace SPOT_API.Models
{
    public class AdminitrativeCost : BaseModel
    {
        public decimal ApCostAmount { get; set; }
        public decimal DutyCostAmount { get; set; }
        public decimal ForwardingCostAmount { get; set; }
        //public decimal ServiceEzCareCostAmount { get; set; }
        //public decimal InteriorCostAmount { get; set; }
        //public decimal PaintCostAmount { get; set; }
        //public decimal TyreCostAmount { get; set; }
        //public decimal RentalCostAmount { get; set; }
        public decimal IntFsCostAmount { get; set; }
        public decimal InsuranceAmount { get; set; }
        public decimal PuspakomRegRoadTax { get; set; }
        public decimal PuspakomAmount { get; set; }
        public decimal TransferFeeAmount { get; set; }
        public decimal RegistrationFeeAmount { get; set; }
        public decimal RoadtaxAmount { get; set; }
        
            
        public virtual ICollection<AdminitrativeCostItem> AdminitrativeCostItems { get; set; }

    }


}
