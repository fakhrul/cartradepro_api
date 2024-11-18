using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class updatesubcompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactPersonEmail",
                table: "SubCompanies");

            migrationBuilder.DropColumn(
                name: "ContactPersonName",
                table: "SubCompanies");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "BankAccounts");

            migrationBuilder.RenameColumn(
                name: "Website",
                table: "SubCompanies",
                newName: "TagLine");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "SubCompanies",
                newName: "RegNo");

            migrationBuilder.RenameColumn(
                name: "ContactPersonPhone",
                table: "SubCompanies",
                newName: "LogoUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TagLine",
                table: "SubCompanies",
                newName: "Website");

            migrationBuilder.RenameColumn(
                name: "RegNo",
                table: "SubCompanies",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "LogoUrl",
                table: "SubCompanies",
                newName: "ContactPersonPhone");

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonEmail",
                table: "SubCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPersonName",
                table: "SubCompanies",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "BankAccounts",
                type: "text",
                nullable: true);
        }
    }
}
