using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Clearance : BaseModel
    {
        //[ForeignKey("Stock")]
        //public Guid StockId { get; set; }
        //public virtual Stock Stock { get; set; }

        public string ApprovedPermitNo { get; set; }
        public string K8DocumentNo { get; set; }
        public string K1DocumentNo { get; set; }

        //[ForeignKey("K8Document")]
        //public Guid? K8DocumentId { get; set; }
        //public virtual K8Document K8Document { get; set; }

        //[ForeignKey("K1Document")]
        //public Guid? K1DocumentId { get; set; }
        //public virtual K1Document K1Document { get; set; }
        public virtual ICollection<K8Document> K8Documentts { get; set; }
        public virtual ICollection<K1Document> K1Documents { get; set; }

    }



}
