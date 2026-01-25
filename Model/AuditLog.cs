using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPOT_API.Models
{
    /// <summary>
    /// Comprehensive audit trail for all system activities
    /// </summary>
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// User who performed the action (nullable for anonymous/system actions)
        /// Note: Using string type to match AppUser.Id (IdentityUser uses string ID)
        /// </summary>
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }

        /// <summary>
        /// Type of event
        /// </summary>
        [Required]
        public AuditEventType EventType { get; set; }

        /// <summary>
        /// Severity level
        /// </summary>
        [Required]
        public AuditSeverity Severity { get; set; }

        /// <summary>
        /// Short description of the action (e.g., "Login", "Create Stock")
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Action { get; set; }

        /// <summary>
        /// Detailed description
        /// </summary>
        [StringLength(2000)]
        public string Description { get; set; }

        /// <summary>
        /// Whether the action was successful
        /// </summary>
        public bool IsSuccess { get; set; } = true;

        /// <summary>
        /// Entity type being affected (e.g., "Stock", "Customer", "User")
        /// </summary>
        [StringLength(100)]
        public string EntityType { get; set; }

        /// <summary>
        /// Entity ID
        /// </summary>
        [StringLength(255)]
        public string EntityId { get; set; }

        /// <summary>
        /// Entity display name
        /// </summary>
        [StringLength(500)]
        public string EntityName { get; set; }

        /// <summary>
        /// Old values (JSON) for update operations
        /// </summary>
        public string OldValues { get; set; }

        /// <summary>
        /// New values (JSON) for create/update operations
        /// </summary>
        public string NewValues { get; set; }

        /// <summary>
        /// Client IP address
        /// </summary>
        [StringLength(50)]
        public string IpAddress { get; set; }

        /// <summary>
        /// User agent (browser/client info)
        /// </summary>
        [StringLength(500)]
        public string UserAgent { get; set; }

        /// <summary>
        /// Request URL
        /// </summary>
        [StringLength(2000)]
        public string RequestUrl { get; set; }

        /// <summary>
        /// HTTP method (GET, POST, PUT, DELETE)
        /// </summary>
        [StringLength(10)]
        public string RequestMethod { get; set; }

        /// <summary>
        /// Request body (for POST/PUT, optional)
        /// </summary>
        public string RequestBody { get; set; }

        /// <summary>
        /// HTTP response status code
        /// </summary>
        public int? ResponseStatusCode { get; set; }

        /// <summary>
        /// Response body (optional)
        /// </summary>
        public string ResponseBody { get; set; }

        /// <summary>
        /// Operation duration in milliseconds
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Error message if action failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Stack trace for exceptions
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Error code (application-specific)
        /// </summary>
        [StringLength(50)]
        public string ErrorCode { get; set; }

        /// <summary>
        /// When this event occurred
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Additional metadata (JSON)
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// Correlation ID for linking related events
        /// </summary>
        public Guid? CorrelationId { get; set; }
    }
}
