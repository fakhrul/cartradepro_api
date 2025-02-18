using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class update_stock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocalImageAvailable",
                table: "Vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSupplierImageAvailable",
                table: "Vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SalesManId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SalesManId",
                table: "Sales",
                column: "SalesManId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Profiles_SalesManId",
                table: "Sales",
                column: "SalesManId",
                principalTable: "Profiles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Profiles_SalesManId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_SalesManId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsLocalImageAvailable",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsSupplierImageAvailable",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SalesManId",
                table: "Sales");
        }
    }
}
