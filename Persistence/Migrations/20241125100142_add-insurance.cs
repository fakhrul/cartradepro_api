using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addinsurance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InsuranceAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PuspakomRegRoadTax",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsuranceAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "PuspakomRegRoadTax",
                table: "AdminitrativeCosts");
        }
    }
}
