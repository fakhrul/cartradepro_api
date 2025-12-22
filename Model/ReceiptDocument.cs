using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SPOT_API.Models
{
    public class ReceiptDocument : BaseModel
    {
        [ForeignKey("Sale")]
        public Guid SaleId { get; set; }
        [JsonIgnore]
        public virtual Sale Sale { get; set; }

        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        public virtual Document Document { get; set; }
    }
}
