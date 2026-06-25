using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesOrderBranchScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PosCashierSessionId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreFrontId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "PosCashierSessionId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "StoreFrontId",
                schema: "SalesOrder",
                table: "SalesOrders");
        }
    }
}
