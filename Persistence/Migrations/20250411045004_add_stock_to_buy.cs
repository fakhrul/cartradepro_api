using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class add_stock_to_buy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StockToBuyId",
                table: "VehiclePhotos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StockToBuys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockNo = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ApCompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    SupplierCurrency = table.Column<string>(type: "text", nullable: true),
                    VehiclePriceSupplierCurrency = table.Column<decimal>(type: "numeric", nullable: false),
                    VehiclePriceLocalCurrency = table.Column<decimal>(type: "numeric", nullable: false),
                    BodyPriceLocalCurrency = table.Column<decimal>(type: "numeric", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockToBuys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockToBuys_ApCompanies_ApCompanyId",
                        column: x => x.ApCompanyId,
                        principalTable: "ApCompanies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockToBuys_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehiclePhotos_StockToBuyId",
                table: "VehiclePhotos",
                column: "StockToBuyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockToBuys_ApCompanyId",
                table: "StockToBuys",
                column: "ApCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockToBuys_SupplierId",
                table: "StockToBuys",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehiclePhotos_StockToBuys_StockToBuyId",
                table: "VehiclePhotos",
                column: "StockToBuyId",
                principalTable: "StockToBuys",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehiclePhotos_StockToBuys_StockToBuyId",
                table: "VehiclePhotos");

            migrationBuilder.DropTable(
                name: "StockToBuys");

            migrationBuilder.DropIndex(
                name: "IX_VehiclePhotos_StockToBuyId",
                table: "VehiclePhotos");

            migrationBuilder.DropColumn(
                name: "StockToBuyId",
                table: "VehiclePhotos");
        }
    }
}
