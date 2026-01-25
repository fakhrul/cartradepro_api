using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    /// <summary>
    /// Implementation of audit service for logging system activities
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly SpotDBContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public AuditService(
            SpotDBContext context,
            IHttpContextAccessor httpContextAccessor,
            IUserAccessor userAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        /// <summary>
        /// Log a successful action
        /// </summary>
        public async Task LogAsync(
            AuditEventType eventType,
            string action,
            string description = null,
            string entityType = null,
            string entityId = null,
            string entityName = null,
            object oldValues = null,
            object newValues = null,
            AuditSeverity severity = AuditSeverity.Info)
        {
            var auditLog = CreateBaseAuditLog(eventType, action, severity);
            auditLog.IsSuccess = true;
            auditLog.Description = description;
            auditLog.EntityType = entityType;
            auditLog.EntityId = entityId;
            auditLog.EntityName = entityName;

            if (oldValues != null)
                auditLog.OldValues = SerializeObject(oldValues);

            if (newValues != null)
                auditLog.NewValues = SerializeObject(newValues);

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Log a failed action with error details
        /// </summary>
        public async Task LogErrorAsync(
            AuditEventType eventType,
            string action,
            string errorMessage,
            string stackTrace = null,
            string errorCode = null,
            string description = null,
            string entityType = null,
            string entityId = null,
            AuditSeverity severity = AuditSeverity.High)
        {
            var auditLog = CreateBaseAuditLog(eventType, action, severity);
            auditLog.IsSuccess = false;
            auditLog.Description = description;
            auditLog.EntityType = entityType;
            auditLog.EntityId = entityId;
            auditLog.ErrorMessage = errorMessage;
            auditLog.StackTrace = stackTrace;
            auditLog.ErrorCode = errorCode;

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Log authentication events (login, logout, failed login)
        /// </summary>
        public async Task LogAuthenticationAsync(
            AuditEventType eventType,
            string userId,
            bool isSuccess,
            string description = null,
            string errorMessage = null)
        {
            var action = eventType == AuditEventType.Login ? "User Login" :
                        eventType == AuditEventType.Logout ? "User Logout" :
                        "Authentication";

            var severity = isSuccess ? AuditSeverity.Info : AuditSeverity.Medium;

            var auditLog = CreateBaseAuditLog(eventType, action, severity);
            auditLog.UserId = userId;
            auditLog.IsSuccess = isSuccess;
            auditLog.Description = description;
            auditLog.ErrorMessage = errorMessage;

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Log entity creation
        /// </summary>
        public async Task LogEntityCreatedAsync<T>(T entity, string entityName = null) where T : class
        {
            var entityTypeName = typeof(T).Name;
            var entityIdValue = GetEntityId(entity);

            await LogAsync(
                AuditEventType.EntityCreated,
                $"Create {entityTypeName}",
                $"Created new {entityTypeName}",
                entityTypeName,
                entityIdValue,
                entityName,
                null,
                entity,
                AuditSeverity.Info);
        }

        /// <summary>
        /// Log entity update
        /// </summary>
        public async Task LogEntityUpdatedAsync<T>(T oldEntity, T newEntity, string entityName = null) where T : class
        {
            var entityTypeName = typeof(T).Name;
            var entityIdValue = GetEntityId(newEntity);

            await LogAsync(
                AuditEventType.EntityUpdated,
                $"Update {entityTypeName}",
                $"Updated {entityTypeName}",
                entityTypeName,
                entityIdValue,
                entityName,
                oldEntity,
                newEntity,
                AuditSeverity.Info);
        }

        /// <summary>
        /// Log entity deletion
        /// </summary>
        public async Task LogEntityDeletedAsync<T>(T entity, string entityName = null) where T : class
        {
            var entityTypeName = typeof(T).Name;
            var entityIdValue = GetEntityId(entity);

            await LogAsync(
                AuditEventType.EntityDeleted,
                $"Delete {entityTypeName}",
                $"Deleted {entityTypeName}",
                entityTypeName,
                entityIdValue,
                entityName,
                entity,
                null,
                AuditSeverity.Medium);
        }

        /// <summary>
        /// Clean up old audit logs based on retention policy
        /// </summary>
        public async Task<int> CleanupOldLogsAsync(int retentionDays)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

            var oldLogs = await _context.AuditLogs
                .Where(a => a.Timestamp < cutoffDate)
                .ToListAsync();

            if (oldLogs.Any())
            {
                _context.AuditLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();
            }

            return oldLogs.Count;
        }

        #region Private Helper Methods

        /// <summary>
        /// Create base audit log with common properties
        /// </summary>
        private AuditLog CreateBaseAuditLog(AuditEventType eventType, string action, AuditSeverity severity)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userId = GetCurrentUserId();

            var auditLog = new AuditLog
            {
                UserId = userId,
                EventType = eventType,
                Severity = severity,
                Action = action,
                Timestamp = DateTime.UtcNow,
                IpAddress = GetClientIpAddress(httpContext),
                UserAgent = httpContext?.Request?.Headers["User-Agent"].ToString(),
                RequestUrl = httpContext?.Request?.Path.ToString(),
                RequestMethod = httpContext?.Request?.Method
            };

            return auditLog;
        }

        /// <summary>
        /// Get current user ID from claims
        /// </summary>
        private string GetCurrentUserId()
        {
            try
            {
                var username = _userAccessor.GetUsername();
                if (!string.IsNullOrEmpty(username))
                {
                    var user = _context.Users.FirstOrDefault(u => u.UserName == username);
                    return user?.Id;
                }
            }
            catch
            {
                // If we can't get user from UserAccessor, try from HttpContext
            }

            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Get client IP address from HTTP context
        /// </summary>
        private string GetClientIpAddress(HttpContext httpContext)
        {
            if (httpContext == null)
                return null;

            // Check for forwarded IP first (in case of proxy/load balancer)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                return ips[0].Trim();
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }

        /// <summary>
        /// Serialize object to JSON
        /// </summary>
        private string SerializeObject(object obj)
        {
            if (obj == null)
                return null;

            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            catch
            {
                return obj.ToString();
            }
        }

        /// <summary>
        /// Get entity ID from entity object
        /// </summary>
        private string GetEntityId(object entity)
        {
            if (entity == null)
                return null;

            // Try to get Id property using reflection
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(entity);
                return idValue?.ToString();
            }

            return null;
        }

        #endregion
    }
}
