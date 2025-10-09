using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class CustomerIcDocument : BaseModel
    {
        [ForeignKey("Sale")]
        public Guid SaleId { get; set; }
        public virtual Sale Sale { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
