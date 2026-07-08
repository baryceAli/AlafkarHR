using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class OdooGapMitigationCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlertTimeDays",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GalleryImageUrls",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemovalTimeDays",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShelfLifeDays",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CostingPolicy",
                schema: "Catalog",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerTaxRate",
                schema: "Catalog",
                table: "Products",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ExpenseAccountId",
                schema: "Catalog",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "Catalog",
                table: "Products",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IncomeAccountId",
                schema: "Catalog",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseDescription",
                schema: "Catalog",
                table: "Products",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesDescription",
                schema: "Catalog",
                table: "Products",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VendorTaxRate",
                schema: "Catalog",
                table: "Products",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertTimeDays",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "GalleryImageUrls",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "RemovalTimeDays",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "ShelfLifeDays",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "CostingPolicy",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CustomerTaxRate",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ExpenseAccountId",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IncomeAccountId",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PurchaseDescription",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalesDescription",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VendorTaxRate",
                schema: "Catalog",
                table: "Products");
        }
    }
}
