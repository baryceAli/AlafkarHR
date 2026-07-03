using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogPolishArchiveGovernance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "VariantValues",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "Variants",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "Units",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                schema: "Catalog",
                table: "ProductPackages",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "ProductPackages",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                schema: "Catalog",
                table: "ProductPackages",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "Catalog",
                table: "ProductPackages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                schema: "Catalog",
                table: "ProductPackages",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                schema: "Catalog",
                table: "ProductPackages",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "Categories",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Catalog",
                table: "Brands",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "VariantValues");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "Variants");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Height",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "Length",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "Weight",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "Width",
                schema: "Catalog",
                table: "ProductPackages");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Catalog",
                table: "Brands");
        }
    }
}
