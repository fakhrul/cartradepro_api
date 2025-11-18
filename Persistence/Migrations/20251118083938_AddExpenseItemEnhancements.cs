using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddExpenseItemEnhancements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillOrInvoiceNo",
                table: "ExpenseItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ExpenseItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "ExpenseItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpenseDate",
                table: "ExpenseItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseItems_DocumentId",
                table: "ExpenseItems",
                column: "DocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseItems_Documents_DocumentId",
                table: "ExpenseItems",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseItems_Documents_DocumentId",
                table: "ExpenseItems");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseItems_DocumentId",
                table: "ExpenseItems");

            migrationBuilder.DropColumn(
                name: "BillOrInvoiceNo",
                table: "ExpenseItems");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ExpenseItems");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "ExpenseItems");

            migrationBuilder.DropColumn(
                name: "ExpenseDate",
                table: "ExpenseItems");
        }
    }
}
