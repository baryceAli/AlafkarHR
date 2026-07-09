using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesQuotationInvoiceDeliveryAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeliveryAddressId",
                schema: "SalesOrder",
                table: "SalesQuotations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceAddressId",
                schema: "SalesOrder",
                table: "SalesQuotations",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryAddressId",
                schema: "SalesOrder",
                table: "SalesQuotations");

            migrationBuilder.DropColumn(
                name: "InvoiceAddressId",
                schema: "SalesOrder",
                table: "SalesQuotations");
        }
    }
}
