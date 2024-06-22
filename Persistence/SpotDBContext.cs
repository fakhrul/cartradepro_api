using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SPOT_API.Models;

namespace SPOT_API.Persistence
{
    public class SpotDBContext : IdentityDbContext<AppUser>
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<StockStatus> StockStatuses { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Model> Models { get; set; }

        public DbSet<VehiclePhoto> VehiclePhotos { get; set; }

        ///

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ForwardingAgent> ClearanceAgents { get; set; }

        public DbSet<Import> Imports { get; set; }
        public DbSet<BillOfLandingDocument> BillOfLandingDocuments { get; set; }
        public DbSet<LetterOfUndertakingDocument> LetterOfUndertakingDocuments { get; set; }

        public DbSet<Clearance> Clearances { get; set; }
        public DbSet<K8Document> K8Documents { get; set; }
        public DbSet<K1Document> K1Documents { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SellingPricing> SellingPricings { get; set; }
        public DbSet<ArrivalChecklist> ArrivalChecklists { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        public DbSet<JpjEHakMilikDocument> JpjEHakMilikDocuments { get; set; }
        public DbSet<JpjEDaftarDocument> JpjEDaftarDocuments { get; set; }
        public DbSet<PuspakomB2SlipDocument> PuspakomB2SlipDocuments { get; set; }
        public DbSet<PuspakomB7SlipDocument> PuspakomB7SlipDocuments { get; set; }

        public DbSet<Completion> Completions { get; set; }

        public DbSet<AdminitrativeCost> AdminitrativeCosts { get; set; }

        ///
        public DbSet<Profile> Profiles { get; set; }
       
        public DbSet<Document> Documents { get; set; }

        public DbSet<Dashboard> Dashboards { get; set; }
        //public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<ApplicationForm> ApplicationForms { get; set; }
        public DbSet<PackageCommision> PackageCommisions { get; set; }
        public DbSet<ProfileCommision> ProfileCommisions { get; set; }
        public DbSet<ApplicationDocument> ApplicationDocuments { get; set; }
        public DbSet<ProfilePackage> ProfilePackages { get; set; }
        public DbSet<Remarks> Remarks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        public DbSet<StockStatusHistory> StockStatusHistories { get; set; }
        
        public SpotDBContext(DbContextOptions<SpotDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<Profile>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            modelBuilder.Entity<Profile>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Stock>()
                .HasIndex(p => p.StockNo)
                .IsUnique();


            base.OnModelCreating(modelBuilder);

        }



    }
}
