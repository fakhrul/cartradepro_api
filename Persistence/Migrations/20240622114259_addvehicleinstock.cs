using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addvehicleinstock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VehicleId",
                table: "Stocks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_VehicleId",
                table: "Stocks",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Vehicles_VehicleId",
                table: "Stocks",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Vehicles_VehicleId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_VehicleId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "Stocks");
        }
    }
}
