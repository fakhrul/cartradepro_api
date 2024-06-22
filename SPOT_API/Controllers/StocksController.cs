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
    public class StocksController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public StocksController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/Stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetAll()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var objs = await _context.Stocks
                .Include(c => c.StockStatusHistories.OrderByDescending(c => c.DateTime))
                .ThenInclude(c => c.StockStatus)
                .ToListAsync();



            foreach (var obj in objs)
                foreach (var s in obj.StockStatusHistories)
                    s.Stock = null;


            return objs;
        }


        // GET: api/Stocks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> Get(Guid id)
        {
            var obj = await _context.Stocks
                .Include(c => c.StockStatusHistories.OrderByDescending(c => c.DateTime))
                .ThenInclude(c => c.Profile)
                .Include(c => c.StockStatusHistories.OrderByDescending(c => c.DateTime))
                .ThenInclude(c => c.StockStatus)
                .Include(c => c.RemarksList)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.Brand)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.Model)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehicleType)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehiclePhotoList)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            if (obj.RemarksList != null)
                foreach (var s in obj.RemarksList)
                    s.Stock = null;

            if (obj.StockStatusHistories != null)
                foreach (var s in obj.StockStatusHistories)
                    s.Stock = null;

            if (obj.Vehicle != null)
                if (obj.Vehicle.VehiclePhotoList != null)
                    foreach (var o in obj.Vehicle.VehiclePhotoList)
                    {
                        o.Vehicle = null;
                        //o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                    }
            return obj;
        }


        // PUT: api/Stocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Stock obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            _context.Entry(obj).State = EntityState.Modified;
            _context.Entry(obj.Vehicle).State = EntityState.Modified;



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

        // POST: api/Stocks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Stock>> Post(Stock obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();

                if (obj.StockStatusHistories == null)
                    obj.StockStatusHistories = new List<StockStatusHistory>();

                var stockStatus = _context.StockStatuses.FirstOrDefault(c => c.Name == "Draft");

                obj.StockStatusHistories.Add(new StockStatusHistory
                {
                    ProfileId = (Guid)user.ProfileId,
                    StockStatusId = stockStatus.Id,
                    //Name = "Draft",
                    StockId = obj.Id
                });


                var vehicle = new Vehicle();
                _context.Vehicles.Add(vehicle);

                obj.VehicleId = vehicle.Id;
                _context.Stocks.Add(obj);
                await _context.SaveChangesAsync();


                foreach (var stockStatusHistory in obj.StockStatusHistories)
                    stockStatusHistory.Stock = null;
            }
            catch (Exception ex)
            {
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var category = await _context.Stocks
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(category);
            await _context.SaveChangesAsync();


            return NoContent();
        }

        private bool IsExists(Guid id)
        {
            return _context.Stocks.Any(e => e.Id == id);
        }


        [HttpPut("UpdateStockStatus/{id}")]
        public async Task<IActionResult> PutUpdateStockStatus(Guid id, StockStatusDto obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.StockStatusHistories)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            var newStatus = new StockStatusHistory
            {
                ProfileId = (Guid)user.ProfileId,
                StockStatusId = obj.StockStatusId,
                //Name = "Draft",
                StockId = id
            };

            _context.StockStatusHistories.Add(newStatus);

            stock.StockStatusHistories.Add(newStatus);
            await _context.SaveChangesAsync();


            _context.Entry(stock).State = EntityState.Modified;


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

        [HttpPut("UpdateRemarks/{id}")]
        public async Task<IActionResult> PutUpdateRemarks(Guid id, RemarkDto obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.RemarksList)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            var newRemark = new Remarks
            {
                ProfileId = (Guid)user.ProfileId,
                StockId = id,
                Description = obj.Remark
            };

            _context.Remarks.Add(newRemark);


            if (stock.RemarksList == null)
                stock.RemarksList = new List<Remarks>();

            stock.RemarksList.Add(newRemark);

            await _context.SaveChangesAsync();

            _context.Entry(stock).State = EntityState.Modified;


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



        [HttpPut("UpdateVehicleImages/{id}")]
        public async Task<IActionResult> PutUpdateVehicleImages(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehiclePhotoList)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            //_context.VehiclePhotos.Add
            //if (stock.Vehicle.VehiclePhotoList == null)
            //    stock.Vehicle.VehiclePhotoList = new List<VehiclePhoto>();

            foreach (var obj in objs)
            {
                _context.VehiclePhotos.Add(new VehiclePhoto
                {
                    VehicleId = stock.VehicleId,
                    DocumentId = obj.Id
                });

            }


            //var newStatus = new StockStatusHistory
            //{
            //    ProfileId = (Guid)user.ProfileId,
            //    StockStatusId = obj.StockStatusId,
            //    //Name = "Draft",
            //    StockId = id
            //};

            //_context.StockStatusHistories.Add(newStatus);

            //stock.StockStatusHistories.Add(newStatus);
            //await _context.SaveChangesAsync();


            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Vehicle).State = EntityState.Modified;


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
                throw ex;

            }
            return NoContent();
        }

    }
}
