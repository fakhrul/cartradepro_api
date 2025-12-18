using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using SPOT_API.DTOs;
using SPOT_API.Models;
using SPOT_API.Persistence;

namespace SPOT_API.Services
{
    public class StockImportService
    {
        private readonly SpotDBContext _context;
        private static Dictionary<string, List<StockImportDto>> _validationCache = new Dictionary<string, List<StockImportDto>>();

        public StockImportService(SpotDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Parses and validates CSV file for stock import
        /// </summary>
        public async Task<StockImportResultDto> ValidateImportAsync(Stream fileStream)
        {
            var result = new StockImportResultDto();
            var rows = new List<StockImportDto>();

            try
            {
                // Parse CSV file
                using (var reader = new StreamReader(fileStream))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    TrimOptions = TrimOptions.Trim,
                    MissingFieldFound = null,
                    HeaderValidated = null
                }))
                {
                    var records = csv.GetRecords<StockImportDto>().ToList();

                    int rowNumber = 2; // Start at 2 (1 is header)
                    foreach (var record in records)
                    {
                        record.RowNumber = rowNumber++;
                        rows.Add(record);
                    }
                }

                result.TotalRows = rows.Count;

                if (rows.Count == 0)
                {
                    result.Errors.Add(new StockImportErrorDto
                    {
                        Row = 0,
                        Field = "File",
                        Error = "CSV file is empty or has no data rows",
                        Value = ""
                    });
                    return result;
                }

                // Load reference data once for validation
                var existingStockNos = await _context.Stocks.Select(s => s.StockNo.ToLower()).ToListAsync();
                var existingChasisNos = await _context.Vehicles
                    .Where(v => !string.IsNullOrEmpty(v.ChasisNo))
                    .Select(v => v.ChasisNo.ToLower())
                    .ToListAsync();

                var brands = await _context.Brands.ToListAsync();
                var models = await _context.Models.ToListAsync();
                var vehicleTypes = await _context.VehicleTypes.ToListAsync();
                var suppliers = await _context.Suppliers.ToListAsync();
                var showrooms = await _context.ShowRooms.ToListAsync();

                // Track duplicates within the CSV file itself
                var csvStockNos = new HashSet<string>();
                var csvChasisNos = new HashSet<string>();

