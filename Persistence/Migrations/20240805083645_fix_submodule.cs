using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class fix_submodule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleModulePermissions_SubModules_SubModuleId",
                table: "RoleModulePermissions");

            migrationBuilder.DropIndex(
                name: "IX_RoleModulePermissions_SubModuleId",
                table: "RoleModulePermissions");

            migrationBuilder.DropColumn(
                name: "SubModuleId",
                table: "RoleModulePermissions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubModuleId",
                table: "RoleModulePermissions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleModulePermissions_SubModuleId",
                table: "RoleModulePermissions",
                column: "SubModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModulePermissions_SubModules_SubModuleId",
                table: "RoleModulePermissions",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id");
        }
    }
}
