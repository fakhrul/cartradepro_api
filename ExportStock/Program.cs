using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using SPOT_API.Models;
using SPOT_API.Persistence;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

class Program
{
    static void Main(string[] args)
    {
        // Specify the path to your Excel file
        string excelPath = @"C:\GitHub\cartradepro_api\LIST2.xlsx";

        // Load the Excel file
        ProcessFile(excelPath);

        Console.ReadLine();
    }

    static void ProcessFile(string excelPath)
    {
        string status = "AVAILABLE";
        string brand = "";
        IList<string> fullLineInfoList = new List<string>();

        FileInfo fileInfo = new FileInfo(excelPath);

        // Make sure EPPlus license context is set (required for non-commercial use)
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        DateTime fileDateTime = DateTime.MinValue;

        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
            var firstSheet = package.Workbook.Worksheets[0];
            string firstLine = GetFormattedCellText(firstSheet, 1, 1);

            fileDateTime = ExtractDateTime(firstLine);
            var imageLink = "";
            // Loop through all worksheets in the workbook
            foreach (var worksheet in package.Workbook.Worksheets)
            {
                int rows = worksheet.Dimension.Rows;
                int columns = worksheet.Dimension.Columns;

                for (int row = 1; row <= rows; row++)
                {
                    string fullLine = "";

                    for (int col = 1; col <= columns; col++)
                    {
                        try
                        {
                            string text = GetFormattedCellText(worksheet, row, col);

                            if(col == 2)
                            {
                                var cell = worksheet.Cells[row, col];
                                if (cell.Hyperlink != null)
                                {
                                    //Console.WriteLine($"Cell {cell.Address} has hyperlink: {cell.Hyperlink.AbsoluteUri}");
                                    imageLink = cell.Hyperlink.AbsoluteUri;
                                }
                            }

                            if (col == 1 && IsStatusIndicator(text))
                            {
                                status = text;
                                brand = "UNKNOWN";
                                break;
                            }

                            if (col == 1 && IsBrand(text))
                            {
                                brand = text;
                                break;
                            }

                            fullLine += text + "|";
                        }
                        catch (Exception ex)
                        {
                            // Log the error
                            Console.WriteLine($"Error processing cell at row {row}, column {col}: {ex.Message}");
                        }
                    }

                    if (!string.IsNullOrEmpty(fullLine) && !ShouldSkipLine(fullLine))
                    {
                        fullLine += $"|{status}|{brand}|";

                        fullLine += imageLink;

                        fullLineInfoList.Add(fullLine.Split('|').Length + "|" + fullLine);
                    }
                }
            }
        }

