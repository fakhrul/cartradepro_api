using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddPermissionsColumnToRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Permissions column as JSONB
            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                table: "Roles",
                type: "jsonb",
                nullable: true);

            // Data migration: Convert existing RoleModulePermissions and RoleSubModulePermissions to JSON
            migrationBuilder.Sql(@"
                UPDATE ""Roles"" r
                SET ""Permissions"" = (
                    SELECT jsonb_build_object(
                        'Modules', (
                            SELECT jsonb_object_agg(
                                m.""Name"",
                                jsonb_build_object(
                                    'CanAdd', COALESCE(rmp.""CanAdd"", false),
                                    'CanUpdate', COALESCE(rmp.""CanUpdate"", false),
                                    'CanDelete', COALESCE(rmp.""CanDelete"", false),
                                    'CanView', COALESCE(rmp.""CanView"", false),
                                    'SubModules', (
                                        SELECT jsonb_object_agg(
                                            sm.""Name"",
                                            jsonb_build_object(
                                                'CanAdd', rsmp.""CanAdd"",
                                                'CanUpdate', rsmp.""CanUpdate"",
                                                'CanDelete', rsmp.""CanDelete"",
                                                'CanView', rsmp.""CanView""
                                            )
                                        )
                                        FROM ""RoleSubModulePermissions"" rsmp
                                        JOIN ""SubModules"" sm ON rsmp.""SubModuleId"" = sm.""Id""
                                        WHERE rsmp.""RoleId"" = r.""Id""
                                            AND sm.""ModuleId"" = m.""Id""
                                    )
                                )
                            )
                            FROM ""RoleModulePermissions"" rmp
                            JOIN ""Modules"" m ON rmp.""ModuleId"" = m.""Id""
                            WHERE rmp.""RoleId"" = r.""Id""
                        )
                    )::jsonb
                )
                WHERE EXISTS (
                    SELECT 1 FROM ""RoleModulePermissions"" WHERE ""RoleId"" = r.""Id""
                );
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the Permissions column
            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "Roles");
        }
    }
}
