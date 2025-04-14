using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class stocktobuy_change_to_SubCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockToBuys_ApCompanies_ApCompanyId",
                table: "StockToBuys");

            migrationBuilder.RenameColumn(
                name: "ApCompanyId",
                table: "StockToBuys",
                newName: "SubCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_StockToBuys_ApCompanyId",
                table: "StockToBuys",
                newName: "IX_StockToBuys_SubCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockToBuys_SubCompanies_SubCompanyId",
                table: "StockToBuys",
                column: "SubCompanyId",
                principalTable: "SubCompanies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockToBuys_SubCompanies_SubCompanyId",
                table: "StockToBuys");

            migrationBuilder.RenameColumn(
                name: "SubCompanyId",
                table: "StockToBuys",
                newName: "ApCompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_StockToBuys_SubCompanyId",
                table: "StockToBuys",
                newName: "IX_StockToBuys_ApCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockToBuys_ApCompanies_ApCompanyId",
                table: "StockToBuys",
                column: "ApCompanyId",
                principalTable: "ApCompanies",
                principalColumn: "Id");
        }
    }
}
