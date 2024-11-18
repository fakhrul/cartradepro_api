using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addsubcompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApCompanyId",
                table: "Stocks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    ContactPersonName = table.Column<string>(type: "text", nullable: true),
                    ContactPersonPhone = table.Column<string>(type: "text", nullable: true),
                    ContactPersonEmail = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCompanies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    AccountName = table.Column<string>(type: "text", nullable: true),
                    AccountNo = table.Column<string>(type: "text", nullable: true),
                    AccountType = table.Column<string>(type: "text", nullable: true),
                    SubCompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_SubCompanies_SubCompanyId",
                        column: x => x.SubCompanyId,
                        principalTable: "SubCompanies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubCompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    BankAccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApCompanies_BankAccounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApCompanies_SubCompanies_SubCompanyId",
                        column: x => x.SubCompanyId,
                        principalTable: "SubCompanies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ApCompanyId",
                table: "Stocks",
                column: "ApCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApCompanies_BankAccountId",
                table: "ApCompanies",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ApCompanies_SubCompanyId",
                table: "ApCompanies",
                column: "SubCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_SubCompanyId",
                table: "BankAccounts",
                column: "SubCompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_ApCompanies_ApCompanyId",
                table: "Stocks",
                column: "ApCompanyId",
                principalTable: "ApCompanies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_ApCompanies_ApCompanyId",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "ApCompanies");

            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "SubCompanies");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ApCompanyId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ApCompanyId",
                table: "Stocks");
        }
    }
}
