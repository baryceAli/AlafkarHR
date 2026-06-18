using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogSkuCapabilityFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAssetTrackable",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInventoryTracked",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPurchasable",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSellable",
                schema: "Catalog",
                table: "ProductSKUs",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAssetTrackable",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "IsInventoryTracked",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "IsPurchasable",
                schema: "Catalog",
                table: "ProductSKUs");

            migrationBuilder.DropColumn(
                name: "IsSellable",
                schema: "Catalog",
                table: "ProductSKUs");
        }
    }
}
