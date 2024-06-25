using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPOT_API.Models
{
    public class ApplicationForm : BaseModel
    {

        //[ForeignKey("ServiceProvider")]
        //public Guid? ServiceProviderId { get; set; }
        //public virtual ServiceProvider ServiceProvider { get; set; }

        [ForeignKey("Package")]
        public Guid? PackageId { get; set; }
        public virtual Package Package { get; set; }
        public string Status { get; set; }

        [ForeignKey("CreatedBy")]
        public Guid? CreatedById { get; set; }
        public virtual Profile CreatedBy { get; set; }

        public bool IsOwnApplication { get; set; }


        [ForeignKey("Agent")]
        public Guid? AgentId { get; set; }
        public virtual Profile Agent { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderIndex { get; set; }

        [NotMapped]
        public string OrderNo
        {
            get
            {
                return this.OrderIndex.ToString("D6");
            }
        }

        public string ProviderOrderNo { get; set; }

        public string CompanyName { get; set; }
        public string CompanyRegNo { get; set; }


        public string CustomerName { get; set; }
        public string MyKadOrPassport { get; set; }
        public string ResidentialType { get; set; }
        public string ResidentialName { get; set; }

        public DateTime AppointmentDate { get; set; } = DateTime.MinValue.ToUniversalTime();

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }

        public string Postcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime SubmittedDate { get; set; } = DateTime.Now.ToUniversalTime();
        public string LatestStatus { get; set; }
        public string Remarks { get; set; }

        public virtual ICollection<ApplicationDocument> ApplicationDocumentList { get; set; }
        public virtual ICollection<Remarks> RemarksList { get; set; }

        public bool IsPaid { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.MinValue.ToUniversalTime();
        public string InternalStatus { get; set; }
        public string ProviderStatus { get; set; }

        public string ConsumerType { get; set; }

        public string ApplicationType { get; set; }

        public string FullAddress { get; set; }

        public string PreferredId1 { get; set; }
        public string PreferredId2 { get; set; }
        public string PreferredId3 { get; set; }
        public string ProjectName { get; set; }


    }
}
