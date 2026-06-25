using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Data.Migrations
{
    /// <inheritdoc />
    public partial class PaymentsCashBankAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BankAccountId",
                schema: "Payments",
                table: "PaymentAttempts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CashAccountId",
                schema: "Payments",
                table: "PaymentAttempts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_CompanyId_BranchId_CashAccountId",
                schema: "Payments",
                table: "PaymentAttempts",
                columns: new[] { "CompanyId", "BranchId", "CashAccountId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentAttempts_CompanyId_BranchId_CashAccountId",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "CashAccountId",
                schema: "Payments",
                table: "PaymentAttempts");
        }
    }
}
