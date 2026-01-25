using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RevampRolePermissionSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "VehicleTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "VehicleTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "VehicleTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Vehicles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "VehiclePhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "VehiclePhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "VehiclePhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SubModules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SubModules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "SubModules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "SubModules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "SubModules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SubModules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SubCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "SubCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SubCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StockToBuys",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "StockToBuys",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StockToBuys",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StockToBuyPhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "StockToBuyPhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StockToBuyPhotos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StockStatusHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "StockStatusHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StockStatusHistories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StockStatuses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "StockStatuses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StockStatuses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Stocks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Stocks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Stocks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ShowRooms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ShowRooms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ShowRooms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RoleSubModulePermissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "RoleSubModulePermissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "RoleSubModulePermissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemRole",
                table: "Roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RoleModulePermissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "RoleModulePermissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "RoleModulePermissions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RoadTaxDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "RoadTaxDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "RoadTaxDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Remarks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Remarks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Remarks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Registrations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Registrations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Registrations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ReceiptKastamDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ReceiptKastamDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ReceiptKastamDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ReceiptEDaftarDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ReceiptEDaftarDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ReceiptEDaftarDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ReceiptDocument",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ReceiptDocument",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ReceiptDocument",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "PuspakomB7SlipDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "PuspakomB7SlipDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PuspakomB7SlipDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "PuspakomB2SlipDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "PuspakomB2SlipDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PuspakomB2SlipDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Purchases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Purchases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Purchases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeLocation",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProfilePackages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ProfilePackages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProfilePackages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ProfileCommisions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ProfileCommisions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ProfileCommisions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Pricings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Pricings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Pricings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Packages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Packages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Packages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "PackageCommisions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "PackageCommisions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "PackageCommisions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Modules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Modules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Modules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Modules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Modules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Modules",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Models",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Models",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Models",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Loans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Loans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Loans",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "LetterOfUndertakingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "LetterOfUndertakingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "LetterOfUndertakingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "K8Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "K8Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "K8Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "K1Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "K1Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "K1Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "JpjGeranDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "JpjGeranDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "JpjGeranDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "JpjEHakMilikDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "JpjEHakMilikDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "JpjEHakMilikDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "JpjEDaftarDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "JpjEDaftarDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "JpjEDaftarDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InsuranceDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "InsuranceDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "InsuranceDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Imports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Imports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Imports",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ExportCertificateDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ExportCertificateDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ExportCertificateDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Expenses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Expenses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Expenses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ExpenseItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ExpenseItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ExpenseItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Dashboards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Dashboards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Dashboards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CustomerTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CustomerTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CustomerTypes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CustomerIcDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CustomerIcDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CustomerIcDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Completions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Completions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Completions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Clearances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Clearances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Clearances",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ClearanceAgents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ClearanceAgents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ClearanceAgents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Categories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Brands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Brands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Brands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BillOfLandingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "BillOfLandingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "BillOfLandingDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Banks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Banks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Banks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BankAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "BankAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "BankAccounts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ArrivalChecklists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ArrivalChecklists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ArrivalChecklists",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ArrivalChecklistItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ArrivalChecklistItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ArrivalChecklistItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ApplicationForms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationForms",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ApplicationDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ApplicationDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ApplicationDocuments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ApCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "ApCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "ApCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Advertisements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Advertisements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Advertisements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AdminitrativeCosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "AdminitrativeCosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AdminitrativeCosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "AdminitrativeCostItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "AdminitrativeCostItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "AdminitrativeCostItems",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    EventType = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Action = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EntityName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RequestUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RequestMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "integer", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    ErrorCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedBy = table.Column<string>(type: "text", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EffectiveUntil = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EventType",
                table: "AuditLogs",
                column: "EventType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Severity",
                table: "AuditLogs",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_IsActive_EffectiveUntil",
                table: "UserRoles",
                columns: new[] { "IsActive", "EffectiveUntil" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "VehicleTypes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "VehicleTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "VehicleTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "VehiclePhotos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "VehiclePhotos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "VehiclePhotos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SubCompanies");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SubCompanies");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SubCompanies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StockToBuys");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockToBuys");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StockToBuys");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StockToBuyPhotos");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockToBuyPhotos");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StockToBuyPhotos");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StockStatusHistories");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockStatusHistories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StockStatusHistories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StockStatuses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockStatuses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StockStatuses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ShowRooms");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ShowRooms");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ShowRooms");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoleSubModulePermissions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RoleSubModulePermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RoleSubModulePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "IsSystemRole",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoleModulePermissions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RoleModulePermissions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RoleModulePermissions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RoadTaxDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RoadTaxDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RoadTaxDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Remarks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Remarks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Remarks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Registrations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ReceiptKastamDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ReceiptKastamDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ReceiptKastamDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ReceiptEDaftarDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ReceiptEDaftarDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ReceiptEDaftarDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ReceiptDocument");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ReceiptDocument");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ReceiptDocument");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PuspakomB7SlipDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "PuspakomB7SlipDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PuspakomB7SlipDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PuspakomB2SlipDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "PuspakomB2SlipDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PuspakomB2SlipDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "OfficeLocation",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProfilePackages");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ProfilePackages");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProfilePackages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ProfileCommisions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ProfileCommisions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ProfileCommisions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Pricings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PackageCommisions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "PackageCommisions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "PackageCommisions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LetterOfUndertakingDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "LetterOfUndertakingDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LetterOfUndertakingDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "K8Documents");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "K8Documents");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "K8Documents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "K1Documents");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "K1Documents");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "K1Documents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "JpjGeranDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "JpjGeranDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "JpjGeranDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "JpjEHakMilikDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "JpjEHakMilikDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "JpjEHakMilikDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "JpjEDaftarDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "JpjEDaftarDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "JpjEDaftarDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InsuranceDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "InsuranceDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "InsuranceDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Imports");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Imports");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Imports");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ExportCertificateDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ExportCertificateDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ExportCertificateDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ExpenseItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ExpenseItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ExpenseItems");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CustomerTypes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CustomerTypes");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CustomerTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CustomerIcDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CustomerIcDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CustomerIcDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Completions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Completions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Completions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Clearances");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Clearances");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Clearances");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ClearanceAgents");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ClearanceAgents");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ClearanceAgents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BillOfLandingDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "BillOfLandingDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "BillOfLandingDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ArrivalChecklists");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ArrivalChecklists");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ArrivalChecklists");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ArrivalChecklistItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ArrivalChecklistItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ArrivalChecklistItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ApplicationForms");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ApplicationForms");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApplicationDocuments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ApplicationDocuments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ApplicationDocuments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ApCompanies");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ApCompanies");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "ApCompanies");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "AdminitrativeCostItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "AdminitrativeCostItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "AdminitrativeCostItems");
        }
    }
}
