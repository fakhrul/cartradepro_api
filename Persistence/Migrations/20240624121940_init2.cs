using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "ExpenseItems",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "Remark",
                table: "AdminitrativeCostItems",
                newName: "Remarks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "ExpenseItems",
                newName: "Remark");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "AdminitrativeCostItems",
                newName: "Remark");
        }
    }
}
