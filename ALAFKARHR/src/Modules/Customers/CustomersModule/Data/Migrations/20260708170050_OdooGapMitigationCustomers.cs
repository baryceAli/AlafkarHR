using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomersModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class OdooGapMitigationCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerPaymentReference",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultCurrencyId",
                schema: "Customer",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FiscalPosition",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IncomeAccountId",
                schema: "Customer",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceivableAccountId",
                schema: "Customer",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                schema: "Customer",
                table: "CustomerContacts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "AddressType",
                schema: "Customer",
                table: "CustomerAddresses",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerPaymentReference",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DefaultCurrencyId",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FiscalPosition",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IncomeAccountId",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ReceivableAccountId",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ContactType",
                schema: "Customer",
                table: "CustomerContacts");

            migrationBuilder.DropColumn(
                name: "AddressType",
                schema: "Customer",
                table: "CustomerAddresses");
        }
    }
}
