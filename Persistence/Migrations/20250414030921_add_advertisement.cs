using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class add_advertisement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdvertisementId",
                table: "Stocks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Advertisements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MudahHadAdviertized = table.Column<bool>(type: "boolean", nullable: false),
                    MudahStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MudahEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CarListHadAdviertized = table.Column<bool>(type: "boolean", nullable: false),
                    CarListStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CarListEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CariCarzHadAdviertized = table.Column<bool>(type: "boolean", nullable: false),
                    CariCarzStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CariCarzEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_AdvertisementId",
                table: "Stocks",
                column: "AdvertisementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Advertisements_AdvertisementId",
                table: "Stocks",
                column: "AdvertisementId",
                principalTable: "Advertisements",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Advertisements_AdvertisementId",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "Advertisements");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_AdvertisementId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "AdvertisementId",
                table: "Stocks");
        }
    }
}
