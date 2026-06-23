using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Data.Migrations
{
    /// <inheritdoc />
    public partial class BranchAwareAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CashAccounts_CompanyId_DisplayName",
                schema: "Accounting",
                table: "CashAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_CompanyId_DisplayName",
                schema: "Accounting",
                table: "BankAccounts");

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Accounting",
                table: "JournalEntries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Accounting",
                table: "CashAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Accounting",
                table: "BankTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Accounting",
                table: "BankAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Accounting",
                table: "Accounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                schema: "Accounting",
                table: "AccountingJournals",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_CompanyId_BranchId_EntryDate",
                schema: "Accounting",
                table: "JournalEntries",
                columns: new[] { "CompanyId", "BranchId", "EntryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_CashAccounts_CompanyId_BranchId_DisplayName",
                schema: "Accounting",
                table: "CashAccounts",
                columns: new[] { "CompanyId", "BranchId", "DisplayName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankTransactions_CompanyId_BranchId_Status",
                schema: "Accounting",
                table: "BankTransactions",
                columns: new[] { "CompanyId", "BranchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CompanyId_BranchId_DisplayName",
                schema: "Accounting",
                table: "BankAccounts",
                columns: new[] { "CompanyId", "BranchId", "DisplayName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId_BranchId",
                schema: "Accounting",
                table: "Accounts",
                columns: new[] { "CompanyId", "BranchId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingJournals_CompanyId_BranchId_Type",
                schema: "Accounting",
                table: "AccountingJournals",
                columns: new[] { "CompanyId", "BranchId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_CompanyId_BranchId_DocumentDate",
                schema: "Accounting",
                table: "AccountingDocuments",
                columns: new[] { "CompanyId", "BranchId", "DocumentDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_CompanyId_BranchId_EntryDate",
                schema: "Accounting",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_CashAccounts_CompanyId_BranchId_DisplayName",
                schema: "Accounting",
                table: "CashAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankTransactions_CompanyId_BranchId_Status",
                schema: "Accounting",
                table: "BankTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_CompanyId_BranchId_DisplayName",
                schema: "Accounting",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CompanyId_BranchId",
                schema: "Accounting",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountingJournals_CompanyId_BranchId_Type",
                schema: "Accounting",
                table: "AccountingJournals");

            migrationBuilder.DropIndex(
                name: "IX_AccountingDocuments_CompanyId_BranchId_DocumentDate",
                schema: "Accounting",
                table: "AccountingDocuments");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Accounting",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Accounting",
                table: "CashAccounts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Accounting",
                table: "BankTransactions");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Accounting",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Accounting",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "BranchId",
                schema: "Accounting",
                table: "AccountingJournals");

            migrationBuilder.CreateIndex(
                name: "IX_CashAccounts_CompanyId_DisplayName",
                schema: "Accounting",
                table: "CashAccounts",
                columns: new[] { "CompanyId", "DisplayName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CompanyId_DisplayName",
                schema: "Accounting",
                table: "BankAccounts",
                columns: new[] { "CompanyId", "DisplayName" },
                unique: true);
        }
    }
}
