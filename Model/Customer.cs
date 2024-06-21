using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class Customer : BaseModel
    {
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }

        public string CustomerType { get; set; }
        public string Name { get; set; }
        public string IcNumber { get; set; }

        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }


    }


}
