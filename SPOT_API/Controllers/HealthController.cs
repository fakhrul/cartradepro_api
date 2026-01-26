using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Persistence;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly SpotDBContext _context;

        public HealthController(SpotDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Basic health check endpoint
        /// GET: api/Health
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                application = "CarTradePro API",
                version = "1.0.0"
            });
        }

        /// <summary>
        /// Database connection status check
        /// GET: api/Health/database
        /// </summary>
        [AllowAnonymous]
        [HttpGet("database")]
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Try to connect to database and execute a simple query
                var canConnect = await _context.Database.CanConnectAsync();

                if (!canConnect)
                {
                    stopwatch.Stop();
                    return StatusCode(503, new
                    {
                        status = "Unhealthy",
                        message = "Cannot connect to database",
                        timestamp = DateTime.UtcNow,
                        responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
                    });
                }

                // Get database info
                var connectionString = _context.Database.GetConnectionString();
                var databaseName = _context.Database.GetDbConnection().Database;

                // Count some basic records to verify read access
                var userCount = await _context.Users.CountAsync();
                var stockCount = await _context.Stocks.CountAsync();

                // Get pending migrations
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();

                stopwatch.Stop();

                return Ok(new
                {
                    status = "Healthy",
                    message = "Database connection successful",
                    timestamp = DateTime.UtcNow,
                    responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
                    database = new
                    {
                        name = databaseName,
                        provider = _context.Database.ProviderName,
                        canConnect = true,
                        statistics = new
                        {
                            userCount,
                            stockCount
                        },
                        migrations = new
                        {
                            totalApplied = appliedMigrations.Count(),
                            totalPending = pendingMigrations.Count(),
                            hasPendingMigrations = pendingMigrations.Any(),
                            latestAppliedMigration = appliedMigrations.LastOrDefault()
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    message = "Database connection failed",
                    error = ex.Message,
                    timestamp = DateTime.UtcNow,
                    responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
                });
            }
        }

        /// <summary>
        /// Detailed system health check including database, memory, etc.
        /// GET: api/Health/detailed
        /// </summary>
        [AllowAnonymous]
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailedHealth()
        {
            var stopwatch = Stopwatch.StartNew();
            var health = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                application = "CarTradePro API",
                version = "1.0.0",
                checks = new System.Collections.Generic.Dictionary<string, object>()
            };

            // Database check
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var databaseName = _context.Database.GetDbConnection().Database;

                health.checks.Add("database", new
                {
                    status = canConnect ? "Healthy" : "Unhealthy",
                    name = databaseName,
                    provider = _context.Database.ProviderName,
                    canConnect
                });
            }
            catch (Exception ex)
            {
                health.checks.Add("database", new
                {
                    status = "Unhealthy",
                    error = ex.Message
                });
            }

            // Memory check
            var process = Process.GetCurrentProcess();
            health.checks.Add("memory", new
            {
                status = "Healthy",
                workingSet = $"{process.WorkingSet64 / 1024 / 1024} MB",
                privateMemory = $"{process.PrivateMemorySize64 / 1024 / 1024} MB"
            });

            // Uptime
            health.checks.Add("uptime", new
            {
                status = "Healthy",
                startTime = Process.GetCurrentProcess().StartTime,
                uptime = DateTime.Now - Process.GetCurrentProcess().StartTime
            });

            stopwatch.Stop();

            return Ok(new
            {
                health.status,
                health.timestamp,
                health.application,
                health.version,
                health.checks,
                responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
            });
        }
    }
}
