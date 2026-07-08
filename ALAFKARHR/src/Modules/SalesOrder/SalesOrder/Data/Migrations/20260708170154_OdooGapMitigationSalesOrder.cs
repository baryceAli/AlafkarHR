using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Data.Migrations
{
    /// <inheritdoc />
    public partial class OdooGapMitigationSalesOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DownPaymentAmount",
                schema: "SalesOrder",
                table: "SalesQuotations",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPaymentPercent",
                schema: "SalesOrder",
                table: "SalesQuotations",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsProForma",
                schema: "SalesOrder",
                table: "SalesQuotations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "QuotationTemplateId",
                schema: "SalesOrder",
                table: "SalesQuotations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresCustomerSignature",
                schema: "SalesOrder",
                table: "SalesQuotations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresOnlinePayment",
                schema: "SalesOrder",
                table: "SalesQuotations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptional",
                schema: "SalesOrder",
                table: "SalesQuotationLines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryAddressId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPaymentAmount",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DownPaymentPercent",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceAddressId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsProForma",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "QuotationTemplateId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresCustomerSignature",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresOnlinePayment",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownPaymentAmount",
                schema: "SalesOrder",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "DownPaymentPercent",
                schema: "SalesOrder",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "IsProForma",
                schema: "SalesOrder",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "QuotationTemplateId",
                schema: "SalesOrder",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "RequiresCustomerSignature",
                schema: "SalesOrder",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "RequiresOnlinePayment",
                schema: "SalesOrder",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "IsOptional",
                schema: "SalesOrder",
                table: "SalesQuotationLines");

            migrationBuilder.DropColumn(
                name: "DeliveryAddressId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "DownPaymentAmount",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "DownPaymentPercent",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "InvoiceAddressId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "IsProForma",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "QuotationTemplateId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "RequiresCustomerSignature",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "RequiresOnlinePayment",
                schema: "SalesOrder",
                table: "SalesOrders");
        }
    }
}
