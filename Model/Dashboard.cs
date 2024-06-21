using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class Dashboard : BaseModel
    {

        /// <summary>
        /// Tenant Relationship
        /// </summary>
             public DateTime Date { get; set; }
        public int TotalAllUser { get; set; }
        public int TotalActiveUser { get; set; }
        public int TotalInActiveUser { get; set; }
        public int TotalHeadCount { get; set; }
        public int TotalMissingUser { get; set; }
        public int TotalPendingApproval { get; set; }
        public int TotalRegisteredStaff { get; set; }
        public int TotalRegisteredVisitor { get; set; }

    }
}
