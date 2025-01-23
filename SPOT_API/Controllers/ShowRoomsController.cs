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
    public class ShowRoomsController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ShowRoomsController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/ShowRooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShowRoom>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.ShowRooms
                .OrderBy(c=> c.Name)
                .ToListAsync();

            return objs;
        }


        // GET: api/ShowRooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ShowRoom>> Get(Guid id)
        {
            var obj = await _context.ShowRooms
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            return obj;
        }


        // PUT: api/ShowRooms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, ShowRoom obj)
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

        // POST: api/ShowRooms
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ShowRoom>> Post(ShowRoom obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                _context.ShowRooms.Add(obj);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/ShowRooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();



                var category = await _context.ShowRooms
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                {
                    return NotFound();
                }


                //var purchases = await _context.Purchases
                //    .Where(c => c.ShowRoomId == id)
                //    .ToListAsync();

                //foreach(var purchase in purchases)
                //{
                //    purchase.ShowRoomId = null;
                //}

                _context.ShowRooms.Remove(category);
                await _context.SaveChangesAsync();


                return NoContent();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private bool IsExists(Guid id)
        {
            return _context.ShowRooms.Any(e => e.Id == id);
        }


    }
}
