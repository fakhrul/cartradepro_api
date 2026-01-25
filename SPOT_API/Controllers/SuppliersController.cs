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
    public class SuppliersController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuditService _auditService;

        public SuppliersController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager, IAuditService auditService)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
            _auditService = auditService;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.Suppliers
                .OrderBy(c=> c.Name)
                .ToListAsync();

            return objs;
        }


        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> Get(Guid id)
        {
            var obj = await _context.Suppliers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return obj;
        }


        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Supplier obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            // Get old values for audit log (before updating)
            var prevObj = await _context.Suppliers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            _context.Entry(obj).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Log the supplier update
                await _auditService.LogAsync(
                    AuditEventType.SupplierUpdated,
                    "Supplier Updated",
                    $"Updated supplier: {obj.Name}",
                    entityType: "Supplier",
                    entityId: obj.Id.ToString(),
                    entityName: obj.Name,
                    oldValues: new
                    {
                        Name = prevObj?.Name,
                        Code = prevObj?.Code,
                        Address = prevObj?.Address,
                        Country = prevObj?.Country,
                        Phone = prevObj?.Phone,
                        Website = prevObj?.Website,
                        ContactPersonName = prevObj?.ContactPersonName,
                        ContactPersonPhone = prevObj?.ContactPersonPhone,
                        ContactPersonEmail = prevObj?.ContactPersonEmail
                    },
                    newValues: new
                    {
                        Name = obj.Name,
                        Code = obj.Code,
                        Address = obj.Address,
                        Country = obj.Country,
                        Phone = obj.Phone,
                        Website = obj.Website,
                        ContactPersonName = obj.ContactPersonName,
                        ContactPersonPhone = obj.ContactPersonPhone,
                        ContactPersonEmail = obj.ContactPersonEmail
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
                        AuditEventType.SupplierUpdated,
                        "Supplier Update Failed - Concurrency Error",
                        ex.Message,
                        ex.StackTrace,
                        errorCode: "CONCURRENCY_ERROR",
                        description: $"Concurrency conflict updating supplier: {obj.Name}",
                        entityId: id.ToString(),
                        severity: AuditSeverity.High
                    );
                    throw;
                }
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SupplierUpdated,
                    "Supplier Update Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "SUPPLIER_UPDATE_ERROR",
                    description: $"Failed to update supplier: {obj.Name}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                throw;
            }

            return NoContent();
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Supplier>> Post(Supplier obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                _context.Suppliers.Add(obj);
                await _context.SaveChangesAsync();

                // Log the supplier creation
                await _auditService.LogAsync(
                    AuditEventType.SupplierCreated,
                    "Supplier Created",
                    $"Created supplier: {obj.Name}",
                    entityType: "Supplier",
                    entityId: obj.Id.ToString(),
                    entityName: obj.Name,
                    newValues: new
                    {
                        Name = obj.Name,
                        Code = obj.Code,
                        Address = obj.Address,
                        Country = obj.Country,
                        Phone = obj.Phone,
                        Website = obj.Website,
                        ContactPersonName = obj.ContactPersonName,
                        ContactPersonPhone = obj.ContactPersonPhone,
                        ContactPersonEmail = obj.ContactPersonEmail
                    },
                    severity: AuditSeverity.Low
                );
            }
            catch (DbUpdateException dbEx)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SupplierCreated,
                    "Supplier Creation Failed - Database Error",
                    dbEx.InnerException?.Message ?? dbEx.Message,
                    dbEx.StackTrace,
                    errorCode: "DB_UPDATE_ERROR",
                    description: $"Failed to create supplier: {obj.Name}",
                    severity: AuditSeverity.High
                );
                throw;
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SupplierCreated,
                    "Supplier Creation Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "SUPPLIER_CREATE_ERROR",
                    description: $"Failed to create supplier: {obj.Name}",
                    severity: AuditSeverity.High
                );
                throw;
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (supplier == null)
                {
                    return NotFound();
                }

                var purchases = await _context.Purchases
                    .Where(c => c.SupplierId == id)
                    .ToListAsync();

                foreach(var purchase in purchases)
                {
                    purchase.SupplierId = null;
                }

                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                // Log the supplier deletion
                await _auditService.LogAsync(
                    AuditEventType.SupplierDeleted,
                    "Supplier Deleted",
                    $"Deleted supplier: {supplier.Name}",
                    entityType: "Supplier",
                    entityId: supplier.Id.ToString(),
                    entityName: supplier.Name,
                    oldValues: new
                    {
                        Name = supplier.Name,
                        Code = supplier.Code,
                        Address = supplier.Address,
                        Country = supplier.Country,
                        Phone = supplier.Phone,
                        Website = supplier.Website,
                        ContactPersonName = supplier.ContactPersonName,
                        ContactPersonPhone = supplier.ContactPersonPhone,
                        ContactPersonEmail = supplier.ContactPersonEmail
                    },
                    severity: AuditSeverity.Medium
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.SupplierDeleted,
                    "Supplier Deletion Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "SUPPLIER_DELETE_ERROR",
                    description: $"Failed to delete supplier with ID: {id}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                throw;
            }
        }

        private bool IsExists(Guid id)
        {
            return _context.Suppliers.Any(e => e.Id == id);
        }


    }
}
