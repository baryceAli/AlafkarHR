using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomAccountingTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingTemplateAccountLine_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingTemplateJournalLine_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingTemplatePostingProfileLine_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingTemplateTaxCodeLine_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountingTemplateTaxCodeLine",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplateTaxCodeLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountingTemplatePostingProfileLine",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplatePostingProfileLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountingTemplateJournalLine",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplateJournalLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountingTemplateAccountLine",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplateAccountLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine");

            migrationBuilder.RenameTable(
                name: "AccountingTemplateTaxCodeLine",
                schema: "Accounting",
                newName: "AccountingTemplateTaxCodes",
                newSchema: "Accounting");

            migrationBuilder.RenameTable(
                name: "AccountingTemplatePostingProfileLine",
                schema: "Accounting",
                newName: "AccountingTemplatePostingProfiles",
                newSchema: "Accounting");

            migrationBuilder.RenameTable(
                name: "AccountingTemplateJournalLine",
                schema: "Accounting",
                newName: "AccountingTemplateJournals",
                newSchema: "Accounting");

            migrationBuilder.RenameTable(
                name: "AccountingTemplateAccountLine",
                schema: "Accounting",
                newName: "AccountingTemplateAccounts",
                newSchema: "Accounting");

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ZatcaCategoryCode",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                type: "decimal(9,4)",
                precision: 9,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ExemptionReasonCode",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RevenueAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReceivableAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PayableAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OutputVatAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "InputVatAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ExpenseAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CashAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BankAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultDebitAccountKey",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DefaultCreditAccountKey",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TemplateKey",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ParentTemplateKey",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameEng",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountingTemplateTaxCodes",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountingTemplatePostingProfiles",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountingTemplateJournals",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountingTemplateAccounts",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplates_CompanyId_Code",
                schema: "Accounting",
                table: "AccountingTemplates",
                columns: new[] { "CompanyId", "Code" },
                unique: true,
                filter: "[CompanyId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateTaxCodes_AccountingTemplateId_Code",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                columns: new[] { "AccountingTemplateId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplatePostingProfiles_AccountingTemplateId_Type",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                columns: new[] { "AccountingTemplateId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateJournals_AccountingTemplateId_Code",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                columns: new[] { "AccountingTemplateId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateAccounts_AccountingTemplateId_Code",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                columns: new[] { "AccountingTemplateId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateAccounts_AccountingTemplateId_TemplateKey",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                columns: new[] { "AccountingTemplateId", "TemplateKey" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingTemplateAccounts_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccounts",
                column: "AccountingTemplateId",
                principalSchema: "Accounting",
                principalTable: "AccountingTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingTemplateJournals_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournals",
                column: "AccountingTemplateId",
                principalSchema: "Accounting",
                principalTable: "AccountingTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingTemplatePostingProfiles_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles",
                column: "AccountingTemplateId",
                principalSchema: "Accounting",
                principalTable: "AccountingTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingTemplateTaxCodes_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes",
                column: "AccountingTemplateId",
                principalSchema: "Accounting",
                principalTable: "AccountingTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountingTemplateAccounts_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingTemplateJournals_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournals");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingTemplatePostingProfiles_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountingTemplateTaxCodes_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplates_CompanyId_Code",
                schema: "Accounting",
                table: "AccountingTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountingTemplateTaxCodes",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplateTaxCodes_AccountingTemplateId_Code",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountingTemplatePostingProfiles",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplatePostingProfiles_AccountingTemplateId_Type",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountingTemplateJournals",
                schema: "Accounting",
                table: "AccountingTemplateJournals");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplateJournals_AccountingTemplateId_Code",
                schema: "Accounting",
                table: "AccountingTemplateJournals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountingTemplateAccounts",
                schema: "Accounting",
                table: "AccountingTemplateAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplateAccounts_AccountingTemplateId_Code",
                schema: "Accounting",
                table: "AccountingTemplateAccounts");

            migrationBuilder.DropIndex(
                name: "IX_AccountingTemplateAccounts_AccountingTemplateId_TemplateKey",
                schema: "Accounting",
                table: "AccountingTemplateAccounts");

            migrationBuilder.RenameTable(
                name: "AccountingTemplateTaxCodes",
                schema: "Accounting",
                newName: "AccountingTemplateTaxCodeLine",
                newSchema: "Accounting");

            migrationBuilder.RenameTable(
                name: "AccountingTemplatePostingProfiles",
                schema: "Accounting",
                newName: "AccountingTemplatePostingProfileLine",
                newSchema: "Accounting");

            migrationBuilder.RenameTable(
                name: "AccountingTemplateJournals",
                schema: "Accounting",
                newName: "AccountingTemplateJournalLine",
                newSchema: "Accounting");

            migrationBuilder.RenameTable(
                name: "AccountingTemplateAccounts",
                schema: "Accounting",
                newName: "AccountingTemplateAccountLine",
                newSchema: "Accounting");

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CurrencyCode",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "CountryCode",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2)",
                oldMaxLength: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Accounting",
                table: "AccountingTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ZatcaCategoryCode",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,4)",
                oldPrecision: 9,
                oldScale: 4);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "ExemptionReasonCode",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "RevenueAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ReceivableAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "PayableAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "OutputVatAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "InputVatAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ExpenseAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "CashAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "BankAccountKey",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "NameAr",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(160)",
                oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                name: "DefaultDebitAccountKey",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DefaultCreditAccountKey",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "TemplateKey",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "ParentTemplateKey",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameEng",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountingTemplateTaxCodeLine",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountingTemplatePostingProfileLine",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountingTemplateJournalLine",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountingTemplateAccountLine",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateTaxCodeLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                column: "AccountingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplatePostingProfileLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                column: "AccountingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateJournalLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                column: "AccountingTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountingTemplateAccountLine_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                column: "AccountingTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingTemplateAccountLine_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateAccountLine",
                column: "AccountingTemplateId",
                principalSchema: "Accounting",
                principalTable: "AccountingTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingTemplateJournalLine_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateJournalLine",
                column: "AccountingTemplateId",
                principalSchema: "Accounting",
                principalTable: "AccountingTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingTemplatePostingProfileLine_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplatePostingProfileLine",
                column: "AccountingTemplateId",
                principalSchema: "Accounting",
                principalTable: "AccountingTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountingTemplateTaxCodeLine_AccountingTemplates_AccountingTemplateId",
                schema: "Accounting",
                table: "AccountingTemplateTaxCodeLine",
                column: "AccountingTemplateId",
                principalSchema: "Accounting",
                principalTable: "AccountingTemplates",
                principalColumn: "Id");
        }
    }
}
