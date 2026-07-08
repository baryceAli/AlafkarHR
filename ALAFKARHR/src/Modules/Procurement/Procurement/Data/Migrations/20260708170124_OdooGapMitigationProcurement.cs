using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Procurement.Data.Migrations
{
    /// <inheritdoc />
    public partial class OdooGapMitigationProcurement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BillControlPolicy",
                schema: "Procurement",
                table: "ProcurementDocuments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "BlanketOrderId",
                schema: "Procurement",
                table: "ProcurementDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBillable",
                schema: "Procurement",
                table: "ProcurementDocuments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseTemplateId",
                schema: "Procurement",
                table: "ProcurementDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                schema: "Procurement",
                table: "ProcurementDocuments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SentBy",
                schema: "Procurement",
                table: "ProcurementDocuments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenderId",
                schema: "Procurement",
                table: "ProcurementDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThreeWayMatchStatus",
                schema: "Procurement",
                table: "ProcurementDocuments",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "BillControlPolicy",
                schema: "Procurement",
                table: "ProcurementDocumentLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BilledQuantity",
                schema: "Procurement",
                table: "ProcurementDocumentLines",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceivedQuantity",
                schema: "Procurement",
                table: "ProcurementDocumentLines",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ThreeWayMatchStatus",
                schema: "Procurement",
                table: "ProcurementDocumentLines",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillControlPolicy",
                schema: "Procurement",
                table: "ProcurementDocuments");

            migrationBuilder.DropColumn(
                name: "BlanketOrderId",
                schema: "Procurement",
                table: "ProcurementDocuments");

            migrationBuilder.DropColumn(
                name: "IsBillable",
                schema: "Procurement",
                table: "ProcurementDocuments");

            migrationBuilder.DropColumn(
                name: "PurchaseTemplateId",
                schema: "Procurement",
                table: "ProcurementDocuments");

            migrationBuilder.DropColumn(
                name: "SentAt",
                schema: "Procurement",
                table: "ProcurementDocuments");

            migrationBuilder.DropColumn(
                name: "SentBy",
                schema: "Procurement",
                table: "ProcurementDocuments");

            migrationBuilder.DropColumn(
                name: "TenderId",
                schema: "Procurement",
                table: "ProcurementDocuments");

            migrationBuilder.DropColumn(
                name: "ThreeWayMatchStatus",
                schema: "Procurement",
                table: "ProcurementDocuments");

            migrationBuilder.DropColumn(
                name: "BillControlPolicy",
                schema: "Procurement",
                table: "ProcurementDocumentLines");

            migrationBuilder.DropColumn(
                name: "BilledQuantity",
                schema: "Procurement",
                table: "ProcurementDocumentLines");

            migrationBuilder.DropColumn(
                name: "ReceivedQuantity",
                schema: "Procurement",
                table: "ProcurementDocumentLines");

            migrationBuilder.DropColumn(
                name: "ThreeWayMatchStatus",
                schema: "Procurement",
                table: "ProcurementDocumentLines");
        }
    }
}
