namespace SPOT_API.Models
{
    /// <summary>
    /// Severity levels for audit events
    /// </summary>
    public enum AuditSeverity
    {
        /// <summary>
        /// Normal operations, read operations
        /// </summary>
        Info = 0,

        /// <summary>
        /// Minor changes, preference updates
        /// </summary>
        Low = 1,

        /// <summary>
        /// Data modifications, status changes
        /// </summary>
        Medium = 2,

        /// <summary>
        /// User account changes, role assignments, deletions
        /// </summary>
        High = 3,

        /// <summary>
        /// Security events, payment transactions, authentication failures
        /// </summary>
        Critical = 4
    }
}
