using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SPOT_API.Models
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        //public bool IsSuperAdmin { get; set; }
        public string Role { get; set; }
        public Guid? TenantId { get; set; }
        public string TenantCode { get; set; }

        [ForeignKey("Profile")]
        public Guid? ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    }
}
