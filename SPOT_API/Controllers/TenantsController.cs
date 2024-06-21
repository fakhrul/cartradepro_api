using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Persistence;
using SPOT_API.Models;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public TenantsController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/Tenants
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tenant>>> GetTenants()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();
            if (!user.IsSuperAdmin)
                return Unauthorized();

            return await _context.Tenants.ToListAsync();
        }

        // GET: api/Tenants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tenant>> GetTenant(Guid id)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(c => c.Id == id);

            if (tenant == null)
            {
                return NotFound();
            }

            return tenant;
        }

        // GET: api/Tenants/5
        [HttpGet("ByCurrentUser")]
        public async Task<ActionResult<Tenant>> GetTenantByUser()
        {
            //try
            //{
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return NotFound();

            var tenant = await _context.Tenants.FirstOrDefaultAsync(c => c.Id == user.TenantId);

            if (tenant == null)
            {
                return NotFound();
            }

            //tenant.Profile = null;
            //tenant.DocumentLogo = null;
            //var tenant = await GetTenant(user.TenantId);

            return tenant;

                //var tenant = await _context.Tenants.Include(c => c.Profile).FirstOrDefaultAsync(c => c.Id == user.TenantId);

                ////var tenant = await _context.Tenants.Include(c => c.Profile).FirstAsync();

                //if (tenant == null)
                //{
                //    return NotFound();
                //}

                //return tenant;
            //}
            //catch (Exception ex)
            //{
            //}
            //return BadRequest();
        }

        [HttpGet("ByCode/{code}")]
        public async Task<ActionResult<Tenant>> GetTenantByCode(string code)
        {
            var tenant = await _context.Tenants.Where(c=> c.Code == code).FirstOrDefaultAsync();
            if (tenant == null)
            {
                return NotFound();
            }

            return tenant;
        }


        // PUT: api/Tenants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTenant(Guid id, Tenant tenant)
        {
            if (id != tenant.Id)
            {
                return BadRequest();
            }

            _context.Entry(tenant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TenantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tenants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tenant>> PostTenant(Tenant tenant)
        {
            //string password = tenant.Profile.Password;
            //tenant.Profile.Password = null;
            //_context.Profiles.Add(tenant.Profile);

            //AppUser user = new AppUser
            //{
            //    DisplayName = tenant.Profile.FullName,
            //    UserName = tenant.Profile.Email,
            //    ProfileId = tenant.Profile.Id,
            //    Email = tenant.Profile.Email,
            //    TenantCode = tenant.Code,
            //    TenantId = tenant.Id,
            //    Role = "admin"
            //};

            _context.Tenants.Add(tenant);

            //await _userManager.CreateAsync(user, password);

            await _context.SaveChangesAsync();

            var tenantInfo = _context.Tenants.Find(tenant.Id);

            return CreatedAtAction("GetTenant", new { id = tenantInfo.Id }, tenantInfo);
        }

        // DELETE: api/Tenants/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TenantExists(Guid id)
        {
            return _context.Tenants.Any(e => e.Id == id);
        }
    }
}
