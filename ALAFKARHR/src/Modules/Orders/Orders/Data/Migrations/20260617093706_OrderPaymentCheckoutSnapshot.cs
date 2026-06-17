using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderPaymentCheckoutSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CheckoutTotal",
                schema: "Orders",
                table: "OrderIntakes",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentApprovedAt",
                schema: "Orders",
                table: "OrderIntakes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentDecisionReason",
                schema: "Orders",
                table: "OrderIntakes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                schema: "Orders",
                table: "OrderIntakes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                schema: "Orders",
                table: "OrderIntakes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                schema: "Orders",
                table: "OrderIntakes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedDiscountRate",
                schema: "Orders",
                table: "OrderIntakeLines",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckoutTotal",
                schema: "Orders",
                table: "OrderIntakes");

            migrationBuilder.DropColumn(
                name: "PaymentApprovedAt",
                schema: "Orders",
                table: "OrderIntakes");

            migrationBuilder.DropColumn(
                name: "PaymentDecisionReason",
                schema: "Orders",
                table: "OrderIntakes");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                schema: "Orders",
                table: "OrderIntakes");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                schema: "Orders",
                table: "OrderIntakes");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                schema: "Orders",
                table: "OrderIntakes");

            migrationBuilder.DropColumn(
                name: "RequestedDiscountRate",
                schema: "Orders",
                table: "OrderIntakeLines");
        }
    }
}
