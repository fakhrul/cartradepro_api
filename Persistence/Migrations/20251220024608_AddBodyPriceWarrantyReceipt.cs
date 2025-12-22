using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddBodyPriceWarrantyReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Warranty",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BodyPrice",
                table: "AdminitrativeCosts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ReceiptDocument",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptDocument_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptDocument_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDocument_DocumentId",
                table: "ReceiptDocument",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptDocument_SaleId",
                table: "ReceiptDocument",
                column: "SaleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptDocument");

            migrationBuilder.DropColumn(
                name: "Warranty",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "BodyPrice",
                table: "AdminitrativeCosts");
        }
    }
}
