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
    public class CustomersController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuditService _auditService;

        public CustomersController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager, IAuditService auditService)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
            _auditService = auditService;
        }

        // GET: api/Customers
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        //    if (user == null)
        //        return Unauthorized();

        //    var objs = await _context.Customers
        //        .OrderBy(c=> c.Name)
        //        .ToListAsync();

        //    return objs;
        //}

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10,
            [FromQuery] string sortColumn = "Name",
            [FromQuery] bool sortAsc = true,
            [FromQuery] string search = null,
            [FromQuery] Dictionary<string, string> filters = null)
        {
            // Retrieve the current user
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            // Base query
            var query = _context.Customers.AsQueryable();

            // Apply search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.Email.Contains(search));
            }

            // Apply filters
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    if (filter.Key == "name" && !string.IsNullOrEmpty(filter.Value))
                    {
                        query = query.Where(c => c.Name.ToLower().Contains(filter.Value.ToLower()));
                    }
                    if (filter.Key == "fullName" && !string.IsNullOrEmpty(filter.Value))
                    {
                        query = query.Where(c => c.Name.ToLower().Contains(filter.Value.ToLower()));
                    }
                    if (filter.Key == "icNumber" && !string.IsNullOrEmpty(filter.Value))
                    {
                        query = query.Where(c => c.IcNumber.Contains(filter.Value));
                    }
                    if (filter.Key == "phone" && !string.IsNullOrEmpty(filter.Value))
                    {
                        query = query.Where(c => c.Phone.Contains(filter.Value));
                    }
                    if (filter.Key == "email" && !string.IsNullOrEmpty(filter.Value))
                    {
                        query = query.Where(c => c.Email.ToLower().Contains(filter.Value.ToLower()));
                    }
                    // Add more filters as needed
                }
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = sortAsc
                    ? query.OrderByDynamic(sortColumn)
                    : query.OrderByDescendingDynamic(sortColumn);
            }

            // Get total count before applying pagination
            var totalItems = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            // Return paginated result
            return Ok(new
            {
                items,
                totalItems
            });
        }



        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> Get(Guid id)
        {
            var obj = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return obj;
        }


        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Customer obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            // Get old values for audit log (before updating)
            var prevObj = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            _context.Entry(obj).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Log the customer update
                await _auditService.LogAsync(
                    AuditEventType.CustomerUpdated,
                    "Customer Updated",
                    $"Updated customer: {obj.Name} - IC: {obj.IcNumber}",
                    entityType: "Customer",
                    entityId: obj.Id.ToString(),
                    entityName: obj.Name,
                    oldValues: new
                    {
                        Name = prevObj?.Name,
                        IcNumber = prevObj?.IcNumber,
                        Phone = prevObj?.Phone,
                        Email = prevObj?.Email,
                        Address = prevObj?.Address
                    },
                    newValues: new
                    {
                        Name = obj.Name,
                        IcNumber = obj.IcNumber,
                        Phone = obj.Phone,
                        Email = obj.Email,
                        Address = obj.Address
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
                        AuditEventType.CustomerUpdated,
                        "Customer Update Failed - Concurrency Error",
                        ex.Message,
                        ex.StackTrace,
                        errorCode: "CONCURRENCY_ERROR",
                        description: $"Concurrency conflict updating customer: {obj.Name}",
                        entityId: id.ToString(),
                        severity: AuditSeverity.High
                    );
                    throw;
                }
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.CustomerUpdated,
                    "Customer Update Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "CUSTOMER_UPDATE_ERROR",
                    description: $"Failed to update customer: {obj.Name}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                throw;
            }

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> Post(Customer obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                _context.Customers.Add(obj);
                await _context.SaveChangesAsync();

                // Log the customer creation
                await _auditService.LogAsync(
                    AuditEventType.CustomerCreated,
                    "Customer Created",
                    $"Created customer: {obj.Name} - IC: {obj.IcNumber}",
                    entityType: "Customer",
                    entityId: obj.Id.ToString(),
                    entityName: obj.Name,
                    newValues: new
                    {
                        Name = obj.Name,
                        IcNumber = obj.IcNumber,
                        Phone = obj.Phone,
                        Email = obj.Email,
                        Address = obj.Address
                    },
                    severity: AuditSeverity.Low
                );
            }
            catch (DbUpdateException dbEx)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.CustomerCreated,
                    "Customer Creation Failed - Database Error",
                    dbEx.InnerException?.Message ?? dbEx.Message,
                    dbEx.StackTrace,
                    errorCode: "DB_UPDATE_ERROR",
                    description: $"Failed to create customer: {obj.Name} - IC: {obj.IcNumber}",
                    severity: AuditSeverity.High
                );
                throw;
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.CustomerCreated,
                    "Customer Creation Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "CUSTOMER_CREATE_ERROR",
                    description: $"Failed to create customer: {obj.Name}",
                    severity: AuditSeverity.High
                );
                throw;
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (customer == null)
                {
                    return NotFound();
                }

                _context.Customers.Remove(customer);

                var sales = await _context.Sales
                    .Where(c => c.CustomerId == id)
                    .ToListAsync();

                foreach(var sale in sales)
                {
                    sale.CustomerId = null;
                    sale.Customer = null;
                }
                await _context.SaveChangesAsync();

                // Log the customer deletion
                await _auditService.LogAsync(
                    AuditEventType.CustomerDeleted,
                    "Customer Deleted",
                    $"Deleted customer: {customer.Name} - IC: {customer.IcNumber}",
                    entityType: "Customer",
                    entityId: customer.Id.ToString(),
                    entityName: customer.Name,
                    oldValues: new
                    {
                        Name = customer.Name,
                        IcNumber = customer.IcNumber,
                        Phone = customer.Phone,
                        Email = customer.Email,
                        Address = customer.Address
                    },
                    severity: AuditSeverity.Medium
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.CustomerDeleted,
                    "Customer Deletion Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "CUSTOMER_DELETE_ERROR",
                    description: $"Failed to delete customer with ID: {id}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                return BadRequest(ex.Message);
            }

        }

        private bool IsExists(Guid id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }


    }
}
