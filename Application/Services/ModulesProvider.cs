using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services
{
    /// <summary>
    /// Provides hardcoded module and submodule structure
    /// Used after migration from database-stored modules to JSONB permissions
    /// </summary>
    public static class ModulesProvider
    {
        /// <summary>
        /// Returns the complete hardcoded module structure with all submodules
        /// This replaces the old Modules and SubModules database tables
        /// </summary>
        public static List<Module> GetModules()
        {
            var modules = new List<Module>
            {
                // 1. Dashboard
                new Module
                {
                    Id = Guid.Parse("bc46048f-eb29-4e69-a975-7ddb3e027dbe"),
                    Name = "Dashboard",
                    Code = "DASHBOARD",
                    DisplayOrder = 1,
                    SubModules = new List<SubModule>()
                },

                // 2. Stock To Buy
                new Module
                {
                    Id = Guid.Parse("5e2f2d89-b719-4bfd-b9eb-d690dab80a4b"),
                    Name = "Stock To Buy",
                    Code = "STOCK_TO_BUY",
                    DisplayOrder = 2,
                    SubModules = new List<SubModule>()
                },

                // 3. Stocks (with 13 submodules)
                new Module
                {
                    Id = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71"),
                    Name = "Stocks",
                    Code = "STOCKS",
                    DisplayOrder = 3,
                    SubModules = new List<SubModule>
                    {
                        new SubModule { Id = Guid.Parse("68e928ec-c627-4d64-b21e-85d591e0cc37"), Name = "Stock Info", Code = "STOCK_INFO", DisplayOrder = 1, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("5e1e8b25-3e1a-4b82-a1ed-b8100013b04a"), Name = "Vehicle Info", Code = "VEHICLE_INFO", DisplayOrder = 2, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("8e35b60a-c87f-49d1-9dfc-a131bc311348"), Name = "Purchase", Code = "PURCHASE", DisplayOrder = 3, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("4badd53c-50a9-407b-8eaa-b09136f607c3"), Name = "Import", Code = "IMPORT", DisplayOrder = 4, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("a2159318-bcba-4d8b-80d6-7519378ff98a"), Name = "Clearance", Code = "CLEARANCE", DisplayOrder = 5, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("f93f7a17-e957-4576-a4df-5a3349637b4a"), Name = "Sale", Code = "SALE", DisplayOrder = 6, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("8eae1f96-acd5-41c8-876f-2c2f5ffcef24"), Name = "Pricing", Code = "PRICING", DisplayOrder = 7, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("1184de3c-d648-4d8a-b549-e34b20724db9"), Name = "Arrival Checklist", Code = "ARRIVAL_CHECKLIST", DisplayOrder = 8, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("481ef5b6-2df8-4bab-ae96-79e210753f7f"), Name = "Registration", Code = "REGISTRATION", DisplayOrder = 9, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("8d9eedd2-7e1d-4fb5-ae21-c404340fdee6"), Name = "Expenses", Code = "EXPENSES", DisplayOrder = 10, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("7d154a97-3fbe-4636-942f-f124f5945439"), Name = "Administrative Cost", Code = "ADMINISTRATIVE_COST", DisplayOrder = 11, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("d8f32175-a289-4f15-a674-6db0a443d08e"), Name = "Disbursement", Code = "DISBURSEMENT", DisplayOrder = 12, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") },
                        new SubModule { Id = Guid.Parse("9f8355e7-1d99-44e3-a83b-fd2a277aa2ed"), Name = "Advertisement", Code = "STOCK_ADVERTISEMENT", DisplayOrder = 13, ModuleId = Guid.Parse("1f105d62-788e-4e23-85b6-379900273f71") }
                    }
                },

                // 4. Customer
                new Module
                {
                    Id = Guid.Parse("de96daab-53e1-4fa5-a935-9dfac0ecda12"),
                    Name = "Customer",
                    Code = "CUSTOMER",
                    DisplayOrder = 4,
                    SubModules = new List<SubModule>()
                },

                // 5. Pricelist
                new Module
                {
                    Id = Guid.Parse("44b66d11-4d56-4b3c-a866-0de1d96b2d7a"),
                    Name = "Pricelist",
                    Code = "VEHICLE",
                    DisplayOrder = 5,
                    SubModules = new List<SubModule>()
                },

                // 6. Advertisement
                new Module
                {
                    Id = Guid.Parse("92e22ad8-da0f-45b8-9963-6824eedb5ef9"),
                    Name = "Advertisement",
                    Code = "ADVERTISEMENT",
                    DisplayOrder = 6,
                    SubModules = new List<SubModule>()
                },

                // 7. MasterData (with 6 submodules)
                new Module
                {
                    Id = Guid.Parse("27134597-6f0e-42e3-9c8a-861878a33d2b"),
                    Name = "MasterData",
                    Code = "MASTER_DATA",
                    DisplayOrder = 7,
                    SubModules = new List<SubModule>
                    {
                        new SubModule { Id = Guid.Parse("5557ca44-641b-47fb-80ba-3e2680cbeeef"), Name = "Suppliers", Code = "SUPPLIERS", DisplayOrder = 1, ModuleId = Guid.Parse("27134597-6f0e-42e3-9c8a-861878a33d2b") },
                        new SubModule { Id = Guid.Parse("98d78cd4-5915-40f2-9c23-9056b18abf3e"), Name = "Forwarders", Code = "FORWARDERS", DisplayOrder = 2, ModuleId = Guid.Parse("27134597-6f0e-42e3-9c8a-861878a33d2b") },
                        new SubModule { Id = Guid.Parse("9a9811c9-7df1-433f-8e9d-a1154e255335"), Name = "Banks", Code = "BANKS", DisplayOrder = 3, ModuleId = Guid.Parse("27134597-6f0e-42e3-9c8a-861878a33d2b") },
                        new SubModule { Id = Guid.Parse("4137dae7-5ca5-4eeb-b9b7-e9fb3923bc60"), Name = "Brands", Code = "BRANDS", DisplayOrder = 4, ModuleId = Guid.Parse("27134597-6f0e-42e3-9c8a-861878a33d2b") },
                        new SubModule { Id = Guid.Parse("5237283d-cae9-4a87-85eb-332ff8ccf448"), Name = "Sub Companies", Code = "SUB_COMPANIES", DisplayOrder = 5, ModuleId = Guid.Parse("27134597-6f0e-42e3-9c8a-861878a33d2b") },
                        new SubModule { Id = Guid.Parse("a1f5e3d9-4c2b-4a87-9d3e-8b9f7c6d5e4f"), Name = "Showrooms", Code = "SHOWROOMS", DisplayOrder = 6, ModuleId = Guid.Parse("27134597-6f0e-42e3-9c8a-861878a33d2b") }
                    }
                },

                // 8. Reports (with 1 submodule)
                new Module
                {
                    Id = Guid.Parse("3c0173c5-16c2-4cb5-bc60-a9c2f9a0455c"),
                    Name = "Reports",
                    Code = "REPORT",
                    DisplayOrder = 8,
                    SubModules = new List<SubModule>
                    {
                        new SubModule { Id = Guid.Parse("51e9a845-ed29-4b6c-9ee0-a9dec31f70b4"), Name = "Sales Report", Code = "SALES_REPORT", DisplayOrder = 1, ModuleId = Guid.Parse("3c0173c5-16c2-4cb5-bc60-a9c2f9a0455c") }
                    }
                },

                // 9. Administration (with 4 submodules)
                new Module
                {
                    Id = Guid.Parse("e4829050-4968-45d9-b323-b28c8d08461e"),
                    Name = "Administration",
                    Code = "ADMINISTRATION",
                    DisplayOrder = 9,
                    SubModules = new List<SubModule>
                    {
                        new SubModule { Id = Guid.Parse("aea41b47-08f8-405b-96d5-9fa1697c9c8b"), Name = "Roles", Code = "ROLES", DisplayOrder = 1, ModuleId = Guid.Parse("e4829050-4968-45d9-b323-b28c8d08461e") },
                        new SubModule { Id = Guid.Parse("6d68da1c-c3e7-4d68-a95d-43e9f872d138"), Name = "User", Code = "USER", DisplayOrder = 2, ModuleId = Guid.Parse("e4829050-4968-45d9-b323-b28c8d08461e") },
                        new SubModule { Id = Guid.Parse("d1608216-583f-4dd2-bc4d-e534c6314bd0"), Name = "Audit Logs", Code = "AUDIT_LOGS", DisplayOrder = 3, ModuleId = Guid.Parse("e4829050-4968-45d9-b323-b28c8d08461e") },
                        new SubModule { Id = Guid.Parse("3ed5d41e-308a-45f1-b9c4-7c87d17b33f8"), Name = "Company", Code = "COMPANY", DisplayOrder = 4, ModuleId = Guid.Parse("e4829050-4968-45d9-b323-b28c8d08461e") }
                    }
                }
            };

            return modules;
        }

        /// <summary>
        /// Get a specific module by name
        /// </summary>
        public static Module GetModuleByName(string moduleName)
        {
            var modules = GetModules();
            return modules.Find(m => m.Name == moduleName);
        }

        /// <summary>
        /// Get a specific module by code
        /// </summary>
        public static Module GetModuleByCode(string moduleCode)
        {
            if (string.IsNullOrEmpty(moduleCode))
                return null;

            var modules = GetModules();
            return modules.Find(m => m.Code == moduleCode);
        }

        /// <summary>
        /// Get a specific submodule by code across all modules
        /// </summary>
        public static SubModule GetSubModuleByCode(string subModuleCode)
        {
            if (string.IsNullOrEmpty(subModuleCode))
                return null;

            var modules = GetModules();
            foreach (var module in modules)
            {
                var subModule = module.SubModules?.FirstOrDefault(sm => sm.Code == subModuleCode);
                if (subModule != null)
                    return subModule;
            }
            return null;
        }
    }
}
