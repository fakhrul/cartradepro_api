using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        //public bool IsSuperAdmin { get; set; }
        public string Role { get; set; }  // Legacy role field - will be replaced by UserRole table
        public Guid? TenantId { get; set; }
        public string TenantCode { get; set; }

        [ForeignKey("Profile")]
        public Guid? ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        /// <summary>
        /// Reference to user's manager/supervisor (string to match AppUser.Id)
        /// </summary>
        public string ManagerId { get; set; }

        /// <summary>
        /// Last successful login timestamp
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Email verification status
        /// </summary>
        public bool IsEmailVerified { get; set; } = false;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        /// <summary>
        /// User's role assignments (many-to-many)
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Audit logs for this user's actions
        /// </summary>
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    }
}
