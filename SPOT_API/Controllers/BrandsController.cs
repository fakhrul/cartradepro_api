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
    public class BrandsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuditService _auditService;

        public BrandsController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager, IAuditService auditService)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
            _auditService = auditService;
        }

        // GET: api/Brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.Brands
                .OrderBy(c=> c.Name)
                .ToListAsync();

            return objs;
        }


        // GET: api/Brands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> Get(Guid id)
        {
            var obj = await _context.Brands
                .Include(x => x.Models)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            foreach (var model in obj.Models)
            {
                model.Brand = null;
            }

            return obj;
        }


        // PUT: api/Brands/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Brand obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            // Get old values for audit log (before updating)
            var prevObj = await _context.Brands
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            _context.Entry(obj).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Log the brand update
                await _auditService.LogAsync(
                    AuditEventType.BrandUpdated,
                    "Brand Updated",
                    $"Updated brand: {obj.Name}",
                    entityType: "Brand",
                    entityId: obj.Id.ToString(),
                    entityName: obj.Name,
                    oldValues: new
                    {
                        Name = prevObj?.Name
                    },
                    newValues: new
                    {
                        Name = obj.Name
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
                        AuditEventType.BrandUpdated,
                        "Brand Update Failed - Concurrency Error",
                        ex.Message,
                        ex.StackTrace,
                        errorCode: "CONCURRENCY_ERROR",
                        description: $"Concurrency conflict updating brand: {obj.Name}",
                        entityId: id.ToString(),
                        severity: AuditSeverity.High
                    );
                    throw;
                }
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.BrandUpdated,
                    "Brand Update Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "BRAND_UPDATE_ERROR",
                    description: $"Failed to update brand: {obj.Name}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                throw;
            }

            return NoContent();
        }

        // POST: api/Brands
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Brand>> Post(Brand obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                _context.Brands.Add(obj);
                await _context.SaveChangesAsync();

                // Log the brand creation
                await _auditService.LogAsync(
                    AuditEventType.BrandCreated,
                    "Brand Created",
                    $"Created brand: {obj.Name}",
                    entityType: "Brand",
                    entityId: obj.Id.ToString(),
                    entityName: obj.Name,
                    newValues: new
                    {
                        Name = obj.Name
                    },
                    severity: AuditSeverity.Low
                );
            }
            catch (DbUpdateException dbEx)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.BrandCreated,
                    "Brand Creation Failed - Database Error",
                    dbEx.InnerException?.Message ?? dbEx.Message,
                    dbEx.StackTrace,
                    errorCode: "DB_UPDATE_ERROR",
                    description: $"Failed to create brand: {obj.Name}",
                    severity: AuditSeverity.High
                );
                throw;
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.BrandCreated,
                    "Brand Creation Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "BRAND_CREATE_ERROR",
                    description: $"Failed to create brand: {obj.Name}",
                    severity: AuditSeverity.High
                );
                throw;
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                var brand = await _context.Brands
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (brand == null)
                {
                    return NotFound();
                }

                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();

                // Log the brand deletion
                await _auditService.LogAsync(
                    AuditEventType.BrandDeleted,
                    "Brand Deleted",
                    $"Deleted brand: {brand.Name}",
                    entityType: "Brand",
                    entityId: brand.Id.ToString(),
                    entityName: brand.Name,
                    oldValues: new
                    {
                        Name = brand.Name
                    },
                    severity: AuditSeverity.Medium
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _auditService.LogErrorAsync(
                    AuditEventType.BrandDeleted,
                    "Brand Deletion Failed",
                    ex.Message,
                    ex.StackTrace,
                    errorCode: "BRAND_DELETE_ERROR",
                    description: $"Failed to delete brand with ID: {id}",
                    entityId: id.ToString(),
                    severity: AuditSeverity.High
                );
                throw;
            }
        }

        private bool IsExists(Guid id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }


        [HttpDelete("RemoveModel/{brandId}/{modelId}")]
        public async Task<IActionResult> PutRemoveModel(Guid brandId, Guid modelId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var model = _context.Models.Find(modelId);
                _context.Models.Remove(model);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }

        [HttpPost("AddModel/{brandId}")]
        public async Task<IActionResult> PostAddModel(Guid brandId, Model model)
        {
            try
            {
                var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                Brand brand = _context.Brands.Find(brandId);
                if (brand == null)
                {
                    return BadRequest();
                }

                model.BrandId = brandId;
                _context.Models.Add(model);

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            //return CreatedAtAction("Get", new { id = brand.Id }, brand);


            return NoContent();
        }

    }
}
