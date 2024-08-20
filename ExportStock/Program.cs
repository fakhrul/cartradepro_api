using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using SPOT_API.Models;
using SPOT_API.Persistence;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Specify the path to your Excel file
        string excelPath = @"C:\Google Drive\01-SAFA INTEGRATED\01-PROJECTS\171-SINAR AUTO SDN BHD\LIST.xlsx";

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

        using (ExcelPackage package = new ExcelPackage(fileInfo))
        {
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

                            if (IsStatusIndicator(text))
                            {
                                status = text;
                                brand = "UNKNOWN";
                                break;
                            }

                            if (IsBrand(text))
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
                        fullLineInfoList.Add(fullLine.Split('|').Length + "|" + fullLine);
                    }
                }
            }
        }

        var options = new DbContextOptionsBuilder<SpotDBContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=cartradepro;Username=postgres;Password=qwe123").Options;

        using (var context = new SpotDBContext(options))
        {
            foreach (var str in fullLineInfoList)
            {
                Console.WriteLine(str);
                try
                {
                    var stockNo = ExtractStockNo(str);
                    var existingStock = context.Stocks
                        .Include(s => s.StockStatusHistories)
                        .FirstOrDefault(s => s.StockNo == stockNo);

                    Stock obj;
                    if (existingStock != null)
                    {
                        // If stock exists, update the existing record
                        obj = existingStock;

                        // Update related entities as necessary
                        UpdateVehicle(context, obj);
                        UpdatePurchase(context, obj);
                        UpdateImport(context, obj);
                        UpdateClearance(context, obj);
                        UpdatePricing(context, obj);
                        UpdateAdministrativeCost(context, obj);
                        UpdateStockStatusHistories(context, obj);
                        UpdateSale(context, obj);
                        UpdateLoan(context, obj);
                        UpdateRegistration(context, obj);
                        UpdateExpense(context, obj);

                        context.Stocks.Update(obj);
                    }
                    else
                    {
                        // If stock doesn't exist, create a new record
                        obj = new Stock { StockNo = stockNo };

                        AddVehicle(context, obj);
                        AddPurchase(context, obj);
                        AddImport(context, obj);
                        AddClearance(context, obj);
                        AddPricing(context, obj);
                        AddAdministrativeCost(context, obj);
                        AddStockStatusHistories(context, obj);
                        AddSale(context, obj);
                        AddLoan(context, obj);
                        AddRegistration(context, obj);
                        AddExpense(context, obj);

                        var arrivalCheckList = new ArrivalChecklist();

                        if (arrivalCheckList.ArrivalChecklists == null)
                            arrivalCheckList.ArrivalChecklists = new List<ArrivalChecklistItem>();

                        arrivalCheckList.ArrivalChecklists.Add(new ArrivalChecklistItem
                        {
                            ArrivalChecklistId = arrivalCheckList.Id,
                            Name = "Extra Key",
                            IsAvailable = true,
                            Remarks = ""
                        });
                        context.ArrivalChecklists.Add(arrivalCheckList);
                        obj.ArrivalChecklistId = arrivalCheckList.Id;



                        context.Stocks.Add(obj);
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
    }

    static string GetFormattedCellText(ExcelWorksheet worksheet, int row, int col)
    {
        var cellValue = worksheet.Cells[row, col].Value;

        if (cellValue != null && DateTime.TryParse(cellValue.ToString(), out DateTime dateValue))
        {
            return dateValue.ToString("yyyy-MM-dd");
        }
        else
        {
            return cellValue?.ToString()?.Replace("\n", " ") ?? "";
        }
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