        string filePath = "temp.txt";
        try
        {
            // Write all lines to the file
            File.WriteAllLines(filePath, fullLineInfoList);
            Console.WriteLine($"Successfully written to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        //foreach (var l in fullLineInfoList)
        //{
        //    Console.WriteLine(l);
        //}

        //Console.ReadLine();

        //DownloadFileAsync(fullLineInfoList);
        InsertToDatabase(fullLineInfoList, fileDateTime);
    }

    //private static async Task DownloadFileAsync(IList<string> fullLineInfoList)
    //{
    //    string filePath = fullLineInfoList[0];
    //    var paths = filePath.Split("|");
    //    string folderUrl = paths[19];// "https://drive.google.com/drive/folders/1hCixwCYDI05ahOxW3ddRe5irZVv40Lgu?usp=sharing";
    //    string localFolderPath = @"C:\CarTradPro\" + paths[2];

    //    Directory.CreateDirectory(localFolderPath);

    //   await DownloadFilesFromPublicFolder(folderUrl, localFolderPath);

    //}

    //static async Task DownloadFilesFromPublicFolder(string folderUrl, string localFolderPath)
    //{
    //    using (HttpClient client = new HttpClient())
    //    {
    //        // Fetch the HTML of the folder page
    //        HttpResponseMessage response = await client.GetAsync(folderUrl);

    //        if (response.IsSuccessStatusCode)
    //        {
    //            string html = await response.Content.ReadAsStringAsync();

    //            // Use Regex to extract file links and IDs
    //            // Pattern to find URLs in Google Drive's folder HTML structure
    //            var regex = new Regex(@"<a href=\""(/file/d/[\w-]+)\"".*?aria-label=\""([^""]+)\""", RegexOptions.Compiled);
    //            var matches = regex.Matches(html);

    //            foreach (Match match in matches)
    //            {
    //                string fileRelativeUrl = match.Groups[1].Value;
    //                string fileName = match.Groups[2].Value;

    //                // Create the full file download URL
    //                string fileUrl = "https://drive.google.com" + fileRelativeUrl + "?export=download";

    //                // Download the file
    //                await DownloadFile(client, fileUrl, Path.Combine(localFolderPath, fileName));
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine($"Failed to retrieve folder page: {response.ReasonPhrase}");
    //        }
    //    }
    //}

    //static async Task DownloadFile(HttpClient client, string fileUrl, string destinationPath)
    //{
    //    Console.WriteLine($"Downloading {destinationPath}...");

    //    HttpResponseMessage fileResponse = await client.GetAsync(fileUrl);

    //    if (fileResponse.IsSuccessStatusCode)
    //    {
    //        byte[] fileBytes = await fileResponse.Content.ReadAsByteArrayAsync();

    //        // Ensure the local folder exists
    //        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

    //        // Save the file
    //        await File.WriteAllBytesAsync(destinationPath, fileBytes);
    //        Console.WriteLine($"Downloaded {destinationPath}");
    //    }
    //    else
    //    {
    //        Console.WriteLine($"Failed to download file from {fileUrl}: {fileResponse.ReasonPhrase}");
    //    }
    //}

    private static DateTime ExtractDateTime(string firstLine)
    {
        string pattern = @"\((\d{1,2}/\d{1,2}/\d{4})\)";

        // Use Regex to find the match
        Match match = System.Text.RegularExpressions.Regex.Match(firstLine, pattern);

        if (match.Success)
        {
            // Extract the date part from the match
            string dateString = match.Groups[1].Value;

            // Parse the date string into a DateTime object (optional)
            DateTime date;
            if (DateTime.TryParse(dateString, out date))
            {
                return date;
            }
            else
            {
                return DateTime.MinValue;

            }
        }
        else
        {
            return DateTime.MinValue;
        }
    }

    private static void InsertToDatabase(IList<string> fullLineInfoList, DateTime fileDateTime)
    {
        var options = new DbContextOptionsBuilder<SpotDBContext>()
            .UseNpgsql("Host=178.128.105.21;Port=5432;Database=cartradepro;Username=postgres;Password=Qwerty@123").Options;
            //.UseNpgsql("Host=localhost;Port=5432;Database=cartradepro;Username=postgres;Password=qwe123").Options;

        using (var context = new SpotDBContext(options))
        {
            foreach (var str in fullLineInfoList)
            {
                Console.WriteLine(str);
                try
                {
                    //var stockNo = ExtractStockNo(str);

                    var parts = str.Split('|');
                    var stockNo = parts.Length > 2 ? parts[2] : "";
                    var yearMade = "";
                    try
                    {
                        yearMade = DateTime.Parse(parts[3]).ToString("yyyy");
                    }
                    catch (Exception)
                    {
                    }
                    var monthMade = "";
                    try
                    {
                        monthMade = DateTime.Parse(parts[3]).ToString("MM");
                    }
                    catch (Exception)
                    {
                    }


                    var model = "";
                    var brand = "";
                    try
                    {
                        brand = parts[5].Split(" ")[0];
                    }
                    catch (Exception ex)
                    {
                    }

                    try
                    {
                        string[] modelBrand = parts[5].Split(' ');
                        model = string.Join(" ", modelBrand, 1, modelBrand.Length - 1);
                    }
                    catch (Exception ex)
                    {
                    }

                    var locationCode = parts[4];

                    var specification = parts[6];
                    string vehicleColor = parts.Length > 7 ? parts[7] : ""; //worksheet.Cells[row, 7].Text.Trim(); // Extracted Color
                    string chassisNo = parts.Length > 8 ? parts[8] : ""; //worksheet.Cells[row, 8].Text.Trim();

                    string supplierCode = parts.Length > 9 ? parts[9] : ""; //worksheet.Cells[row, 9].Text.Trim(); // Extracted Supplier

                    var eta = DateTime.MinValue;
                    try
                    {
                        eta = DateTime.Parse(parts[10]);

                    }
                    catch (Exception ex)
                    {
                    }

                    var status = parts[17];
                    ////


                    //string stockNo = worksheet.Cells[row, 2].Text.Trim();
                    //string status = parts.Length > 17 ? parts[17] : ""; //worksheet.Cells[row, 12].Text.Trim();
                    //string model = parts.Length > 5 ? parts[5] : ""; //worksheet.Cells[row, 5].Text.Trim();
                    //string etaText = parts.Length > 2 ? parts[2] : ""; //worksheet.Cells[row, 10].Text.Trim();


                    //DateTime eta = DateTime.MinValue; // Use a default value

                    //if (etaText != "N/A" && DateTime.TryParseExact(etaText, "yyyy.MM.dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedEta))
                    //{
                    //    eta = parsedEta;
                    //}

                    string sellingPrice = parts.Length > 11 ? parts[11] : "";// worksheet.Cells[row, 11].Text.Trim();
                    decimal sellingPriceValue = ExtractPrice(sellingPrice);

                    var remarks1 = parts[12];
                    var remarks2 = parts[13];
                    var remarks3 = parts[14];

                    //continue;
                    var existingStock = context.Stocks
                        .Include(s => s.StockStatusHistories)
                        .FirstOrDefault(s => s.StockNo == stockNo);

                    //Stock obj;
                    if (existingStock != null)
                    {
                        // If stock exists, update the existing record
                        //Stock obj = existingStock;

                        // Update related entities as necessary
                        //UpdateVehicle(context, obj);
                        //UpdatePurchase(context, obj);
                        //UpdateImport(context, obj);
                        //UpdateClearance(context, obj);
                        //UpdatePricing(context, obj);
                        //UpdateAdministrativeCost(context, obj);
                        //UpdateStockStatusHistories(context, obj);
                        //UpdateSale(context, obj);
                        //UpdateLoan(context, obj);
                        //UpdateRegistration(context, obj);
                        //UpdateExpense(context, obj);

                        //context.Stocks.Update(obj);
                    }
                    else
                    {
                        // If stock doesn't exist, create a new record
                        Stock obj = new Stock { StockNo = stockNo };


                        var vehicle = new Vehicle();
                        vehicle.Year = yearMade;
                        vehicle.Month = monthMade;


                        var brandInDb = context.Brands.Where(c => c.Name == brand).FirstOrDefault();
                        if (brandInDb == null)
                        {
                            brandInDb = new Brand { Name = brand };
                            context.Brands.Add(brandInDb);
                            context.SaveChanges();
                        }

                        var modelInDb = context.Models
                            .Where(c => c.BrandId == brandInDb.Id)
                            .Where(c => c.Name == model)
                            .FirstOrDefault();
                        if (modelInDb == null)
                        {
                            modelInDb = new Model { Name = model, BrandId = brandInDb.Id };
                            context.Models.Add(modelInDb);
                            context.SaveChanges();
                        }


                        vehicle.BrandId = brandInDb.Id;
                        vehicle.ModelId = modelInDb.Id;
                        vehicle.Color = vehicleColor;
                        vehicle.Description = specification;
                        vehicle.ChasisNo = chassisNo;

                        context.Vehicles.Add(vehicle);
                        obj.VehicleId = vehicle.Id;

                        obj.LocationCode = locationCode;

                        //Purchase 
                        var purchase = new Purchase();

                        var supplierInDb = context.Suppliers
                            .Where(c => c.Code == supplierCode)
                            .FirstOrDefault();

                        if (supplierInDb == null)
                        {
                            supplierInDb = new Supplier { Name = supplierCode, Code = supplierCode };
                            context.Suppliers.Add(supplierInDb);
                            context.SaveChanges();
                        }


                        purchase.SupplierId = supplierInDb.Id;
                        purchase.SupplierCurrency = "USD";
                        purchase.VehiclePriceSupplierCurrency = 0;
                        purchase.VehiclePriceLocalCurrency = 0;

                        context.Purchases.Add(purchase);
                        obj.PurchaseId = purchase.Id;


                        var import = new Import();

                        try
                        {
                            import.EstimateDateOfArrival = eta;
                            import.EstimateDateOfDeparture = import.EstimateDateOfArrival.AddMonths(-1);
                        }
                        catch (Exception ex)
                        {
                        }

                        context.Imports.Add(import);
                        obj.ImportId = import.Id;


                        var pricing = new Pricing();
                        pricing.RecommendedSalePrice = (float)sellingPriceValue;
                        pricing.MinimumSalePrice = (float)sellingPriceValue;

                        context.Pricings.Add(pricing);
                        obj.PricingId = pricing.Id;

                        //Stock obj = new Stock { StockNo = "20240621-" + stockNo.ToString() };

                        var stockStatus = context.StockStatuses.FirstOrDefault(c => c.Name == "Draft");
                        if (obj.StockStatusHistories == null)
                            obj.StockStatusHistories = new List<StockStatusHistory>();

                        obj.StockStatusHistories.Add(new StockStatusHistory
                        {
                            ProfileId = context.Profiles.Where(c => c.Role == "Admin").FirstOrDefault().Id,
                            StockStatusId = stockStatus.Id,
                            StockId = obj.Id,
                             DateTime = fileDateTime.AddDays(-1),
                        });


                        var currentStockStatus = context.StockStatuses.FirstOrDefault(c => c.Name == status);
                        if (currentStockStatus == null)
                        {
                            currentStockStatus = new StockStatus { Name = status };

                            context.StockStatuses.Add(currentStockStatus);
                            context.SaveChanges();
                        }

                        obj.StockStatusHistories.Add(new StockStatusHistory
                        {
                            ProfileId = context.Profiles.Where(c => c.Role == "Admin").FirstOrDefault().Id,
                            StockStatusId = currentStockStatus.Id,
                            StockId = obj.Id,
                             DateTime = fileDateTime,
                        });

                        //for (int i = 0; i < 5; i++)
                        //    obj.StockStatusHistories.Add(new StockStatusHistory
                        //    {
                        //        ProfileId = context.Profiles.Where(c => c.Role == "Admin").FirstOrDefault().Id,
                        //        StockStatusId = GetRandomStockStatus(context).Id,
                        //        StockId = obj.Id
                        //    });


                        var clearance = new Clearance();
                        context.Clearances.Add(clearance);
                        obj.ClearanceId = clearance.Id;

                        var arrivalCheckList = new ArrivalChecklist();

                        if (arrivalCheckList.ArrivalChecklists == null)
                            arrivalCheckList.ArrivalChecklists = new List<ArrivalChecklistItem>();

                        arrivalCheckList.ArrivalChecklists.Add(new ArrivalChecklistItem
                        {
                            ArrivalChecklistId = arrivalCheckList.Id,
                            Name = "Extra Key",
                            IsAvailable = false,
                            Remarks = ""
                        });
                        context.ArrivalChecklists.Add(arrivalCheckList);
                        obj.ArrivalChecklistId = arrivalCheckList.Id;

                        //var sellingPrice = new SellingPricing();
                        //obj.SellingPricingId = sellingPrice.Id;

                        //var loan = new Loan();
                        //_context.Loans.Add(loan);
                        var sale = new Sale();
                        //sale.SaleDateTime = GetRandomDate(2022, 2024);
                        //sale.CustomerId = context.Customers.Skip(random.Next(0, context.Customers.Count())).FirstOrDefault().Id;
                        //sale.SaleAmount = random.Next(150000, 250000);
                        //sale.DepositAmount = random.Next(1000, 5000);
                        //sale.TradeInAmount = random.Next(0, 50000);
                        //sale.IsUseLoan = true;
                        //sale.EoeAmount = 600;

                        //var loan = new Loan();
                        //sale.LoanId = loan.Id;
                        //_context.Sales.Add(sale);
                        obj.Sale = sale;
                        obj.SaleId = sale.Id;

                        var loan = new Loan();
                        //loan.BankId = context.Banks.Skip(random.Next(0, context.Banks.Count())).FirstOrDefault().Id;
                        context.Loans.Add(loan);
                        obj.Sale.LoanId = loan.Id;


                        var registration = new Registration();
                        //registration.VehicleRegistrationNumber = "XXX 1234";
                        context.Registrations.Add(registration);
                        //_context.Imports.Add(import);
                        obj.RegistrationId = registration.Id;


                        var expense = new Expense();
                        context.Expenses.Add(expense);
                        obj.ExpenseId = expense.Id;



                        var administrativeCost = new AdminitrativeCost();
                        if (administrativeCost.AdminitrativeCostItems == null)
                            administrativeCost.AdminitrativeCostItems = new List<AdminitrativeCostItem>();
                        administrativeCost.AdminitrativeCostItems.Add(new AdminitrativeCostItem
                        {
                            AdminitrativeCostId = administrativeCost.Id,
                            Name = "Vehicle Regn. Fee",
                            Amount = 200,
                        });
                        //administrativeCost.AdminitrativeCostItems.Add(new AdminitrativeCostItem
                        //{
                        //    AdminitrativeCostId = administrativeCost.Id,
                        //    Name = "Road Tax",
                        //    Amount = 20,
                        //});

                        //administrativeCost.AdminitrativeCostItems.Add(new AdminitrativeCostItem
                        //{
                        //    AdminitrativeCostId = administrativeCost.Id,
                        //    Name = "Insurance",
                        //    Amount = 1676.66F,
                        //});



                        context.AdminitrativeCosts.Add(administrativeCost);
                        obj.AdminitrativeCostId = administrativeCost.Id;





                        ///


                        //AddVehicle(context, obj);
                        //AddPurchase(context, obj);
                        //AddImport(context, obj);
                        //AddClearance(context, obj);
                        //AddPricing(context, obj);
                        //AddAdministrativeCost(context, obj);
                        //AddStockStatusHistories(context, obj);
                        //AddSale(context, obj);
                        //AddLoan(context, obj);
                        //AddRegistration(context, obj);
                        //AddExpense(context, obj);

                        //var arrivalCheckList = new ArrivalChecklist();

                        //if (arrivalCheckList.ArrivalChecklists == null)
                        //    arrivalCheckList.ArrivalChecklists = new List<ArrivalChecklistItem>();

                        //arrivalCheckList.ArrivalChecklists.Add(new ArrivalChecklistItem
                        //{
                        //    ArrivalChecklistId = arrivalCheckList.Id,
                        //    Name = "Extra Key",
                        //    IsAvailable = false,
                        //    Remarks = ""
                        //});
                        //context.ArrivalChecklists.Add(arrivalCheckList);
                        //obj.ArrivalChecklistId = arrivalCheckList.Id;



                        context.Stocks.Add(obj);

                        var remarkList = new List<Remarks>
                        {
                            new Remarks
                            {
                                ProfileId = context.Profiles.Where(c => c.Role == "Admin").FirstOrDefault().Id,
                                Description = remarks1,
                                StockId = obj.Id,
                            },
                            new Remarks
                            {
                                ProfileId = context.Profiles.Where(c => c.Role == "Admin").FirstOrDefault().Id,
                                Description = remarks2,
                                StockId = obj.Id,
                            },
                            new Remarks
                            {
                                ProfileId = context.Profiles.Where(c => c.Role == "Admin").FirstOrDefault().Id,
                                Description = remarks3,
                                StockId = obj.Id,
                            },

                        };
                        context.Remarks.AddRange(remarkList);
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Log the error
                    Console.WriteLine($"Error processing stock record: {ex.Message}");
                }
            }
        }
        Console.WriteLine("DONE");
        Console.ReadLine();
    }

    static decimal ExtractPrice(string priceText)
    {
        // Extract numerical value from price text, e.g., "(RM100K)" -> 100000
        priceText = priceText.Trim().Replace("(", "").Replace(")", "");
        if (priceText.StartsWith("RM"))
        {
            priceText = priceText.Substring(2, priceText.Length - 2); // Remove "(RM" and ")"
            if (priceText.EndsWith("K"))
            {
                priceText = priceText.Substring(0, priceText.Length - 1); // Remove "K"
                if (decimal.TryParse(priceText, out decimal price))
                {
                    return price * 1000; // Convert to full amount
                }
            }
        }
        return 0; // Default if parsing fails
    }
    static string GetFormattedCellText(ExcelWorksheet worksheet, int row, int col)
    {
        var cellValue = worksheet.Cells[row, col].Value;
        if (cellValue != null && (
            col == 3 ||
            col == 10 || //ETA
            col == 13
            )) // date column
        {
            try
            {
                if (cellValue is double)
                {
                    DateTime date = DateTime.FromOADate((double)cellValue);
                    return date.ToString("yyyy-MM-dd");
                }
                DateTime.TryParse(cellValue.ToString(), out DateTime dateValue);
                return dateValue.ToString("yyyy-MM-dd");

            }
            catch (Exception ex)
            {
                return DateTime.MinValue.ToString("yyyy-MM-dd");
            }
        }
        else
        {
            return cellValue?.ToString()?.Replace("\n", " ") ?? "";
        }

        //if (cellValue != null && DateTime.TryParse(cellValue.ToString(), out DateTime dateValue))
        //{
        //    return dateValue.ToString("yyyy-MM-dd");
        //}
        //else
        //{
        //    return cellValue?.ToString()?.Replace("\n", " ") ?? "";
        //}
    }

    static bool IsStatusIndicator(string text)
    {
        return text.StartsWith("BOOKING") ||
               text.StartsWith("LOU") ||
               text.StartsWith("DUTY TO BE PAID") ||
               text.StartsWith("INCOMING STOCK");
    }

    static bool IsBrand(string text)
    {
        return text.Equals("DAIHATSU") ||
               text.Equals("LAND ROVER") ||
               text.Equals("HONDA") ||
               text.Equals("LEXUS") ||
               text.Equals("MERCEDES") ||
               text.Equals("SUZUKI") ||
               text.Equals("TOYOTA");
    }

    static bool ShouldSkipLine(string fullLine)
    {
        return fullLine.StartsWith("PRICELIST") ||
               fullLine.StartsWith("NO") ||
               fullLine.StartsWith("Booking");
    }

    static string ExtractStockNo(string str)
    {
        var parts = str.Split('|');
        return parts.Length > 2 ? parts[2] : "";
    }

    static void AddVehicle(SpotDBContext context, Stock obj)
    {
        var vehicle = new Vehicle();
        context.Vehicles.Add(vehicle);
        obj.VehicleId = vehicle.Id;
    }

    static void UpdateVehicle(SpotDBContext context, Stock obj)
    {
        var vehicle = context.Vehicles.FirstOrDefault(v => v.Id == obj.VehicleId);
        if (vehicle != null)
        {
            // Update vehicle fields as necessary
            // Example:
            // vehicle.BrandId = newBrandId;
            // vehicle.ModelId = newModelId;

            context.Vehicles.Update(vehicle);
        }
    }

    static void AddPurchase(SpotDBContext context, Stock obj)
    {
        var purchase = new Purchase();
        context.Purchases.Add(purchase);
        obj.PurchaseId = purchase.Id;
    }

    static void UpdatePurchase(SpotDBContext context, Stock obj)
    {
        var purchase = context.Purchases.FirstOrDefault(p => p.Id == obj.PurchaseId);
        if (purchase != null)
        {
            // Update purchase fields as necessary

            context.Purchases.Update(purchase);
        }
    }

    static void AddImport(SpotDBContext context, Stock obj)
    {
        var import = new Import();
        context.Imports.Add(import);
        obj.ImportId = import.Id;
    }

    static void UpdateImport(SpotDBContext context, Stock obj)
    {
        var import = context.Imports.FirstOrDefault(i => i.Id == obj.ImportId);
        if (import != null)
        {
            // Update import fields as necessary

            context.Imports.Update(import);
        }
    }

    static void AddClearance(SpotDBContext context, Stock obj)
    {
        var clearance = new Clearance();
        context.Clearances.Add(clearance);
        obj.ClearanceId = clearance.Id;
    }

    static void UpdateClearance(SpotDBContext context, Stock obj)
    {
        var clearance = context.Clearances.FirstOrDefault(c => c.Id == obj.ClearanceId);
        if (clearance != null)
        {
            // Update clearance fields as necessary

            context.Clearances.Update(clearance);
        }
    }

    static void AddPricing(SpotDBContext context, Stock obj)
    {
        var pricing = new Pricing
        {
            RecommendedSalePrice = 250000,
            MinimumSalePrice = 250000
        };
        context.Pricings.Add(pricing);
        obj.PricingId = pricing.Id;
    }

    static void UpdatePricing(SpotDBContext context, Stock obj)
    {
        var pricing = context.Pricings.FirstOrDefault(p => p.Id == obj.PricingId);
        if (pricing != null)
        {
            // Update pricing fields as necessary

            context.Pricings.Update(pricing);
        }
    }

    static void AddAdministrativeCost(SpotDBContext context, Stock obj)
    {
        var administrativeCost = new AdminitrativeCost();
        administrativeCost.AdminitrativeCostItems = new List<AdminitrativeCostItem>
        {
            new AdminitrativeCostItem { Name = "Vehicle Regn. Fee", Amount = 200 },
            new AdminitrativeCostItem { Name = "Road Tax", Amount = 20 },
            new AdminitrativeCostItem { Name = "Insurance", Amount = 1676.66F }
        };
        context.AdminitrativeCosts.Add(administrativeCost);
        obj.AdminitrativeCostId = administrativeCost.Id;
    }

    static void UpdateAdministrativeCost(SpotDBContext context, Stock obj)
    {
        var administrativeCost = context.AdminitrativeCosts.FirstOrDefault(a => a.Id == obj.AdminitrativeCostId);
        if (administrativeCost != null)
        {
            // Update administrative cost fields as necessary

            context.AdminitrativeCosts.Update(administrativeCost);
        }
    }

    static void AddStockStatusHistories(SpotDBContext context, Stock obj)
    {
        var stockStatusDraft = context.StockStatuses.FirstOrDefault(c => c.Name == "Draft");
        var stockStatusAvailable = context.StockStatuses.FirstOrDefault(c => c.Name == "Available");

        obj.StockStatusHistories = new List<StockStatusHistory>
        {
            new StockStatusHistory
            {
                ProfileId = context.Profiles.FirstOrDefault(c => c.Role == "Admin").Id,
                StockStatusId = stockStatusDraft.Id,
                StockId = obj.Id
            },
            new StockStatusHistory
            {
                ProfileId = context.Profiles.FirstOrDefault(c => c.Role == "Admin").Id,
                StockStatusId = stockStatusAvailable.Id,
                StockId = obj.Id
            }
        };
    }

    static void UpdateStockStatusHistories(SpotDBContext context, Stock obj)
    {
        // Implement logic to update stock status histories if needed
        // This could involve checking existing histories and updating or adding new ones
    }

    static void AddSale(SpotDBContext context, Stock obj)
    {
        var sale = new Sale
        {
            IsUseLoan = true,
            EoeAmount = 600
        };
        context.Sales.Add(sale);
        obj.Sale = sale;
        obj.SaleId = sale.Id;
    }

    static void UpdateSale(SpotDBContext context, Stock obj)
    {
        var sale = context.Sales.FirstOrDefault(s => s.Id == obj.SaleId);
        if (sale != null)
        {
            // Update sale fields as necessary

            context.Sales.Update(sale);
        }
    }

    static void AddLoan(SpotDBContext context, Stock obj)
    {
        var loan = new Loan();
        context.Loans.Add(loan);
        obj.Sale.LoanId = loan.Id;
    }

    static void UpdateLoan(SpotDBContext context, Stock obj)
    {
        var loan = context.Loans.FirstOrDefault(l => l.Id == obj.Sale.LoanId);
        if (loan != null)
        {
            // Update loan fields as necessary

            context.Loans.Update(loan);
        }
    }

    static void AddRegistration(SpotDBContext context, Stock obj)
    {
        var registration = new Registration
        {
            VehicleRegistrationNumber = "XXX 1234"
        };
        context.Registrations.Add(registration);
        obj.RegistrationId = registration.Id;
    }

    static void UpdateRegistration(SpotDBContext context, Stock obj)
    {
        var registration = context.Registrations.FirstOrDefault(r => r.Id == obj.RegistrationId);
        if (registration != null)
        {
            // Update registration fields as necessary

            context.Registrations.Update(registration);
        }
    }

    static void AddExpense(SpotDBContext context, Stock obj)
    {
        var expense = new Expense();
        context.Expenses.Add(expense);
        obj.ExpenseId = expense.Id;
    }

    static void UpdateExpense(SpotDBContext context, Stock obj)
    {
        var expense = context.Expenses.FirstOrDefault(e => e.Id == obj.ExpenseId);
        if (expense != null)
        {
            // Update expense fields as necessary

            context.Expenses.Update(expense);
        }
    }
}
