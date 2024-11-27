using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addfieldsinadministrative : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ApAmount",
                table: "Purchases",
                newName: "BodyPriceLocalCurrency");

            migrationBuilder.AddColumn<float>(
                name: "ApCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "DutyCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ForwardingCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "IntFsCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "InteriorCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PaintCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "RentalCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ServiceEzCareCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TyreCostAmount",
                table: "AdminitrativeCosts",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "DutyCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "ForwardingCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "IntFsCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "InteriorCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "PaintCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "RentalCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "ServiceEzCareCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.DropColumn(
                name: "TyreCostAmount",
                table: "AdminitrativeCosts");

            migrationBuilder.RenameColumn(
                name: "BodyPriceLocalCurrency",
                table: "Purchases",
                newName: "ApAmount");
        }
    }
}
