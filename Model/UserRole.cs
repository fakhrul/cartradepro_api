using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    /// <summary>
    /// Junction table for many-to-many relationship between AppUser and Role
    /// Supports time-based role activation
    /// </summary>
    public class UserRole : BaseModel
    {
        [ForeignKey("User")]
        public string UserId { get; set; }  // Using string to match AppUser.Id (IdentityUser uses string)
        public virtual AppUser User { get; set; }

        [ForeignKey("Role")]
        public Guid RoleId { get; set; }
        public virtual Role Role { get; set; }

        /// <summary>
        /// User who assigned this role
        /// </summary>
        public string AssignedBy { get; set; }  // Using string to match AppUser.Id

        /// <summary>
        /// When the role was assigned
        /// </summary>
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Role becomes active from this date (null = immediate)
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>
        /// Role expires on this date (null = permanent)
        /// </summary>
        public DateTime? EffectiveUntil { get; set; }

        /// <summary>
        /// Manual activation toggle
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Check if this role assignment is currently effective
        /// </summary>
        /// <returns>True if role is active and within effective date range</returns>
        public bool IsCurrentlyEffective()
        {
            if (!IsActive) return false;

            var now = DateTime.UtcNow;
            if (EffectiveFrom.HasValue && now < EffectiveFrom.Value) return false;
            if (EffectiveUntil.HasValue && now > EffectiveUntil.Value) return false;

            return true;
        }
    }
}
