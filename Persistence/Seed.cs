using Microsoft.AspNetCore.Identity;
using SPOT_API.Persistence;
using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SPOT_API.Persistence
{
    public class Seed
    {
        public static async Task SeedData(SpotDBContext context, UserManager<AppUser> userManager)
        {
            //if (!userManager.Users.Any())
            //{
            //if (context.Profiles.Any())
            //    return;
            try
            {
                await SeedUsers(context, userManager);
                await SeedBrand(context);
                await SeedVehicleType(context);
                await SeedStockStatuses(context);
                await SeedSupplier(context);
                await SeedBank(context);
                await SeedCustomer(context);

                await SeedStocks(context);


                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
            //}

        }
        private static async Task SeedCustomer(SpotDBContext context)
        {

            if (context.Customers.Any())
                return;
            var objs = new List<Customer>
            {
                new Customer {

                    Name = "Customer Name 1",
                    Address = @"No 23A, Jalan Bangi,
Bandar Baru Bangi, 43650, Selangor, Malaysia.",
                    Phone = "+801238917231",
                    CustomerType = "Individu",
                     Email = "asdasd@asdasd.com",
                      IcNumber = "900212-12-3213",


                },
            };

            context.Customers.AddRange(objs);
            await context.SaveChangesAsync();
        }

        private static async Task SeedBank(SpotDBContext context)
        {

            if (context.Banks.Any())
                return;
            var objs = new List<Bank>
            {
                new Bank {

                    Name = "Maybank Islamic",
                    Address = @"No 23A, Jalan Bangi,
Bandar Baru Bangi, 43650, Selangor, Malaysia.",
                    Country = "Malaysia",
                    Phone = "+801238917231",
                    Website = "www.keretajepun.online",
                    ContactPersonEmail = "me@keretajepun.online",
                    ContactPersonName = "Izumaki Naruto",
                    ContactPersonPhone = "+80112322222",


                },
            };

            context.Banks.AddRange(objs);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSupplier(SpotDBContext context)
        {

            if (context.Suppliers.Any())
                return;
            var objs = new List<Supplier>
            {
                new Supplier {

                    Name = "Pengedar Kereta Dari Jepun",
                    Address = @"No 23A, Jalan Hiroshiman,
Tokyo, 43650, Jepun.",
                    Country = "JAPAN",
                    Phone = "+801238917231",
                    Website = "www.keretajepun.online",
                    ContactPersonEmail = "me@keretajepun.online",
                    ContactPersonName = "Izumaki Naruto",
                    ContactPersonPhone = "+80112322222",


                },
            };

            context.Suppliers.AddRange(objs);
            await context.SaveChangesAsync();
        }

        private static async Task SeedStocks(SpotDBContext context)
        {
            if (context.Stocks.Any())
                return;
            var objs = new List<Stock>
            {
                new Stock { StockNo = "20240621-1"},
                new Stock { StockNo = "20240621-2"},
            };

            context.Stocks.AddRange(objs);
            await context.SaveChangesAsync();

        }
        private static async Task SeedStockStatuses(SpotDBContext context)
        {
            if (context.StockStatuses.Any())
                return;
            var objs = new List<StockStatus>
            {
                new StockStatus { Name = "Draft" },
                new StockStatus { Name = "Available" },
                new StockStatus { Name = "Booked" },
                new StockStatus { Name = "Shipped" },
                new StockStatus { Name = "ArriveAtPort" },
                new StockStatus { Name = "ShowRoom" },
                new StockStatus { Name = "Registered" },
                new StockStatus { Name = "Cancelled" },
            };

            context.StockStatuses.AddRange(objs);
            await context.SaveChangesAsync();

        }

        private static async Task SeedBrand(SpotDBContext context)
        {
            if (context.Brands.Any())
                return;
            var objs = new List<Brand>
            {
                new Brand { Name = "BMW" },
                new Brand { Name = "AUDI" },
                new Brand { Name = "VOLKSWAGEN" },
                new Brand { Name = "MCLAREN" },
                new Brand { Name = "LAND ROVER" },
            };

            context.Brands.AddRange(objs);
            await context.SaveChangesAsync();

            var bmw = await context.Brands.FirstOrDefaultAsync(x => x.Name == "BMW");

            await SeedModel(context, bmw);
        }

        private static async Task SeedModel(SpotDBContext context, Brand brand)
        {
            if (context.Models.Any())
                return;
            var objs = new List<Model>
            {
                 new Model
                 {
                     Name = "1 SERIES 118I M SPORT (DBA-1R15)",
                      Brand = brand,
                       BrandId = brand.Id
                 },

                    new Model
                 {
                     Name = "1 SERIES M COUPE",
                      Brand = brand,
                       BrandId = brand.Id
                 },
            };

            context.Models.AddRange(objs);
            await context.SaveChangesAsync();

        }

        private static async Task SeedVehicleType(SpotDBContext context)
        {
            if (context.VehicleTypes.Any())
                return;

            var vehicleTypes = new List<VehicleType>
            {
                new VehicleType { Name = "Sedan" },
                new VehicleType { Name = "Saloon" },
                new VehicleType { Name = "Hatchback" },
                new VehicleType { Name = "4WD" },
                new VehicleType { Name = "SUV" },
                new VehicleType { Name = "MPV" },
                new VehicleType { Name = "Van" },
            };

            context.VehicleTypes.AddRange(vehicleTypes);
            await context.SaveChangesAsync();
        }

        private static async Task SeedUsers(SpotDBContext context, UserManager<AppUser> userManager)
        {
            await SeedAdminUser(context, userManager);
        }

        private static async Task SeedAdminUser(SpotDBContext context, UserManager<AppUser> userManager)
        {
            if (context.Profiles.Any())
                return;

            Profile userAdmin = new Profile
            {
                FullName = "Administrator",
                Email = "admin@email.com",
                Phone = "0192563019",
                Role = "super",
            };

            await context.Profiles.AddAsync(userAdmin);


            AppUser user = new AppUser
            {
                DisplayName = "Super Admin",
                UserName = "admin@email.com",
                ProfileId = userAdmin.Id,
                Email = "admin@email.com",
                //IsSuperAdmin = true
                Role = "super"
            };

            await CreateAppUser(userManager, user);

            //modelBuilder.Entity<User>().HasData(userAdmin);
            //Login loginAdmin = new Login
            //{
            //    Username = "admin",
            //    Password = "admin",
            //    UserId = userAdmin.Id,
            //    //Email = "admin@email.com"
            //};
            //await context.Logins.AddAsync(loginAdmin);
            //modelBuilder.Entity<Login>().HasData(loginAdmin);
        }

        private static async Task CreateAppUser(UserManager<AppUser> userManager, AppUser user)
        {
            Console.WriteLine(string.Format("Email:{0}, UserName:{1}", user.Email, user.UserName));
            await userManager.CreateAsync(user, "Qwerty@123");
        }

    }
}
