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
    public class SubCompaniesController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public SubCompaniesController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/SubCompanies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubCompany>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.SubCompanies
                .OrderBy(c=> c.Name)
                .ToListAsync();

            return objs;
        }


        // GET: api/SubCompanies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubCompany>> Get(Guid id)
        {
            var obj = await _context.SubCompanies
                .Include(c=> c.BankAccounts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            if(obj.BankAccounts != null)
            {
                foreach(var ba in obj.BankAccounts)
                {
                    ba.SubCompany = null;
                }
            }

            return obj;
        }


        // PUT: api/SubCompanies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, SubCompany obj)
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

        // POST: api/SubCompanies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SubCompany>> Post(SubCompany obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                _context.SubCompanies.Add(obj);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/SubCompanies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var category = await _context.SubCompanies
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _context.SubCompanies.Remove(category);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool IsExists(Guid id)
        {
            return _context.SubCompanies.Any(e => e.Id == id);
        }


        //
        [HttpPut("AddBankAccount/{id}")]
        public async Task<IActionResult> PutAddBankAccount(Guid id, BankAccount obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

            if (user == null)
                return Unauthorized();

            var subCompany = await _context.SubCompanies
                .Include(c => c.BankAccounts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (subCompany == null)
            {
                return NotFound("SubCompany not found.");
            }

            if (obj == null)
            {
                return BadRequest("BankAccount object is null.");
            }

            // Ensure that the SubCompanyId is set correctly
            obj.SubCompanyId = subCompany.Id;

            // Attach the new BankAccount to the context
            await _context.BankAccounts.AddAsync(obj);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return NoContent();
        }


        [HttpPut("UpdateBankAccount/{id}")]
        public async Task<IActionResult> PutUpdateBankAccount(Guid id, BankAccount obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

            if (user == null)
                return Unauthorized();

            var ba = _context.BankAccounts.Find(obj.Id);

            if (ba == null)
            {
                return NotFound("Not found.");
            }

            ba.Name =  obj.Name;
            ba.Address = obj.Address;
            ba.AccountName = obj.AccountName;
            ba.AccountNo = obj.AccountNo;
            ba.AccountType = obj.AccountType;

            _context.Entry(ba).State = EntityState.Modified;



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


        [HttpPut("RemoveBankAccount/{id}")]
        public async Task<IActionResult> PutRemoveBankAccount(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var item = _context.BankAccounts.Find(id);
                _context.BankAccounts.Remove(item);
                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }

    }
}
