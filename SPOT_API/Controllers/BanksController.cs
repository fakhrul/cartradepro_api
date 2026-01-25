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
    public class BanksController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuditService _auditService;

        public BanksController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager, IAuditService auditService)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
            _auditService = auditService;
        }

        // GET: api/Banks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bank>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.Banks
                .OrderBy(c=> c.Name)
                .ToListAsync();

            return objs;
        }


        // GET: api/Banks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bank>> Get(Guid id)
        {
            var obj = await _context.Banks
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return obj;
        }


        // PUT: api/Banks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Bank obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            // Get old values for audit log (before updating)
            var prevObj = await _context.Banks
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            _context.Entry(obj).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Log the bank update
                await _auditService.LogAsync(
                    AuditEventType.BankUpdated,
                    "Bank Updated",
                    $"Updated bank: {obj.Name}",
                    entityType: "Bank",
                    entityId: obj.Id.ToString(),
                    entityName: obj.Name,
                    oldValues: new
                    {
                        Name = prevObj?.Name,
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
                        AuditEventType.BankUpdated,
                        "Bank Update Failed - Concurrency Error",
                        ex.Message,
                        ex.StackTrace,
                        errorCode: "CONCURRENCY_ERROR",
                        description: $"Concurrency conflict updating bank: {obj.Name}",
                        entityId: id.ToString(),
                        severity: AuditSeverity.High
                    );
                    throw;
                }
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.BankUpdated,
                    "Bank Update Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "BANK_UPDATE_ERROR",
                    description: $"Failed to update bank: {obj.Name}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                throw;
            }

            return NoContent();
        }

        // POST: api/Banks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Bank>> Post(Bank obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                _context.Banks.Add(obj);
                await _context.SaveChangesAsync();

                // Log the bank creation
                await _auditService.LogAsync(
                    AuditEventType.BankCreated,
                    "Bank Created",
                    $"Created bank: {obj.Name}",
                    entityType: "Bank",
                    entityId: obj.Id.ToString(),
                    entityName: obj.Name,
                    newValues: new
                    {
                        Name = obj.Name,
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
                    AuditEventType.BankCreated,
                    "Bank Creation Failed - Database Error",
                    dbEx.InnerException?.Message ?? dbEx.Message,
                    dbEx.StackTrace,
                    errorCode: "DB_UPDATE_ERROR",
                    description: $"Failed to create bank: {obj.Name}",
                    severity: AuditSeverity.High
                );
                throw;
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.BankCreated,
                    "Bank Creation Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "BANK_CREATE_ERROR",
                    description: $"Failed to create bank: {obj.Name}",
                    severity: AuditSeverity.High
                );
                throw;
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/Banks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var bank = await _context.Banks
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (bank == null)
                {
                    return NotFound();
                }

                // Unlink all Loans that reference this bank (set BankId to null)
                var loans = await _context.Loans.Where(l => l.BankId == id).ToListAsync();
                foreach (var loan in loans)
                {
                    loan.BankId = null;
                }

                // Now delete the bank
                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync();

                // Log the bank deletion
                await _auditService.LogAsync(
                    AuditEventType.BankDeleted,
                    "Bank Deleted",
                    $"Deleted bank: {bank.Name}",
                    entityType: "Bank",
                    entityId: bank.Id.ToString(),
                    entityName: bank.Name,
                    oldValues: new
                    {
                        Name = bank.Name,
                        Address = bank.Address,
                        Country = bank.Country,
                        Phone = bank.Phone,
                        Website = bank.Website,
                        ContactPersonName = bank.ContactPersonName,
                        ContactPersonPhone = bank.ContactPersonPhone,
                        ContactPersonEmail = bank.ContactPersonEmail
                    },
                    severity: AuditSeverity.Medium
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.BankDeleted,
                    "Bank Deletion Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "BANK_DELETE_ERROR",
                    description: $"Failed to delete bank with ID: {id}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                return BadRequest(new
                {
                    message = "Error deleting bank",
                    error = ex.Message
                });
            }
        }

        private bool IsExists(Guid id)
        {
            return _context.Banks.Any(e => e.Id == id);
        }


    }
}
