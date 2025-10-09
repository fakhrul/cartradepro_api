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
    public class StockToBuysController : ControllerBase
    {
        private readonly SpotDBContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly UserManager<AppUser> _userManager;

        public StockToBuysController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
        }

        // GET: api/StockToBuys
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<StockToBuy>>> GetAll()
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        //    if (user == null)
        //        return Unauthorized();

        //    var objs = await _context.StockToBuys
        //        .OrderBy(c=> c.Name)
        //        .ToListAsync();

        //    return objs;
        //}

        // GET: api/StockToBuys
        [HttpGet]
        public async Task<ActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10,
            [FromQuery] string sortColumn = "StockNo",
            [FromQuery] bool sortAsc = true,
            [FromQuery] string search = null,
            [FromQuery] Dictionary<string, string> filters = null)
        {
            // Retrieve the current user
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                // Base query
                var query = _context.StockToBuys
                    .Include(c=> c.SubCompany)
                    .Include(c=> c.Supplier)
                    .Where(c=> c.IsActive)
                    .AsQueryable();

                // Apply search
                //if (!string.IsNullOrEmpty(search))
                //{
                //    query = query.Where(c => c.Name.Contains(search) || c.Email.Contains(search));
                //}

                //// Apply filters
                //if (filters != null)
                //{
                //    foreach (var filter in filters)
                //    {
                //        if (filter.Key == "icNumber" && !string.IsNullOrEmpty(filter.Value))
                //        {
                //            query = query.Where(c => c.IcNumber.Contains(filter.Value));
                //        }
                //        if (filter.Key == "phone" && !string.IsNullOrEmpty(filter.Value))
                //        {
                //            query = query.Where(c => c.Phone.Contains(filter.Value));
                //        }
                //        // Add more filters as needed
                //    }
                //}

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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        // PUT: api/Stocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("imageposition/{id}")]
        public async Task<IActionResult> PutImagePosition(Guid id, StockToBuy obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            var photoIds = obj.VehiclePhotoList.Select(u => u.Id).ToList();
            var vehiclePhotos = await _context.StockToBuyPhotos // or Documents, depending on your entity
                                        .Where(p => photoIds.Contains(p.Id))
                                        .ToListAsync();

            // Update the positions based on the incoming data
            foreach (var update in obj.VehiclePhotoList)
            {
                var photo = vehiclePhotos.FirstOrDefault(p => p.Id == update.Id);
                if (photo != null)
                {
                    photo.Position = update.Position;
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Vehicle photo positions updated successfully.");
        }

        [HttpDelete("image/{id}")]
        public async Task<IActionResult> DeleteVehicleImage(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var vehicleImage = await _context.StockToBuyPhotos
                .FirstOrDefaultAsync(c => c.Id == id);
            if (vehicleImage == null)
            {
                return NotFound();
            }

            _context.StockToBuyPhotos.Remove(vehicleImage);
            await _context.SaveChangesAsync();


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

            var stock = await _context.StockToBuys
                .Include(c => c.VehiclePhotoList)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.StockToBuyPhotos.Add(new StockToBuyPhoto
                {
                     StockToBuyId = stock.Id,
                    DocumentId = obj.Id
                });

            }


            _context.Entry(stock).State = EntityState.Modified;
            //_context.Entry(stock.Vehicle).State = EntityState.Modified;


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



        // GET: api/StockToBuys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StockToBuy>> Get(Guid id)
        {
            var obj = await _context.StockToBuys
                .Include(c=> c.SubCompany)
                .Include(c=> c.Supplier)
                .Include(c=> c.VehiclePhotoList.OrderBy(c => c.Position))
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            if(obj.VehiclePhotoList != null)
            foreach (var o in obj.VehiclePhotoList)
            {
                o.StockToBuy = null;
            }

            return obj;
        }

        // PUT: api/Stocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("defaultimage/{id}")]
        public async Task<IActionResult> PutDefaultImage(Guid id, StockToBuy obj)
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


        // PUT: api/StockToBuys/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, StockToBuy obj)
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

        // POST: api/StockToBuys
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StockToBuy>> Post(StockToBuy obj)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();


                _context.StockToBuys.Add(obj);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
return BadRequest(ex.Message);
            }

            return CreatedAtAction("Get", new { id = obj.Id }, obj);
        }

        // DELETE: api/StockToBuys/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();



                var category = await _context.StockToBuys
                    .FirstOrDefaultAsync(c => c.Id == id);
                if (category == null)
                {
                    return NotFound();
                }

                _context.StockToBuys.Remove(category);
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
            return _context.StockToBuys.Any(e => e.Id == id);
        }


        [HttpPut("MoveToStock/{id}")]
        public async Task<IActionResult> PutMoveToStock(Guid id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                    if (user == null)
                        return Unauthorized();

                    var toBuy = _context.StockToBuys.Include(c => c.VehiclePhotoList).FirstOrDefault(c => c.Id == id);

                    if (toBuy == null)
                        return NotFound();


                    var obj = new Stock();

                    obj.StockNo = toBuy.StockNo;

                    if (obj.StockStatusHistories == null)
                        obj.StockStatusHistories = new List<StockStatusHistory>();

                    var stockStatus = await _context.StockStatuses.FirstOrDefaultAsync(c => c.Name == "Draft");
                    obj.StockStatusHistories.Add(new StockStatusHistory
                    {
                        ProfileId = (Guid)user.ProfileId,
                        StockStatusId = stockStatus.Id,
                        StockId = obj.Id
                    });

                    // Initialize other entities
                    obj.Vehicle = new Vehicle();
                    obj.VehicleId = obj.Vehicle.Id;
                    obj.Vehicle.Description = toBuy.Description;
                    obj.Vehicle.DefaultImageUrl = toBuy.DefaultImageUrl;
                    obj.Vehicle.Color = toBuy.Color;

                    foreach(var item in toBuy.VehiclePhotoList)
                    {
                        _context.VehiclePhotos.Add(new VehiclePhoto
                        {
                            VehicleId = obj.VehicleId,
                            DocumentId = item.DocumentId
                        });
                    }

                    obj.Purchase= new Purchase();
                    obj.Purchase.SupplierId = toBuy.SupplierId;
                    obj.Purchase.SupplierCurrency = toBuy.SupplierCurrency;
                    obj.Purchase.VehiclePriceSupplierCurrency = toBuy.VehiclePriceSupplierCurrency;
                    obj.Purchase.VehiclePriceLocalCurrency = toBuy.VehiclePriceLocalCurrency;
                    obj.Purchase.BodyPriceLocalCurrency = toBuy.BodyPriceLocalCurrency;
                    obj.PurchaseId = obj.Purchase.Id;

                    var import = new Import();
                    
                    obj.ImportId = import.Id;
                    await _context.Imports.AddAsync(import);

                    var clearance = new Clearance();
                    obj.ClearanceId = clearance.Id;
                    await _context.Clearances.AddAsync(clearance);

                    var pricing = new Pricing();
                    await _context.Pricings.AddAsync(pricing);
                    obj.PricingId = pricing.Id;

                    // Arrival checklist
                    var arrivalCheckList = new ArrivalChecklist();
                    arrivalCheckList.ArrivalChecklists = new List<ArrivalChecklistItem>
            {
                new ArrivalChecklistItem
                {
                    ArrivalChecklistId = arrivalCheckList.Id,
                    Name = "Extra Key",
                    IsAvailable = true,
                    Remarks = ""
                }
            };
                    await _context.ArrivalChecklists.AddAsync(arrivalCheckList);
                    obj.ArrivalChecklistId = arrivalCheckList.Id;

                    // Sale & Loan
                    var sale = new Sale { SaleDateTime = DateTime.Now };
                    var loan = new Loan();
                    sale.LoanId = loan.Id;
                    obj.Sale = sale;
                    obj.SaleId = sale.Id;
                    await _context.Loans.AddAsync(loan);

                    // Registration
                    var registration = new Registration();
                    await _context.Registrations.AddAsync(registration);
                    obj.RegistrationId = registration.Id;

                    // Expense
                    var expense = new Expense();
                    await _context.Expenses.AddAsync(expense);
                    obj.ExpenseId = expense.Id;

                    var ads = new Advertisement();
                    await _context.Advertisements.AddAsync(ads);
                    obj.AdvertisementId = ads.Id;

                    // Administrative cost
                    var administrativeCost = new AdminitrativeCost();
                    await _context.AdminitrativeCosts.AddAsync(administrativeCost);
                    obj.AdminitrativeCostId = administrativeCost.Id;


                    var apCompany = new ApCompany();
                    apCompany.SubCompanyId = toBuy.SubCompanyId;
                    await _context.ApCompanies.AddAsync(apCompany);
                    obj.ApCompanyId = apCompany.Id;

                    // Add stock object
                    await _context.Stocks.AddAsync(obj);

                    toBuy.IsActive = false;
                    _context.Entry(toBuy).State = EntityState.Modified;


                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // Nullify circular references for clean JSON response
                    foreach (var stockStatusHistory in obj.StockStatusHistories)
                        stockStatusHistory.Stock = null;

                    foreach (var item in obj.ArrivalChecklist.ArrivalChecklists)
                        item.ArrivalChecklist = null;

                    if (obj.AdminitrativeCost.AdminitrativeCostItems != null)
                        foreach (var item in obj.AdminitrativeCost.AdminitrativeCostItems)
                            item.AdminitrativeCost = null;

                    obj.LatestStockStatus.Stock = null;

                    if (obj.Vehicle != null && obj.Vehicle.VehiclePhotoList != null)
                    {
                        foreach (var p in obj.Vehicle.VehiclePhotoList)
                        {
                            p.Vehicle = null;
                            p.Document = null;
                        }
                    }

                    return CreatedAtAction("Get", new { id = obj.Id }, obj);

                }
                catch (DbUpdateException dbEx)
                {
                    // Rollback transaction in case of failure
                    await transaction.RollbackAsync();

                    // Check for the inner exception for more details
                    if (dbEx.InnerException != null)
                    {
                        var sqlException = dbEx.InnerException; // Could be SqlException or another type based on the database provider
                        return BadRequest($"A database error occurred: {sqlException.Message}");
                    }

                    return BadRequest("An error occurred while saving the entity changes.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(ex.Message); // General exception fallback
                }

            }


        }

    }
}
