using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class stocktobuy_udpate_photo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehiclePhotos_StockToBuys_StockToBuyId",
                table: "VehiclePhotos");

            migrationBuilder.DropIndex(
                name: "IX_VehiclePhotos_StockToBuyId",
                table: "VehiclePhotos");

            migrationBuilder.DropColumn(
                name: "StockToBuyId",
                table: "VehiclePhotos");

            migrationBuilder.CreateTable(
                name: "StockToBuyPhotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockToBuyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockToBuyPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockToBuyPhotos_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockToBuyPhotos_StockToBuys_StockToBuyId",
                        column: x => x.StockToBuyId,
                        principalTable: "StockToBuys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockToBuyPhotos_DocumentId",
                table: "StockToBuyPhotos",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockToBuyPhotos_StockToBuyId",
                table: "StockToBuyPhotos",
                column: "StockToBuyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockToBuyPhotos");

            migrationBuilder.AddColumn<Guid>(
                name: "StockToBuyId",
                table: "VehiclePhotos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehiclePhotos_StockToBuyId",
                table: "VehiclePhotos",
                column: "StockToBuyId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehiclePhotos_StockToBuys_StockToBuyId",
                table: "VehiclePhotos",
                column: "StockToBuyId",
                principalTable: "StockToBuys",
                principalColumn: "Id");
        }
    }
}
