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
        private readonly Services.StockImportService _importService;

        public StocksController(SpotDBContext context, IUserAccessor userAccessor, UserManager<AppUser> userManager, Services.StockImportService importService)
        {
            _context = context;
            _userAccessor = userAccessor;
            _userManager = userManager;
            _importService = importService;
        }

        // GET: api/Stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetAll(
           [FromQuery] int page = 1,
            [FromQuery] int itemsPerPage = 10,
            [FromQuery] string sortColumn = "MovedToStockAt",
            [FromQuery] bool sortAsc = false,
            [FromQuery] string search = null,
            [FromQuery] string priceMin = null,
            [FromQuery] string priceMax = null,
            [FromQuery] bool isOpen = false,
            [FromQuery] string monthFrom = null,
            [FromQuery] string yearFrom = null,
            [FromQuery] string monthTo = null,
            [FromQuery] string yearTo = null,
            [FromQuery] Dictionary<string, string> filters = null)

        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                if (user == null)
                    return Unauthorized();


                // Start building the query
                var query = _context.Stocks
                    .Include(c => c.StockStatusHistories.OrderByDescending(c => c.DateTime))
                    .ThenInclude(c => c.StockStatus)
                    .Include(c => c.Vehicle)
                    .ThenInclude(c => c.Brand)
                    .Include(c => c.Vehicle)
                    .ThenInclude(c => c.Model)
                    .Include(c => c.Pricing)
                    .Include(c => c.Vehicle)
                    .Include(c => c.Registration)
                    .Include(c => c.AdminitrativeCost)
                    .Include(_ => _.Sale)
                    .ThenInclude(_ => _.Customer)
                    .Include(c => c.Purchase)
                    .Include(c => c.Sale)
                    .ThenInclude(c => c.Loan)
                    .ThenInclude(c => c.Bank)
                    .Include(c => c.Clearance)
                    .ThenInclude(c => c.K8Documents)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Clearance)
                    .ThenInclude(c => c.K1Documents)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Clearance)
                    .ThenInclude(c => c.ExportCertificateDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Sale)
                    .ThenInclude(c => c.CustomerIcDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Registration)
                    .ThenInclude(c => c.InsuranceDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Registration)
                    .ThenInclude(c => c.RoadTaxDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Registration)
                    .ThenInclude(c => c.ReceiptEDaftarDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Registration)
                    .ThenInclude(c => c.ReceiptKastamDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Registration)
                    .ThenInclude(c => c.PuspakomB2SlipDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Registration)
                    .ThenInclude(c => c.JpjGeranDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Expense)
                    .ThenInclude(c => c.Expenses)
                    .Include(c => c.ShowRoom)
                                    .Include(c => c.Import)
                    .ThenInclude(c => c.BillOfLandingDocuments)
                    .ThenInclude(c => c.Document)
                    .Include(c => c.Advertisement)
                    .OrderBy(c => c.MovedToStockAt)
                    .AsQueryable();

                // Apply Brand Filter (case-insensitive)
                if (!string.IsNullOrEmpty(priceMin))
                {
                    try
                    {
                        query = query.Where(s => s.Pricing.RecommendedSalePrice >= decimal.Parse(priceMin));

                    }
                    catch (Exception ex)
                    {
                    }
                }

                if (!string.IsNullOrEmpty(priceMax))
                {
                    try
                    {
                        query = query.Where(s => s.Pricing.RecommendedSalePrice <= decimal.Parse(priceMax));

                    }
                    catch (Exception ex)
                    {
                    }
                }

                // Apply Month/Year Range Filter (From Date)
                // If year is specified but month is not, default to January (1)
                if (!string.IsNullOrEmpty(yearFrom))
                {
                    try
                    {
                        // Default to January if month not specified
                        string effectiveMonthFrom = string.IsNullOrEmpty(monthFrom) ? "1" : monthFrom;

                        // Parse and pad values for string comparison
                        string fromYearPadded = yearFrom.PadLeft(4, '0');
                        string fromMonthPadded = effectiveMonthFrom.PadLeft(2, '0');
                        string fromDateString = fromYearPadded + fromMonthPadded; // e.g., "202001"

                        query = query.Where(s =>
                            s.Vehicle != null &&
                            !string.IsNullOrEmpty(s.Vehicle.Year) &&
                            !string.IsNullOrEmpty(s.Vehicle.Month) &&
                            (s.Vehicle.Year.PadLeft(4, '0') + s.Vehicle.Month.PadLeft(2, '0')).CompareTo(fromDateString) >= 0
                        );
                    }
                    catch (Exception ex)
                    {
                        // Log or handle invalid month/year input
                    }
                }

                // Apply Month/Year Range Filter (To Date)
                // If year is specified but month is not, default to December (12)
                if (!string.IsNullOrEmpty(yearTo))
                {
                    try
                    {
                        // Default to December if month not specified
                        string effectiveMonthTo = string.IsNullOrEmpty(monthTo) ? "12" : monthTo;

                        // Parse and pad values for string comparison
                        string toYearPadded = yearTo.PadLeft(4, '0');
                        string toMonthPadded = effectiveMonthTo.PadLeft(2, '0');
                        string toDateString = toYearPadded + toMonthPadded; // e.g., "202412"

                        query = query.Where(s =>
                            s.Vehicle != null &&
                            !string.IsNullOrEmpty(s.Vehicle.Year) &&
                            !string.IsNullOrEmpty(s.Vehicle.Month) &&
                            (s.Vehicle.Year.PadLeft(4, '0') + s.Vehicle.Month.PadLeft(2, '0')).CompareTo(toDateString) <= 0
                        );
                    }
                    catch (Exception ex)
                    {
                        // Log or handle invalid month/year input
                    }
                }

                // Apply Model Filter (case-insensitive)
                //if (!string.IsNullOrEmpty(model))
                //{
                //    query = query.Where(s => s.Vehicle.Model.Name.ToLower().Contains(model.ToLower()));
                //}

                // Apply Status Filter (case-insensitive)
                //if (!string.IsNullOrEmpty(status))
                //{
                //    query = query.Where(s => s.StockStatusHistories.Any(h => h.StockStatus.Name.ToLower().Contains(status.ToLower())));
                //}
                // Apply filters
                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        if (filter.Key == "stockNo" && !string.IsNullOrEmpty(filter.Value))
                        {
                            query = query.Where(c => c.StockNo.ToLower().Contains(filter.Value.ToLower()));
                        }

                        if (filter.Key == "lotNo" && !string.IsNullOrEmpty(filter.Value))
                        {
                            query = query.Where(c => c.ShowRoom != null &&
                                                     c.ShowRoom.LotNo != null &&
                                                     c.ShowRoom.LotNo.ToLower().Contains(filter.Value.ToLower()));
                        }

                        if (filter.Key == "yearMonth" && !string.IsNullOrEmpty(filter.Value))
                        {
                            query = query.Where(c => c.Vehicle.Year.Contains(filter.Value));
                        }

                        if (filter.Key == "vehicleDescription" && !string.IsNullOrEmpty(filter.Value))
                        {
                            query = query.Where(c => c.Vehicle.Description.ToLower().Contains(filter.Value.ToLower()));
                        }


                        if (filter.Key == "brandModelName" && !string.IsNullOrEmpty(filter.Value))
                        {
                            query = query.Where(c => c.Vehicle.Brand.Name.ToLower().Contains(filter.Value.ToLower()) || c.Vehicle.Model.Name.ToLower().Contains(filter.Value.ToLower()));
                        }

                        if (filter.Key == "arrivalStatus" && !string.IsNullOrEmpty(filter.Value))
                        {
                            if (filter.Value.ToLower() == "i")
                                query = query.Where(c => c.ArrivalState == ArrivalState.Incoming);
                            if (filter.Value.ToLower() == "r")
                                query = query.Where(c => c.ArrivalState == ArrivalState.Received);
                        }
                        if (filter.Key == "stockStatus" && !string.IsNullOrEmpty(filter.Value))
                        {
                            if (filter.Value.ToLower().Contains("open"))
                                query = query.Where(c => c.IsOpen == true);
                            if (filter.Value.ToLower().Contains("lou"))
                                query = query.Where(c => c.IsLou == true);
                            if (filter.Value.ToLower().Contains("sold"))
                                query = query.Where(c => c.IsSold == true);
                            if (filter.Value.ToLower().Contains("cancelled"))
                                query = query.Where(c => c.IsCancelled == true);
                            if (filter.Value.ToLower().Contains("booking"))
                                query = query.Where(c => c.IsBooked == true);

                        }

                        //if (filter.Key == "phone" && !string.IsNullOrEmpty(filter.Value))
                        //{
                        //    query = query.Where(c => c.Phone.Contains(filter.Value));
                        //}
                        // Add more filters as needed
                    }
                }

                if (isOpen)
                {
                    query = query.Where(c => c.IsOpen == true);
                }

                // Apply sorting with special handling for MovedToStockAt nulls and brandModelName
                if (!string.IsNullOrEmpty(sortColumn))
                {
                    if (sortColumn == "MovedToStockAt")
                    {
                        // Custom sorting for MovedToStockAt to handle nulls properly
                        query = sortAsc
                            ? query.OrderBy(c => c.MovedToStockAt == null ? DateTime.MaxValue : c.MovedToStockAt)
                                   .ThenBy(c => c.StockNo)
                            : query.OrderByDescending(c => c.MovedToStockAt == null ? DateTime.MinValue : c.MovedToStockAt)
                                   .ThenBy(c => c.StockNo);
                    }
                    else if (sortColumn == "brandModelName")
                    {
                        // Custom sorting for brandModelName (Vehicle.Brand.Name + Vehicle.Model.Name)
                        query = sortAsc
                            ? query.OrderBy(c => c.Vehicle.Brand.Name)
                                   .ThenBy(c => c.Vehicle.Model.Name)
                                   .ThenBy(c => c.StockNo)
                            : query.OrderByDescending(c => c.Vehicle.Brand.Name)
                                   .ThenByDescending(c => c.Vehicle.Model.Name)
                                   .ThenBy(c => c.StockNo);
                    }
                    else if (sortColumn == "lotNo")
                    {
                        // Custom sorting for lotNo (ShowRoom.LotNo)
                        query = sortAsc
                            ? query.OrderBy(c => c.ShowRoom.LotNo ?? "")
                                   .ThenBy(c => c.StockNo)
                            : query.OrderByDescending(c => c.ShowRoom.LotNo ?? "")
                                   .ThenBy(c => c.StockNo);
                    }
                    else if (sortColumn == "yearMonth")
                    {
                        // Custom sorting for yearMonth (Vehicle.Year + Month combined)
                        // Nulls go to the end for both ascending and descending
                        query = sortAsc
                            ? query.OrderBy(c => string.IsNullOrEmpty(c.Vehicle.Year) ? "9999" : c.Vehicle.Year)
                                   .ThenBy(c => string.IsNullOrEmpty(c.Vehicle.Month) ? "99" : c.Vehicle.Month.PadLeft(2, '0'))
                                   .ThenBy(c => c.StockNo)
                            : query.OrderByDescending(c => string.IsNullOrEmpty(c.Vehicle.Year) ? "0000" : c.Vehicle.Year)
                                   .ThenByDescending(c => string.IsNullOrEmpty(c.Vehicle.Month) ? "00" : c.Vehicle.Month.PadLeft(2, '0'))
                                   .ThenBy(c => c.StockNo);
                    }
                    else if (sortColumn == "price")
                    {
                        // Custom sorting for price (Pricing.RecommendedSalePrice)
                        query = sortAsc
                            ? query.OrderBy(c => c.Pricing.RecommendedSalePrice)
                                   .ThenBy(c => c.StockNo)
                            : query.OrderByDescending(c => c.Pricing.RecommendedSalePrice)
                                   .ThenBy(c => c.StockNo);
                    }
                    else if (sortColumn == "arrivalStatus")
                    {
                        // Custom sorting for arrivalStatus (ArrivalState enum)
                        query = sortAsc
                            ? query.OrderBy(c => c.ArrivalState)
                                   .ThenBy(c => c.StockNo)
                            : query.OrderByDescending(c => c.ArrivalState)
                                   .ThenBy(c => c.StockNo);
                    }
                    else if (sortColumn == "eta")
                    {
                        // Custom sorting for eta (Import.EstimateDateOfArrival)
                        // Nulls go to the end for both ascending and descending
                        query = sortAsc
                            ? query.OrderBy(c => c.Import.EstimateDateOfArrival == null ? DateTime.MaxValue : c.Import.EstimateDateOfArrival)
                                   .ThenBy(c => c.StockNo)
                            : query.OrderByDescending(c => c.Import.EstimateDateOfArrival == null ? DateTime.MinValue : c.Import.EstimateDateOfArrival)
                                   .ThenBy(c => c.StockNo);
                    }
                    else
                    {
                        query = sortAsc
                            ? query.OrderByDynamic(sortColumn)
                            : query.OrderByDescendingDynamic(sortColumn);
                    }
                }

                // Get total count before applying pagination
                var totalItems = await query.CountAsync();

                // Apply pagination
                var items = await query
                    .Skip((page - 1) * itemsPerPage)
                    .Take(itemsPerPage)
                    .ToListAsync();

                //var items = await query.ToListAsync();

                // Remove circular references and other unwanted data
                foreach (var obj in items)
                {
                    foreach (var s in obj.StockStatusHistories)
                        s.Stock = null;
                    if (obj.Vehicle != null && obj.Vehicle.Model != null)
                        obj.Vehicle.Model.Brand = null;

                    if (obj.Vehicle != null && obj.Vehicle.VehiclePhotoList != null)
                    {
                        foreach (var p in obj.Vehicle.VehiclePhotoList)
                        {
                            p.Vehicle = null;
                            p.Document = null;
                        }
                    }

                    if (obj.Clearance != null)
                    {
                        if (obj.Clearance.K8Documents != null)
                            foreach (var k in obj.Clearance.K8Documents)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Clearance = null;
                            }
                    }

                    if (obj.Import != null)
                    {
                        if (obj.Import.BillOfLandingDocuments != null)
                            foreach (var k in obj.Import.BillOfLandingDocuments)
                            {
                                //k.Document = null;
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Import = null;
                            }
                    }

                    if (obj.Clearance != null)
                    {
                        if (obj.Clearance.K8Documents != null)
                            foreach (var k in obj.Clearance.K8Documents)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Clearance = null;
                            }

                        if (obj.Clearance.K1Documents != null)
                            foreach (var k in obj.Clearance.K1Documents)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Clearance = null;
                            }

                        if (obj.Clearance.ExportCertificateDocuments != null)
                            foreach (var k in obj.Clearance.ExportCertificateDocuments)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Clearance = null;
                            }
                    }

                    if (obj.Import != null)
                    {
                        if (obj.Import.BillOfLandingDocuments != null)
                            foreach (var k in obj.Import.BillOfLandingDocuments)
                            {
                                //k.Document = null;
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Import = null;
                            }
                    }

                    if (obj.Sale != null)
                    {
                        if (obj.Sale.CustomerIcDocuments != null)
                            foreach (var k in obj.Sale.CustomerIcDocuments)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Sale = null;
                            }
                    }

                    if (obj.Registration != null)
                    {
                        if (obj.Registration.InsuranceDocuments != null)
                            foreach (var k in obj.Registration.InsuranceDocuments)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Registration = null;
                            }

                        if (obj.Registration.RoadTaxDocuments != null)
                            foreach (var k in obj.Registration.RoadTaxDocuments)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Registration = null;
                            }

                        if (obj.Registration.ReceiptEDaftarDocuments != null)
                            foreach (var k in obj.Registration.ReceiptEDaftarDocuments)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Registration = null;
                            }

                        if (obj.Registration.ReceiptKastamDocuments != null)
                            foreach (var k in obj.Registration.ReceiptKastamDocuments)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Registration = null;
                            }

                        if (obj.Registration.PuspakomB2SlipDocuments != null)
                            foreach (var k in obj.Registration.PuspakomB2SlipDocuments)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Registration = null;
                            }

                        if (obj.Registration.JpjGeranDocuments != null)
                            foreach (var k in obj.Registration.JpjGeranDocuments)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Registration = null;
                            }
                    }

                    if (obj.Expense != null)
                    {
                        if (obj.Expense.Expenses != null)
                            foreach (var k in obj.Expense.Expenses)
                            {
                                if (k.Document != null)
                                    k.Document.Content = null;
                                k.Expense = null;
                            }
                    }

                }

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

        [HttpGet("forSale")]
        public async Task<ActionResult<IEnumerable<Stock>>> GetAllForSale()
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();


            try
            {
                var query = _context.Stocks
                    .Include(c => c.StockStatusHistories.OrderByDescending(c => c.DateTime))
                    .ThenInclude(c => c.StockStatus)
                    .Include(c => c.Vehicle)
                    .ThenInclude(c => c.Brand)
                    .Include(c => c.Vehicle)
                    .ThenInclude(c => c.Model)
                    .Include(c => c.Pricing)
                    .Include(c => c.Vehicle)
                    .OrderByDescending(c => c.CreatedOn)
                    .Include(c => c.Clearance)
                    .ThenInclude(c => c.K8Documents)
                    .ThenInclude(c => c.Document)
                    .AsQueryable();


                var objs = await query.ToListAsync();

                // Remove circular references and other unwanted data
                foreach (var obj in objs)
                {
                    foreach (var s in obj.StockStatusHistories)
                        s.Stock = null;
                    if (obj.Vehicle != null && obj.Vehicle.Model != null)
                        obj.Vehicle.Model.Brand = null;

                    if (obj.Vehicle != null && obj.Vehicle.VehiclePhotoList != null)
                    {
                        foreach (var p in obj.Vehicle.VehiclePhotoList)
                        {
                            p.Vehicle = null;
                            p.Document = null;
                        }
                    }

                    if (obj.Clearance != null && obj.Clearance.K8Documents != null && obj.Clearance.K8Documents.Count > 0)
                    {
                        foreach (var doc in obj.Clearance.K8Documents)
                        {
                            doc.Clearance = null;
                        }
                    }
                }


                objs = objs.ToList();

                //objs = objs.Where(x =>
                //        x.LatestStockStatus != null &&
                //        x.LatestStockStatus.StockStatus != null &&
                //        (
                //            (x.LatestStockStatus.StockStatus.Name.ToLower() != "draft") &&
                //            (x.LatestStockStatus.StockStatus.Name.ToLower() != "sold")
                //        )
                //    ).ToList();


                //var filteredList = objs.Where(x =>
                //x.LatestStockStatus.StockStatus.Name == "Draft" ||
                //x.LatestStockStatus.StockStatus.Name.ToLower() != "sold" ||
                //x.LatestStockStatus.StockStatus.Name.ToLower() != "booked"

                //).ToList();




                return objs;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            // Start building the query



        }

        //// GET: api/Stocks
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Stock>>> GetAll()
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        //    if (user == null)
        //        return Unauthorized();

        //    var objs = await _context.Stocks
        //        .Include(c => c.StockStatusHistories.OrderByDescending(c => c.DateTime))
        //        .ThenInclude(c => c.StockStatus)
        //        .Include(c=> c.Vehicle)
        //        .ThenInclude(c=> c.Brand)
        //        .Include(c => c.Vehicle)
        //        .ThenInclude(c => c.Model)
        //        .Include(c=> c.Pricing)
        //        .Include(c=> c.Vehicle)
        //        .ThenInclude(c=> c.VehiclePhotoList)
        //        .OrderByDescending(c=> c.CreatedOn)
        //        .ToListAsync();



        //    foreach (var obj in objs)
        //    {
        //        foreach (var s in obj.StockStatusHistories)
        //            s.Stock = null;
        //        if (obj.Vehicle != null && obj.Vehicle.Model != null)
        //            obj.Vehicle.Model.Brand = null;

        //        if (obj.Vehicle != null && obj.Vehicle.VehiclePhotoList != null)
        //        {
        //            foreach(var p in obj.Vehicle.VehiclePhotoList)
        //            {
        //                p.Vehicle = null;
        //                p.Document = null;
        //            }
        //        }
        //    } 




        //    return objs;
        //}

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
                .Include(c => c.RemarksList.OrderByDescending(c => c.DateTime))
                .ThenInclude(c => c.Profile)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.Brand)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.Model)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehicleType)
                .Include(c => c.Vehicle)
                .ThenInclude(c => c.VehiclePhotoList.OrderBy(c => c.Position))
                .Include(c => c.Purchase)
                .ThenInclude(c => c.Supplier)
                .Include(c => c.Import)
                .ThenInclude(c => c.ForwardingAgent)
                .Include(c => c.Import)
                .ThenInclude(c => c.BillOfLandingDocuments)
                //.ThenInclude(c => c.Document)
                .Include(c => c.Clearance)
                .ThenInclude(c => c.K8Documents)
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
                .Include(c=>c.Sale)
                .ThenInclude(c=> c.SalesMan)
                .Include(c => c.Pricing)
                .Include(c => c.ArrivalChecklist)
                .ThenInclude(c => c.ArrivalChecklists)
                .Include(c => c.Registration)
                .ThenInclude(c => c.JpjEHakMilikDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.JpjGeranDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.JpjEDaftarDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.PuspakomB2SlipDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.PuspakomB7SlipDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.InsuranceDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.RoadTaxDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.ReceiptEDaftarDocuments)
                .Include(c => c.Registration)
                .ThenInclude(c => c.ReceiptKastamDocuments)
                .Include(c => c.Clearance)
                .ThenInclude(c => c.ExportCertificateDocuments)
                .Include(c => c.Sale)
                .ThenInclude(c => c.CustomerIcDocuments)
                .Include(c => c.Expense)
                .ThenInclude(c => c.Expenses)
                .ThenInclude(c => c.Document)
                .Include(c => c.AdminitrativeCost)
                .ThenInclude(c => c.AdminitrativeCostItems)
                .Include(c => c.ApCompany)
                .ThenInclude(c => c.SubCompany)
                .Include(c => c.ApCompany)
                .ThenInclude(c => c.BankAccount)
                .Include(c=> c.ShowRoom)
                .Include(c=> c.Advertisement)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            if (obj.ApCompany == null)
            {
                obj.ApCompany = new ApCompany();
                _context.ApCompanies.Add(obj.ApCompany);
                obj.ApCompanyId = obj.ApCompany.Id;
                _context.SaveChanges();

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

                if (obj.Clearance.ExportCertificateDocuments != null)
                    foreach (var o in obj.Clearance.ExportCertificateDocuments)
                    {
                        o.Clearance = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
            }

            if (obj.Sale != null)
            {
                if (obj.Sale.Loan != null)
                {
                    if (obj.Sale.Loan.LetterOfUndertakingDocuments != null)
                    {
                        foreach (var o in obj.Sale.Loan.LetterOfUndertakingDocuments)
                        {
                            o.Loan = null;
                            o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                            o.Document.Content = null;
                        }

                    }
                }

                if (obj.Sale.CustomerIcDocuments != null)
                {
                    foreach (var o in obj.Sale.CustomerIcDocuments)
                    {
                        o.Sale = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }
            }

            if (obj.ArrivalChecklist != null)
            {
                if (obj.ArrivalChecklist.ArrivalChecklists != null)
                {
                    foreach (var o in obj.ArrivalChecklist.ArrivalChecklists)
                    {
                        o.ArrivalChecklist = null;
                    }
                }
            }

            if (obj.Registration != null)
            {
                if (obj.Registration.JpjEHakMilikDocuments != null)
                {
                    foreach (var o in obj.Registration.JpjEHakMilikDocuments)
                    {
                        o.Registration = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }

                if (obj.Registration.JpjGeranDocuments != null)
                {
                    foreach (var o in obj.Registration.JpjGeranDocuments)
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

                if (obj.Registration.InsuranceDocuments != null)
                {
                    foreach (var o in obj.Registration.InsuranceDocuments)
                    {
                        o.Registration = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }

                if (obj.Registration.RoadTaxDocuments != null)
                {
                    foreach (var o in obj.Registration.RoadTaxDocuments)
                    {
                        o.Registration = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }

                if (obj.Registration.ReceiptEDaftarDocuments != null)
                {
                    foreach (var o in obj.Registration.ReceiptEDaftarDocuments)
                    {
                        o.Registration = null;
                        o.Document = _context.Documents.FirstOrDefault(c => c.Id == o.DocumentId);
                        o.Document.Content = null;
                    }
                }

                if (obj.Registration.ReceiptKastamDocuments != null)
                {
                    foreach (var o in obj.Registration.ReceiptKastamDocuments)
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

            if (obj.Vehicle != null && obj.Vehicle.Brand != null)
            {
                obj.Vehicle.Brand.Models = null;

            }
            if (obj.Vehicle != null && obj.Vehicle.Model != null)
            {
                obj.Vehicle.Model.Brand = null;
            }

            if (obj.ApCompany != null)
            {
                if (obj.ApCompany.SubCompany != null)
                    obj.ApCompany.SubCompany.BankAccounts = null;
                if (obj.ApCompany.BankAccount != null)
                    obj.ApCompany.BankAccount.SubCompany = null;
            }
            return obj;
        }

        // PUT: api/Stocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("defaultimage/{id}")]
        public async Task<IActionResult> PutDefaultImage(Guid id, Stock obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }
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

        // PUT: api/Stocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("imageposition/{id}")]
        public async Task<IActionResult> PutImagePosition(Guid id, Stock obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }

            var photoIds = obj.Vehicle.VehiclePhotoList.Select(u => u.Id).ToList();
            var vehiclePhotos = await _context.VehiclePhotos // or Documents, depending on your entity
                                        .Where(p => photoIds.Contains(p.Id))
                                        .ToListAsync();

            // Update the positions based on the incoming data
            foreach (var update in obj.Vehicle.VehiclePhotoList)
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


        // PUT: api/Stocks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, Stock obj)
        {
            if (id != obj.Id)
            {
                return BadRequest();
            }
            //obj.Advertisement.MudahStartDate = obj.Advertisement.MudahStartDate.Date;
            // Ensure that the dates are converted to UTC
            //obj.Advertisement.MudahStartDate = obj.Advertisement.MudahStartDate.Date; // Removes the time part (00:00:00)
            // Assign 12:00 noon UTC instead of midnight
            try
            {
                obj.Advertisement.MudahStartDate = DateTime.SpecifyKind(obj.Advertisement.MudahStartDate.Date.AddHours(12), DateTimeKind.Utc);
            obj.Advertisement.MudahEndDate = DateTime.SpecifyKind(obj.Advertisement.MudahEndDate.Date.AddHours(12), DateTimeKind.Utc);

            obj.Advertisement.CarListStartDate = DateTime.SpecifyKind(obj.Advertisement.CarListStartDate.Date.AddHours(12), DateTimeKind.Utc);
            obj.Advertisement.CarListEndDate = DateTime.SpecifyKind(obj.Advertisement.CarListEndDate.Date.AddHours(12), DateTimeKind.Utc);

            obj.Advertisement.CariCarzStartDate= DateTime.SpecifyKind(obj.Advertisement.CariCarzStartDate.Date.AddHours(12), DateTimeKind.Utc);
            obj.Advertisement.CariCarzEndDate = DateTime.SpecifyKind(obj.Advertisement.CariCarzEndDate.Date.AddHours(12), DateTimeKind.Utc);

            obj.Sale.BookingDate= DateTime.SpecifyKind(obj.Sale.BookingDate.Date.AddHours(12), DateTimeKind.Utc);
            obj.Sale.BookingExpiryDate = DateTime.SpecifyKind(obj.Sale.BookingExpiryDate.Date.AddHours(12), DateTimeKind.Utc);

            //obj.Advertisement.MudahStartDate = obj.Advertisement.MudahStartDate.ToUniversalTime();
            //obj.Advertisement.MudahEndDate = obj.Advertisement.MudahEndDate.ToUniversalTime();
            //obj.Advertisement.CarListStartDate = obj.Advertisement.CarListStartDate.ToUniversalTime();
            //obj.Advertisement.CarListEndDate = obj.Advertisement.CarListEndDate.ToUniversalTime();
            //obj.Advertisement.CariCarzStartDate = obj.Advertisement.CariCarzStartDate.ToUniversalTime();
            //obj.Advertisement.CariCarzEndDate = obj.Advertisement.CariCarzEndDate.ToUniversalTime();

          
                var prevObj = _context.Stocks.Include(c => c.Pricing).AsNoTracking().FirstOrDefault(c => c.Id == id);
                if (prevObj != null)
                    if (obj.Pricing.RecommendedSalePrice != prevObj.Pricing.RecommendedSalePrice)
                        obj.Pricing.LastPriceChange = DateTime.UtcNow;

                // Check for duplicate ChasisNo (only if ChasisNo is not null or empty)
                if (!string.IsNullOrWhiteSpace(obj.Vehicle.ChasisNo))
                {
                    var duplicateChasisExists = await _context.Vehicles
                        .AnyAsync(v => v.ChasisNo.ToLower() == obj.Vehicle.ChasisNo.ToLower() && v.Id != obj.VehicleId);

                    if (duplicateChasisExists)
                    {
                        return BadRequest($"Chasis No '{obj.Vehicle.ChasisNo}' already exists for another vehicle. Please use a different Chasis No.");
                    }
                }

            _context.Entry(obj).State = EntityState.Modified;
            _context.Entry(obj.Vehicle).State = EntityState.Modified;
            _context.Entry(obj.Purchase).State = EntityState.Modified;
            _context.Entry(obj.Import).State = EntityState.Modified;
            _context.Entry(obj.Clearance).State = EntityState.Modified;
            //var saleAmount = Math.Round(obj.Sale.SaleAmount, 2);
            //obj.Sale.SaleAmount = (float)saleAmount;

            _context.Entry(obj.Sale).State = EntityState.Modified;


            _context.Entry(obj.Sale.Loan).State = EntityState.Modified;
            _context.Entry(obj.Registration).State = EntityState.Modified;
            _context.Entry(obj.Pricing).State = EntityState.Modified;

            _context.Entry(obj.ApCompany).State = EntityState.Modified;

            _context.Entry(obj.AdminitrativeCost).State = EntityState.Modified;
            _context.Entry(obj.Expense).State = EntityState.Modified;
            _context.Entry(obj.Advertisement).State = EntityState.Modified;

            //_context.Entry(obj.Vehicle.VehiclePhotoList).State = EntityState.Modified;

            //_context.Entry(obj.SellingPricing).State = EntityState.Modified;


          
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

        // PUT: api/Stocks/UpdateNullMovedToStockDates
        // One-time endpoint to update all stocks with null MovedToStockAt to default date
        [HttpPut("UpdateNullMovedToStockDates")]
        public async Task<IActionResult> UpdateNullMovedToStockDates()
        {
            try
            {
                var defaultDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                var stocksWithNullDates = await _context.Stocks
                    .Where(s => s.MovedToStockAt == null)
                    .ToListAsync();

                foreach (var stock in stocksWithNullDates)
                {
                    stock.MovedToStockAt = defaultDate;
                }

                await _context.SaveChangesAsync();

                return Ok(new {
                    message = $"Updated {stocksWithNullDates.Count} stock records to default date (01/01/2025)",
                    count = stocksWithNullDates.Count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error updating stock dates", error = ex.Message });
            }
        }

        // POST: api/Stocks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Stock>> Post(Stock obj)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                    if (user == null)
                        return Unauthorized();

                    // Initialize stock status history
                    if (obj.StockStatusHistories == null)
                        obj.StockStatusHistories = new List<StockStatusHistory>();

                    var stockStatus = await _context.StockStatuses.FirstOrDefaultAsync(c => c.Name == "Draft");
                    obj.StockStatusHistories.Add(new StockStatusHistory
                    {
                        ProfileId = (Guid)user.ProfileId,
                        StockStatusId = stockStatus.Id,
                        StockId = obj.Id
                    });

                    // Set the record date when stock is created
                    obj.MovedToStockAt = DateTime.UtcNow;

                    // Initialize other entities
                    var vehicle = new Vehicle();
                    obj.VehicleId = vehicle.Id;

                    var purchase = new Purchase();
                    obj.PurchaseId = purchase.Id;

                    var import = new Import();
                    obj.ImportId = import.Id;

                    var clearance = new Clearance();
                    obj.ClearanceId = clearance.Id;

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
                    await _context.ApCompanies.AddAsync(apCompany);
                    obj.ApCompanyId = apCompany.Id;

                    // Check for duplicate ChasisNo (only if ChasisNo is not null or empty)
                    if (obj.Vehicle != null && !string.IsNullOrWhiteSpace(obj.Vehicle.ChasisNo))
                    {
                        var duplicateChasisExists = await _context.Vehicles
                            .AnyAsync(v => v.ChasisNo.ToLower() == obj.Vehicle.ChasisNo.ToLower());

                        if (duplicateChasisExists)
                        {
                            return BadRequest($"Chasis No '{obj.Vehicle.ChasisNo}' already exists for another vehicle. Please use a different Chasis No.");
                        }
                    }

                    // Add stock object
                    await _context.Stocks.AddAsync(obj);
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

        //[HttpPost]
        //public async Task<ActionResult<Stock>> Post(Stock obj)
        //{
        //    try
        //    {
        //        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
        //        if (user == null)
        //            return Unauthorized();

        //        if (obj.StockStatusHistories == null)
        //            obj.StockStatusHistories = new List<StockStatusHistory>();

        //        var stockStatus = _context.StockStatuses.FirstOrDefault(c => c.Name == "Draft");

        //        obj.StockStatusHistories.Add(new StockStatusHistory
        //        {
        //            ProfileId = (Guid)user.ProfileId,
        //            StockStatusId = stockStatus.Id,
        //            //Name = "Draft",
        //            StockId = obj.Id
        //        });


        //        var vehicle = new Vehicle();
        //        //_context.Vehicles.Add(vehicle);
        //        obj.VehicleId = vehicle.Id;

        //        var purchase = new Purchase();
        //        //_context.Purchases.Add(purchase);
        //        obj.PurchaseId = purchase.Id;

        //        var import = new Import();
        //        //_context.Imports.Add(import);
        //        obj.ImportId = import.Id;

        //        var clearance = new Clearance();
        //        //_context.Clearances.Add(clearance);
        //        obj.ClearanceId = clearance.Id;

        //        var pricing = new Pricing();
        //        _context.Pricings.Add(pricing);
        //        obj.PricingId = pricing.Id;

        //        var arrivalCheckList = new ArrivalChecklist();

        //        if (arrivalCheckList.ArrivalChecklists == null)
        //            arrivalCheckList.ArrivalChecklists = new List<ArrivalChecklistItem>();

        //        arrivalCheckList.ArrivalChecklists.Add(new ArrivalChecklistItem
        //        {
        //            ArrivalChecklistId = arrivalCheckList.Id,
        //            Name = "Extra Key",
        //            IsAvailable = true,
        //            Remarks = ""
        //        });
        //        _context.ArrivalChecklists.Add(arrivalCheckList);
        //        obj.ArrivalChecklistId = arrivalCheckList.Id;

        //        //var sellingPrice = new SellingPricing();
        //        //obj.SellingPricingId = sellingPrice.Id;

        //        //var loan = new Loan();
        //        //_context.Loans.Add(loan);
        //        var sale = new Sale();
        //        sale.SaleDateTime = DateTime.Now;
        //        //var loan = new Loan();
        //        //sale.LoanId = loan.Id;
        //        //_context.Sales.Add(sale);
        //        obj.Sale = sale;
        //        obj.SaleId = sale.Id;

        //        var loan = new Loan();
        //        _context.Loans.Add(loan);
        //        obj.Sale.LoanId = loan.Id;


        //        var registration = new Registration();
        //        _context.Registrations.Add(registration);
        //        //_context.Imports.Add(import);
        //        obj.RegistrationId = registration.Id;


        //        var expense = new Expense();
        //        _context.Expenses.Add(expense);
        //        obj.ExpenseId = expense.Id;



        //        var administrativeCost = new AdminitrativeCost();
        //        _context.AdminitrativeCosts.Add(administrativeCost);
        //        obj.AdminitrativeCostId = administrativeCost.Id;

        //        //Stocks
        //        _context.Stocks.Add(obj);


        //        await _context.SaveChangesAsync();


        //        //var loan = new Loan();
        //        //_context.Loans.Add(loan);
        //        //sale.LoanId = loan.Id;

        //        //_context.Entry(sale).State = EntityState.Modified;
        //        //await _context.SaveChangesAsync();
        //        //var loan = new Loan();
        //        //sale.LoanId = loan.Id;

        //        foreach (var stockStatusHistory in obj.StockStatusHistories)
        //            stockStatusHistory.Stock = null;

        //        foreach (var o in obj.ArrivalChecklist.ArrivalChecklists)
        //            o.ArrivalChecklist = null;
        //        if (obj.AdminitrativeCost.AdminitrativeCostItems != null)
        //            foreach (var o in obj.AdminitrativeCost.AdminitrativeCostItems)
        //                o.AdminitrativeCost = null;

        //        obj.LatestStockStatus.Stock = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //        //throw ex;
        //    }

        //    return CreatedAtAction("Get", new { id = obj.Id }, obj);
        //}

        // DELETE: api/Stocks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            // Load stock with all related entities to enable proper cascade deletion
            var stock = await _context.Stocks
                .Include(s => s.RemarksList)
                .Include(s => s.StockStatusHistories)
                .Include(s => s.Vehicle).ThenInclude(v => v.VehiclePhotoList)
                .Include(s => s.Vehicle).ThenInclude(v => v.Brand)
                .Include(s => s.Vehicle).ThenInclude(v => v.Model)
                .Include(s => s.Vehicle).ThenInclude(v => v.VehicleType)
                .Include(s => s.Purchase).ThenInclude(p => p.Supplier)
                .Include(s => s.Import).ThenInclude(i => i.BillOfLandingDocuments)
                .Include(s => s.Clearance).ThenInclude(c => c.K8Documents)
                .Include(s => s.Clearance).ThenInclude(c => c.K1Documents)
                .Include(s => s.Clearance).ThenInclude(c => c.ExportCertificateDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.JpjEHakMilikDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.JpjEDaftarDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.JpjGeranDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.PuspakomB2SlipDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.PuspakomB7SlipDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.InsuranceDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.RoadTaxDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.ReceiptEDaftarDocuments)
                .Include(s => s.Registration).ThenInclude(r => r.ReceiptKastamDocuments)
                .Include(s => s.Sale).ThenInclude(s => s.CustomerIcDocuments)
                .Include(s => s.Sale).ThenInclude(s => s.Loan).ThenInclude(l => l.LouDocuments)
                .Include(s => s.Pricing)
                .Include(s => s.ArrivalChecklist).ThenInclude(a => a.ArrivalChecklists)
                .Include(s => s.Expense).ThenInclude(e => e.ExpenseItems)
                .Include(s => s.AdminitrativeCost).ThenInclude(a => a.AdministrativeCostItems)
                .Include(s => s.Advertisement)
                .Include(s => s.ApCompany)
                .Include(s => s.ShowRoom)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stock == null)
            {
                return NotFound();
            }

            // Delete in proper order to avoid foreign key violations

            // 1. Delete Stock's direct dependents
            if (stock.RemarksList != null && stock.RemarksList.Any())
                _context.Remarks.RemoveRange(stock.RemarksList);

            if (stock.StockStatusHistories != null && stock.StockStatusHistories.Any())
                _context.StockStatusHistories.RemoveRange(stock.StockStatusHistories);

            // 2. Delete nested document collections and child entities
            if (stock.Vehicle?.VehiclePhotoList != null && stock.Vehicle.VehiclePhotoList.Any())
                _context.VehiclePhotos.RemoveRange(stock.Vehicle.VehiclePhotoList);

            if (stock.Import?.BillOfLandingDocuments != null && stock.Import.BillOfLandingDocuments.Any())
                _context.Documents.RemoveRange(stock.Import.BillOfLandingDocuments);

            if (stock.Clearance != null)
            {
                if (stock.Clearance.K8Documents != null && stock.Clearance.K8Documents.Any())
                    _context.Documents.RemoveRange(stock.Clearance.K8Documents);
                if (stock.Clearance.K1Documents != null && stock.Clearance.K1Documents.Any())
                    _context.Documents.RemoveRange(stock.Clearance.K1Documents);
                if (stock.Clearance.ExportCertificateDocuments != null && stock.Clearance.ExportCertificateDocuments.Any())
                    _context.Documents.RemoveRange(stock.Clearance.ExportCertificateDocuments);
            }

            if (stock.Registration != null)
            {
                if (stock.Registration.JpjEHakMilikDocuments != null && stock.Registration.JpjEHakMilikDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.JpjEHakMilikDocuments);
                if (stock.Registration.JpjEDaftarDocuments != null && stock.Registration.JpjEDaftarDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.JpjEDaftarDocuments);
                if (stock.Registration.JpjGeranDocuments != null && stock.Registration.JpjGeranDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.JpjGeranDocuments);
                if (stock.Registration.PuspakomB2SlipDocuments != null && stock.Registration.PuspakomB2SlipDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.PuspakomB2SlipDocuments);
                if (stock.Registration.PuspakomB7SlipDocuments != null && stock.Registration.PuspakomB7SlipDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.PuspakomB7SlipDocuments);
                if (stock.Registration.InsuranceDocuments != null && stock.Registration.InsuranceDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.InsuranceDocuments);
                if (stock.Registration.RoadTaxDocuments != null && stock.Registration.RoadTaxDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.RoadTaxDocuments);
                if (stock.Registration.ReceiptEDaftarDocuments != null && stock.Registration.ReceiptEDaftarDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.ReceiptEDaftarDocuments);
                if (stock.Registration.ReceiptKastamDocuments != null && stock.Registration.ReceiptKastamDocuments.Any())
                    _context.Documents.RemoveRange(stock.Registration.ReceiptKastamDocuments);
            }

            if (stock.Sale != null)
            {
                if (stock.Sale.CustomerIcDocuments != null && stock.Sale.CustomerIcDocuments.Any())
                    _context.Documents.RemoveRange(stock.Sale.CustomerIcDocuments);

                if (stock.Sale.Loan != null)
                {
                    if (stock.Sale.Loan.LouDocuments != null && stock.Sale.Loan.LouDocuments.Any())
                        _context.Documents.RemoveRange(stock.Sale.Loan.LouDocuments);
                    _context.Loans.Remove(stock.Sale.Loan);
                }
            }

            if (stock.ArrivalChecklist?.ArrivalChecklists != null && stock.ArrivalChecklist.ArrivalChecklists.Any())
                _context.ArrivalChecklistItems.RemoveRange(stock.ArrivalChecklist.ArrivalChecklists);

            if (stock.Expense?.ExpenseItems != null && stock.Expense.ExpenseItems.Any())
                _context.ExpenseItems.RemoveRange(stock.Expense.ExpenseItems);

            if (stock.AdminitrativeCost?.AdministrativeCostItems != null && stock.AdminitrativeCost.AdministrativeCostItems.Any())
                _context.AdministrativeCostItems.RemoveRange(stock.AdminitrativeCost.AdministrativeCostItems);

            // 3. Delete owned entities (parent entities that Stock directly owns)
            if (stock.Vehicle != null)
                _context.Vehicles.Remove(stock.Vehicle);

            if (stock.Purchase != null)
                _context.Purchases.Remove(stock.Purchase);

            if (stock.Import != null)
                _context.Imports.Remove(stock.Import);

            if (stock.Clearance != null)
                _context.Clearances.Remove(stock.Clearance);

            if (stock.Registration != null)
                _context.Registrations.Remove(stock.Registration);

            if (stock.Sale != null)
                _context.Sales.Remove(stock.Sale);

            if (stock.Pricing != null)
                _context.Pricings.Remove(stock.Pricing);

            if (stock.ArrivalChecklist != null)
                _context.ArrivalChecklists.Remove(stock.ArrivalChecklist);

            if (stock.Expense != null)
                _context.Expenses.Remove(stock.Expense);

            if (stock.AdminitrativeCost != null)
                _context.AdminitrativeCosts.Remove(stock.AdminitrativeCost);

            if (stock.Advertisement != null)
                _context.Advertisements.Remove(stock.Advertisement);

            if (stock.ApCompany != null)
                _context.ApCompanies.Remove(stock.ApCompany);

            // 4. Finally, delete the stock itself
            _context.Stocks.Remove(stock);

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


        [HttpDelete("image/{id}")]
        public async Task<IActionResult> DeleteVehicleImage(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();



            var vehicleImage = await _context.VehiclePhotos
                .FirstOrDefaultAsync(c => c.Id == id);
            if (vehicleImage == null)
            {
                return NotFound();
            }

            _context.VehiclePhotos.Remove(vehicleImage);
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
                _context.BillOfLandingDocuments.Add(new BillOfLandingDocument
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


        //
        [HttpPut("UpdateK1Documents/{id}")]
        public async Task<IActionResult> PutUpdateK1Documents(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Clearance)
                .ThenInclude(c => c.K1Documents)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.K1Documents.Add(new K1Document
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


        [HttpPut("RemoveK1Document/{clearanceId}/{documentId}")]
        public async Task<IActionResult> PutRemoveK1Document(Guid clearanceId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.K1Documents
                    .Where(c => c.ClearanceId == clearanceId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.K1Documents.Remove(inDataBase);
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

        [HttpPut("UpdateLouDocuments/{id}")]
        public async Task<IActionResult> PutUpdateLouDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Sale)
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
                .ThenInclude(c => c.ArrivalChecklists)
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

        [HttpPut("UpdateArrivalItem/{id}")]
        public async Task<IActionResult> PutUpdateArrivalItem(Guid id, ArrivalChecklistItem obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

            if (user == null)
                return Unauthorized();

            var ba = _context.ArrivalChecklistItems.Find(obj.Id);

            if (ba == null)
            {
                return NotFound("Not found.");
            }

            ba.Name = obj.Name;
            ba.IsAvailable = obj.IsAvailable;
            ba.Remarks = obj.Remarks;

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


        [HttpPut("RemoveJpjHakMilikDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemoveJpjHakMilikDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.JpjEHakMilikDocuments
                    .Where(c => c.RegistrationId == registrationId)
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
        [HttpPut("UpdateJpjGeranDocuments/{id}")]
        public async Task<IActionResult> PutUpdateJpjGeranDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.JpjGeranDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.JpjGeranDocuments.Add(new JpjGeranDocument
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


        [HttpPut("RemoveJpjGeranDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemoveJpjGeranDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.JpjGeranDocuments
                    .Where(c => c.RegistrationId == registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.JpjGeranDocuments.Remove(inDataBase);
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

        // Insurance Documents
        [HttpPut("UpdateInsuranceDocuments/{id}")]
        public async Task<IActionResult> PutUpdateInsuranceDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.InsuranceDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.InsuranceDocuments.Add(new InsuranceDocument
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

        [HttpPut("RemoveInsuranceDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemoveInsuranceDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.InsuranceDocuments
                    .Where(c => c.RegistrationId == registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.InsuranceDocuments.Remove(inDataBase);
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

        // RoadTax Documents
        [HttpPut("UpdateRoadTaxDocuments/{id}")]
        public async Task<IActionResult> PutUpdateRoadTaxDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.RoadTaxDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.RoadTaxDocuments.Add(new RoadTaxDocument
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

        [HttpPut("RemoveRoadTaxDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemoveRoadTaxDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.RoadTaxDocuments
                    .Where(c => c.RegistrationId == registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.RoadTaxDocuments.Remove(inDataBase);
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

        // ReceiptEDaftar Documents
        [HttpPut("UpdateReceiptEDaftarDocuments/{id}")]
        public async Task<IActionResult> PutUpdateReceiptEDaftarDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.ReceiptEDaftarDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.ReceiptEDaftarDocuments.Add(new ReceiptEDaftarDocument
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

        [HttpPut("RemoveReceiptEDaftarDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemoveReceiptEDaftarDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.ReceiptEDaftarDocuments
                    .Where(c => c.RegistrationId == registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.ReceiptEDaftarDocuments.Remove(inDataBase);
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

        // ReceiptKastam Documents
        [HttpPut("UpdateReceiptKastamDocuments/{id}")]
        public async Task<IActionResult> PutUpdateReceiptKastamDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Registration)
                .ThenInclude(c => c.ReceiptKastamDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.ReceiptKastamDocuments.Add(new ReceiptKastamDocument
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

        [HttpPut("RemoveReceiptKastamDocument/{registrationId}/{documentId}")]
        public async Task<IActionResult> PutRemoveReceiptKastamDocument(Guid registrationId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.ReceiptKastamDocuments
                    .Where(c => c.RegistrationId == registrationId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.ReceiptKastamDocuments.Remove(inDataBase);
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

        // ExportCertificate Documents
        [HttpPut("UpdateExportCertificateDocuments/{id}")]
        public async Task<IActionResult> PutUpdateExportCertificateDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Clearance)
                .ThenInclude(c => c.ExportCertificateDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.ExportCertificateDocuments.Add(new ExportCertificateDocument
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

        [HttpPut("RemoveExportCertificateDocument/{clearanceId}/{documentId}")]
        public async Task<IActionResult> PutRemoveExportCertificateDocument(Guid clearanceId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.ExportCertificateDocuments
                    .Where(c => c.ClearanceId == clearanceId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.ExportCertificateDocuments.Remove(inDataBase);
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

        // CustomerIc Documents
        [HttpPut("UpdateCustomerIcDocuments/{id}")]
        public async Task<IActionResult> PutUpdateCustomerIcDocuments(Guid id, List<VehicleImageDto> objs)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Sale)
                .ThenInclude(c => c.CustomerIcDocuments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (stock == null)
            {
                return BadRequest();
            }

            foreach (var obj in objs)
            {
                _context.CustomerIcDocuments.Add(new CustomerIcDocument
                {
                    SaleId = stock.SaleId,
                    DocumentId = obj.Id
                });
            }

            _context.Entry(stock).State = EntityState.Modified;
            _context.Entry(stock.Sale).State = EntityState.Modified;

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

        [HttpPut("RemoveCustomerIcDocument/{saleId}/{documentId}")]
        public async Task<IActionResult> PutRemoveCustomerIcDocument(Guid saleId, Guid documentId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                var documents = _context.CustomerIcDocuments
                    .Where(c => c.SaleId == saleId)
                    .Where(c => c.DocumentId == documentId)
                    .ToList();

                foreach (var inDataBase in documents)
                {
                    _context.CustomerIcDocuments.Remove(inDataBase);
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

            var expenseItem = new ExpenseItem
            {
                ExpenseId = stock.ExpenseId,
                ExpenseDate = obj.ExpenseDate,
                Category = obj.Category,
                Name = obj.Name,
                BillOrInvoiceNo = obj.BillOrInvoiceNo,
                Amount = obj.Amount,
                Remarks = obj.Remarks,
                DocumentId = obj.DocumentId
            };

            _context.ExpenseItems.Add(expenseItem);
            stock.Expense.Expenses.Add(expenseItem);

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

        [HttpPut("UpdateExpenseItem/{id}")]
        public async Task<IActionResult> PutUpdateExpenseItem(Guid id, ExpenseItem obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

            if (user == null)
                return Unauthorized();

            var ba = _context.ExpenseItems.Find(obj.Id);

            if (ba == null)
            {
                return NotFound("Not found.");
            }

            ba.ExpenseDate = obj.ExpenseDate;
            ba.Category = obj.Category;
            ba.Name = obj.Name;
            ba.BillOrInvoiceNo = obj.BillOrInvoiceNo;
            ba.Amount = obj.Amount;
            ba.Remarks = obj.Remarks;
            ba.DocumentId = obj.DocumentId;

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


        
 [HttpPut("adsinfo/{id}")]
        public async Task<IActionResult> PutAddAdministrativeCostItem(Guid id, Stock obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            var stock = await _context.Stocks
                .Include(c => c.Vehicle)
                .Include(c => c.Advertisement)
                .FirstOrDefaultAsync(c => c.Id == id);


            if (stock == null)
            {
                return BadRequest();
            }

            obj.Advertisement.MudahStartDate = DateTime.SpecifyKind(obj.Advertisement.MudahStartDate.Date.AddHours(12), DateTimeKind.Utc);
            obj.Advertisement.MudahEndDate = DateTime.SpecifyKind(obj.Advertisement.MudahEndDate.Date.AddHours(12), DateTimeKind.Utc);

            obj.Advertisement.CarListStartDate = DateTime.SpecifyKind(obj.Advertisement.CarListStartDate.Date.AddHours(12), DateTimeKind.Utc);
            obj.Advertisement.CarListEndDate = DateTime.SpecifyKind(obj.Advertisement.CarListEndDate.Date.AddHours(12), DateTimeKind.Utc);

            obj.Advertisement.CariCarzStartDate = DateTime.SpecifyKind(obj.Advertisement.CariCarzStartDate.Date.AddHours(12), DateTimeKind.Utc);
            obj.Advertisement.CariCarzEndDate = DateTime.SpecifyKind(obj.Advertisement.CariCarzEndDate.Date.AddHours(12), DateTimeKind.Utc);


            stock.Vehicle.IsLocalImageAvailable = obj.Vehicle.IsLocalImageAvailable;
            stock.Vehicle.IsSupplierImageAvailable = obj.Vehicle.IsSupplierImageAvailable;

            stock.Advertisement.CariCarzStartDate = obj.Advertisement.CariCarzStartDate;
            stock.Advertisement.MudahStartDate = obj.Advertisement.MudahStartDate;
            stock.Advertisement.CarListStartDate = obj.Advertisement.CarListStartDate;

            stock.Advertisement.CarListEndDate = obj.Advertisement.CarListEndDate;
            stock.Advertisement.MudahEndDate = obj.Advertisement.MudahEndDate;
            stock.Advertisement.CariCarzEndDate = obj.Advertisement.CariCarzEndDate;

            stock.Advertisement.CariCarzHadAdviertized = obj.Advertisement.CariCarzHadAdviertized;
            stock.Advertisement.MudahHadAdviertized = obj.Advertisement.MudahHadAdviertized;
            stock.Advertisement.CarListHadAdviertized = obj.Advertisement.CarListHadAdviertized;





            _context.Entry(stock.Vehicle).State = EntityState.Modified;
            _context.Entry(stock.Advertisement).State = EntityState.Modified;


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

        [HttpPut("UpdateAdministrativeCostItem/{id}")]
        public async Task<IActionResult> PutUpdateAdministrativeCostItem(Guid id, AdminitrativeCostItem obj)
        {
            var user = await _context.Users
                .Include(c => c.Profile)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());

            if (user == null)
                return Unauthorized();

            var ba = _context.AdminitrativeCostItems.Find(obj.Id);

            if (ba == null)
            {
                return NotFound("Not found.");
            }

            ba.Name = obj.Name;
            ba.Amount = obj.Amount;
            ba.Remarks = obj.Remarks;

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


        [HttpGet("arrival-states")]
        public IActionResult GetStockArrivalStates()
        {
            var states = Enum.GetValues(typeof(ArrivalState))
                             .Cast<ArrivalState>()
                             .Select(e => new { Id = (int)e, Name = e.ToString() })
                             .ToList();

            return Ok(states);
        }


        [HttpPut("UpdateStockNo/{id}")]
        public async Task<IActionResult> PutUpdateStockNo(Guid id, UpdateStockNoDto newStockNo)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)
                return Unauthorized();

            try
            {
                // Check if the stock exists
                var stock = await _context.Stocks.FirstOrDefaultAsync(c => c.Id == id);
                if (stock == null)
                {
                    return NotFound("Stock not found.");
                }

                // Check for duplicate stock number (excluding current stock)
                var duplicateExists = await _context.Stocks
                    .AnyAsync(s => s.StockNo.ToLower() == newStockNo.NewStockNo.ToLower() && s.Id != id);

                if (duplicateExists)
                {
                    return BadRequest($"Stock number '{newStockNo.NewStockNo}' already exists. Please choose a different stock number.");
                }

                // Update the stock number
                stock.StockNo = newStockNo.NewStockNo;
                stock.UpdatedOn = DateTime.UtcNow;

                _context.Entry(stock).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Stock number updated successfully.", stockId = id, newStockNo = newStockNo.NewStockNo });
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
                return BadRequest($"An error occurred while updating the stock number: {ex.Message}");

            }
        }
        // POST: api/Stocks/bulk-upload
        // Validates CSV file and returns validation results
        [HttpPost("bulk-upload")]
        public async Task<ActionResult<StockImportResultDto>> BulkUpload([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded or file is empty" });
                }

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Only CSV files are allowed" });
                }

                using (var stream = file.OpenReadStream())
                {
                    var result = await _importService.ValidateImportAsync(stream);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error processing file: {ex.Message}" });
            }
        }

        // POST: api/Stocks/bulk-import/confirm
        // Executes the import after validation
        [HttpPost("bulk-import/confirm")]
        public async Task<ActionResult<StockImportCompleteDto>> ConfirmBulkImport([FromBody] System.Text.Json.JsonElement requestBody)
        {
            Console.WriteLine("===============================================");
            Console.WriteLine("[CONTROLLER] ConfirmBulkImport METHOD ENTERED!");
            Console.WriteLine($"[CONTROLLER] requestBody ValueKind: {requestBody.ValueKind}");
            Console.WriteLine($"[CONTROLLER] requestBody raw: {requestBody}");
            Console.WriteLine("===============================================");

            // Try to extract the properties from JsonElement
            string validationToken = null;
            bool importOnlyValid = true;

            try
            {
                if (requestBody.TryGetProperty("validationToken", out var tokenElement))
                {
                    validationToken = tokenElement.GetString();
                }
                else if (requestBody.TryGetProperty("ValidationToken", out var tokenElement2))
                {
                    validationToken = tokenElement2.GetString();
                }

                if (requestBody.TryGetProperty("importOnlyValid", out var importElement))
                {
                    importOnlyValid = importElement.GetBoolean();
                }
                else if (requestBody.TryGetProperty("ImportOnlyValid", out var importElement2))
                {
                    importOnlyValid = importElement2.GetBoolean();
                }

                Console.WriteLine($"[CONTROLLER] Extracted - ValidationToken: {validationToken}, ImportOnlyValid: {importOnlyValid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONTROLLER] Error extracting properties: {ex.Message}");
                Console.WriteLine($"[CONTROLLER] Stack trace: {ex.StackTrace}");
            }

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
                Console.WriteLine($"[CONTROLLER] User found: {user?.UserName ?? "NULL"}");
                if (user == null)
                    return Unauthorized();

                if (user.ProfileId == null)
                    return BadRequest(new { message = "User profile not found" });

                if (string.IsNullOrEmpty(validationToken))
                    return BadRequest(new { message = "Validation token is required" });

                Console.WriteLine($"[CONTROLLER] Calling ExecuteImportAsync with ProfileId: {user.ProfileId}");
                var result = await _importService.ExecuteImportAsync(
                    validationToken,
                    importOnlyValid,
                    (Guid)user.ProfileId
                );
                Console.WriteLine($"[CONTROLLER] ExecuteImportAsync returned. Success: {result.Success}");

                if (!result.Success)
                {
                    Console.WriteLine($"[CONTROLLER] Import failed with message: {result.Message}");
                    return BadRequest(result);
                }

                Console.WriteLine($"[CONTROLLER] Import succeeded! Returning OK");
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CONTROLLER] Exception in ConfirmBulkImport: {ex.Message}");
                Console.WriteLine($"[CONTROLLER] Stack trace: {ex.StackTrace}");
                return BadRequest(new { message = $"Error executing import: {ex.Message}" });
            }
        }

        // TEST ENDPOINT - POST: api/Stocks/test-confirm
        [HttpPost("test-confirm")]
        public ActionResult<object> TestConfirm([FromBody] StockImportConfirmDto confirmDto)
        {
            Console.WriteLine("===============================================");
            Console.WriteLine("[TEST] TEST-CONFIRM ENDPOINT HIT!");
            Console.WriteLine($"[TEST] confirmDto is null: {confirmDto == null}");
            if (confirmDto != null)
            {
                Console.WriteLine($"[TEST] ValidationToken: {confirmDto.ValidationToken}");
                Console.WriteLine($"[TEST] ImportOnlyValid: {confirmDto.ImportOnlyValid}");
            }
            Console.WriteLine("===============================================");
            return Ok(new { success = true, message = "Test endpoint works!", token = confirmDto?.ValidationToken });
        }

        // GET: api/Stocks/bulk-import/template
        // Downloads CSV template file
        [HttpGet("bulk-import/template")]
        public IActionResult DownloadTemplate()
        {
            try
            {
                var templateBytes = _importService.GenerateTemplate();
                return File(templateBytes, "text/csv", "stock_import_template.csv");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error generating template: {ex.Message}" });
            }
        }
    }
}
