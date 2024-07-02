using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Persistence;
using SPOT_API.Models;
using Microsoft.AspNetCore.Identity;
using Application.Interfaces;

namespace SPOT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public UsersController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager )
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Profile>>> GetUsers()
        {

            return await _context.Profiles.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetUser(Guid id)
        {
            var user = await _context.Profiles.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, Profile user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Profile>> PostUser(Profile user)
        {
            _context.Profiles.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Profiles.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Profiles.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }

        [HttpGet("GetRolesAndPermissions")]
        public async Task<IActionResult> GetRolesPermissionsModules()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user.Profile.AppUser);
            var permissions = GetPermissionsForRoles(roles);
            var modules = await GetModules();

            return Ok(new { roles, permissions, modules });
        }

        private Dictionary<string, Dictionary<string, bool>> GetPermissionsForRoles(IList<string> roles)
        {
            // Implement your logic to get permissions based on roles
            // This is a placeholder example
            var permissions = new Dictionary<string, Dictionary<string, bool>>();

            foreach (var role in roles)
            {
                // Fetch permissions for each role from your database or service
                // This is just a placeholder logic
                permissions[role] = new Dictionary<string, bool>
            {
                { "canAdd", role == "Admin" },
                { "canEdit", role == "Admin" },
                { "canDelete", role == "Admin" },
                { "canView", true }
            };
            }

            return permissions;
        }

        private async Task<List<string>> GetModules()
        {
            // Implement your logic to fetch modules
            // This is a placeholder example
            return new List<string>
        {
            "Stock",
            "StockVehicle",
            "StockPurchase",
            "StockImport",
            "StockClearance",
            "StockSale",

            "ParameterCustomer",
            "ParameterInvoice",
            
                "ReportDisburstment",
            "SalesVehicle",
            // Add other modules
        };
        }


        //[HttpGet("GetRolesAndPermissions")]
        //public async Task<IActionResult> GetRolesAndPermissions()
        //{
        //    try
        //    {
        //        //var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        //        //var roles = await _userManager.GetRolesAsync(user.Profile.AppUser);
        //        // Assume you have a method to get permissions based on roles

        //        var roles = _context.Roles.ToListAsync();
        //        var permissions = GetPermissionsForRoles(roles);

        //        return Ok(new { roles, permissions });

        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        //private Dictionary<string, bool> GetPermissionsForRoles(IList<string> roles)
        //{
        //    // Implement your logic to get permissions based on roles
        //    // This is a placeholder
        //    return new Dictionary<string, bool>
        //{
        //    { "canAdd", roles.Contains("Admin") || roles.Contains("Stock Manager") },
        //    { "canEdit", roles.Contains("Admin") || roles.Contains("Stock Manager") },
        //    { "canDelete", roles.Contains("Admin") },
        //    { "canView", true }
        //};
        //}
    }
}
