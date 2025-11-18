using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class SeparateAdministrativeCostFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PuspakomAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RegistrationFeeAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RoadtaxAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TransferFeeAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PuspakomAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "RegistrationFeeAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "RoadtaxAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "TransferFeeAmount",
                table: "AdminitrativeCosts");
        }
    }
}
