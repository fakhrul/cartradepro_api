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
    public class ServiceProvidersController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ServiceProvidersController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/ServiceProviders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceProvider>>> GetServiceProviders()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.ServiceProviders
                .ToListAsync();

            return objs;
        }


        // GET: api/ServiceProviders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceProvider>> GetServiceProvider(Guid id)
        {
            var serviceProvider = await _context.ServiceProviders
                .Include(c=> c.PackageCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (serviceProvider == null)
            {
                return NotFound();
            }

            foreach(var obj in serviceProvider.PackageCategories)
            {
                obj.ServiceProvider = null;
            }
            return serviceProvider;
        }


        // PUT: api/ServiceProviders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceProvider(Guid id, ServiceProvider serviceProvider)
        {
            if (id != serviceProvider.Id)
            {
                return BadRequest();
            }

            _context.Entry(serviceProvider).State = EntityState.Modified;



            try
            {
                await _context.SaveChangesAsync();

                var categories = await _context.Categories.Where(c => c.ServiceProviderId == serviceProvider.Id).ToListAsync();
                _context.Categories.RemoveRange(categories);
                await _context.SaveChangesAsync();

                foreach (var category in serviceProvider.PackageCategories)
                {
                    category.ServiceProviderId = serviceProvider.Id;
                    _context.Categories.Add(category);
                }


                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceProviderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            catch (Exception ex)
            {
                throw ex;

            }





            return NoContent();
        }

        // POST: api/ServiceProviders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServiceProvider>> PostServiceProvider(ServiceProvider serviceProvider)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

              
                _context.ServiceProviders.Add(serviceProvider);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("GetServiceProvider", new { id = serviceProvider.Id }, serviceProvider);
        }

        // DELETE: api/ServiceProviders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceProvider(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var serviceProvider = await _context.ServiceProviders
                .FirstOrDefaultAsync(c => c.Id == id);
            if (serviceProvider == null)
            {
                return NotFound();
            }

            _context.ServiceProviders.Remove(serviceProvider);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool ServiceProviderExists(Guid id)
        {
            return _context.ServiceProviders.Any(e => e.Id == id);
        }
    }
}