                // Validate each row
                foreach (var row in rows)
                {
                    bool isValid = true;

                    // 1. MANDATORY FIELDS
                    if (string.IsNullOrWhiteSpace(row.StockNo))
                    {
                        result.Errors.Add(CreateError(row.RowNumber, "StockNo", "Stock No is required", row.StockNo));
                        isValid = false;
                        result.Summary.ValidationErrors++;
                    }
                    else
                    {
                        var stockNoLower = row.StockNo.Trim().ToLower();

                        // Check database duplicates
                        if (existingStockNos.Contains(stockNoLower))
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "StockNo", "Stock No already exists in database", row.StockNo));
                            isValid = false;
                            result.Summary.DuplicateStockNo++;
                        }
                        // Check CSV duplicates
                        else if (csvStockNos.Contains(stockNoLower))
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "StockNo", "Duplicate Stock No within CSV file", row.StockNo));
                            isValid = false;
                            result.Summary.DuplicateStockNo++;
                        }
                        else
                        {
                            csvStockNos.Add(stockNoLower);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(row.ChasisNo))
                    {
                        result.Errors.Add(CreateError(row.RowNumber, "ChasisNo", "Chasis No is required", row.ChasisNo));
                        isValid = false;
                        result.Summary.ValidationErrors++;
                    }
                    else
                    {
                        var chasisNoLower = row.ChasisNo.Trim().ToLower();

                        // Check database duplicates
                        if (existingChasisNos.Contains(chasisNoLower))
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "ChasisNo", "Chasis No already exists in database", row.ChasisNo));
                            isValid = false;
                            result.Summary.DuplicateChasisNo++;
                        }
                        // Check CSV duplicates
                        else if (csvChasisNos.Contains(chasisNoLower))
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "ChasisNo", "Duplicate Chasis No within CSV file", row.ChasisNo));
                            isValid = false;
                            result.Summary.DuplicateChasisNo++;
                        }
                        else
                        {
                            csvChasisNos.Add(chasisNoLower);
                        }
                    }

                    // 2. REFERENCE DATA VALIDATION
                    if (!string.IsNullOrWhiteSpace(row.BrandName))
                    {
                        if (!brands.Any(b => b.Name.Equals(row.BrandName.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "BrandName", "Brand not found in system", row.BrandName));
                            isValid = false;
                            result.Summary.MissingReferenceData++;
                        }
                    }

                    // ModelName - auto-created if doesn't exist, no validation error needed
                    // if (!string.IsNullOrWhiteSpace(row.ModelName))
                    // {
                    //     if (!models.Any(m => m.Name.Equals(row.ModelName.Trim(), StringComparison.OrdinalIgnoreCase)))
                    //     {
                    //         result.Errors.Add(CreateError(row.RowNumber, "ModelName", "Model not found in system", row.ModelName));
                    //         isValid = false;
                    //         result.Summary.MissingReferenceData++;
                    //     }
                    // }

                    if (!string.IsNullOrWhiteSpace(row.VehicleTypeName))
                    {
                        if (!vehicleTypes.Any(vt => vt.Name.Equals(row.VehicleTypeName.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "VehicleTypeName", "Vehicle Type not found in system", row.VehicleTypeName));
                            isValid = false;
                            result.Summary.MissingReferenceData++;
                        }
                    }

                    // SupplierName - auto-created if doesn't exist, no validation error needed
                    // if (!string.IsNullOrWhiteSpace(row.SupplierName))
                    // {
                    //     if (!suppliers.Any(s => s.Name.Equals(row.SupplierName.Trim(), StringComparison.OrdinalIgnoreCase)))
                    //     {
                    //         result.Errors.Add(CreateError(row.RowNumber, "SupplierName", "Supplier not found in system", row.SupplierName));
                    //         isValid = false;
                    //         result.Summary.MissingReferenceData++;
                    //     }
                    // }

                    // ShowRoomLotNo - auto-created if doesn't exist, no validation error needed
                    // if (!string.IsNullOrWhiteSpace(row.ShowRoomLotNo))
                    // {
                    //     if (!showrooms.Any(s => s.LotNo.Equals(row.ShowRoomLotNo.Trim(), StringComparison.OrdinalIgnoreCase)))
                    //     {
                    //         result.Errors.Add(CreateError(row.RowNumber, "ShowRoomLotNo", "Showroom not found in system", row.ShowRoomLotNo));
                    //         isValid = false;
                    //         result.Summary.MissingReferenceData++;
                    //     }
                    // }

                    // 3. DATA TYPE VALIDATION
                    if (!string.IsNullOrWhiteSpace(row.Year))
                    {
                        if (!int.TryParse(row.Year, out int year) || year < 1900 || year > DateTime.Now.Year + 1)
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "Year", "Invalid year format (expected 1900-" + (DateTime.Now.Year + 1) + ")", row.Year));
                            isValid = false;
                            result.Summary.ValidationErrors++;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(row.Month))
                    {
                        if (!int.TryParse(row.Month, out int month) || month < 1 || month > 12)
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "Month", "Invalid month (expected 1-12)", row.Month));
                            isValid = false;
                            result.Summary.ValidationErrors++;
                        }
                    }

                    // Pricing validation
                    if (row.RecommendedSalePrice < 0)
                    {
                        result.Errors.Add(CreateError(row.RowNumber, "RecommendedSalePrice", "Price cannot be negative", row.RecommendedSalePrice.ToString()));
                        isValid = false;
                        result.Summary.ValidationErrors++;
                    }

                    if (row.MinimumSalePrice < 0)
                    {
                        result.Errors.Add(CreateError(row.RowNumber, "MinimumSalePrice", "Price cannot be negative", row.MinimumSalePrice.ToString()));
                        isValid = false;
                        result.Summary.ValidationErrors++;
                    }

                    if (row.VehiclePriceSupplierCurrency < 0)
                    {
                        result.Errors.Add(CreateError(row.RowNumber, "VehiclePriceSupplierCurrency", "Price cannot be negative", row.VehiclePriceSupplierCurrency.ToString()));
                        isValid = false;
                        result.Summary.ValidationErrors++;
                    }

                    if (row.VehiclePriceLocalCurrency < 0)
                    {
                        result.Errors.Add(CreateError(row.RowNumber, "VehiclePriceLocalCurrency", "Price cannot be negative", row.VehiclePriceLocalCurrency.ToString()));
                        isValid = false;
                        result.Summary.ValidationErrors++;
                    }

                    // 4. ARRIVAL STATE VALIDATION
                    if (!string.IsNullOrWhiteSpace(row.ArrivalState))
                    {
                        var validStates = new[] { "Incoming", "Received" };
                        if (!validStates.Any(s => s.Equals(row.ArrivalState.Trim(), StringComparison.OrdinalIgnoreCase)))
                        {
                            result.Errors.Add(CreateError(row.RowNumber, "ArrivalState", "Invalid Arrival State (expected: Incoming or Received)", row.ArrivalState));
                            isValid = false;
                            result.Summary.ValidationErrors++;
                        }
                    }

                    if (isValid)
                    {
                        result.ValidRows++;
                        result.Summary.NewStocks++;
                    }
                    else
                    {
                        result.InvalidRows++;
                    }
                }

                // Generate validation token for confirmation
                if (result.ValidRows > 0)
                {
                    var validationToken = Guid.NewGuid().ToString();
                    result.ValidationToken = validationToken;

                    // Cache valid rows for later import
                    _validationCache[validationToken] = rows.Where(r =>
                        !result.Errors.Any(e => e.Row == r.RowNumber)
                    ).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new StockImportErrorDto
                {
                    Row = 0,
                    Field = "File",
                    Error = $"Error parsing CSV file: {ex.Message}",
                    Value = ""
                });
                return result;
            }
        }

        /// <summary>
        /// Executes the confirmed import using validation token
        /// </summary>
        public async Task<StockImportCompleteDto> ExecuteImportAsync(string validationToken, bool importOnlyValid, Guid userId)
        {
            var result = new StockImportCompleteDto();

            try
            {
                Console.WriteLine($"[IMPORT] Starting import with token: {validationToken}, userId: {userId}");

                // Retrieve cached validation data
                if (!_validationCache.ContainsKey(validationToken))
                {
                    Console.WriteLine($"[IMPORT] ERROR: Validation token not found in cache");
                    result.Success = false;
                    result.Message = "Invalid or expired validation token";
                    return result;
                }

                var rows = _validationCache[validationToken];
                Console.WriteLine($"[IMPORT] Found {rows.Count} rows to import");

                // Load reference data (filter out null/empty names to avoid duplicate key errors)
                var brands = await _context.Brands
                    .Where(b => !string.IsNullOrWhiteSpace(b.Name))
                    .GroupBy(b => b.Name.ToLower())
                    .Select(g => g.First())
                    .ToDictionaryAsync(b => b.Name.ToLower(), b => b.Id);
                var models = await _context.Models
                    .Where(m => !string.IsNullOrWhiteSpace(m.Name))
                    .GroupBy(m => m.Name.ToLower())
                    .Select(g => g.First())
                    .ToDictionaryAsync(m => m.Name.ToLower(), m => m.Id);
                var vehicleTypes = await _context.VehicleTypes
                    .Where(vt => !string.IsNullOrWhiteSpace(vt.Name))
                    .GroupBy(vt => vt.Name.ToLower())
                    .Select(g => g.First())
                    .ToDictionaryAsync(vt => vt.Name.ToLower(), vt => vt.Id);
                var suppliers = await _context.Suppliers
                    .Where(s => !string.IsNullOrWhiteSpace(s.Name))
                    .GroupBy(s => s.Name.ToLower())
                    .Select(g => g.First())
                    .ToDictionaryAsync(s => s.Name.ToLower(), s => s.Id);
                var showrooms = await _context.ShowRooms
                    .Where(s => !string.IsNullOrWhiteSpace(s.LotNo))
                    .GroupBy(s => s.LotNo.ToLower())
                    .Select(g => g.First())
                    .ToDictionaryAsync(s => s.LotNo.ToLower(), s => s.Id);
                var draftStatus = await _context.StockStatuses.FirstOrDefaultAsync(s => s.Name == "Draft");
                Console.WriteLine($"[IMPORT] Loaded reference data: {brands.Count} brands, {models.Count} models, {suppliers.Count} suppliers, {showrooms.Count} showrooms");
                Console.WriteLine($"[IMPORT] Draft status: {(draftStatus != null ? draftStatus.Id.ToString() : "NULL")}");

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    Console.WriteLine($"[IMPORT] Transaction started");
                    try
                    {
                        foreach (var row in rows)
                        {
                            Console.WriteLine($"[IMPORT] Processing row {row.RowNumber}: StockNo={row.StockNo}");
                            try
                            {
                                // Create all child entities (following Stock.Post pattern)
                                var vehicle = new Vehicle
                                {
                                    ChasisNo = row.ChasisNo?.Trim(),
                                    EngineNo = row.EngineNo?.Trim(),
                                    EngineCapacity = row.EngineCapacity?.Trim(),
                                    Year = !string.IsNullOrWhiteSpace(row.Year) ? row.Year.Trim() : null,
                                    Month = !string.IsNullOrWhiteSpace(row.Month) ? row.Month.Trim() : null,
                                    Color = row.Color?.Trim(),
                                    CodeModel = row.CodeModel?.Trim(),
                                    Description = row.Description?.Trim(),
                                    ExternalLink = row.ExternalLink?.Trim()
                                };

                                // Set brand if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.BrandName))
                                {
                                    var brandKey = row.BrandName.ToLower();
                                    if (!brands.ContainsKey(brandKey))
                                    {
                                        var newBrand = new Brand { Name = row.BrandName.Trim() };
                                        await _context.Brands.AddAsync(newBrand);
                                        await _context.SaveChangesAsync();
                                        brands[brandKey] = newBrand.Id;
                                    }
                                    vehicle.BrandId = brands[brandKey];
                                }

                                // Set model if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.ModelName))
                                {
                                    var modelKey = row.ModelName.ToLower();
                                    if (!models.ContainsKey(modelKey))
                                    {
                                        // Model requires a BrandId - use vehicle's BrandId or Guid.Empty if no brand
                                        var newModel = new Model
                                        {
                                            Name = row.ModelName.Trim(),
                                            BrandId = vehicle.BrandId ?? Guid.Empty
                                        };
                                        await _context.Models.AddAsync(newModel);
                                        await _context.SaveChangesAsync();
                                        models[modelKey] = newModel.Id;
                                    }
                                    vehicle.ModelId = models[modelKey];
                                }

                                // Set vehicle type if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.VehicleTypeName))
                                {
                                    var vehicleTypeKey = row.VehicleTypeName.ToLower();
                                    if (!vehicleTypes.ContainsKey(vehicleTypeKey))
                                    {
                                        var newVehicleType = new VehicleType { Name = row.VehicleTypeName.Trim() };
                                        await _context.VehicleTypes.AddAsync(newVehicleType);
                                        await _context.SaveChangesAsync();
                                        vehicleTypes[vehicleTypeKey] = newVehicleType.Id;
                                    }
                                    vehicle.VehicleTypeId = vehicleTypes[vehicleTypeKey];
                                }

                                var purchase = new Purchase();

                                // Set supplier if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.SupplierName))
                                {
                                    var supplierKey = row.SupplierName.ToLower();
                                    if (!suppliers.ContainsKey(supplierKey))
                                    {
                                        var newSupplier = new Supplier { Name = row.SupplierName.Trim() };
                                        await _context.Suppliers.AddAsync(newSupplier);
                                        await _context.SaveChangesAsync();
                                        suppliers[supplierKey] = newSupplier.Id;
                                    }
                                    purchase.SupplierId = suppliers[supplierKey];
                                }


                                // Set purchase prices
                                if (row.VehiclePriceSupplierCurrency > 0)
                                    purchase.VehiclePriceSupplierCurrency = row.VehiclePriceSupplierCurrency;

                                if (row.VehiclePriceLocalCurrency > 0)
                                    purchase.VehiclePriceLocalCurrency = row.VehiclePriceLocalCurrency;

                                // Set currency if provided
                                if (!string.IsNullOrWhiteSpace(row.SupplierCurrency))
                                {
                                    purchase.SupplierCurrency = row.SupplierCurrency.Trim();
                                }

                                var import = new Import();
                                var clearance = new Clearance();

                                var pricing = new Pricing();
                                if (row.RecommendedSalePrice > 0)
                                    pricing.RecommendedSalePrice = row.RecommendedSalePrice;

                                if (row.MinimumSalePrice > 0)
                                    pricing.MinimumSalePrice = row.MinimumSalePrice;

                                await _context.Pricings.AddAsync(pricing);

                                // Arrival checklist
                                var arrivalCheckList = new ArrivalChecklist
                                {
                                    ArrivalChecklists = new List<ArrivalChecklistItem>
                                    {
                                        new ArrivalChecklistItem
                                        {
                                            Name = "Extra Key",
                                            IsAvailable = true,
                                            Remarks = ""
                                        }
                                    }
                                };
                                await _context.ArrivalChecklists.AddAsync(arrivalCheckList);

                                // Sale & Loan
                                var loan = new Loan();
                                var sale = new Sale
                                {
                                    SaleDateTime = DateTime.UtcNow,
                                    LoanId = loan.Id
                                };
                                await _context.Loans.AddAsync(loan);

                                // Registration
                                var registration = new Registration();
                                await _context.Registrations.AddAsync(registration);

                                // Expense
                                var expense = new Expense();
                                await _context.Expenses.AddAsync(expense);

                                // Advertisement
                                var ads = new Advertisement();
                                await _context.Advertisements.AddAsync(ads);

                                // Administrative cost
                                var administrativeCost = new AdminitrativeCost();
                                await _context.AdminitrativeCosts.AddAsync(administrativeCost);

                                // ApCompany
                                var apCompany = new ApCompany();
                                await _context.ApCompanies.AddAsync(apCompany);

                                // Create Stock
                                var stock = new Stock
                                {
                                    StockNo = row.StockNo.Trim(),
                                    VehicleId = vehicle.Id,
                                    PurchaseId = purchase.Id,
                                    ImportId = import.Id,
                                    ClearanceId = clearance.Id,
                                    PricingId = pricing.Id,
                                    ArrivalChecklistId = arrivalCheckList.Id,
                                    SaleId = sale.Id,
                                    RegistrationId = registration.Id,
                                    ExpenseId = expense.Id,
                                    AdvertisementId = ads.Id,
                                    AdminitrativeCostId = administrativeCost.Id,
                                    ApCompanyId = apCompany.Id,
                                    LocationCode = row.LocationCode?.Trim(),
                                    MovedToStockAt = DateTime.UtcNow,
                                    RegistrationDate = DateTime.UtcNow
                                };

                                // Set showroom if provided
                                // Set showroom if provided - auto-create if doesn't exist (case-insensitive)
                                if (!string.IsNullOrWhiteSpace(row.ShowRoomLotNo))
                                {
                                    var showroomKey = row.ShowRoomLotNo.ToLower();
                                    if (!showrooms.ContainsKey(showroomKey))
                                    {
                                        var newShowroom = new ShowRoom { LotNo = row.ShowRoomLotNo.Trim(), Name = row.ShowRoomLotNo.Trim() };
                                        await _context.ShowRooms.AddAsync(newShowroom);
                                        await _context.SaveChangesAsync();
                                        showrooms[showroomKey] = newShowroom.Id;
                                    }
                                    stock.ShowRoomId = showrooms[showroomKey];
                                }

                                // Set arrival state
                                if (!string.IsNullOrWhiteSpace(row.ArrivalState))
                                {
                                    if (row.ArrivalState.Equals("Received", StringComparison.OrdinalIgnoreCase))
                                        stock.ArrivalState = ArrivalState.Received;
                                    else
                                        stock.ArrivalState = ArrivalState.Incoming;
                                }

                                // Add stock status history
                                stock.StockStatusHistories = new List<StockStatusHistory>
                                {
                                    new StockStatusHistory
                                    {
                                        ProfileId = userId,
                                        StockStatusId = draftStatus.Id,
                                        StockId = stock.Id,
                                        DateTime = DateTime.UtcNow
                                    }
                                };

                                // Add all entities to context
                                await _context.Vehicles.AddAsync(vehicle);
                                await _context.Purchases.AddAsync(purchase);
                                await _context.Imports.AddAsync(import);
                                await _context.Clearances.AddAsync(clearance);
                                await _context.Sales.AddAsync(sale);
                                await _context.Stocks.AddAsync(stock);
                                Console.WriteLine($"[IMPORT] Row {row.RowNumber}: Stock entity created with ID={stock.Id}, StockNo={stock.StockNo}");

                                result.ImportedCount++;
                            }
                            catch (Exception rowEx)
                            {
                                Console.WriteLine($"[IMPORT] Row {row.RowNumber} ERROR: {rowEx.Message}");
                                Console.WriteLine($"[IMPORT] Stack trace: {rowEx.StackTrace}");
                                result.FailedCount++;
                                result.Errors.Add(new StockImportErrorDto
                                {
                                    Row = row.RowNumber,
                                    Field = "Stock",
                                    Error = $"Failed to create stock: {rowEx.Message}",
                                    Value = row.StockNo
                                });

                                if (!importOnlyValid)
                                {
                                    Console.WriteLine($"[IMPORT] Rolling back entire transaction due to error");
                                    throw; // Rollback entire transaction
                                }
                            }
                        }

                        Console.WriteLine($"[IMPORT] Saving changes to database...");
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"[IMPORT] Changes saved. Committing transaction...");
                        await transaction.CommitAsync();
                        Console.WriteLine($"[IMPORT] Transaction committed successfully!");

                        result.Success = true;
                        result.Message = $"Successfully imported {result.ImportedCount} stocks. Failed: {result.FailedCount}";

                        // Clear cache
                        _validationCache.Remove(validationToken);
                        Console.WriteLine($"[IMPORT] Cache cleared for token: {validationToken}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[IMPORT] TRANSACTION ERROR: {ex.Message}");
                        Console.WriteLine($"[IMPORT] Stack trace: {ex.StackTrace}");
                        await transaction.RollbackAsync();
                        Console.WriteLine($"[IMPORT] Transaction rolled back");
                        result.Success = false;
                        result.Message = $"Import failed: {ex.Message}";
                        result.ImportedCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IMPORT] EXECUTION ERROR: {ex.Message}");
                Console.WriteLine($"[IMPORT] Stack trace: {ex.StackTrace}");
                result.Success = false;
                result.Message = $"Import execution error: {ex.Message}";
            }

            Console.WriteLine($"[IMPORT] Import completed. Success: {result.Success}, Imported: {result.ImportedCount}, Failed: {result.FailedCount}");
            return result;
        }

        /// <summary>
        /// Generates a CSV template file with sample data
        /// </summary>
        public byte[] GenerateTemplate()
        {
            var sb = new StringBuilder();

            // CSV Header
            sb.AppendLine("StockNo,ChasisNo,BrandName,ModelName,VehicleTypeName,EngineNo,EngineCapacity,Year,Month,Color,CodeModel,Description,ExternalLink,RecommendedSalePrice,MinimumSalePrice,SupplierName,VehiclePriceSupplierCurrency,VehiclePriceLocalCurrency,SupplierCurrency,ShowRoomLotNo,LocationCode,ArrivalState");

            // Sample Data Row 1
            sb.AppendLine("STK001,ABC123XYZ456,Toyota,Camry,Sedan,1AZ123456,2500,2020,3,White,ACV30,Premium sedan with sunroof and leather seats,https://example.com/vehicles/camry-2020,85000,80000,Japan Auto Supplier,2500000,85000,JPY,LOT001,YARD-A,Incoming");

            // Sample Data Row 2
            sb.AppendLine("STK002,DEF789GHI012,Honda,Accord,Sedan,K24A987654,2400,2019,11,Black,CU2,Sporty sedan with excellent fuel economy,https://example.com/vehicles/accord-2019,78000,73000,Tokyo Motors,2300000,78000,JPY,LOT002,YARD-B,Received");

            // Sample Data Row 3
            sb.AppendLine("STK003,JKL345MNO678,Nissan,X-Trail,SUV,QR25456789,2500,2021,7,Silver,T32,Family SUV with 4WD capability,https://example.com/vehicles/xtrail-2021,95000,90000,Osaka Trading,2800000,95000,JPY,LOT001,YARD-A,Incoming");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private StockImportErrorDto CreateError(int row, string field, string error, string value)
        {
            return new StockImportErrorDto
            {
                Row = row,
                Field = field,
                Error = error,
                Value = value ?? ""
            };
        }
    }
}
