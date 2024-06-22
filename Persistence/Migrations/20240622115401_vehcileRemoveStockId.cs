using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class vehcileRemoveStockId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Vehicles_VehicleId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Stocks_StockId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_StockId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "Vehicles");

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleId",
                table: "Stocks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Vehicles_VehicleId",
                table: "Stocks",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Vehicles_VehicleId",
                table: "Stocks");

            migrationBuilder.AddColumn<Guid>(
                name: "StockId",
                table: "Vehicles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleId",
                table: "Stocks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_StockId",
                table: "Vehicles",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Vehicles_VehicleId",
                table: "Stocks",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Stocks_StockId",
                table: "Vehicles",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
