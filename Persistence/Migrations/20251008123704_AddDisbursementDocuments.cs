using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class AddDisbursementDocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerIcDocuments",
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
                    table.PrimaryKey("PK_CustomerIcDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerIcDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerIcDocuments_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExportCertificateDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClearanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportCertificateDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportCertificateDocuments_Clearances_ClearanceId",
                        column: x => x.ClearanceId,
                        principalTable: "Clearances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExportCertificateDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsuranceDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuranceDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InsuranceDocuments_Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptEDaftarDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptEDaftarDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptEDaftarDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptEDaftarDocuments_Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptKastamDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptKastamDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptKastamDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptKastamDocuments_Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoadTaxDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoadTaxDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoadTaxDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoadTaxDocuments_Registrations_RegistrationId",
                        column: x => x.RegistrationId,
                        principalTable: "Registrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIcDocuments_DocumentId",
                table: "CustomerIcDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerIcDocuments_SaleId",
                table: "CustomerIcDocuments",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportCertificateDocuments_ClearanceId",
                table: "ExportCertificateDocuments",
                column: "ClearanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportCertificateDocuments_DocumentId",
                table: "ExportCertificateDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDocuments_DocumentId",
                table: "InsuranceDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceDocuments_RegistrationId",
                table: "InsuranceDocuments",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptEDaftarDocuments_DocumentId",
                table: "ReceiptEDaftarDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptEDaftarDocuments_RegistrationId",
                table: "ReceiptEDaftarDocuments",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptKastamDocuments_DocumentId",
                table: "ReceiptKastamDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptKastamDocuments_RegistrationId",
                table: "ReceiptKastamDocuments",
                column: "RegistrationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadTaxDocuments_DocumentId",
                table: "RoadTaxDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoadTaxDocuments_RegistrationId",
                table: "RoadTaxDocuments",
                column: "RegistrationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerIcDocuments");

            migrationBuilder.DropTable(
                name: "ExportCertificateDocuments");

            migrationBuilder.DropTable(
                name: "InsuranceDocuments");

            migrationBuilder.DropTable(
                name: "ReceiptEDaftarDocuments");

            migrationBuilder.DropTable(
                name: "ReceiptKastamDocuments");

            migrationBuilder.DropTable(
                name: "RoadTaxDocuments");
        }
    }
}
