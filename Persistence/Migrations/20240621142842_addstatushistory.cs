using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class addstatushistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockStatusHistory_Profiles_ProfileId",
                table: "StockStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_StockStatusHistory_Stocks_StockId",
                table: "StockStatusHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_StockStatusHistory_StockStatuses_StockStatusId",
                table: "StockStatusHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockStatusHistory",
                table: "StockStatusHistory");

            migrationBuilder.RenameTable(
                name: "StockStatusHistory",
                newName: "StockStatusHistories");

            migrationBuilder.RenameIndex(
                name: "IX_StockStatusHistory_StockStatusId",
                table: "StockStatusHistories",
                newName: "IX_StockStatusHistories_StockStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_StockStatusHistory_StockId",
                table: "StockStatusHistories",
                newName: "IX_StockStatusHistories_StockId");

            migrationBuilder.RenameIndex(
                name: "IX_StockStatusHistory_ProfileId",
                table: "StockStatusHistories",
                newName: "IX_StockStatusHistories_ProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockStatusHistories",
                table: "StockStatusHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockStatusHistories_Profiles_ProfileId",
                table: "StockStatusHistories",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockStatusHistories_Stocks_StockId",
                table: "StockStatusHistories",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockStatusHistories_StockStatuses_StockStatusId",
                table: "StockStatusHistories",
                column: "StockStatusId",
                principalTable: "StockStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockStatusHistories_Profiles_ProfileId",
                table: "StockStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_StockStatusHistories_Stocks_StockId",
                table: "StockStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_StockStatusHistories_StockStatuses_StockStatusId",
                table: "StockStatusHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockStatusHistories",
                table: "StockStatusHistories");

            migrationBuilder.RenameTable(
                name: "StockStatusHistories",
                newName: "StockStatusHistory");

            migrationBuilder.RenameIndex(
                name: "IX_StockStatusHistories_StockStatusId",
                table: "StockStatusHistory",
                newName: "IX_StockStatusHistory_StockStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_StockStatusHistories_StockId",
                table: "StockStatusHistory",
                newName: "IX_StockStatusHistory_StockId");

            migrationBuilder.RenameIndex(
                name: "IX_StockStatusHistories_ProfileId",
                table: "StockStatusHistory",
                newName: "IX_StockStatusHistory_ProfileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockStatusHistory",
                table: "StockStatusHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockStatusHistory_Profiles_ProfileId",
                table: "StockStatusHistory",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockStatusHistory_Stocks_StockId",
                table: "StockStatusHistory",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockStatusHistory_StockStatuses_StockStatusId",
                table: "StockStatusHistory",
                column: "StockStatusId",
                principalTable: "StockStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
