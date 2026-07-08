using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuppliersModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class OdooGapMitigationSuppliers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DefaultCurrencyId",
                schema: "Supplier",
                table: "Suppliers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExpenseAccountId",
                schema: "Supplier",
                table: "Suppliers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiscalPosition",
                schema: "Supplier",
                table: "Suppliers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PayableAccountId",
                schema: "Supplier",
                table: "Suppliers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VendorPaymentReference",
                schema: "Supplier",
                table: "Suppliers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                schema: "Supplier",
                table: "SupplierContacts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                schema: "Supplier",
                table: "SupplierAddresses",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultCurrencyId",
                schema: "Supplier",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ExpenseAccountId",
                schema: "Supplier",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "FiscalPosition",
                schema: "Supplier",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "PayableAccountId",
                schema: "Supplier",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "VendorPaymentReference",
                schema: "Supplier",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ContactType",
                schema: "Supplier",
                table: "SupplierContacts");

            migrationBuilder.DropColumn(
                name: "AddressType",
                schema: "Supplier",
                table: "SupplierAddresses");
        }
    }
}
