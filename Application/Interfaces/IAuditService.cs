using SPOT_API.Models;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    /// <summary>
    /// Service for logging audit trails of system activities
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Log a successful action
        /// </summary>
        Task LogAsync(
            AuditEventType eventType,
            string action,
            string description = null,
            string entityType = null,
            string entityId = null,
            string entityName = null,
            object oldValues = null,
            object newValues = null,
            AuditSeverity severity = AuditSeverity.Info);

        /// <summary>
        /// Log a failed action with error details
        /// </summary>
        Task LogErrorAsync(
            AuditEventType eventType,
            string action,
            string errorMessage,
            string stackTrace = null,
            string errorCode = null,
            string description = null,
            string entityType = null,
            string entityId = null,
            AuditSeverity severity = AuditSeverity.High);

        /// <summary>
        /// Log authentication events (login, logout, failed login)
        /// </summary>
        Task LogAuthenticationAsync(
            AuditEventType eventType,
            string userId,
            bool isSuccess,
            string description = null,
            string errorMessage = null);

        /// <summary>
        /// Log entity creation
        /// </summary>
        Task LogEntityCreatedAsync<T>(
            T entity,
            string entityName = null) where T : class;

        /// <summary>
        /// Log entity update
        /// </summary>
        Task LogEntityUpdatedAsync<T>(
            T oldEntity,
            T newEntity,
            string entityName = null) where T : class;

        /// <summary>
        /// Log entity deletion
        /// </summary>
        Task LogEntityDeletedAsync<T>(
            T entity,
            string entityName = null) where T : class;

        /// <summary>
        /// Clean up old audit logs based on retention policy
        /// </summary>
        /// <param name="retentionDays">Number of days to retain logs</param>
        /// <returns>Number of records deleted</returns>
        Task<int> CleanupOldLogsAsync(int retentionDays);
    }
}
