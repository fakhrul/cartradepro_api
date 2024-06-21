using Microsoft.AspNetCore.Identity;
using SPOT_API.Persistence;
using SPOT_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPOT_API.Persistence
{
    public class Seed
    {
        public static async Task SeedData(SpotDBContext context, UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                try
                {
                    await SeedUsers(context, userManager);

                    await SeedGeoFenceByArea(context);
                    await SeedGeoFenceByMap(context);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                }
            }

        }

        private static async Task SeedGeoFenceByMap(SpotDBContext context)
        {
            GeoFenceByMap fences = new GeoFenceByMap
            {
                Name = "Fence Map 1",
                TenantId = context.Tenants.First().Id,
            };

            if (fences.GeoFenceCoordItemList == null)
                fences.GeoFenceCoordItemList = new List<GeoFenceCoordItem>();

            await context.GeoFenceByMaps.AddAsync(fences);

            var area = new GeoFenceCoordItem
            {
                GeoFenceByMap = fences,
                Lat = 2.986875670556447,
                Lng = 101.74782594860265
            };
            fences.GeoFenceCoordItemList.Add(area);
            await context.GeoFenceCoordItems.AddAsync(area);

            var area1 = new GeoFenceCoordItem
            {
                GeoFenceByMap = fences,
                Lat = 2.9475292935731305,
                Lng = 101.72577872975442
            };
            fences.GeoFenceCoordItemList.Add(area1);
            await context.GeoFenceCoordItems.AddAsync(area1);

            var area2 = new GeoFenceCoordItem
            {
                GeoFenceByMap = fences,
                Lat = 2.927855582294908,
                Lng = 101.80508747494576
            };
            fences.GeoFenceCoordItemList.Add(area2);
            await context.GeoFenceCoordItems.AddAsync(area2);

            var area3 = new GeoFenceCoordItem
            {
                GeoFenceByMap = fences,
                Lat = 2.9806578129251293,
                Lng = 101.80284192488888
            };
            fences.GeoFenceCoordItemList.Add(area3);
            await context.GeoFenceCoordItems.AddAsync(area3);





        }

        private static async Task SeedGeoFenceByArea(SpotDBContext context)
        {
            GeoFenceByArea fences = new GeoFenceByArea
            {
                Name = "Fence 1",
                TenantId = context.Tenants.First().Id,
            };

            if (fences.GeoFenceAreaItemList == null)
                fences.GeoFenceAreaItemList = new List<GeoFenceAreaItem>();

            var area = new GeoFenceAreaItem
            {
                AreaId = context.Areas.FirstOrDefault().Id,
                //GeoFenceByAreaId = fences.Id,
                GeoFenceByArea = fences
            };

            fences.GeoFenceAreaItemList.Add(area);

            await context.GeoFenceByAreas.AddAsync(fences);

            await context.GeoFenceAreaItems.AddAsync(area);

            var geoFenceRule = new GeoFenceRuleByArea
            {
                Name = "Restricted Area",
                GeoFenceByAreaId = area.Id,
                GeoFencePolicy = "inside",
                IsEnable = true,
                TenantId = context.Tenants.First().Id,
            };



            await context.GeoFenceRuleByAreas.AddAsync(geoFenceRule);
        }

        private static async Task SeedUsers(SpotDBContext context, UserManager<AppUser> userManager)
        {
            await SeedAdminUser(context, userManager);
            await SeedTenantUsers(context, userManager);
        }

        private static async Task SeedTenantUsers(SpotDBContext context, UserManager<AppUser> userManager)
        {
            await SeedPrsbTenant(context, userManager);

            for (int i = 1; i <= 2; i++)
            {
                await SeedTenantUser(context, userManager, i);
            }

        }

        private static async Task SeedPrsbTenant(SpotDBContext context, UserManager<AppUser> userManager)
        {
            Profile userAdmin = new Profile
            {
                FullName = "Admin for PRSB ",
                Email = "prsb@email.com",
                Phone = "019-2563019",
                Role = "admin",
            };
            await context.Profiles.AddAsync(userAdmin);

            Tenant tenant = new Tenant
            {
                Name = "Petronas Research Sdn Bhd",
                Address1 = "Jalan Bahagia 1",
                Address2 = "Jalan Bahagia 2",
                Address3 = "Jalan Bahagia 3",
                ProfileId = userAdmin.Id,
                Code = "PRSB",
                Latitude = 2.9712234099573998,
                Longitude = 101.73835427789933,
                IsEnable = true
            };

            //userAdmin.TenantId = tenant.Id;

            AppUser user = new AppUser
            {
                DisplayName = userAdmin.FullName,
                UserName = userAdmin.Email,
                ProfileId = userAdmin.Id,
                Email = userAdmin.Email,
                TenantCode = tenant.Code,
                TenantId = tenant.Id,
            };

            await userManager.CreateAsync(user, "Qwerty@123");

            Department accountDepartment = new Department
            {
                Code = "ACC",
                Name = "Account Department",
                TenantId = tenant.Id,
                Tenant = tenant,
            };

            await context.Departments.AddAsync(accountDepartment);
            await SeedStaffUsers(context, accountDepartment, 50);

            Department saleDepartment = new Department
            {
                Code = "SAL",
                Name = "Sales Department",
                TenantId = tenant.Id,
                Tenant = tenant,
            };

            await context.Departments.AddAsync(saleDepartment);
            await SeedStaffUsers(context, saleDepartment, 50);

            Department engenieringDepartment = new Department
            {
                Code = "ENG",
                Name = "Engineering Department",
                TenantId = tenant.Id,
                Tenant = tenant,
            };

            await context.Departments.AddAsync(engenieringDepartment);

            await SeedStaffUsers(context, engenieringDepartment, 50);

            //Document document2 = new Document
            //{
            //    Driver = "azure",
            //    Path = "/path/",
            //};

            //await context.Documents.AddAsync(document2);

            Map siteMap = new Map
            {
                Name = "SITE MAP",
                Code = tenant.Code + "-SITE",
                TenantId = tenant.Id,
                Latitude = 2.969356755713358,
                Longitude = 101.73778970061079,
                //DocumentMapId = document2.Id,
            };

            await context.Maps.AddAsync(siteMap);


            Map map = new Map
            {
                Name = "PRSB MAP",
                Code = tenant.Code + "-MAP",
                TenantId = tenant.Id,
                Latitude = 2.971503726354883,
                Longitude = 101.74022663293312,
                Count = 150,
                CountLastUpdated = DateTime.Now,

                //DocumentMapId = document.Id,
            };

            await context.Maps.AddAsync(map);


            Location blockA = new Location
            {
                Name = "Block A ",
                Code = map.Code + "-B-A",
                MapId = map.Id,
                Latitude = 2.971013172612587,
                Longitude = 101.73949300149397,
                Count = 50,
                CountLastUpdated = DateTime.Now,

                //DocumentMapId = document.Id,
            };

            await context.Locations.AddAsync(blockA);

            Document document = new Document
            {
                Driver = "azure",
                Path = "/path/",
            };

            await context.Documents.AddAsync(document);

            Level level = new Level
            {
                DocumentMapId = document.Id,
                LocationId = blockA.Id,
                Name = "Ground Floor",
                Code = blockA.Code + "-G",
                Count = 50,
                CountLastUpdated = DateTime.Now,

            };
            await context.Levels.AddAsync(level);

            await SeedAreas(context, level, 5, 10);


            Location blockB = new Location
            {
                Name = "Block B ",
                Code = map.Code + "-B-B",
                MapId = map.Id,
                Latitude = 2.9717458177400204,
                Longitude = 101.74026171965754,
                DocumentMapId = document.Id,
                Count = 100,
                CountLastUpdated = DateTime.Now,

            };

            await context.Locations.AddAsync(blockB);

            Level blockBLevel1 = new Level
            {
                DocumentMapId = document.Id,
                LocationId = blockB.Id,
                Name = "Level 1",
                Code = blockB.Code + "-1",
                Count = 50,
                CountLastUpdated = DateTime.Now,

            };
            await context.Levels.AddAsync(blockBLevel1);

            await SeedAreas(context, blockBLevel1, 2, 25);


            //await SeedAreas(context, level);

            Level blockBLevel2 = new Level
            {
                DocumentMapId = document.Id,
                LocationId = blockB.Id,
                Name = "Level 2",
                Code = blockB.Code + "-2",
                Count = 50,
                CountLastUpdated = DateTime.Now,

            };
            await context.Levels.AddAsync(blockBLevel2);

            await SeedAreas(context, blockBLevel2, 2, 25);

            //await SeedMaps(context, tenant);

            //await SeedDashboard(context, tenant);
            //await SeedDeviceTypes(context, tenant);
            await SeedDashboard(context, tenant);
            await SeedDeviceTypes(context, tenant);


            Area areaSchedule = new Area
            {
                DocumentMapId = level.DocumentMapId,
                ImageCoordX = 20,
                ImageCoordY = 20,
                LevelId = blockBLevel2.Id,
                Name = "Zone/Area " + 20.ToString(),
                Code = blockBLevel2.Code + "Z" + 20.ToString(),
                FingerPrintCode = level.Code + "-" + "Z" + 20.ToString(),
                Count = 1,
                CountLastUpdated = DateTime.Now
            };
            await context.Areas.AddAsync(areaSchedule);

            Profile scheduleProfile = new Profile
            {
                StaffNo = "ID-S" + 1.ToString(),
                DepartmentId = engenieringDepartment.Id,
                PassportNo = "P123" + 1.ToString("D5"),
                MyKad = "M123" + 1.ToString("D12"),
                FullName = "Staff Schedule Test " + 1.ToString(),
                Email = "staffschedule" + engenieringDepartment.Tenant.Code + engenieringDepartment.Code + 1.ToString() + "@email.com",
                Phone = "019-2563019",
                Role = "staff",
            };
            await context.Profiles.AddAsync(scheduleProfile);

            Schedule schedule = new Schedule
            {
                AreaId = areaSchedule.Id,
                StartDateTime = DateTime.Now.AddHours(-4),
                EndDateTime = DateTime.Now,
                Name = "Office Schedule",
                ProfileId = scheduleProfile.Id,
            };
            await context.Schedules.AddAsync(schedule);

            MissingUser mu = new MissingUser
            {
                ScheduleId = schedule.Id,
                ProfileId = schedule.ProfileId,
                TenantId = tenant.Id,
            };
            await context.MissingUsers.AddAsync(mu);


            Emergency e = new Emergency
            {
                AreaId = schedule.AreaId,
                Name = "Fire",
                IsActive = true,
                TenantId = tenant.Id,
            };

            await context.Emergencies.AddAsync(e);

            EmergencyUser eu = new EmergencyUser
            {
                EmergencyId = e.Id,
                ProfileId = schedule.ProfileId,
                IsAvailable = true,
            };
            await context.EmergencyUsers.AddAsync(eu);

            await context.Tenants.AddAsync(tenant);

        }

        private static async Task SeedTenantUser(SpotDBContext context, UserManager<AppUser> userManager, int count)
        {
            Profile userAdmin = new Profile
            {
                FullName = "Tenant " + count.ToString(),
                Email = "admin" + count.ToString() + "@email.com",
                Phone = "019-2563019",
                Role = "admin",
            };
            await context.Profiles.AddAsync(userAdmin);



            Tenant tenant = new Tenant
            {
                Name = "Company " + count.ToString(),
                Address1 = "Jalan Bahagia " + count.ToString(),
                ProfileId = userAdmin.Id,
                Code = "C" + count.ToString(),
                Latitude = 2.900845539314246,
                Longitude = 101.7912551264661,
                IsEnable = true
            };

            //userAdmin.TenantId = tenant.Id;

            AppUser user = new AppUser
            {
                DisplayName = "Tenant",
                UserName = "tenant" + count.ToString(),
                ProfileId = userAdmin.Id,
                Email = "tenant" + count.ToString() + "@email.com",
                TenantCode = tenant.Code,
                TenantId = tenant.Id,
                //Role = 
            };

            await userManager.CreateAsync(user, "Qwerty@123");

            await SeedMaps(context, tenant);

            await SeedDashboard(context, tenant);
            await SeedDeviceTypes(context, tenant);

            await SeedDepartments(context, tenant);

            await context.Tenants.AddAsync(tenant);
            //modelBuilder.Entity<Tenant>().HasData(tenant);
        }

        private static bool _isAlreadySeedDeviceType;
        private static async Task SeedDeviceTypes(SpotDBContext context, Tenant tenant)
        {
            //if (_isAlreadySeedDeviceType == true)
            //    return;
            //_isAlreadySeedDeviceType = true;
            DeviceType dev1 = new DeviceType
            {
                Code = "1",
                Name = "Wifi Device",
                TenantId = tenant.Id,
            };
            await context.DeviceTypes.AddAsync(dev1);
            await SeedDevices(context, tenant, dev1);

            DeviceType dev2 = new DeviceType
            {
                TenantId = tenant.Id,
                Code = "2",
                Name = "Wifi Mobile App"
            };
            await context.DeviceTypes.AddAsync(dev2);
            await SeedDevices(context, tenant, dev2);

            DeviceType dev3 = new DeviceType
            {
                TenantId = tenant.Id,
                Code = "3",
                Name = "Bluetooth Device"
            };
            await context.DeviceTypes.AddAsync(dev3);
            await SeedDevices(context, tenant, dev3);

            DeviceType dev4 = new DeviceType
            {
                TenantId = tenant.Id,
                Code = "4",
                Name = "Bluetooth Mobile App"
            };
            await context.DeviceTypes.AddAsync(dev4);
            await SeedDevices(context, tenant, dev4);
        }

        private static async Task SeedDepartments(SpotDBContext context, Tenant tenant)
        {
            for (int i = 1; i <= 2; i++)
            {
                await SeedDepartment(context, tenant, i);
            }
        }

        private static async Task SeedDepartment(SpotDBContext context, Tenant tenant, int i)
        {
            try
            {
                Department map = new Department
                {
                    Code = "DEP" + i.ToString(),
                    Name = "Department " + i.ToString(),
                    TenantId = tenant.Id,
                    Tenant = tenant,
                };

                await context.Departments.AddAsync(map);

                await SeedStaffUsers(context, map);

            }
            catch (Exception ex)
            {
            }

        }

        private static async Task SeedStaffUsers(SpotDBContext context, Department tenant)
        {
            await SeedStaffUsers(context, tenant, 2);
        }
        private static async Task SeedStaffUsers(SpotDBContext context, Department tenant, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                await SeedStaffUser(context, tenant, i);
            }
        }

        private static async Task SeedStaffUser(SpotDBContext context, Department department, int count)
        {
            Profile userAdmin = new Profile
            {
                StaffNo = "ID-" + count.ToString(),
                DepartmentId = department.Id,
                PassportNo = "P" + count.ToString("D5"),
                MyKad = "M" + count.ToString("D12"),
                FullName = "Staff " + count.ToString(),
                Email = "staff" + department.Tenant.Code + department.Code + count.ToString() + "@email.com",
                Phone = "019-2563019",
                Role = "staff",
            };
            await context.Profiles.AddAsync(userAdmin);
        }


        private static async Task SeedDevices(SpotDBContext context, Tenant tenant, DeviceType dev)
        {

            for (int i = 1; i <= 2; i++)
            {
                await SeedDevice(context, tenant, dev, i);
            }
        }

        private static async Task SeedDevice(SpotDBContext context, Tenant tenant, DeviceType dev, int i)
        {
            Device map = new Device
            {
                Name = "DEV" + i.ToString(),
                Code = "T-" + tenant.Code + "-T-" + dev.Code + "-D-" + i.ToString("D4"),
                TenantId = tenant.Id,
                DeviceTypeId = dev.Id,
                MacAddress = "8C:AA:B5:1B:4D:" + i.ToString("D2")
            };

            await context.Devices.AddAsync(map);
        }


        private static async Task SeedDashboard(SpotDBContext context, Tenant tenant)
        {

            Dashboard map = new Dashboard
            {
                Date = DateTime.Now.Date,
                TenantId = tenant.Id,
                TotalAllUser = 800,
                TotalActiveUser = 600,
                TotalInActiveUser = 200,
                TotalHeadCount = 812,
                TotalMissingUser = 2,
                TotalPendingApproval = 10,
                TotalRegisteredStaff = 782,
                TotalRegisteredVisitor = 18,
            };

            await context.Dashboards.AddAsync(map);
        }

        private static async Task SeedMaps(SpotDBContext context, Tenant tenant)
        {
            for (int i = 1; i <= 2; i++)
            {
                await SeedMap(context, tenant, i);
            }
        }

        private static async Task SeedMap(SpotDBContext context, Tenant tenant, int i)
        {
            Document document = new Document
            {
                Driver = "azure",
                Path = "/path/",
            };

            await context.Documents.AddAsync(document);

            Map map = new Map
            {
                Name = "Map " + i.ToString(),
                Code = tenant.Code + "M" + i.ToString(),
                TenantId = tenant.Id,
                Latitude = tenant.Latitude,
                Longitude = tenant.Longitude,
                DocumentMapId = document.Id,
            };

            await context.Maps.AddAsync(map);

            await SeedLocations(context, map);
        }


        private static async Task SeedLocations(SpotDBContext context, Map map)
        {
            for (int i = 1; i <= 2; i++)
            {
                await SeedLocation(context, map, i);
            }
        }


        private static async Task SeedLocation(SpotDBContext context, Map map, int i)
        {
            Document document = new Document
            {
                Driver = "azure",
                Path = "/paht/",
            };

            await context.Documents.AddAsync(document);

            Location location = new Location
            {
                Name = "Block " + i.ToString(),
                Code = map.Code + "B" + i.ToString(),
                MapId = map.Id,
                Latitude = map.Latitude,
                Longitude = map.Longitude,
                DocumentMapId = document.Id,
            };

            await context.Locations.AddAsync(location);

            await SeedLevels(context, location);

            //await SeedAreas(context, location);
        }

        private static async Task SeedLevels(SpotDBContext context, Location location)
        {
            for (int i = 1; i <= 2; i++)
            {
                await SeedLevel(context, location, i);
            }
        }

        private static async Task SeedLevel(SpotDBContext context, Location location, int i)
        {
            Level level = new Level
            {
                DocumentMapId = location.DocumentMapId,
                //ImageCoordX = i,
                //ImageCoordY = i,
                LocationId = location.Id,
                Name = "Floor " + i.ToString(),
                Code = location.Code + "L" + i.ToString(),
            };
            await context.Levels.AddAsync(level);

            await SeedAreas(context, level);

        }

        private static async Task SeedAreas(SpotDBContext context, Level level)
        {
            await SeedAreas(context, level, 2);
        }
        private static async Task SeedAreas(SpotDBContext context, Level level, int count)
        {
            await SeedAreas(context, level, count, 0);
        }

        private static async Task SeedAreas(SpotDBContext context, Level level, int count, int totalUser)
        {
            for (int i = 1; i <= count; i++)
            {
                await SeedArea(context, level, i, totalUser);
            }
        }

        private static async Task SeedArea(SpotDBContext context, Level level, int i, int totalUser)
        {
            Area area = new Area
            {
                DocumentMapId = level.DocumentMapId,
                ImageCoordX = i,
                ImageCoordY = i,
                LevelId = level.Id,
                Name = "Zone/Area " + i.ToString(),
                Code = level.Code + "Z" + i.ToString(),
                FingerPrintCode = level.Code + "-" + "Z" + i.ToString(),
                Count = totalUser,
                CountLastUpdated = DateTime.Now
            };
            await context.Areas.AddAsync(area);

        }

        private static async Task SeedAdminUser(SpotDBContext context, UserManager<AppUser> userManager)
        {
            Profile userAdmin = new Profile
            {
                //LoginId = loginAdmin.Id,
                //Login = loginAdmin,
                FullName = "Administrator",
                Email = "admin@email.com",
                Phone = "019-2563019",
                Role = "admin",
            };

            await context.Profiles.AddAsync(userAdmin);


            AppUser user = new AppUser
            {
                DisplayName = "Super Admin",
                UserName = "admin",
                ProfileId = userAdmin.Id,
                Email = "admin@email.com",
                IsSuperAdmin = true
                //Role = 
            };

            await userManager.CreateAsync(user, "Qwerty@123");

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

    }
}
