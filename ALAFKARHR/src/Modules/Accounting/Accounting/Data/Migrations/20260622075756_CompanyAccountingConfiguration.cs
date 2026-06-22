using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompanyAccountingConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceModule",
                schema: "Accounting",
                table: "AccountingDocuments",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountingTemplates",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Visibility = table.Column<int>(type: "int", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    FiscalYearStartMonth = table.Column<int>(type: "int", nullable: false),
                    FiscalYearStartDay = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Iban = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    BranchCode = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Swift = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    LedgerAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JournalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CashAccounts",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    LedgerAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JournalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAccountingSettings",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceivableAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PayableAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RevenueAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExpenseAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CogsAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InventoryAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InputVatAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutputVatAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VatSettlementAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CashAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BankAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RoundingAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SuspenseAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RetainedEarningsAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FiscalYearStartMonth = table.Column<int>(type: "int", nullable: false),
                    FiscalYearStartDay = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAccountingSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountingTemplateAccountLine",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    NormalBalance = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    ParentTemplateKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsPostingAccount = table.Column<bool>(type: "bit", nullable: false),
                    AccountingTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingTemplateAccountLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingTemplateAccountLine_AccountingTemplates_AccountingTemplateId",
                        column: x => x.AccountingTemplateId,
                        principalSchema: "Accounting",
                        principalTable: "AccountingTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountingTemplateJournalLine",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DefaultDebitAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultCreditAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSystemJournal = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AccountingTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingTemplateJournalLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingTemplateJournalLine_AccountingTemplates_AccountingTemplateId",
                        column: x => x.AccountingTemplateId,
                        principalSchema: "Accounting",
                        principalTable: "AccountingTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountingTemplatePostingProfileLine",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ReceivableAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayableAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RevenueAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpenseAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputVatAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputVatAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CashAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankAccountKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    AccountingTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingTemplatePostingProfileLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingTemplatePostingProfileLine_AccountingTemplates_AccountingTemplateId",
                        column: x => x.AccountingTemplateId,
                        principalSchema: "Accounting",
                        principalTable: "AccountingTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountingTemplateTaxCodeLine",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsExempt = table.Column<bool>(type: "bit", nullable: false),
                    ZatcaCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExemptionReasonCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AccountingTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountingTemplateTaxCodeLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountingTemplateTaxCodeLine_AccountingTemplates_AccountingTemplateId",
                        column: x => x.AccountingTemplateId,
                        principalSchema: "Accounting",
                        principalTable: "AccountingTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingDocuments_CompanyId_Type_SourceModule_SourceDocumentId",
                schema: "Accounting",
                table: "AccountingDocuments",
                columns: new[] { "CompanyId", "Type", "SourceModule", "SourceDocumentId" },
                unique: true,
                filter: "[SourceModule] IS NOT NULL AND [SourceDocumentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateAccountLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                column: "AccountingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateJournalLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                column: "AccountingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplatePostingProfileLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                column: "AccountingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateTaxCodeLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                column: "AccountingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CompanyId_DisplayName",
                schema: "Accounting",
                table: "BankAccounts",
                columns: new[] { "CompanyId", "DisplayName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CompanyId_IsDefault",
                schema: "Accounting",
                table: "BankAccounts",
                columns: new[] { "CompanyId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_CashAccounts_CompanyId_DisplayName",
                schema: "Accounting",
                table: "CashAccounts",
                columns: new[] { "CompanyId", "DisplayName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CashAccounts_CompanyId_IsDefault",
                schema: "Accounting",
                table: "CashAccounts",
                columns: new[] { "CompanyId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAccountingSettings_CompanyId",
                schema: "Accounting",
                table: "CompanyAccountingSettings",
                column: "CompanyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingTemplateAccountLine",
                schema: "Accounting");

            migrationBuilder.DropTable(
                name: "AccountingTemplateJournalLine",
                schema: "Accounting");

            migrationBuilder.DropTable(
                name: "AccountingTemplatePostingProfileLine",
                schema: "Accounting");

            migrationBuilder.DropTable(
                name: "AccountingTemplateTaxCodeLine",
                schema: "Accounting");

            migrationBuilder.DropTable(
                name: "BankAccounts",
                schema: "Accounting");

            migrationBuilder.DropTable(
                name: "CashAccounts",
                schema: "Accounting");

            migrationBuilder.DropTable(
                name: "CompanyAccountingSettings",
                schema: "Accounting");

            migrationBuilder.DropTable(
                name: "AccountingTemplates",
                schema: "Accounting");

            migrationBuilder.DropIndex(
                name: "IX_AccountingDocuments_CompanyId_Type_SourceModule_SourceDocumentId",
                schema: "Accounting",
                table: "AccountingDocuments");

            migrationBuilder.DropColumn(
                name: "SourceModule",
                schema: "Accounting",
                table: "AccountingDocuments");
        }
    }
}
