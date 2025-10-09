using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Registration : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }

        public string RACNo { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public DateTime VehicleRegistrationDate { get; set; } = DateTime.Now.ToUniversalTime();

        public virtual ICollection<JpjEHakMilikDocument> JpjEHakMilikDocuments { get; set; }
        public virtual ICollection<JpjEDaftarDocument> JpjEDaftarDocuments { get; set; }
        public virtual ICollection<PuspakomB2SlipDocument> PuspakomB2SlipDocuments { get; set; }
        public virtual ICollection<PuspakomB7SlipDocument> PuspakomB7SlipDocuments { get; set; }

        public virtual ICollection<JpjGeranDocument> JpjGeranDocuments { get; set; }

        public virtual ICollection<InsuranceDocument> InsuranceDocuments { get; set; }
        public virtual ICollection<RoadTaxDocument> RoadTaxDocuments { get; set; }
        public virtual ICollection<ReceiptEDaftarDocument> ReceiptEDaftarDocuments { get; set; }
        public virtual ICollection<ReceiptKastamDocument> ReceiptKastamDocuments { get; set; }


        //[ForeignKey("JpjEHakMilikDocument")]
        //public Guid? JpjEHakMilikDocumentId { get; set; }
        //public virtual JpjEHakMilikDocument JpjEHakMilikDocument { get; set; }

        //[ForeignKey("JpjEDaftarDocument")]
        //public Guid? JpjEDaftarDocumentId { get; set; }
        //public virtual JpjEDaftarDocument JpjEDaftarDocument { get; set; }

        //[ForeignKey("PuspakomB2SlipDocument")]
        //public Guid? PuspakomB2SlipDocumentId { get; set; }
        //public virtual PuspakomB2SlipDocument PuspakomB2SlipDocument  { get; set; }

        //[ForeignKey("PuspakomB7SlipDocument")]
        //public Guid? PuspakomB7SlipDocumentId { get; set; }
        //public virtual PuspakomB7SlipDocument PuspakomB7SlipDocument { get; set; }


    }



}
