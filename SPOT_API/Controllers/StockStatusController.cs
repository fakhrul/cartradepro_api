﻿using System;
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
    public class StockStatusController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public StockStatusController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/StockStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockStatus>>> GetAll()
        {
            //var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            //if (user == null)
            //    return Unauthorized();

            var objs = await _context.StockStatuses
                .ToListAsync();

            return objs;
        }


        // GET: api/StockStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockStatus>> Get(Guid id)
        {
            var category = await _context.StockStatuses
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }


        // PUT: api/StockStatus/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, StockStatus obj)
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

        // POST: api/StockStatus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StockStatus>> Post(StockStatus obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

              
                _context.StockStatuses.Add(obj);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/StockStatus/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var category = await _context.StockStatuses
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _context.StockStatuses.Remove(category);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool IsExists(Guid id)
        {
            return _context.StockStatuses.Any(e => e.Id == id);
        }




    }
}
