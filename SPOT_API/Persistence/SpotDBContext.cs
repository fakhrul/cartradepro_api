using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
//using SPOT_API.Authentication;
using SPOT_API.Models;

namespace SPOT_API.Persistence
{
    public class SpotDBContext : IdentityDbContext<AppUser>
    {
        public DbSet<Alarm> Alarms { get; set; }
        public DbSet<Area> Areas { get; set; }
        //public DbSet<Beacon> Beacons { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Emergency> Emergencies { get; set; }
        public DbSet<EmergencyUser> EmergencyUsers { get; set; }
        public DbSet<GeoFenceByMap> GeoFenceByMaps { get; set; }
        public DbSet<GeoFenceCoordItem> GeoFenceCoords { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Performance> Performances { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Map> Maps { get; set; }
        public DbSet<Level> Levels { get; set; }

        //public DbSet<Login> Logins { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
        //public DbSet<TestModel> Test { get; set; }
        public DbSet<SPOT_API.Models.Activity> Activities { get; set; }
        public DbSet<SPOT_API.Models.Document> Documents { get; set; }

        public DbSet<Dashboard> Dashboards { get; set; }

        public SpotDBContext(DbContextOptions<SpotDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<Location>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<Level>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<Area>()
                .HasIndex(x => x.Code)
                .IsUnique();

            modelBuilder.Entity<Profile>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            modelBuilder.Entity<Profile>()
                .HasIndex(x => x.Email)
                .IsUnique();

            //modelBuilder.Entity<Device>()
            //    .HasIndex(x => x.Code)
            //    .IsUnique();

    //        modelBuilder.Entity<DeviceType>()
    //.HasIndex(x => x.Code)
    //.IsUnique();

            modelBuilder.Entity<GeoFenceByArea>()
                  .HasMany(c => c.GeoFenceAreaItemList)
                  .WithOne(e => e.GeoFenceByArea)
                  .HasForeignKey(d => d.GeoFenceByAreaId);

            //    modelBuilder.Entity<GeoFenceByArea>()
            //.HasRequired<GeoFenceAreaItem>(s => s.CurrentGrade)
            //.WithMany(g => g.Students)
            //.HasForeignKey<int>(s => s.CurrentGradeId);

            //        builder.Entity<ActivityAttendee>()
            //.HasOne(u => u.Activity)
            //.WithMany(a => a.Attendees)
            //.HasForeignKey(aa => aa.ActivityId);
            //modelBuilder.Entity<GeoFenceAreaItem>()
            //    .HasOne(u => u.GeoFenceByArea)
            //    .WithMany(a => a.GeoFenceAreaItemList)
            //    .HasForeignKey(aa => aa.AreaId);

            base.OnModelCreating(modelBuilder);

        }

        public DbSet<SPOT_API.Models.FingerPrint> FingerPrint { get; set; }

        public DbSet<SPOT_API.Models.GeoFenceByArea> GeoFenceByAreas { get; set; }
        public DbSet<SPOT_API.Models.GeoFenceAreaItem> GeoFenceAreaItems { get; set; }
        public DbSet<SPOT_API.Models.GeoFenceCoordItem> GeoFenceCoordItems { get; set; }
        public DbSet<SPOT_API.Models.MissingUser> MissingUsers { get; set; }
        public DbSet<SPOT_API.Models.FingerPrintDetail> FingerPrintDetails { get; set; }
        public DbSet<SPOT_API.Models.GeoFenceRuleByArea> GeoFenceRuleByAreas { get; set; }

        public DbSet<SPOT_API.Models.GeoFenceRuleByAreaProfile> GeoFenceRuleByAreaProfiles { get; set; }

        public DbSet<SPOT_API.Models.GeoFenceRuleByMap> GeoFenceRuleByMaps { get; set; }

        public DbSet<SPOT_API.Models.GeoFenceRuleByMapProfile> GeoFenceRuleByMapProfiles { get; set; }


        //public DbSet<SPOT_API.Models.DeviceLog> DeviceLog { get; set; }

    }
}
