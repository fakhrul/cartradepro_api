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
                .Include(c=> c.Vehicle)
                .ThenInclude(c=> c.Brand)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.Model)
                .Include(c=> c.Pricing)
                .Include(c=> c.Vehicle)
                .ThenInclude(c=> c.VehiclePhotoList)
                .OrderByDescending(c=> c.CreatedOn)
                .ToListAsync();



            foreach (var obj in objs)
            {
                foreach (var s in obj.StockStatusHistories)
                    s.Stock = null;
                if (obj.Vehicle != null && obj.Vehicle.Model != null)
                    obj.Vehicle.Model.Brand = null;

                if (obj.Vehicle != null && obj.Vehicle.VehiclePhotoList != null)
                {
                    foreach(var p in obj.Vehicle.VehiclePhotoList)
                    {
                        p.Vehicle = null;
                        p.Document = null;
                    }
                }
            } 




            return objs;
        }

        [HttpGet("GetNextStockNumber")]
        public ActionResult<string> GetNextStockNumber()
        {
            // Get the latest serial number that follows a numeric pattern
            var latestSerialNumber = _context.Stocks
                .OrderByDescending(p => p.CreatedOn)
                .Select(p => p.StockNo)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(latestSerialNumber))
            {
                return "SN0001"; // Default initial serial number
            }

            // Extract numeric part from the latest serial number
            var numericPart = latestSerialNumber.SkipWhile(c => !char.IsDigit(c)).ToArray();
            if (numericPart.Length == 0)
            {
                return latestSerialNumber + "1"; // Fallback if no numeric part found
            }

            var number = new string(numericPart);
            if (int.TryParse(number, out int currentNumber))
            {
                var newNumber = currentNumber + 1;
                var newNumberString = newNumber.ToString().PadLeft(number.Length, '0');

                // Replace the old numeric part with the new incremented numeric part
                return latestSerialNumber.Substring(0, latestSerialNumber.Length - number.Length) + newNumberString;
            }

            return latestSerialNumber + "1"; // Fallback if parsing fails
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
                .Include(c=> c.Purchase)
                .ThenInclude(c=> c.Supplier)
                .Include(c=> c.Import)
                .ThenInclude(c => c.ForwardingAgent)
                .Include(c=> c.Import)
                .ThenInclude(c=> c.BillOfLandingDocuments)
                .Include(c=> c.Clearance)
                .ThenInclude(c=> c.K8Documents)
                .Include(c => c.Clearance)
                .ThenInclude(c => c.K1Documents)
                .Include(c => c.Sale)
                .ThenInclude(c => c.Loan)
                .Include(c => c.Sale)
                .ThenInclude(c => c.Loan.LetterOfUndertakingDocuments)
                .Include(c => c.Sale)
                .ThenInclude(c => c.Loan.Bank)
                .Include(c => c.Sale)
                .ThenInclude(c => c.Customer)
                .Include(c => c.Pricing)
                .Include(c=> c.ArrivalChecklist)
                .ThenInclude(c=> c.ArrivalChecklists)
                .Include(c=> c.Registration)
                .ThenInclude(c=> c.JpjEHakMilikDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.JpjEDaftarDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.PuspakomB2SlipDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.PuspakomB7SlipDocuments)
                .Include(c=> c.Expense)
                .ThenInclude(c=> c.Expenses)
                .Include(c=> c.AdminitrativeCost)
                .ThenInclude(c=> c.AdminitrativeCostItems)
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

            if (obj.Import != null)
                if (obj.Import.BillOfLandingDocuments != null)
                    foreach (var o in obj.Import.BillOfLandingDocuments)
                    {
                        o.Import = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }

            if (obj.Clearance != null)
            {
                if (obj.Clearance.K8Documents != null)
                    foreach (var o in obj.Clearance.K8Documents)
                    {
                        o.Clearance = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }

                if (obj.Clearance.K1Documents != null)
                    foreach (var o in obj.Clearance.K1Documents)
                    {
                        o.Clearance = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
            }

            if(obj.Sale != null)
            {
                if(obj.Sale.Loan != null)
                {
                    if(obj.Sale.Loan.LetterOfUndertakingDocuments != null)
                    {
                        foreach (var o in obj.Sale.Loan.LetterOfUndertakingDocuments)
                        {
                            o.Loan = null;
                            o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                            o.Document.Content = null;
                        }

                    }
                }
            }

            if(obj.ArrivalChecklist != null)
            {
                if(obj.ArrivalChecklist.ArrivalChecklists != null)
                {
                    foreach(var o in obj.ArrivalChecklist.ArrivalChecklists)
                    {
                        o.ArrivalChecklist = null;
                    }
                }
            }

            if (obj.Registration != null)
            {
                if (obj.Registration.JpjEHakMilikDocuments!= null)
                {
                    foreach (var o in obj.Registration.JpjEHakMilikDocuments)
                    {
                        o.Registration = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }

                if (obj.Registration.JpjEDaftarDocuments != null)
                {
                    foreach (var o in obj.Registration.JpjEDaftarDocuments)
                    {
                        o.Registration = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }

                if (obj.Registration.PuspakomB2SlipDocuments != null)
                {
                    foreach (var o in obj.Registration.PuspakomB2SlipDocuments)
                    {
                        o.Registration = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }

                if (obj.Registration.PuspakomB7SlipDocuments != null)
                {
                    foreach (var o in obj.Registration.PuspakomB7SlipDocuments)
                    {
                        o.Registration = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }
            }

            if (obj.Expense != null)
            {
                if (obj.Expense.Expenses != null)
                {
                    foreach (var o in obj.Expense.Expenses)
                    {
                        o.Expense = null;
                    }
                }
            }

            if (obj.AdminitrativeCost != null)
            {
                if (obj.AdminitrativeCost.AdminitrativeCostItems != null)
                {
                    foreach (var o in obj.AdminitrativeCost.AdminitrativeCostItems)
                    {
                        o.AdminitrativeCost = null;
                    }
                }
            }

            if(obj.Vehicle != null && obj.Vehicle.Brand != null)
            {
                obj.Vehicle.Brand.Models = null;

            }
            if (obj.Vehicle != null && obj.Vehicle.Model != null)
            {
                obj.Vehicle.Model.Brand = null;
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
            _context.Entry(obj.Purchase).State = EntityState.Modified;
            _context.Entry(obj.Import).State = EntityState.Modified;
            _context.Entry(obj.Clearance).State = EntityState.Modified;
            _context.Entry(obj.Sale).State = EntityState.Modified;
            _context.Entry(obj.Sale.Loan).State = EntityState.Modified;
            _context.Entry(obj.Registration).State = EntityState.Modified;
            _context.Entry(obj.Pricing).State = EntityState.Modified;

            //_context.Entry(obj.SellingPricing).State = EntityState.Modified;


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
                //_context.Vehicles.Add(vehicle);
                obj.VehicleId = vehicle.Id;

                var purchase = new Purchase();
                //_context.Purchases.Add(purchase);
                obj.PurchaseId = purchase.Id;

                var import = new Import();
                //_context.Imports.Add(import);
                obj.ImportId = import.Id;

                var clearance = new Clearance();
                //_context.Clearances.Add(clearance);
                obj.ClearanceId = clearance.Id;

                var pricing = new Pricing();
                _context.Pricings.Add(pricing);
                obj.PricingId = pricing.Id;

                var arrivalCheckList = new ArrivalChecklist();

                if (arrivalCheckList.ArrivalChecklists == null)
                    arrivalCheckList.ArrivalChecklists = new List<ArrivalChecklistItem>();

                arrivalCheckList.ArrivalChecklists.Add(new ArrivalChecklistItem
                {
                    ArrivalChecklistId = arrivalCheckList.Id,
                    Name = "Extra Key",
                    IsAvailable = true,
                    Remarks = ""
                }) ;
                _context.ArrivalChecklists.Add(arrivalCheckList);
                obj.ArrivalChecklistId = arrivalCheckList.Id;

                //var sellingPrice = new SellingPricing();
                //obj.SellingPricingId = sellingPrice.Id;

                //var loan = new Loan();
                //_context.Loans.Add(loan);
                var sale = new Sale();
                //var loan = new Loan();
                //sale.LoanId = loan.Id;
                //_context.Sales.Add(sale);
                obj.Sale = sale;
                obj.SaleId = sale.Id;

                var loan = new Loan();
                _context.Loans.Add(loan);
                obj.Sale.LoanId = loan.Id;


                var registration = new Registration();
                _context.Registrations.Add(registration);
                //_context.Imports.Add(import);
                obj.RegistrationId = registration.Id;


                var expense = new Expense();
                _context.Expenses.Add(expense);
                obj.ExpenseId = expense.Id;



                var administrativeCost = new AdminitrativeCost();
                _context.AdminitrativeCosts.Add(administrativeCost);
                obj.AdminitrativeCostId = administrativeCost.Id;

                //Stocks
                _context.Stocks.Add(obj);


                await _context.SaveChangesAsync();


                //var loan = new Loan();
                //_context.Loans.Add(loan);
                //sale.LoanId = loan.Id;

                //_context.Entry(sale).State = EntityState.Modified;
                //await _context.SaveChangesAsync();
                //var loan = new Loan();
                //sale.LoanId = loan.Id;

                foreach (var stockStatusHistory in obj.StockStatusHistories)
                    stockStatusHistory.Stock = null;

                foreach (var o in obj.ArrivalChecklist.ArrivalChecklists)
                    o.ArrivalChecklist = null;
                if(obj.AdminitrativeCost.AdminitrativeCostItems != null)
                foreach (var o in obj.AdminitrativeCost.AdminitrativeCostItems)
                    o.AdminitrativeCost = null;

                obj.LatestStockStatus.Stock = null;
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

            foreach (var obj in objs)
            {
                _context.VehiclePhotos.Add(new VehiclePhoto
                {
                    VehicleId = stock.VehicleId,
                    DocumentId = obj.Id
                });

            }


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

        [HttpPut("UpdateBolDocuments/{id}")]
        public async Task<IActionResult> PutUpdateBolDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Import)
                .ThenInclude(c => c.BillOfLandingDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.BillOfLandingDocuments.Add(new  BillOfLandingDocument
                {
                    ImportId = stock.ImportId,
                    DocumentId = obj.Id
                });

            }


            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Import).State = EntityState.Modified;


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


        [HttpPut("RemoveBolDocument/{importId}/{documentId}")]
        public async Task<IActionResult> PutRemoveBolDocument(Guid importId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.BillOfLandingDocuments
                    .Where(c => c.ImportId == importId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.BillOfLandingDocuments.Remove(inDataBase);
                }

                var document = _context.Documents.Find(documentId);

                    _context.Documents.Remove(document);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }


        [HttpPut("UpdateK8Documents/{id}")]
        public async Task<IActionResult> PutUpdateK8Documents(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Clearance)
                .ThenInclude(c => c.K8Documents)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.K8Documents.Add(new K8Document
                {
                    ClearanceId = stock.ClearanceId,
                    DocumentId = obj.Id
                });

            }


            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Clearance).State = EntityState.Modified;


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


        [HttpPut("RemoveK8Document/{clearanceId}/{documentId}")]
        public async Task<IActionResult> PutRemoveK8Document(Guid clearanceId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.K8Documents
                    .Where(c => c.ClearanceId == clearanceId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.K8Documents.Remove(inDataBase);
                }

                var document = _context.Documents.Find(documentId);

                _context.Documents.Remove(document);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }

        [HttpPut("UpdateLouDocuments/{id}")]
        public async Task<IActionResult> PutUpdateLouDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c=> c.Sale)
                .Include(c => c.Sale.Loan)
                .ThenInclude(c => c.LetterOfUndertakingDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.LetterOfUndertakingDocuments.Add(new LetterOfUndertakingDocument
                {
                    LoanId = (Guid)stock.Sale.LoanId,
                    DocumentId = obj.Id
                });

            }


            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Sale).State = EntityState.Modified;
            _context.Entry(stock.Sale.Loan).State = EntityState.Modified;


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


        [HttpPut("RemoveLouDocument/{loanId}/{documentId}")]
        public async Task<IActionResult> PutRemoveLouDocument(Guid loanId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.LetterOfUndertakingDocuments
                    .Where(c => c.LoanId == loanId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.LetterOfUndertakingDocuments.Remove(inDataBase);
                }

                var document = _context.Documents.Find(documentId);

                _context.Documents.Remove(document);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }


        [HttpPut("AddArrivalCheckLisItem/{id}")]
        public async Task<IActionResult> PutAddArrivalCheckLisItem(Guid id, ArrivalChecklistItemDto obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.ArrivalChecklist)
                .ThenInclude(c=> c.ArrivalChecklists)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            var arrivalChecklist = new ArrivalChecklistItem
            {
                ArrivalChecklistId = stock.ArrivalChecklistId,
                Name = obj.Name,
                IsAvailable = obj.IsAvailable,
                Remarks = obj.Remarks,
            };

            _context.ArrivalChecklistItems.Add(arrivalChecklist);

            stock.ArrivalChecklist.ArrivalChecklists.Add(arrivalChecklist);


            _context.Entry(stock.ArrivalChecklist).State = EntityState.Modified;


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

        [HttpPut("RemoveArrivalCheckLisItem/{id}")]
        public async Task<IActionResult> PutRemoveArrivalCheckLisItem(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var item = _context.ArrivalChecklistItems.Find(id);

                _context.ArrivalChecklistItems.Remove(item);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }


        [HttpPut("UpdateJpjHakMilikDocuments/{id}")]
        public async Task<IActionResult> PutUpdateJpjHakMilikDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.JpjEHakMilikDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.JpjEHakMilikDocuments.Add(new JpjEHakMilikDocument
                {
                    RegistrationId= stock.RegistrationId,
                    DocumentId = obj.Id
                });

            }


            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Registration).State = EntityState.Modified;


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


        [HttpPut("RemoveJpjHakMilikDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemoveJpjHakMilikDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.JpjEHakMilikDocuments
                    .Where(c => c.RegistrationId== registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.JpjEHakMilikDocuments.Remove(inDataBase);
                }

                var document = _context.Documents.Find(documentId);

                _context.Documents.Remove(document);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }


        //

        [HttpPut("UpdateJpjDaftarDocuments/{id}")]
        public async Task<IActionResult> PutUpdateJpjDaftarDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.JpjEDaftarDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.JpjEDaftarDocuments.Add(new JpjEDaftarDocument
                {
                    RegistrationId = stock.RegistrationId,
                    DocumentId = obj.Id
                });

            }


            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Registration).State = EntityState.Modified;


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


        [HttpPut("RemoveJpjDaftarDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemoveDaftarDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.JpjEDaftarDocuments
                    .Where(c => c.RegistrationId == registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.JpjEDaftarDocuments.Remove(inDataBase);
                }

                var document = _context.Documents.Find(documentId);

                _context.Documents.Remove(document);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }

        //
        [HttpPut("UpdatePuspakomB2SlipDocuments/{id}")]
        public async Task<IActionResult> PutUpdatePuspakomB2SlipDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.PuspakomB2SlipDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.PuspakomB2SlipDocuments.Add(new PuspakomB2SlipDocument
                {
                    RegistrationId = stock.RegistrationId,
                    DocumentId = obj.Id
                });

            }


            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Registration).State = EntityState.Modified;


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


        [HttpPut("RemovePuspakomB2SlipDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemovePuspakomB2SlipDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.PuspakomB2SlipDocuments
                    .Where(c => c.RegistrationId == registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.PuspakomB2SlipDocuments.Remove(inDataBase);
                }

                var document = _context.Documents.Find(documentId);

                _context.Documents.Remove(document);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }

        //
        [HttpPut("UpdatePuspakomB7SlipDocuments/{id}")]
        public async Task<IActionResult> PutUpdatePuspakomB7SlipDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.PuspakomB7SlipDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.PuspakomB7SlipDocuments.Add(new PuspakomB7SlipDocument
                {
                    RegistrationId = stock.RegistrationId,
                    DocumentId = obj.Id
                });

            }


            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Registration).State = EntityState.Modified;


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


        [HttpPut("RemovePuspakomB7SlipDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemovePuspakomB7SlipDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.PuspakomB7SlipDocuments
                    .Where(c => c.RegistrationId == registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.PuspakomB7SlipDocuments.Remove(inDataBase);
                }

                var document = _context.Documents.Find(documentId);

                _context.Documents.Remove(document);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }



        [HttpPut("AddExpenseItem/{id}")]
        public async Task<IActionResult> PutAddExpenseItem(Guid id, ExpenseItemDto obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Expense)
                .ThenInclude(c => c.Expenses)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            var arrivalChecklist = new ExpenseItem
            {
                ExpenseId = stock.ExpenseId,
                Name = obj.Name,
                Amount = obj.Amount,
                Remarks = obj.Remarks,
            };

            _context.ExpenseItems.Add(arrivalChecklist);
            stock.Expense.Expenses.Add(arrivalChecklist);

            _context.Entry(stock.Expense).State = EntityState.Modified;


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

        [HttpPut("RemoveExpenseItem/{id}")]
        public async Task<IActionResult> PutRemoveExpenseItem(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var item = _context.ExpenseItems.Find(id);

                _context.ExpenseItems.Remove(item);

                await _context.SaveChangesAsync();

            }

            catch (Exception ex)
            {
                throw ex;

            }


            return NoContent();
        }


        [HttpPut("AddAdministrativeCostItem/{id}")]
        public async Task<IActionResult> PutAddAdministrativeCostItem(Guid id, ExpenseItemDto obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.AdminitrativeCost)
                .ThenInclude(c => c.AdminitrativeCostItems)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            var arrivalChecklist = new AdminitrativeCostItem
            {
                 AdminitrativeCostId = stock.AdminitrativeCostId,
                Name = obj.Name,
                Amount = obj.Amount,
                Remarks = obj.Remarks,
            };

            _context.AdminitrativeCostItems.Add(arrivalChecklist);
            stock.AdminitrativeCost.AdminitrativeCostItems.Add(arrivalChecklist);

            _context.Entry(stock.AdminitrativeCost).State = EntityState.Modified;


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

        [HttpPut("RemoveAdministrativeCostItem/{id}")]
        public async Task<IActionResult> PutRemoveAdministrativeCostItem(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var item = _context.AdminitrativeCostItems.Find(id);

                _context.AdminitrativeCostItems.Remove(item);

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
