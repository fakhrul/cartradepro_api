using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.DTOs;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuditService _auditService;

        public VehiclesController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager, IAuditService auditService)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
            _auditService = auditService;
        }

        // GET: api/Vehicles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehicle>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.Vehicles
                .ToListAsync();

            return objs;
        }


        // GET: api/Vehicles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vehicle>> Get(Guid id)
        {
            var category = await _context.Vehicles
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }


        // PUT: api/Vehicles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Vehicle obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            // Get old values for audit log (before updating)
            var prevObj = await _context.Vehicles
                .Include(v => v.Brand)
                .Include(v => v.Model)
                .Include(v => v.VehicleType)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id);

            _context.Entry(obj).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Load related entities for audit log
                await _context.Entry(obj).Reference(v => v.Brand).LoadAsync();
                await _context.Entry(obj).Reference(v => v.Model).LoadAsync();
                await _context.Entry(obj).Reference(v => v.VehicleType).LoadAsync();

                // Log the vehicle update
                await _auditService.LogAsync(
                    AuditEventType.VehicleUpdated,
                    "Vehicle Updated",
                    $"Updated vehicle: {obj.Brand?.Name} {obj.Model?.Name} - Chasis: {obj.ChasisNo}",
                    entityType: "Vehicle",
                    entityId: obj.Id.ToString(),
                    entityName: $"{obj.Brand?.Name} {obj.Model?.Name} - {obj.ChasisNo}",
                    oldValues: new
                    {
                        ChasisNo = prevObj?.ChasisNo,
                        Brand = prevObj?.Brand?.Name,
                        Model = prevObj?.Model?.Name,
                        VehicleType = prevObj?.VehicleType?.Name,
                        Year = prevObj?.Year,
                        Month = prevObj?.Month,
                        Color = prevObj?.Color,
                        EngineNo = prevObj?.EngineNo,
                        EngineCapacity = prevObj?.EngineCapacity
                    },
                    newValues: new
                    {
                        ChasisNo = obj.ChasisNo,
                        Brand = obj.Brand?.Name,
                        Model = obj.Model?.Name,
                        VehicleType = obj.VehicleType?.Name,
                        Year = obj.Year,
                        Month = obj.Month,
                        Color = obj.Color,
                        EngineNo = obj.EngineNo,
                        EngineCapacity = obj.EngineCapacity
                    },
                    severity: AuditSeverity.Low
                );
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!IsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    await _auditService.LogErrorAsync(
                        AuditEventType.VehicleUpdated,
                        "Vehicle Update Failed - Concurrency Error",
                        ex.Message,
                        ex.StackTrace,
                        errorCode: "CONCURRENCY_ERROR",
                        description: $"Concurrency conflict updating vehicle - Chasis: {obj.ChasisNo}",
                        entityId: id.ToString(),
                        severity: AuditSeverity.High
                    );
                    throw;
                }
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.VehicleUpdated,
                    "Vehicle Update Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "VEHICLE_UPDATE_ERROR",
                    description: $"Failed to update vehicle - Chasis: {obj.ChasisNo}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                throw;
            }

            return NoContent();
        }

        // POST: api/Vehicles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vehicle>> Post(Vehicle obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                _context.Vehicles.Add(obj);
                await _context.SaveChangesAsync();

                // Load related entities for audit log
                await _context.Entry(obj).Reference(v => v.Brand).LoadAsync();
                await _context.Entry(obj).Reference(v => v.Model).LoadAsync();
                await _context.Entry(obj).Reference(v => v.VehicleType).LoadAsync();

                // Log the vehicle creation
                await _auditService.LogAsync(
                    AuditEventType.VehicleCreated,
                    "Vehicle Created",
                    $"Created vehicle: {obj.Brand?.Name} {obj.Model?.Name} - Chasis: {obj.ChasisNo}",
                    entityType: "Vehicle",
                    entityId: obj.Id.ToString(),
                    entityName: $"{obj.Brand?.Name} {obj.Model?.Name} - {obj.ChasisNo}",
                    newValues: new
                    {
                        ChasisNo = obj.ChasisNo,
                        Brand = obj.Brand?.Name,
                        Model = obj.Model?.Name,
                        VehicleType = obj.VehicleType?.Name,
                        Year = obj.Year,
                        Month = obj.Month,
                        Color = obj.Color,
                        EngineNo = obj.EngineNo,
                        EngineCapacity = obj.EngineCapacity
                    },
                    severity: AuditSeverity.Low
                );
            }
            catch (DbUpdateException dbEx)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.VehicleCreated,
                    "Vehicle Creation Failed - Database Error",
                    dbEx.InnerException?.Message ?? dbEx.Message,
                    dbEx.StackTrace,
                    errorCode: "DB_UPDATE_ERROR",
                    description: $"Failed to create vehicle - Chasis: {obj.ChasisNo}",
                    severity: AuditSeverity.High
                );
                throw;
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.VehicleCreated,
                    "Vehicle Creation Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "VEHICLE_CREATE_ERROR",
                    description: $"Failed to create vehicle - Chasis: {obj.ChasisNo}",
                    severity: AuditSeverity.High
                );
                throw;
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/Vehicles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var vehicle = await _context.Vehicles
                    .Include(v => v.Brand)
                    .Include(v => v.Model)
                    .Include(v => v.VehicleType)
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (vehicle == null)
                {
                    return NotFound();
                }

                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();

                // Log the vehicle deletion
                await _auditService.LogAsync(
                    AuditEventType.VehicleDeleted,
                    "Vehicle Deleted",
                    $"Deleted vehicle: {vehicle.Brand?.Name} {vehicle.Model?.Name} - Chasis: {vehicle.ChasisNo}",
                    entityType: "Vehicle",
                    entityId: vehicle.Id.ToString(),
                    entityName: $"{vehicle.Brand?.Name} {vehicle.Model?.Name} - {vehicle.ChasisNo}",
                    oldValues: new
                    {
                        ChasisNo = vehicle.ChasisNo,
                        Brand = vehicle.Brand?.Name,
                        Model = vehicle.Model?.Name,
                        VehicleType = vehicle.VehicleType?.Name,
                        Year = vehicle.Year,
                        Month = vehicle.Month,
                        Color = vehicle.Color,
                        EngineNo = vehicle.EngineNo,
                        EngineCapacity = vehicle.EngineCapacity
                    },
                    severity: AuditSeverity.Medium
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.VehicleDeleted,
                    "Vehicle Deletion Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "VEHICLE_DELETE_ERROR",
                    description: $"Failed to delete vehicle with ID: {id}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                throw;
            }
        }

        private bool IsExists(Guid id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
