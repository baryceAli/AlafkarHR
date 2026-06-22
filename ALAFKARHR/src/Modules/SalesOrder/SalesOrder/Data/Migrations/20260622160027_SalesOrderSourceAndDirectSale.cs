using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesOrderSourceAndDirectSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountingDocumentId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPurchaseOrderNumber",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceDocumentId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceDocumentNumber",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceType",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Terms",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ZatcaEInvoiceId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountingDocumentId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "CustomerPurchaseOrderNumber",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "SourceDocumentId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "SourceDocumentNumber",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "SourceType",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "Terms",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "ZatcaEInvoiceId",
                schema: "SalesOrder",
                table: "SalesOrders");
        }
    }
}
