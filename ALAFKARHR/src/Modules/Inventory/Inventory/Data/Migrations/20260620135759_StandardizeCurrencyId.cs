using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Data.Migrations
{
    /// <inheritdoc />
    public partial class StandardizeCurrencyId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Currency",
                schema: "Inventory",
                table: "StockMovements",
                newName: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CurrencyId",
                schema: "Inventory",
                table: "StockMovements",
                column: "CurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockMovements_CurrencyId",
                schema: "Inventory",
                table: "StockMovements");

            migrationBuilder.RenameColumn(
                name: "CurrencyId",
                schema: "Inventory",
                table: "StockMovements",
                newName: "Currency");
        }
    }
}
