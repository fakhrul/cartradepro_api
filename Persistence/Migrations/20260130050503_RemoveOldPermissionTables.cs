using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class RemoveOldPermissionTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old permission junction tables
            migrationBuilder.DropTable(name: "RoleSubModulePermissions");
            migrationBuilder.DropTable(name: "RoleModulePermissions");

            // Drop the old module/submodule tables (now hardcoded in controller)
            migrationBuilder.DropTable(name: "SubModules");
            migrationBuilder.DropTable(name: "Modules");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration cannot be reversed as it would require recreating
            // the old table structures and migrating data back from JSONB.
            // If rollback is needed, restore from database backup.
            throw new NotSupportedException(
                "This migration cannot be reversed. " +
                "To rollback, restore from database backup taken before migration.");
        }
    }
}
