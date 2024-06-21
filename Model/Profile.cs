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
