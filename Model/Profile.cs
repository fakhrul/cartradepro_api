using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    /// <summary>
    /// User Model
    /// </summary>
    public class Profile : BaseModel
    {
        public virtual AppUser AppUser { get; set; }

        [ForeignKey("Leader")]
        public Guid? LeaderId { get; set; }
        public virtual Profile Leader { get; set; }


        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(255)]
        public string Email { get; set; }
        public string Phone { get; set; }

        public string Role { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string MyKad { get; set; }

        public string PassportNo { get; set; }

        public bool IsEnable { get; set; }

        /// <summary>
        /// User's job title/position
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// User's office or showroom location
        /// </summary>
        public string OfficeLocation { get; set; }

        // Company Information
        [ForeignKey("SubCompany")]
        public Guid? SubCompanyId { get; set; }
        public virtual SubCompany SubCompany { get; set; }

        public string BankAccount { get; set; }
        public string TinNo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyBankAccount { get; set; }
        public string Address { get; set; }

        // Document References
        [ForeignKey("IcCopyDocument")]
        public Guid? IcCopyDocumentId { get; set; }
        public virtual Document IcCopyDocument { get; set; }

        [ForeignKey("PhotoDocument")]
        public Guid? PhotoDocumentId { get; set; }
        public virtual Document PhotoDocument { get; set; }

        public virtual ICollection<ProfilePackage> ProfilePackages { get; set; }

    }

    //public enum AdminRole
    //{
    //    /// <summary>
    //    /// 1. Manage Supervisor
    //    /// 2. Configure Tenant
    //    /// 3. All SuperVisor Ability
    //    /// </summary>
    //    Admin = 1,
    //    /// <summary>
    //    /// 1. Register Device to Tenant Location
    //    /// 2. Check In User to Tenant Location
    //    /// 3. Download Report
    //    /// </summary>
    //    Tenant = 2,
    //    Staff = 3
    //}



}
