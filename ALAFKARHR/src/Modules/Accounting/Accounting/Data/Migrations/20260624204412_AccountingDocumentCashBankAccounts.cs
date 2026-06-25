using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Data.Migrations
{
    /// <inheritdoc />
    public partial class AccountingDocumentCashBankAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BankAccountId",
                schema: "Accounting",
                table: "AccountingDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CashAccountId",
                schema: "Accounting",
                table: "AccountingDocuments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_CompanyId_BranchId_BankAccountId",
                schema: "Accounting",
                table: "AccountingDocuments",
                columns: new[] { "CompanyId", "BranchId", "BankAccountId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_CompanyId_BranchId_CashAccountId",
                schema: "Accounting",
                table: "AccountingDocuments",
                columns: new[] { "CompanyId", "BranchId", "CashAccountId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountingDocuments_CompanyId_BranchId_BankAccountId",
                schema: "Accounting",
                table: "AccountingDocuments");

            migrationBuilder.DropIndex(
                name: "IX_AccountingDocuments_CompanyId_BranchId_CashAccountId",
                schema: "Accounting",
                table: "AccountingDocuments");

            migrationBuilder.DropColumn(
                name: "BankAccountId",
                schema: "Accounting",
                table: "AccountingDocuments");

            migrationBuilder.DropColumn(
                name: "CashAccountId",
                schema: "Accounting",
                table: "AccountingDocuments");
        }
    }
}
