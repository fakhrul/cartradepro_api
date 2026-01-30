using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class FixSubModuleDisplayOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update DisplayOrder for Stock submodules using Name
            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 1
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Stock Info'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 2
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Vehicle Info'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 3
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Purchase'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 4
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Import'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 5
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Clearance'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 6
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Sale'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 7
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Pricing'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 8
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Arrival Checklist'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 9
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Registration'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 10
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Expenses'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 11
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Administrative Cost'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 12
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Disbursement'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 13
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Stocks'
                AND s.""Name"" = 'Advertisement'");

            // Update DisplayOrder for MasterData submodules
            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 1
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'MasterData'
                AND s.""Name"" = 'Suppliers'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 2
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'MasterData'
                AND s.""Name"" = 'Forwarders'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 3
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'MasterData'
                AND s.""Name"" = 'Banks'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 4
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'MasterData'
                AND s.""Name"" = 'Brands'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 5
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'MasterData'
                AND s.""Name"" = 'Sub Companies'");

            // Update DisplayOrder for Administration submodules
            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 1
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Administration'
                AND s.""Name"" = 'Roles'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 2
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Administration'
                AND s.""Name"" = 'User'");

            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 4
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Administration'
                AND s.""Name"" = 'Company'");

            // Update DisplayOrder for Reports submodules
            migrationBuilder.Sql(@"
                UPDATE ""SubModules"" s
                SET ""DisplayOrder"" = 1
                FROM ""Modules"" m
                WHERE s.""ModuleId"" = m.""Id""
                AND m.""Name"" = 'Reports'
                AND s.""Name"" = 'Sales Report'");

            // Create Audit Logs submodule if it doesn't exist
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    admin_module_id uuid;
                BEGIN
                    -- Get the Administration module ID
                    SELECT ""Id"" INTO admin_module_id FROM ""Modules"" WHERE ""Name"" = 'Administration';

                    -- Insert Audit Logs if it doesn't exist
                    IF NOT EXISTS (SELECT 1 FROM ""SubModules"" s WHERE s.""Name"" = 'Audit Logs' AND s.""ModuleId"" = admin_module_id) THEN
                        INSERT INTO ""SubModules"" (""Id"", ""Name"", ""ModuleId"", ""DisplayOrder"", ""CreatedBy"", ""CreatedDate"")
                        VALUES (gen_random_uuid(), 'Audit Logs', admin_module_id, 3, 'System', NOW());
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
