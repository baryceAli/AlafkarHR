using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cart.Data.Migrations
{
    /// <inheritdoc />
    public partial class CartStoreFrontBranchScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Cart",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PosCashierSessionId",
                schema: "Cart",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreFrontId",
                schema: "Cart",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CompanyId_BranchId",
                schema: "Cart",
                table: "Carts",
                columns: new[] { "CompanyId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_StoreFrontId_PosCashierSessionId",
                schema: "Cart",
                table: "Carts",
                columns: new[] { "StoreFrontId", "PosCashierSessionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_CompanyId_BranchId",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_StoreFrontId_PosCashierSessionId",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "PosCashierSessionId",
                schema: "Cart",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "StoreFrontId",
                schema: "Cart",
                table: "Carts");
        }
    }
}
