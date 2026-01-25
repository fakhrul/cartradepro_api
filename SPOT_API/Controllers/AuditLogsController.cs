using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;
using SPOT_API.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditLogsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IAuditService _auditService;

        public AuditLogsController(SpotDBContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        /// <summary>
        /// Get paginated audit logs with filtering
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string userId = null,
            [FromQuery] string userName = null,
            [FromQuery] string eventType = null,
            [FromQuery] string severity = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string entityType = null,
            [FromQuery] string entityId = null,
            [FromQuery] string action = null)
        {
            try
            {
                var query = _context.AuditLogs
                    .Include(a => a.User)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(userId))
                    query = query.Where(a => a.UserId == userId);

                if (!string.IsNullOrEmpty(userName))
                    query = query.Where(a => a.User != null && a.User.UserName.Contains(userName));

                if (!string.IsNullOrEmpty(eventType) && Enum.TryParse<AuditEventType>(eventType, out var parsedEventType))
                    query = query.Where(a => a.EventType == parsedEventType);

                if (!string.IsNullOrEmpty(severity) && Enum.TryParse<AuditSeverity>(severity, out var parsedSeverity))
                    query = query.Where(a => a.Severity == parsedSeverity);

                if (startDate.HasValue)
                    query = query.Where(a => a.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(a => a.Timestamp <= endDate.Value);

                if (!string.IsNullOrEmpty(entityType))
                    query = query.Where(a => a.EntityType == entityType);

                if (!string.IsNullOrEmpty(entityId))
                    query = query.Where(a => a.EntityId == entityId);

                if (!string.IsNullOrEmpty(action))
                    query = query.Where(a => a.Action.Contains(action));

                // Get total count
                var totalCount = await query.CountAsync();

                // Apply pagination
                var logs = await query
                    .OrderByDescending(a => a.Timestamp)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new
                    {
                        a.Id,
                        a.UserId,
                        UserName = a.User != null ? a.User.UserName : null,
                        EventType = a.EventType.ToString(),
                        Severity = a.Severity.ToString(),
                        a.Action,
                        a.Description,
                        a.IsSuccess,
                        a.EntityType,
                        a.EntityId,
                        a.EntityName,
                        a.IpAddress,
                        a.Timestamp,
                        a.ErrorMessage
                    })
                    .ToListAsync();

                return Ok(new
                {
                    data = logs,
                    totalCount,
                    page,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                });
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "GetAuditLogs",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    "Error retrieving audit logs",
                    null,
                    null,
                    AuditSeverity.High);
                return StatusCode(500, "Error retrieving audit logs");
            }
        }

        /// <summary>
        /// Get single audit log by ID with full details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditLog(Guid id)
        {
            try
            {
                var log = await _context.AuditLogs
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (log == null)
                    return NotFound();

                return Ok(new
                {
                    log.Id,
                    log.UserId,
                    UserName = log.User?.UserName,
                    UserDisplayName = log.User?.DisplayName,
                    log.EventType,
                    log.Severity,
                    log.Action,
                    log.Description,
                    log.IsSuccess,
                    log.EntityType,
                    log.EntityId,
                    log.EntityName,
                    log.OldValues,
                    log.NewValues,
                    log.IpAddress,
                    log.UserAgent,
                    log.RequestUrl,
                    log.RequestMethod,
                    log.RequestBody,
                    log.ResponseStatusCode,
                    log.ResponseBody,
                    log.DurationMs,
                    log.ErrorMessage,
                    log.StackTrace,
                    log.ErrorCode,
                    log.Timestamp,
                    log.Metadata,
                    log.CorrelationId
                });
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "GetAuditLog",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    $"Error retrieving audit log: {id}",
                    "AuditLog",
                    id.ToString(),
                    AuditSeverity.Medium);
                return StatusCode(500, "Error retrieving audit log");
            }
        }

        /// <summary>
        /// Get audit statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] int days = 7)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddDays(-days);

                var stats = await _context.AuditLogs
                    .Where(a => a.Timestamp >= startDate)
                    .GroupBy(a => 1)
                    .Select(g => new
                    {
                        TotalEvents = g.Count(),
                        SuccessfulEvents = g.Count(a => a.IsSuccess),
                        FailedEvents = g.Count(a => !a.IsSuccess),
                        CriticalEvents = g.Count(a => a.Severity == AuditSeverity.Critical),
                        HighSeverityEvents = g.Count(a => a.Severity == AuditSeverity.High),
                        UniqueUsers = g.Select(a => a.UserId).Distinct().Count()
                    })
                    .FirstOrDefaultAsync();

                // Get event type breakdown
                var eventTypeBreakdown = await _context.AuditLogs
                    .Where(a => a.Timestamp >= startDate)
                    .GroupBy(a => a.EventType)
                    .Select(g => new
                    {
                        EventType = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync();

                // Get severity breakdown
                var severityBreakdown = await _context.AuditLogs
                    .Where(a => a.Timestamp >= startDate)
                    .GroupBy(a => a.Severity)
                    .Select(g => new
                    {
                        Severity = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                return Ok(new
                {
                    period = $"Last {days} days",
                    startDate,
                    endDate = DateTime.UtcNow,
                    summary = stats ?? new
                    {
                        TotalEvents = 0,
                        SuccessfulEvents = 0,
                        FailedEvents = 0,
                        CriticalEvents = 0,
                        HighSeverityEvents = 0,
                        UniqueUsers = 0
                    },
                    eventTypeBreakdown,
                    severityBreakdown
                });
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "GetAuditStatistics",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    "Error retrieving audit statistics",
                    null,
                    null,
                    AuditSeverity.Medium);
                return StatusCode(500, "Error retrieving audit statistics");
            }
        }

        /// <summary>
        /// Cleanup old audit logs based on retention policy
        /// </summary>
        [HttpPost("cleanup")]
        public async Task<IActionResult> CleanupOldLogs([FromQuery] int retentionDays = 90)
        {
            try
            {
                if (retentionDays < 30)
                    return BadRequest("Retention period must be at least 30 days");

                var deletedCount = await _auditService.CleanupOldLogsAsync(retentionDays);

                await _auditService.LogAsync(
                    AuditEventType.AuditLogsCleanedUp,
                    "CleanupAuditLogs",
                    $"Cleaned up {deletedCount} audit log records older than {retentionDays} days",
                    null,
                    null,
                    null,
                    null,
                    new { RetentionDays = retentionDays, DeletedCount = deletedCount },
                    AuditSeverity.Info);

                return Ok(new
                {
                    success = true,
                    deletedCount,
                    retentionDays,
                    message = $"Successfully deleted {deletedCount} old audit log records"
                });
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "CleanupAuditLogs",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    "Error cleaning up audit logs",
                    null,
                    null,
                    AuditSeverity.High);
                return StatusCode(500, "Error cleaning up audit logs");
            }
        }

        /// <summary>
        /// Get audit trail for a specific entity
        /// </summary>
        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<IActionResult> GetEntityAuditTrail(string entityType, string entityId)
        {
            try
            {
                var logs = await _context.AuditLogs
                    .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                    .Include(a => a.User)
                    .OrderByDescending(a => a.Timestamp)
                    .Select(a => new
                    {
                        a.Id,
                        a.UserId,
                        UserName = a.User != null ? a.User.UserName : null,
                        a.EventType,
                        a.Action,
                        a.Description,
                        a.IsSuccess,
                        a.OldValues,
                        a.NewValues,
                        a.Timestamp
                    })
                    .ToListAsync();

                return Ok(new
                {
                    entityType,
                    entityId,
                    totalEvents = logs.Count,
                    auditTrail = logs
                });
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SystemError,
                    "GetEntityAuditTrail",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    $"Error retrieving audit trail for {entityType}/{entityId}",
                    entityType,
                    entityId,
                    AuditSeverity.Medium);
                return StatusCode(500, "Error retrieving entity audit trail");
            }
        }
    }
}
