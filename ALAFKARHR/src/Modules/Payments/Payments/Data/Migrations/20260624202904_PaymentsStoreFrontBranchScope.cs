using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Data.Migrations
{
    /// <inheritdoc />
    public partial class PaymentsStoreFrontBranchScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Payments",
                table: "PaymentAttempts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PosCashierSessionId",
                schema: "Payments",
                table: "PaymentAttempts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StoreFrontId",
                schema: "Payments",
                table: "PaymentAttempts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_CompanyId_BranchId_CreatedAt",
                schema: "Payments",
                table: "PaymentAttempts",
                columns: new[] { "CompanyId", "BranchId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_StoreFrontId_PosCashierSessionId",
                schema: "Payments",
                table: "PaymentAttempts",
                columns: new[] { "StoreFrontId", "PosCashierSessionId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentAttempts_CompanyId_BranchId_CreatedAt",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropIndex(
                name: "IX_PaymentAttempts_StoreFrontId_PosCashierSessionId",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "PosCashierSessionId",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "StoreFrontId",
                schema: "Payments",
                table: "PaymentAttempts");
        }
    }
}
