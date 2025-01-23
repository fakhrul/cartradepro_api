using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class add_show_room : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ShowRoomId",
                table: "Stocks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShowRooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LotNo = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    ContactPersonName = table.Column<string>(type: "text", nullable: true),
                    ContactPersonPhone = table.Column<string>(type: "text", nullable: true),
                    ContactPersonEmail = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShowRooms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ShowRoomId",
                table: "Stocks",
                column: "ShowRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_ShowRooms_ShowRoomId",
                table: "Stocks",
                column: "ShowRoomId",
                principalTable: "ShowRooms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_ShowRooms_ShowRoomId",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "ShowRooms");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ShowRoomId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ShowRoomId",
                table: "Stocks");
        }
    }
}
