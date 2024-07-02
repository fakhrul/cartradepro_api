using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleModulePermissions_SubModules_SubModuleModuleId",
                table: "RoleModulePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleSubModulePermissions_SubModules_SubModuleId",
                table: "RoleSubModulePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubModules",
                table: "SubModules");

            migrationBuilder.RenameColumn(
                name: "SubModuleModuleId",
                table: "RoleModulePermissions",
                newName: "SubModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleModulePermissions_SubModuleModuleId",
                table: "RoleModulePermissions",
                newName: "IX_RoleModulePermissions_SubModuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubModules",
                table: "SubModules",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubModules_ModuleId",
                table: "SubModules",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModulePermissions_SubModules_SubModuleId",
                table: "RoleModulePermissions",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleSubModulePermissions_SubModules_SubModuleId",
                table: "RoleSubModulePermissions",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleModulePermissions_SubModules_SubModuleId",
                table: "RoleModulePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleSubModulePermissions_SubModules_SubModuleId",
                table: "RoleSubModulePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubModules",
                table: "SubModules");

            migrationBuilder.DropIndex(
                name: "IX_SubModules_ModuleId",
                table: "SubModules");

            migrationBuilder.RenameColumn(
                name: "SubModuleId",
                table: "RoleModulePermissions",
                newName: "SubModuleModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleModulePermissions_SubModuleId",
                table: "RoleModulePermissions",
                newName: "IX_RoleModulePermissions_SubModuleModuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubModules",
                table: "SubModules",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleModulePermissions_SubModules_SubModuleModuleId",
                table: "RoleModulePermissions",
                column: "SubModuleModuleId",
                principalTable: "SubModules",
                principalColumn: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleSubModulePermissions_SubModules_SubModuleId",
                table: "RoleSubModulePermissions",
                column: "SubModuleId",
                principalTable: "SubModules",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
