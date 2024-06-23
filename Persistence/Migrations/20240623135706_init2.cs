using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imports_BillOfLandingDocuments_BillOfLandingDocumentId",
                table: "Imports");

            migrationBuilder.DropIndex(
                name: "IX_Imports_BillOfLandingDocumentId",
                table: "Imports");

            migrationBuilder.DropColumn(
                name: "BillOfLandingDocumentId",
                table: "Imports");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BillOfLandingDocumentId",
                table: "Imports",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imports_BillOfLandingDocumentId",
                table: "Imports",
                column: "BillOfLandingDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imports_BillOfLandingDocuments_BillOfLandingDocumentId",
                table: "Imports",
                column: "BillOfLandingDocumentId",
                principalTable: "BillOfLandingDocuments",
                principalColumn: "Id");
        }
    }
}
