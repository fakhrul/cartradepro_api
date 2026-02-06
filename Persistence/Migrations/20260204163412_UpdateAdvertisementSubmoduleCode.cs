using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class UpdateAdvertisementSubmoduleCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Permissions",
                table: "Roles",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            // Update Advertisement submodule code from "ADVERTISEMENT" to "STOCK_ADVERTISEMENT"
            // Only update the submodule under Stocks module, NOT the standalone Advertisement module
            migrationBuilder.Sql(@"
                UPDATE ""SubModules""
                SET ""Code"" = 'STOCK_ADVERTISEMENT'
                WHERE ""Code"" = 'ADVERTISEMENT'
                AND ""ModuleId"" = (
                    SELECT ""Id"" FROM ""Modules"" WHERE ""Code"" = 'STOCKS'
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Advertisement submodule code back to "ADVERTISEMENT"
            migrationBuilder.Sql(@"
                UPDATE ""SubModules""
                SET ""Code"" = 'ADVERTISEMENT'
                WHERE ""Code"" = 'STOCK_ADVERTISEMENT'
                AND ""ModuleId"" = (
                    SELECT ""Id"" FROM ""Modules"" WHERE ""Code"" = 'STOCKS'
                );
            ");

            migrationBuilder.AlterColumn<string>(
                name: "Permissions",
                table: "Roles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
