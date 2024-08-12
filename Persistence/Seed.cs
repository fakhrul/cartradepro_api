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
        public static async Task SeedData(SpotDBContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //if (!userManager.Users.Any())
            //{
            //if (context.Profiles.Any())
            //    return;
            try
            {
                await SeedRoles(context, roleManager);
                await SeedModulesAndSubModules(context);
                await SeedRolePermissionsForMainModule(context);
                await SeedUsers(context, userManager);

                await SeedBrand(context);
                await SeedModel(context);
                await SeedVehicleType(context);
                await SeedStockStatuses(context);
                await SeedSupplier(context);
                await SeedBank(context);
                await SeedCustomer(context);
                await SeedCompany(context);
                await SeedClearanceAgent(context);


                await SeedStocks(context);


                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
            //}

        }

        private static async Task SeedCompany(SpotDBContext context)
        {

            if (context.Companies.Any())
                return;
            var objs = new List<Company>
            {
                new Company {
                            Name= "SINAR AUTO SDN BHD",
        TagLine= "(Formerly known as Asmara Auto Sdn Bhd)",
        Address= @"No. 6, Jalan P10/16, Seksyen 10, 
Taman Perindustrian Selaman, 
Bandar Baru Bangi, 43650 Selangor",
        Phone= "+603-8920 8080",
        LogoUrl= "path/to/logo.png", // replace with your actual logo path
        BankAccountName= "Sinar Auto Sdn Bhd",
        BankAccountNo= "MBB-5620 2165 5099",
        BankName= "MAYBANK ISLAMIC BERHAD (JLN KELANG LAMA)",
        BankAddress= @"2nd & 3rd Floor, 
18 & 3 Jalan Desa 
Taman Desa, 58100 Kuala Lumpur",
                        RegNo = "1231212-D"

                },
            };

            context.Companies.AddRange(objs);
            await context.SaveChangesAsync();
        }
        private static async Task SeedCustomer(SpotDBContext context)
        {
            if (context.Customers.Any())
            {
                Console.WriteLine("Customers already exist in the database. Skipping seeding.");
                return;
            }

            var customers = new List<Customer>();
            for (int i = 1; i <= 100; i++)
            {
                customers.Add(new Customer
                {
                    Name = $"Customer Name {i}",
                    Address = $"No {i}, Jalan Example, City {i}, State {i}, Country {i}.",
                    Phone = $"+6012{i:D8}",
                    CustomerType = (i % 2 == 0) ? "Company" : "Individu",
                    Email = $"customer{i}@example.com",
                    IcNumber = $"{900000 + i:000000}-{i % 12:D2}-{1000 + i:D4}"
                });
            }

            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();

            Console.WriteLine("Customers seeded successfully.");
        }


        private static async Task SeedClearanceAgent(SpotDBContext context)
        {
            if (context.ClearanceAgents.Any())
            {
                Console.WriteLine("Clearance Angentalready exist in the database. Skipping seeding.");
                return;
            }

            var banks = new List<ForwardingAgent>
    {
          new ForwardingAgent
            {
                Name = "Global Forwarders Ltd.",
                Address = "123 International Road, Suite 100",
                Country = "USA",
                Phone = "+1-555-1234-567",
                Website = "http://www.globalforwarders.com",
                ContactPersonName = "John Doe",
                ContactPersonPhone = "+1-555-5678-910",
                ContactPersonEmail = "johndoe@globalforwarders.com"
            },
            new ForwardingAgent
            {
                Name = "FastTrack Logistics",
                Address = "456 Cargo Lane, Warehouse 5",
                Country = "Germany",
                Phone = "+49-30-1234-567",
                Website = "http://www.fasttracklogistics.de",
                ContactPersonName = "Anna Schmidt",
                ContactPersonPhone = "+49-30-5678-910",
                ContactPersonEmail = "annaschmidt@fasttracklogistics.de"
            },
            new ForwardingAgent
            {
                Name = "Pacific Shipping Co.",
                Address = "789 Ocean Blvd, Building 2",
                Country = "Japan",
                Phone = "+81-3-1234-5678",
                Website = "http://www.pacificshipping.co.jp",
                ContactPersonName = "Taro Yamada",
                ContactPersonPhone = "+81-3-5678-9101",
                ContactPersonEmail = "taroyamada@pacificshipping.co.jp"
            },
            new ForwardingAgent
            {
                Name = "EuroTrans Services",
                Address = "101 Main St, Office 45",
                Country = "France",
                Phone = "+33-1-2345-6789",
                Website = "http://www.eurotransservices.fr",
                ContactPersonName = "Marie Dupont",
                ContactPersonPhone = "+33-1-6789-0123",
                ContactPersonEmail = "mariedupont@eurotransservices.fr"
            },
            new ForwardingAgent
            {
                Name = "Aussie Freight Forwarders",
                Address = "202 Outback Rd, Unit 3",
                Country = "Australia",
                Phone = "+61-2-1234-5678",
                Website = "http://www.aussiefreight.com.au",
                ContactPersonName = "Bruce Smith",
                ContactPersonPhone = "+61-2-5678-9101",
                ContactPersonEmail = "brucesmith@aussiefreight.com.au"
            }
    };

            context.ClearanceAgents.AddRange(banks);
            await context.SaveChangesAsync();

            Console.WriteLine("Clearance Agent seeded successfully.");
        }


        private static async Task SeedBank(SpotDBContext context)
        {
            if (context.Banks.Any())
            {
                Console.WriteLine("Banks already exist in the database. Skipping seeding.");
                return;
            }

            var banks = new List<Bank>
    {
        new Bank
        {
            Name = "Maybank Islamic",
            Address = @"No 23A, Jalan Bangi,
Bandar Baru Bangi, 43650, Selangor, Malaysia.",
            Country = "Malaysia",
            Phone = "+60312345678",
            Website = "www.maybankislamic.com.my",
            ContactPersonEmail = "contact@maybankislamic.com.my",
            ContactPersonName = "Izumaki Naruto",
            ContactPersonPhone = "+60387654321",
        },
        new Bank
        {
            Name = "CIMB Bank",
            Address = @"CIMB Tower, No. 1, Jalan Stesen Sentral 2,
Kuala Lumpur Sentral, 50470, Kuala Lumpur, Malaysia.",
            Country = "Malaysia",
            Phone = "+60322611234",
            Website = "www.cimb.com",
            ContactPersonEmail = "service@cimb.com",
            ContactPersonName = "Lee Chong Wei",
            ContactPersonPhone = "+60322611345",
        },
        new Bank
        {
            Name = "Public Bank",
            Address = @"146 Jalan Ampang,
50450, Kuala Lumpur, Malaysia.",
            Country = "Malaysia",
            Phone = "+60321712345",
            Website = "www.publicbank.com.my",
            ContactPersonEmail = "info@publicbank.com.my",
            ContactPersonName = "Tan Sri Teh Hong Piow",
            ContactPersonPhone = "+60321713456",
        },
        new Bank
        {
            Name = "Hong Leong Bank",
            Address = @"Menara Hong Leong, No. 6, Jalan Damanlela,
Bukit Damansara, 50490, Kuala Lumpur, Malaysia.",
            Country = "Malaysia",
            Phone = "+60320811234",
            Website = "www.hlb.com.my",
            ContactPersonEmail = "contact@hlb.com.my",
            ContactPersonName = "Nguyen Hoang Minh",
            ContactPersonPhone = "+60320811345",
        },
        new Bank
        {
            Name = "RHB Bank",
            Address = @"RHB Centre, Jalan Tun Razak,
50400, Kuala Lumpur, Malaysia.",
            Country = "Malaysia",
            Phone = "+60392811234",
            Website = "www.rhbgroup.com",
            ContactPersonEmail = "support@rhbgroup.com",
            ContactPersonName = "Zulkifli Hassan",
            ContactPersonPhone = "+60392811345",
        }
    };

            context.Banks.AddRange(banks);
            await context.SaveChangesAsync();

            Console.WriteLine("Banks seeded successfully.");
        }

        private static async Task SeedSupplier(SpotDBContext context)
        {
            if (context.Suppliers.Any())
            {
                Console.WriteLine("Suppliers already exist in the database. Skipping seeding.");
                return;
            }

            var suppliers = new List<Supplier>
    {
        new Supplier
        {
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
        new Supplier
        {
            Name = "European Auto Exporters",
            Address = @"1234 Auto Avenue,
Berlin, 10115, Germany.",
            Country = "GERMANY",
            Phone = "+491234567890",
            Website = "www.euroautoexporters.de",
            ContactPersonEmail = "contact@euroautoexporters.de",
            ContactPersonName = "Hans Müller",
            ContactPersonPhone = "+491234567891",
        },
        new Supplier
        {
            Name = "American Car Dealers",
            Address = @"5678 Car Street,
Detroit, MI, 48201, USA.",
            Country = "USA",
            Phone = "+13125551234",
            Website = "www.amcar.com",
            ContactPersonEmail = "sales@amcar.com",
            ContactPersonName = "John Doe",
            ContactPersonPhone = "+13125551235",
        },
        new Supplier
        {
            Name = "Auto Imports from Korea",
            Address = @"789 Car Lane,
Seoul, 12345, South Korea.",
            Country = "SOUTH KOREA",
            Phone = "+821012345678",
            Website = "www.koreacarimports.kr",
            ContactPersonEmail = "info@koreacarimports.kr",
            ContactPersonName = "Kim Seo-Yeon",
            ContactPersonPhone = "+821012345679",
        },
        new Supplier
        {
            Name = "Luxury Motors UK",
            Address = @"456 Prestige Road,
London, W1A 1AA, United Kingdom.",
            Country = "UNITED KINGDOM",
            Phone = "+442071234567",
            Website = "www.luxurymotors.co.uk",
            ContactPersonEmail = "service@luxurymotors.co.uk",
            ContactPersonName = "James Smith",
            ContactPersonPhone = "+442071234568",
        }
    };

            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();

            Console.WriteLine("Suppliers seeded successfully.");
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
                new StockStatus { Name = "Sold" },
                new StockStatus { Name = "Cancelled" },
            };

            context.StockStatuses.AddRange(objs);
            await context.SaveChangesAsync();

        }

        //    private static async Task SeedVehicle(SpotDBContext context)
        //    {
        //        if (context.Vehicles.Any())
        //        {
        //            Console.WriteLine("Brands already exist in the database. Skipping seeding.");
        //            return;
        //        }

        //        var vehicles = new List<Vehicle>
        //{
        //   new Vehicle
        //   {
        //        BrandId = context.Brands.FirstOrDefault(c=> c.Name == "BMW").Id,
        //         Model
        //   },
        //};

        //        await context.Brands.AddRangeAsync(brands);
        //        await context.SaveChangesAsync();

        //        Console.WriteLine("Brands seeded successfully.");

        //        var bmw = await context.Brands.FirstOrDefaultAsync(x => x.Name == "BMW");

        //        if (bmw != null)
        //        {
        //            await SeedModel(context, bmw);
        //        }
        //        else
        //        {
        //            Console.WriteLine("BMW brand not found after seeding.");
        //        }
        //    }

        private static async Task SeedModel(SpotDBContext context)
        {
            if (context.Models.Any())
            {
                Console.WriteLine("Models already exist in the database. Skipping seeding.");
                return;
            }

            var brands = context.Brands.ToList();

            var models = new List<Model>
    {
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "BMW").Id,
            Name = "BMW 320i Sport 2.0",
            ShortName = "320i Sport"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "BMW").Id,
            Name = "BMW X5 xDrive40i",
            ShortName = "X5"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "AUDI").Id,
            Name = "Audi A4 2.0 TFSI",
            ShortName = "A4"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "AUDI").Id,
            Name = "Audi Q7 3.0 TDI",
            ShortName = "Q7"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "VOLKSWAGEN").Id,
            Name = "Volkswagen Golf GTI",
            ShortName = "Golf GTI"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "VOLKSWAGEN").Id,
            Name = "Volkswagen Passat",
            ShortName = "Passat"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MCLAREN").Id,
            Name = "McLaren 720S",
            ShortName = "720S"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MCLAREN").Id,
            Name = "McLaren P1",
            ShortName = "P1"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "LAND ROVER").Id,
            Name = "Land Rover Range Rover",
            ShortName = "Range Rover"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "LAND ROVER").Id,
            Name = "Land Rover Discovery",
            ShortName = "Discovery"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MERCEDES-BENZ").Id,
            Name = "Mercedes-Benz C-Class",
            ShortName = "C-Class"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MERCEDES-BENZ").Id,
            Name = "Mercedes-Benz GLE",
            ShortName = "GLE"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "TOYOTA").Id,
            Name = "Toyota Corolla",
            ShortName = "Corolla"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "TOYOTA").Id,
            Name = "Toyota Camry",
            ShortName = "Camry"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "HONDA").Id,
            Name = "Honda Civic",
            ShortName = "Civic"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "HONDA").Id,
            Name = "Honda CR-V",
            ShortName = "CR-V"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "NISSAN").Id,
            Name = "Nissan Altima",
            ShortName = "Altima"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "NISSAN").Id,
            Name = "Nissan Rogue",
            ShortName = "Rogue"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "FORD").Id,
            Name = "Ford Mustang",
            ShortName = "Mustang"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "FORD").Id,
            Name = "Ford F-150",
            ShortName = "F-150"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "CHEVROLET").Id,
            Name = "Chevrolet Camaro",
            ShortName = "Camaro"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "CHEVROLET").Id,
            Name = "Chevrolet Tahoe",
            ShortName = "Tahoe"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "TESLA").Id,
            Name = "Tesla Model S",
            ShortName = "Model S"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "TESLA").Id,
            Name = "Tesla Model X",
            ShortName = "Model X"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "FIAT").Id,
            Name = "Fiat 500",
            ShortName = "500"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "FIAT").Id,
            Name = "Fiat Panda",
            ShortName = "Panda"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "KIA").Id,
            Name = "Kia Soul",
            ShortName = "Soul"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "KIA").Id,
            Name = "Kia Sportage",
            ShortName = "Sportage"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "HYUNDAI").Id,
            Name = "Hyundai Elantra",
            ShortName = "Elantra"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "HYUNDAI").Id,
            Name = "Hyundai Santa Fe",
            ShortName = "Santa Fe"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "JAGUAR").Id,
            Name = "Jaguar XE",
            ShortName = "XE"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "JAGUAR").Id,
            Name = "Jaguar F-Pace",
            ShortName = "F-Pace"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MAZDA").Id,
            Name = "Mazda 3",
            ShortName = "Mazda 3"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MAZDA").Id,
            Name = "Mazda CX-5",
            ShortName = "CX-5"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SUBARU").Id,
            Name = "Subaru Impreza",
            ShortName = "Impreza"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SUBARU").Id,
            Name = "Subaru Forester",
            ShortName = "Forester"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "PORSCHE").Id,
            Name = "Porsche 911",
            ShortName = "911"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "PORSCHE").Id,
            Name = "Porsche Cayenne",
            ShortName = "Cayenne"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "LEXUS").Id,
            Name = "Lexus RX",
            ShortName = "RX"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "LEXUS").Id,
            Name = "Lexus ES",
            ShortName = "ES"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "INFINITI").Id,
            Name = "Infiniti Q50",
            ShortName = "Q50"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "INFINITI").Id,
            Name = "Infiniti QX60",
            ShortName = "QX60"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "VOLVO").Id,
            Name = "Volvo XC90",
            ShortName = "XC90"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "VOLVO").Id,
            Name = "Volvo S60",
            ShortName = "S60"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "ACURA").Id,
            Name = "Acura TLX",
            ShortName = "TLX"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "ACURA").Id,
            Name = "Acura MDX",
            ShortName = "MDX"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "CADILLAC").Id,
            Name = "Cadillac Escalade",
            ShortName = "Escalade"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "CADILLAC").Id,
            Name = "Cadillac XT5",
            ShortName = "XT5"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "BUICK").Id,
            Name = "Buick Enclave",
            ShortName = "Enclave"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "BUICK").Id,
            Name = "Buick Regal",
            ShortName = "Regal"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "CHRYSLER").Id,
            Name = "Chrysler 300",
            ShortName = "300"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "CHRYSLER").Id,
            Name = "Chrysler Pacifica",
            ShortName = "Pacifica"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "DODGE").Id,
            Name = "Dodge Charger",
            ShortName = "Charger"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "DODGE").Id,
            Name = "Dodge Durango",
            ShortName = "Durango"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "JEEP").Id,
            Name = "Jeep Grand Cherokee",
            ShortName = "Grand Cherokee"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "JEEP").Id,
            Name = "Jeep Wrangler",
            ShortName = "Wrangler"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "RAM").Id,
            Name = "Ram 1500",
            ShortName = "1500"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "RAM").Id,
            Name = "Ram 2500",
            ShortName = "2500"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "GMC").Id,
            Name = "GMC Sierra",
            ShortName = "Sierra"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "GMC").Id,
            Name = "GMC Yukon",
            ShortName = "Yukon"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "LINCOLN").Id,
            Name = "Lincoln Navigator",
            ShortName = "Navigator"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "LINCOLN").Id,
            Name = "Lincoln MKC",
            ShortName = "MKC"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MINI").Id,
            Name = "MINI Cooper",
            ShortName = "Cooper"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MINI").Id,
            Name = "MINI Countryman",
            ShortName = "Countryman"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MITSUBISHI").Id,
            Name = "Mitsubishi Outlander",
            ShortName = "Outlander"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MITSUBISHI").Id,
            Name = "Mitsubishi Lancer",
            ShortName = "Lancer"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "PEUGEOT").Id,
            Name = "Peugeot 308",
            ShortName = "308"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "PEUGEOT").Id,
            Name = "Peugeot 5008",
            ShortName = "5008"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "RENAULT").Id,
            Name = "Renault Clio",
            ShortName = "Clio"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "RENAULT").Id,
            Name = "Renault Megane",
            ShortName = "Megane"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SAAB").Id,
            Name = "Saab 9-3",
            ShortName = "9-3"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SAAB").Id,
            Name = "Saab 9-5",
            ShortName = "9-5"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SEAT").Id,
            Name = "SEAT Ibiza",
            ShortName = "Ibiza"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SEAT").Id,
            Name = "SEAT Leon",
            ShortName = "Leon"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SKODA").Id,
            Name = "Skoda Octavia",
            ShortName = "Octavia"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SKODA").Id,
            Name = "Skoda Superb",
            ShortName = "Superb"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SUZUKI").Id,
            Name = "Suzuki Swift",
            ShortName = "Swift"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SUZUKI").Id,
            Name = "Suzuki Vitara",
            ShortName = "Vitara"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "ALFA ROMEO").Id,
            Name = "Alfa Romeo Giulia",
            ShortName = "Giulia"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "ALFA ROMEO").Id,
            Name = "Alfa Romeo Stelvio",
            ShortName = "Stelvio"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "ASTON MARTIN").Id,
            Name = "Aston Martin DB11",
            ShortName = "DB11"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "ASTON MARTIN").Id,
            Name = "Aston Martin Vantage",
            ShortName = "Vantage"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "BENTLEY").Id,
            Name = "Bentley Bentayga",
            ShortName = "Bentayga"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "BENTLEY").Id,
            Name = "Bentley Continental GT",
            ShortName = "Continental GT"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "BUGATTI").Id,
            Name = "Bugatti Chiron",
            ShortName = "Chiron"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "BUGATTI").Id,
            Name = "Bugatti Veyron",
            ShortName = "Veyron"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "FERRARI").Id,
            Name = "Ferrari 488 GTB",
            ShortName = "488 GTB"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "FERRARI").Id,
            Name = "Ferrari F8 Tributo",
            ShortName = "F8 Tributo"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "LAMBORGHINI").Id,
            Name = "Lamborghini Aventador",
            ShortName = "Aventador"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "LAMBORGHINI").Id,
            Name = "Lamborghini Huracan",
            ShortName = "Huracan"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MASERATI").Id,
            Name = "Maserati Ghibli",
            ShortName = "Ghibli"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "MASERATI").Id,
            Name = "Maserati Levante",
            ShortName = "Levante"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "ROLLS-ROYCE").Id,
            Name = "Rolls-Royce Phantom",
            ShortName = "Phantom"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "ROLLS-ROYCE").Id,
            Name = "Rolls-Royce Ghost",
            ShortName = "Ghost"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SMART").Id,
            Name = "Smart Fortwo",
            ShortName = "Fortwo"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "SMART").Id,
            Name = "Smart Forfour",
            ShortName = "Forfour"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "TATA").Id,
            Name = "Tata Nano",
            ShortName = "Nano"
        },
        new Model {
            BrandId = brands.FirstOrDefault(c => c.Name == "TATA").Id,
            Name = "Tata Harrier",
            ShortName = "Harrier"
        },
    };

            //models = models.Where(m => m.BrandId != null).ToList();

            await context.Models.AddRangeAsync(models);
            await context.SaveChangesAsync();

            Console.WriteLine("Models seeded successfully.");
        }

        private static async Task SeedBrand(SpotDBContext context)
        {
            if (context.Brands.Any())
            {
                Console.WriteLine("Brands already exist in the database. Skipping seeding.");
                return;
            }

            var brands = new List<Brand>
    {
        new Brand { Name = "BMW" },
        new Brand { Name = "AUDI" },
        new Brand { Name = "VOLKSWAGEN" },
        new Brand { Name = "MCLAREN" },
        new Brand { Name = "LAND ROVER" },
        new Brand { Name = "MERCEDES-BENZ" },
        new Brand { Name = "TOYOTA" },
        new Brand { Name = "HONDA" },
        new Brand { Name = "NISSAN" },
        new Brand { Name = "FORD" },
        new Brand { Name = "CHEVROLET" },
        new Brand { Name = "TESLA" },
        new Brand { Name = "FIAT" },
        new Brand { Name = "KIA" },
        new Brand { Name = "HYUNDAI" },
        new Brand { Name = "JAGUAR" },
        new Brand { Name = "MAZDA" },
        new Brand { Name = "SUBARU" },
        new Brand { Name = "PORSCHE" },
        new Brand { Name = "LEXUS" },
        new Brand { Name = "INFINITI" },
        new Brand { Name = "VOLVO" },
        new Brand { Name = "ACURA" },
        new Brand { Name = "CADILLAC" },
        new Brand { Name = "BUICK" },
        new Brand { Name = "CHRYSLER" },
        new Brand { Name = "DODGE" },
        new Brand { Name = "JEEP" },
        new Brand { Name = "RAM" },
        new Brand { Name = "GMC" },
        new Brand { Name = "LINCOLN" },
        new Brand { Name = "MINI" },
        new Brand { Name = "MITSUBISHI" },
        new Brand { Name = "PEUGEOT" },
        new Brand { Name = "RENAULT" },
        new Brand { Name = "SAAB" },
        new Brand { Name = "SEAT" },
        new Brand { Name = "SKODA" },
        new Brand { Name = "SUZUKI" },
        new Brand { Name = "ALFA ROMEO" },
        new Brand { Name = "ASTON MARTIN" },
        new Brand { Name = "BENTLEY" },
        new Brand { Name = "BUGATTI" },
        new Brand { Name = "FERRARI" },
        new Brand { Name = "LAMBORGHINI" },
        new Brand { Name = "MASERATI" },
        new Brand { Name = "ROLLS-ROYCE" },
        new Brand { Name = "SMART" },
        new Brand { Name = "TATA" },
    };

            await context.Brands.AddRangeAsync(brands);
            await context.SaveChangesAsync();

            Console.WriteLine("Brands seeded successfully.");

            //var bmw = await context.Brands.FirstOrDefaultAsync(x => x.Name == "BMW");

            //if (bmw != null)
            //{
            //    await SeedModel(context, bmw);
            //}
            //else
            //{
            //    Console.WriteLine("BMW brand not found after seeding.");
            //}
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
            {
                Console.WriteLine("Vehicle types already exist in the database. Skipping seeding.");
                return;
            }

            var vehicleTypes = new List<VehicleType>
    {
        new VehicleType { Name = "Sedan" },
        new VehicleType { Name = "Saloon" },
        new VehicleType { Name = "Hatchback" },
        new VehicleType { Name = "4WD" },
        new VehicleType { Name = "SUV" },
        new VehicleType { Name = "MPV" },
        new VehicleType { Name = "Van" },
        new VehicleType { Name = "Coupe" },
        new VehicleType { Name = "Convertible" },
        new VehicleType { Name = "Pickup Truck" },
        new VehicleType { Name = "Crossover" },
        new VehicleType { Name = "Wagon" },
        new VehicleType { Name = "Sports Car" },
        new VehicleType { Name = "Electric" },
        new VehicleType { Name = "Hybrid" },
        new VehicleType { Name = "Minivan" },
        new VehicleType { Name = "Roadster" },
        new VehicleType { Name = "Compact" },
        new VehicleType { Name = "Luxury Car" },
    };

            await context.VehicleTypes.AddRangeAsync(vehicleTypes);
            await context.SaveChangesAsync();

            Console.WriteLine("Vehicle types seeded successfully.");
        }

        //private static async Task SeedUsers(SpotDBContext context, UserManager<AppUser> userManager)
        //{
        //    await SeedSuperAdminUser(context, userManager);
        //    await SeedStockManagerUser(context, userManager);
        //    await SeedCustomerManagerUser(context, userManager);
        //    await SeedFinanceManagerUser(context, userManager);
        //    await SeedSalesUser(context, userManager);

        //}

        //private static async Task SeedRolesForSubmodule(SpotDBContext context, RoleManager<IdentityRole> roleManager)
        //{
        //    if (context.SubModules.Any())
        //        return;

        //    var stock = context.Modules.FirstOrDefault(c => c.Name == "Stock");
        //    if (stock == null)
        //        return;

        //    context.SubModules.AddRange(
        //        new SubModule
        //        {
        //            Name = "Vehicle",
        //            ModuleId = stock.Id,
        //        }, new SubModule
        //        {
        //            Name = "Purchase",
        //            ModuleId = stock.Id,
        //        }, new SubModule
        //        {
        //            Name = "Import",
        //            ModuleId = stock.Id,
        //        }, new SubModule
        //        {
        //            Name = "Clearance",
        //            ModuleId = stock.Id,
        //        }, new SubModule
        //        {
        //            Name = "Sale",
        //            ModuleId = stock.Id,
        //        }, new SubModule
        //        {
        //            Name = "ArrivalChecklist",
        //            ModuleId = stock.Id,
        //        }, new SubModule
        //        {
        //            Name = "Pricing",
        //            ModuleId = stock.Id,
        //        }, new SubModule
        //        {
        //            Name = "Registration",
        //            ModuleId = stock.Id,
        //        }, new SubModule
        //        {
        //            Name = "AdminCost",
        //            ModuleId = stock.Id,
        //        }
        //        );
        //    await context.SaveChangesAsync();

        //    var roleModulePermissions = new List<RoleSubModulePermission>();
        //    roleModulePermissions.Add(new RoleSubModulePermission
        //    {
        //        RoleId = context.Roles.FirstOrDefault(c => c.Name == "Admin").Id,
        //        SubModuleId = context.SubModules.FirstOrDefault(c => c.Name == "Vehicle").Id,
        //        CanView = true,
        //    });

        //    context.RoleSubModulePermissions.AddRange(roleModulePermissions);
        //    await context.SaveChangesAsync();

        //}

        //private static async Task SeedRoles(SpotDBContext context, RoleManager<IdentityRole> roleManager)
        //{
        //    if (context.Roles.Any())
        //        return;
        //    context.Roles.AddRange(
        //        new Role { Name = "Admin" },
        //        new Role { Name = "Stock Manager" },
        //        new Role { Name = "Customer Manager" },
        //        new Role { Name = "Finance Manager" },
        //        new Role { Name = "Sales" }
        //    );
        //    await context.SaveChangesAsync();


        //    context.Modules.AddRange(
        //        new Module { Name = "Dashboard" },
        //        new Module { Name = "Stock" },
        //        new Module { Name = "Invoice" },
        //        new Module { Name = "Customer" },
        //        new Module { Name = "Supplier" },
        //        new Module { Name = "Forwarder" },
        //        new Module { Name = "Bank" },
        //        new Module { Name = "Brand" },
        //        new Module { Name = "Model" },
        //        new Module { Name = "Report" },
        //        new Module { Name = "Sales" },
        //        new Module { Name = "User" }
        //    );
        //    await context.SaveChangesAsync();


        //    var roles = context.Roles.ToList();
        //    var modules = context.Modules.ToList();
        //    var roleModulePermissions = new List<RoleModulePermission>();

        //    foreach (var role in roles)
        //    {
        //        foreach (var module in modules)
        //        {
        //            var permission = new RoleModulePermission
        //            {
        //                RoleId = role.Id,
        //                ModuleId = module.Id
        //            };
        //            if (role.Name == "Admin")
        //            {
        //                permission.CanAdd = true;
        //                permission.CanUpdate = true;
        //                permission.CanDelete = true;
        //                permission.CanView = true;
        //            }
        //            else if (role.Name == "Stock Manager")
        //            {
        //                if (module.Name == "Dashboard" ||
        //                    module.Name == "Customer" ||
        //                    module.Name == "Supplier" ||
        //                    module.Name == "Forwarder" ||
        //                    module.Name == "Bank" ||
        //                    module.Name == "Brand" ||
        //                    module.Name == "Model" ||
        //                    module.Name == "Stock"
        //                    )
        //                {
        //                    permission.CanAdd = true;
        //                    permission.CanUpdate = true;
        //                    permission.CanDelete = true;
        //                    permission.CanView = true;
        //                }
        //            }
        //            else if (role.Name == "Customer Manager")
        //            {
        //                if (module.Name == "Dashboard" ||
        //                    module.Name == "Customer" ||
        //                    module.Name == "Supplier" ||
        //                    module.Name == "Forwarder" ||
        //                    module.Name == "Bank" ||
        //                    module.Name == "Brand" ||
        //                    module.Name == "Model" ||
        //                    module.Name == "Stock"
        //                    )
        //                {
        //                    permission.CanAdd = true;
        //                    permission.CanUpdate = true;
        //                    permission.CanDelete = true;
        //                    permission.CanView = true;
        //                }
        //            }
        //            else if (role.Name == "Finance Manager")
        //            {
        //                if (module.Name == "Dashboard" ||
        //                    module.Name == "Customer" ||
        //                    module.Name == "Supplier" ||
        //                    module.Name == "Forwarder" ||
        //                    module.Name == "Bank" ||
        //                    module.Name == "Brand" ||
        //                    module.Name == "Model" ||
        //                    module.Name == "Stock" ||
        //                    module.Name == "Report" ||
        //                    module.Name == "Invoice"
        //                    )
        //                {
        //                    permission.CanAdd = true;
        //                    permission.CanUpdate = true;
        //                    permission.CanDelete = true;
        //                    permission.CanView = true;
        //                }
        //            }
        //            else if (role.Name == "Sales")
        //            {
        //                if (module.Name == "Sales")
        //                {
        //                    permission.CanAdd = true;
        //                    permission.CanUpdate = true;
        //                    permission.CanDelete = true;
        //                    permission.CanView = true;
        //                }
        //            }

        //            roleModulePermissions.Add(permission);
        //        }
        //    }

        //    context.RoleModulePermissions.AddRange(roleModulePermissions);
        //    await context.SaveChangesAsync();


        //    //var roles = new[] { "Admin", "Stock Manager" };
        //    foreach (var role in roles)
        //    {
        //        if (!await roleManager.RoleExistsAsync(role.Name))
        //        {
        //            await roleManager.CreateAsync(new IdentityRole(role.Name));
        //        }

        //        //if (!await _userManager.IsInRoleAsync(user, role))
        //        //{
        //        //    await _userManager.AddToRoleAsync(user, role);
        //        //}
        //    }


        //}


        //private static async Task SeedSuperAdminUser(SpotDBContext context, UserManager<AppUser> userManager)
        //{
        //    if (context.Profiles.Where(c => c.Role == "Admin").Any())
        //        return;

        //    Profile userAdmin = new Profile
        //    {
        //        FullName = "Administrator",
        //        Email = "admin@email.com",
        //        Phone = "0192563019",
        //        Role = "Admin",
        //    };

        //    await context.Profiles.AddAsync(userAdmin);

        //    AppUser user = new AppUser
        //    {
        //        DisplayName = "Super Admin",
        //        UserName = "admin@email.com",
        //        ProfileId = userAdmin.Id,
        //        Email = "admin@email.com",
        //        //IsSuperAdmin = true
        //        Role = "Admin"
        //    };

        //    await CreateAppUser(userManager, user);
        //    //await userManager.AddToRoleAsync(user, "Admin");
        //    if (!await userManager.IsInRoleAsync(user, "Admin"))
        //    {
        //        await userManager.AddToRoleAsync(user, "Admin");
        //    }

        //    //modelBuilder.Entity<User>().HasData(userAdmin);
        //    //Login loginAdmin = new Login
        //    //{
        //    //    Username = "admin",
        //    //    Password = "admin",
        //    //    UserId = userAdmin.Id,
        //    //    //Email = "admin@email.com"
        //    //};
        //    //await context.Logins.AddAsync(loginAdmin);
        //    //modelBuilder.Entity<Login>().HasData(loginAdmin);
        //}
        /// <summary>
        ///       new Role { Name = "Admin" },
        //new Role { Name = "Stock Manager" },
        //new Role { Name = "Customer Manager" },
        //new Role { Name = "Finance Manager" },
        //new Role { Name = "Sales" }
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <returns></returns>
        /// 

        //private static async Task SeedSalesUser(SpotDBContext context, UserManager<AppUser> userManager)
        //{
        //    if (context.Profiles.Where(c => c.Role == "Sales").Any())

        //        return;

        //    Profile userAdmin = new Profile
        //    {
        //        FullName = "Sales",
        //        Email = "sale@email.com",
        //        Phone = "0192563019",
        //        Role = "Sales",
        //    };

        //    await context.Profiles.AddAsync(userAdmin);

        //    AppUser user = new AppUser
        //    {
        //        DisplayName = "Sales",
        //        UserName = "sale@email.com",
        //        ProfileId = userAdmin.Id,
        //        Email = "sale@email.com",
        //        //IsSuperAdmin = true
        //        Role = "Sales"
        //    };

        //    await CreateAppUser(userManager, user);
        //    //await userManager.AddToRoleAsync(user, "Sales");
        //    if (!await userManager.IsInRoleAsync(user, "Sales"))
        //    {
        //        await userManager.AddToRoleAsync(user, "Sales");
        //    }

        //}


        //private static async Task SeedFinanceManagerUser(SpotDBContext context, UserManager<AppUser> userManager)
        //{
        //    if (context.Profiles.Where(c => c.Role == "Finance Manager").Any())

        //        return;

        //    Profile userAdmin = new Profile
        //    {
        //        FullName = "Finance Manager",
        //        Email = "finance@email.com",
        //        Phone = "0192563019",
        //        Role = "Finance Manager",
        //    };

        //    await context.Profiles.AddAsync(userAdmin);

        //    AppUser user = new AppUser
        //    {
        //        DisplayName = "Finance Manager",
        //        UserName = "finance@email.com",
        //        ProfileId = userAdmin.Id,
        //        Email = "finance@email.com",
        //        //IsSuperAdmin = true
        //        Role = "Finance Manager"
        //    };

        //    await CreateAppUser(userManager, user);
        //    //await userManager.AddToRoleAsync(user, "Finance Manager");
        //    if (!await userManager.IsInRoleAsync(user, "Finance Manager"))
        //    {
        //        await userManager.AddToRoleAsync(user, "Finance Manager");
        //    }

        //}

        //private static async Task SeedCustomerManagerUser(SpotDBContext context, UserManager<AppUser> userManager)
        //{
        //    if (context.Profiles.Where(c => c.Role == "Customer Manager").Any())
        //        return;

        //    Profile userAdmin = new Profile
        //    {
        //        FullName = "Customer Manager",
        //        Email = "customer@email.com",
        //        Phone = "0192563019",
        //        Role = "Customer Manager",
        //    };

        //    await context.Profiles.AddAsync(userAdmin);

        //    AppUser user = new AppUser
        //    {
        //        DisplayName = "Customer Manager",
        //        UserName = "customer@email.com",
        //        ProfileId = userAdmin.Id,
        //        Email = "customer@email.com",
        //        //IsSuperAdmin = true
        //        Role = "Customer Manager"
        //    };

        //    await CreateAppUser(userManager, user);
        //    if (!await userManager.IsInRoleAsync(user, "Customer Manager"))
        //    {
        //        await userManager.AddToRoleAsync(user, "Customer Manager");
        //    }
        //}


        //private static async Task SeedStockManagerUser(SpotDBContext context, UserManager<AppUser> userManager)
        //{
        //    if (context.Profiles.Where(c => c.Role == "Stock Manager").Any())
        //        return;

        //    Profile userAdmin = new Profile
        //    {
        //        FullName = "Stock Manager",
        //        Email = "stock@email.com",
        //        Phone = "0192563019",
        //        Role = "Stock Manager",
        //    };

        //    await context.Profiles.AddAsync(userAdmin);

        //    AppUser user = new AppUser
        //    {
        //        DisplayName = "Stock Manager",
        //        UserName = "stock@email.com",
        //        ProfileId = userAdmin.Id,
        //        Email = "stock@email.com",
        //        //IsSuperAdmin = true
        //        Role = "Stock Manager"
        //    };

        //    await CreateAppUser(userManager, user);
        //    if (!await userManager.IsInRoleAsync(user, "Stock Manager"))
        //    {
        //        await userManager.AddToRoleAsync(user, "Stock Manager");
        //    }


        //}


        //private static async Task CreateAppUser(UserManager<AppUser> userManager, AppUser user)
        //{
        //    Console.WriteLine(string.Format("Email:{0}, UserName:{1}", user.Email, user.UserName));
        //    await userManager.CreateAsync(user, "Qwerty@123");
        //}


        ///
        private static async Task SeedRoles(SpotDBContext context, RoleManager<IdentityRole> roleManager)
        {
            if (context.Roles.Any())
                return;

            var roles = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "Stock Manager" },
            new Role { Name = "Customer Manager" },
            new Role { Name = "Finance Manager" },
            new Role { Name = "Sales" }
        };

            context.Roles.AddRange(roles);
            await context.SaveChangesAsync();

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(new IdentityRole(role.Name));
                }
            }
        }

        private static async Task SeedModulesAndSubModules(SpotDBContext context)
        {
            if (!context.Modules.Any())
            {
                // Add main modules
                var modules = new List<Module>
            {
                new Module { Name = "Dashboard" },
                new Module { Name = "Stocks" },
                new Module { Name = "Customer" },
                new Module { Name = "MasterData" },
                new Module { Name = "Letters" },
                new Module { Name = "Reports" },
                new Module { Name = "Vehicle" },
                new Module { Name = "Administration" },
                new Module { Name = "System" }
            };

                context.Modules.AddRange(modules);
                await context.SaveChangesAsync();

                // Add submodules for "Stocks"
                var stocksModule = context.Modules.FirstOrDefault(m => m.Name == "Stocks");
                if (stocksModule != null)
                {
                    var stockSubModules = new List<SubModule>
                {
                    new SubModule { Name = "Vehicle", ModuleId = stocksModule.Id },
                    new SubModule { Name = "Purchase", ModuleId = stocksModule.Id },
                    new SubModule { Name = "Import", ModuleId = stocksModule.Id },
                    new SubModule { Name = "Clearance", ModuleId = stocksModule.Id },
                    new SubModule { Name = "Sale", ModuleId = stocksModule.Id },
                    new SubModule { Name = "ArrivalChecklist", ModuleId = stocksModule.Id },
                    new SubModule { Name = "Pricing", ModuleId = stocksModule.Id },
                    new SubModule { Name = "Registration", ModuleId = stocksModule.Id },
                    new SubModule { Name = "AdminCost", ModuleId = stocksModule.Id }
                };
                    context.SubModules.AddRange(stockSubModules);
                }

                // Add submodules for "MasterData"
                var masterDataModule = context.Modules.FirstOrDefault(m => m.Name == "MasterData");
                if (masterDataModule != null)
                {
                    var masterDataSubModules = new List<SubModule>
                {
                    new SubModule { Name = "Suppliers", ModuleId = masterDataModule.Id },
                    new SubModule { Name = "Forwarders", ModuleId = masterDataModule.Id },
                    new SubModule { Name = "Banks", ModuleId = masterDataModule.Id },
                    new SubModule { Name = "Brands", ModuleId = masterDataModule.Id },
                    new SubModule { Name = "Models", ModuleId = masterDataModule.Id }
                };
                    context.SubModules.AddRange(masterDataSubModules);
                }

                // Add submodules for "Letters"
                var lettersModule = context.Modules.FirstOrDefault(m => m.Name == "Letters");
                if (lettersModule != null)
                {
                    var lettersSubModules = new List<SubModule>
                {
                    new SubModule { Name = "Disrbustments", ModuleId = lettersModule.Id }
                };
                    context.SubModules.AddRange(lettersSubModules);
                }

                // Add submodules for "Reports"
                var reportsModule = context.Modules.FirstOrDefault(m => m.Name == "Reports");
                if (reportsModule != null)
                {
                    var reportsSubModules = new List<SubModule>
                {
                    new SubModule { Name = "SalesMonthly", ModuleId = reportsModule.Id },
                    new SubModule { Name = "SalesYearly", ModuleId = reportsModule.Id }
                };
                    context.SubModules.AddRange(reportsSubModules);
                }

                // Add submodules for "Administration"
                var administrationModule = context.Modules.FirstOrDefault(m => m.Name == "Administration");
                if (administrationModule != null)
                {
                    var administrationSubModules = new List<SubModule>
                {
                    new SubModule { Name = "User", ModuleId = administrationModule.Id },
                    new SubModule { Name = "Roles", ModuleId = administrationModule.Id }
                };
                    context.SubModules.AddRange(administrationSubModules);
                }

                // Add submodules for "System"
                var systemModule = context.Modules.FirstOrDefault(m => m.Name == "System");
                if (systemModule != null)
                {
                    var systemSubModules = new List<SubModule>
                {
                    new SubModule { Name = "Company", ModuleId = systemModule.Id }
                };
                    context.SubModules.AddRange(systemSubModules);
                }

                await context.SaveChangesAsync();
            }
        }
        private static async Task SeedRolePermissionsForMainModule(SpotDBContext context)
        {
            var roles = context.Roles.ToList();
            var modules = context.Modules.Include(c => c.SubModules).ToList();
            var roleModulePermissions = new List<RoleModulePermission>();

            foreach (var role in roles)
            {
                foreach (var module in modules)
                {
                    var permission = new RoleModulePermission
                    {
                        RoleId = role.Id,
                        ModuleId = module.Id
                    };
                    if (role.Name == "Admin")
                    {
                        permission.CanAdd = true;
                        permission.CanUpdate = true;
                        permission.CanDelete = true;
                        permission.CanView = true;

                        var roleSubModulePermissions = new List<RoleSubModulePermission>();
                        foreach (var subModule in module.SubModules)
                        {
                            var subModulePermission = new RoleSubModulePermission
                            {
                                SubModuleId = subModule.Id,
                                RoleId = role.Id,
                                CanAdd = true,
                                CanUpdate = true,
                                CanDelete = true,
                                CanView = true
                            };
                            roleSubModulePermissions.Add(subModulePermission);
                        }
                        context.RoleSubModulePermissions.AddRange(roleSubModulePermissions);
                        await context.SaveChangesAsync();
                    }
                    else if (role.Name == "Customer Manager")
                    {
                        if (module.Name == "Dashboard" ||
                            //module.Name == "Stocks" ||
                            module.Name == "Customer" ||
                            //module.Name == "MasterData" ||
                            //module.Name == "Letters" ||
                            //module.Name == "Reports" ||
                            module.Name == "Vehicle" 
                            //module.Name == "Administration" ||
                            //module.Name == "System"
                            )
                        {
                            permission.CanAdd = true;
                            permission.CanUpdate = true;
                            permission.CanDelete = true;
                            permission.CanView = true;

                            var roleSubModulePermissions = new List<RoleSubModulePermission>();
                            foreach (var subModule in module.SubModules)
                            {
                                var subModulePermission = new RoleSubModulePermission
                                {
                                    SubModuleId = subModule.Id,
                                    RoleId = role.Id,
                                    CanAdd = true,
                                    CanUpdate = true,
                                    CanDelete = true,
                                    CanView = true
                                };
                                roleSubModulePermissions.Add(subModulePermission);
                            }
                            context.RoleSubModulePermissions.AddRange(roleSubModulePermissions);
                            await context.SaveChangesAsync();

                        }
                    }
                    else if (role.Name == "Sales")
                    {
                        if (
                            module.Name == "Vehicle"
                            )
                        {
                            permission.CanAdd = true;
                            permission.CanUpdate = true;
                            permission.CanDelete = true;
                            permission.CanView = true;

                            var roleSubModulePermissions = new List<RoleSubModulePermission>();
                            foreach (var subModule in module.SubModules)
                            {
                                var subModulePermission = new RoleSubModulePermission
                                {
                                    SubModuleId = subModule.Id,
                                    RoleId = role.Id,
                                    CanAdd = true,
                                    CanUpdate = true,
                                    CanDelete = true,
                                    CanView = true
                                };
                                roleSubModulePermissions.Add(subModulePermission);
                            }
                            context.RoleSubModulePermissions.AddRange(roleSubModulePermissions);
                            await context.SaveChangesAsync();

                        }
                    }

                    roleModulePermissions.Add(permission);


                }
            }
            context.RoleModulePermissions.AddRange(roleModulePermissions);
            await context.SaveChangesAsync();

        }

        //private static async Task SeedRolePermissionsForSubModule(SpotDBContext context)
        //{

        //    var roles = context.Roles.ToList();
        //    var modules = context.SubModules.ToList();
        //    var roleModulePermissions = new List<RoleModulePermission>();
        //    var adminPermisions = context.RoleModulePermissions.Where(c => c.Role.Name == "Admin");

        //    foreach(var admin in adminPermisions)
        //    {
        //        var roleSubModulePermission = new RoleSubModulePermission();
        //    }


        //    foreach (var role in roles)
        //    {
        //        foreach (var module in modules)
        //        {
        //            var permission = new RoleModulePermission
        //            {
        //                RoleId = role.Id,
        //                ModuleId = module.Id
        //            };
        //            if (role.Name == "Admin")
        //            {
        //                permission.CanAdd = true;
        //                permission.CanUpdate = true;
        //                permission.CanDelete = true;
        //                permission.CanView = true;

        //                if (role.SubModules != null)
        //                {
        //                    var roleSubModulePermissions = new List<RoleSubModulePermission>();

        //                    foreach (var subModule in module.SubModules)
        //                    {
        //                        //if (!context.RoleModulePermissions.Any(rmp => rmp.RoleId == role.Id && rmp.ModuleId == subModule.ModuleId))
        //                        //{
        //                        var subModulePermission = new RoleSubModulePermission
        //                        {
        //                            RoleId = role.Id,
        //                            SubModuleId = subModule.ModuleId,
        //                            CanAdd = true,
        //                            CanUpdate = true,
        //                            CanDelete = true,
        //                            CanView = true // Example, adjust as needed
        //                        };

        //                        roleSubModulePermissions.Add(subModulePermission);
        //                        //}
        //                    }



        //                    //    var roleModulePermissions = new List<RoleSubModulePermission>();
        //                    //    roleModulePermissions.Add(new RoleSubModulePermission
        //                    //    {
        //                    //        RoleId = context.Roles.FirstOrDefault(c => c.Name == "Admin").Id,
        //                    //        SubModuleId = context.SubModules.FirstOrDefault(c => c.Name == "Vehicle").Id,
        //                    //        CanView = true,
        //                    //    });

        //                    context.RoleSubModulePermissions.AddRange(roleSubModulePermissions);
        //                    await context.SaveChangesAsync();

        //                }

        //            }

        //            roleModulePermissions.Add(permission);
        //        }
        //    }
        //    context.RoleModulePermissions.AddRange(roleModulePermissions);
        //    await context.SaveChangesAsync();

        //}



        private static async Task SeedUsers(SpotDBContext context, UserManager<AppUser> userManager)
        {
            await SeedSuperAdminUser(context, userManager);
            await SeedStockManagerUser(context, userManager);
            await SeedCustomerManagerUser(context, userManager);
            await SeedFinanceManagerUser(context, userManager);
            await SeedSalesUser(context, userManager);
        }

        private static async Task SeedSuperAdminUser(SpotDBContext context, UserManager<AppUser> userManager)
        {
            if (context.Profiles.Where(c => c.Role == "Admin").Any())
                return;

            var userAdminProfile = new Profile
            {
                FullName = "Administrator",
                Email = "admin@email.com",
                Phone = "0192563019",
                Role = "Admin",
            };

            await context.Profiles.AddAsync(userAdminProfile);
            await context.SaveChangesAsync();

            var userAdmin = new AppUser
            {
                DisplayName = "Super Admin",
                UserName = "admin@email.com",
                ProfileId = userAdminProfile.Id,
                Email = "admin@email.com",
                Role = "Admin"
            };

            await CreateAppUser(userManager, userAdmin);
            if (!await userManager.IsInRoleAsync(userAdmin, "Admin"))
            {
                await userManager.AddToRoleAsync(userAdmin, "Admin");
            }
        }

        private static async Task SeedSalesUser(SpotDBContext context, UserManager<AppUser> userManager)
        {
            if (context.Profiles.Where(c => c.Role == "Sales").Any())
                return;

            var salesProfile = new Profile
            {
                FullName = "Sales",
                Email = "sale@email.com",
                Phone = "0192563019",
                Role = "Sales",
            };

            await context.Profiles.AddAsync(salesProfile);
            await context.SaveChangesAsync();

            var salesUser = new AppUser
            {
                DisplayName = "Sales",
                UserName = "sale@email.com",
                ProfileId = salesProfile.Id,
                Email = "sale@email.com",
                Role = "Sales"
            };

            await CreateAppUser(userManager, salesUser);
            if (!await userManager.IsInRoleAsync(salesUser, "Sales"))
            {
                await userManager.AddToRoleAsync(salesUser, "Sales");
            }
        }

        private static async Task SeedFinanceManagerUser(SpotDBContext context, UserManager<AppUser> userManager)
        {
            if (context.Profiles.Where(c => c.Role == "Finance Manager").Any())
                return;

            var financeManagerProfile = new Profile
            {
                FullName = "Finance Manager",
                Email = "finance@email.com",
                Phone = "0192563019",
                Role = "Finance Manager",
            };

            await context.Profiles.AddAsync(financeManagerProfile);
            await context.SaveChangesAsync();

            var financeManagerUser = new AppUser
            {
                DisplayName = "Finance Manager",
                UserName = "finance@email.com",
                ProfileId = financeManagerProfile.Id,
                Email = "finance@email.com",
                Role = "Finance Manager"
            };

            await CreateAppUser(userManager, financeManagerUser);
            if (!await userManager.IsInRoleAsync(financeManagerUser, "Finance Manager"))
            {
                await userManager.AddToRoleAsync(financeManagerUser, "Finance Manager");
            }
        }

        private static async Task SeedCustomerManagerUser(SpotDBContext context, UserManager<AppUser> userManager)
        {
            if (context.Profiles.Where(c => c.Role == "Customer Manager").Any())
                return;

            var customerManagerProfile = new Profile
            {
                FullName = "Customer Manager",
                Email = "customer@email.com",
                Phone = "0192563019",
                Role = "Customer Manager",
            };

            await context.Profiles.AddAsync(customerManagerProfile);
            await context.SaveChangesAsync();

            var customerManagerUser = new AppUser
            {
                DisplayName = "Customer Manager",
                UserName = "customer@email.com",
                ProfileId = customerManagerProfile.Id,
                Email = "customer@email.com",
                Role = "Customer Manager"
            };

            await CreateAppUser(userManager, customerManagerUser);
            if (!await userManager.IsInRoleAsync(customerManagerUser, "Customer Manager"))
            {
                await userManager.AddToRoleAsync(customerManagerUser, "Customer Manager");
            }
        }

        private static async Task SeedStockManagerUser(SpotDBContext context, UserManager<AppUser> userManager)
        {
            if (context.Profiles.Where(c => c.Role == "Stock Manager").Any())
                return;

            var stockManagerProfile = new Profile
            {
                FullName = "Stock Manager",
                Email = "stock@email.com",
                Phone = "0192563019",
                Role = "Stock Manager",
            };

            await context.Profiles.AddAsync(stockManagerProfile);
            await context.SaveChangesAsync();

            var stockManagerUser = new AppUser
            {
                DisplayName = "Stock Manager",
                UserName = "stock@email.com",
                ProfileId = stockManagerProfile.Id,
                Email = "stock@email.com",
                Role = "Stock Manager"
            };

            await CreateAppUser(userManager, stockManagerUser);
            if (!await userManager.IsInRoleAsync(stockManagerUser, "Stock Manager"))
            {
                await userManager.AddToRoleAsync(stockManagerUser, "Stock Manager");
            }
        }

        private static async Task CreateAppUser(UserManager<AppUser> userManager, AppUser user)
        {
            var result = await userManager.CreateAsync(user, "qwe123");
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }


    }
}
