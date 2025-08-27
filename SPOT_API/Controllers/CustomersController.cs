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

        public CustomersController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
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

            _context.Entry(obj).State = EntityState.Modified;



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(id))
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

            }
            catch (Exception ex)
            {
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



                var category = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                {
                    return NotFound();
                }

                _context.Customers.Remove(category);

                var sales = await _context.Sales
                    .Where(c => c.CustomerId == id)
                    .ToListAsync();

                foreach(var sale in sales)
                {
                    sale.CustomerId = null;
                    sale.Customer = null;
                }
                await _context.SaveChangesAsync();


                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        private bool IsExists(Guid id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }


    }
}
