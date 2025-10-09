using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Clearance : BaseModel
    {

        //[ForeignKey("ForwardingAgent")]
        //public Guid? ForwardingAgentId { get; set; }
        //public virtual ForwardingAgent ForwardingAgent { get; set; }

        public string ApprovedPermitNo { get; set; }
        public string K8DocumentNo { get; set; }
        public string K1DocumentNo { get; set; }


        public virtual ICollection<K8Document> K8Documents { get; set; }
        public virtual ICollection<K1Document> K1Documents { get; set; }
        public virtual ICollection<ExportCertificateDocument> ExportCertificateDocuments { get; set; }

    }



}
