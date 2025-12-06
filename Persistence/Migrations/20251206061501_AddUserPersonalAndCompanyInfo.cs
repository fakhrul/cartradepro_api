using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddUserPersonalAndCompanyInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyBankAccount",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IcCopyDocumentId",
                table: "Profiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhotoDocumentId",
                table: "Profiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SubCompanyId",
                table: "Profiles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TinNo",
                table: "Profiles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_IcCopyDocumentId",
                table: "Profiles",
                column: "IcCopyDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_PhotoDocumentId",
                table: "Profiles",
                column: "PhotoDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_SubCompanyId",
                table: "Profiles",
                column: "SubCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Documents_IcCopyDocumentId",
                table: "Profiles",
                column: "IcCopyDocumentId",
                principalTable: "Documents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Documents_PhotoDocumentId",
                table: "Profiles",
                column: "PhotoDocumentId",
                principalTable: "Documents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_SubCompanies_SubCompanyId",
                table: "Profiles",
                column: "SubCompanyId",
                principalTable: "SubCompanies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Documents_IcCopyDocumentId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Documents_PhotoDocumentId",
                table: "Profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_SubCompanies_SubCompanyId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_IcCopyDocumentId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_PhotoDocumentId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_SubCompanyId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "CompanyBankAccount",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "IcCopyDocumentId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "PhotoDocumentId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "SubCompanyId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "TinNo",
                table: "Profiles");
        }
    }
}
