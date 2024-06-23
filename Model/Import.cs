using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    /// <summary>
    /// Apply BOL here
    /// </summary>
    public class Import : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }
        public string ShipName { get; set; }
        public DateTime EstimateDateOfDeparture { get; set; } = DateTime.UtcNow;
        public DateTime EstimateDateOfArrival { get; set; } = DateTime.UtcNow;
        //public string ClearanceAgent { get; set; }
        public DateTime DateOfBillOfLading { get; set; } = DateTime.UtcNow;

        [ForeignKey("ForwardingAgent")]
        public Guid? ForwardingAgentId { get; set; }
        public virtual ForwardingAgent ForwardingAgent { get; set; }

        //[ForeignKey("LetterOfUndertakingDocument")]
        //public Guid? LetterOfUndertakingDocumentId { get; set; }
        //public virtual LetterOfUndertakingDocument LetterOfUndertakingDocument { get; set; }

        //[ForeignKey("BillOfLandingDocument")]
        //public Guid? BillOfLandingDocumentId { get; set; }
        //public virtual BillOfLandingDocument BillOfLandingDocument { get; set; }


        public virtual ICollection<BillOfLandingDocument> BillOfLandingDocuments { get; set; }

        //public virtual ICollection<ApplicationDocument> DocumentList { get; set; }

    }


}
