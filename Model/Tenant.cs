using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    /// <summary>
    /// Tenant DbSet for EF Core
    /// </summary>
    public class Tenant : BaseModel 
    {
        //[ForeignKey("AppUser")]
        //public Guid AppUserId { get; set; }
        //public virtual AppUser AppUser { get; set; }

        //[ForeignKey("Profile")]
        //public Guid ProfileId { get; set; }
        //public virtual Profile Profile { get; set; }

        //public virtual ICollection<Profile> ProfileList { get; set; }
        public virtual ICollection<Location> LocationList { get; set; }
        public virtual ICollection<Department> DepartmentList { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }

        //public string Email { get; set; }
        //[ForeignKey("DocumentLogo")]
        public Guid? DocumentLogoId { get; set; }

        //public virtual Document DocumentLogo { get; set; }

        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }

        public string Postcode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }


        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool IsEnable { get; set; }

        //public string LdapUri { get; set; }
        //public string LdapUserDn { get; set; }

    }
}
