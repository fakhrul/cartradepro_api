using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class updatesubcompanyrelationtobankaccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_SubCompanies_SubCompanyId",
                table: "BankAccounts");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_SubCompanies_SubCompanyId",
                table: "BankAccounts",
                column: "SubCompanyId",
                principalTable: "SubCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_SubCompanies_SubCompanyId",
                table: "BankAccounts");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_SubCompanies_SubCompanyId",
                table: "BankAccounts",
                column: "SubCompanyId",
                principalTable: "SubCompanies",
                principalColumn: "Id");
        }
    }
}
