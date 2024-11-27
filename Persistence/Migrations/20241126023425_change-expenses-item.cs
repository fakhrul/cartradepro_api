using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class changeexpensesitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<decimal>(
                name: "InteriorCostAmount",
                table: "Expenses",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaintCostAmount",
                table: "Expenses",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RentalCostAmount",
                table: "Expenses",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceEzCareCostAmount",
                table: "Expenses",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TyreCostAmount",
                table: "Expenses",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InteriorCostAmount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PaintCostAmount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "RentalCostAmount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ServiceEzCareCostAmount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "TyreCostAmount",
                table: "Expenses");

            migrationBuilder.AddColumn<decimal>(
                name: "InteriorCostAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaintCostAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RentalCostAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceEzCareCostAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TyreCostAmount",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
