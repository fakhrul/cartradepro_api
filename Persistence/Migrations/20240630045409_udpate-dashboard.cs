using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class udpatedashboard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalActiveUser",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "TotalAllUser",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "TotalHeadCount",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "TotalInActiveUser",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "TotalMissingUser",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "TotalPendingApproval",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "TotalRegisteredStaff",
                table: "Dashboards");

            migrationBuilder.DropColumn(
                name: "TotalRegisteredVisitor",
                table: "Dashboards");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalActiveUser",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalAllUser",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalHeadCount",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalInActiveUser",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalMissingUser",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalPendingApproval",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRegisteredStaff",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRegisteredVisitor",
                table: "Dashboards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
