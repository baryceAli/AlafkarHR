using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Data.Migrations
{
    /// <inheritdoc />
    public partial class AccountingSetupTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystemAccount",
                schema: "Accounting",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                schema: "Accounting",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TemplateKey",
                schema: "Accounting",
                table: "Accounts",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountingJournals",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DefaultDebitAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DefaultCreditAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ZatcaDeviceSerial = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IsSystemJournal = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AccountingJournals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId_TemplateKey",
                schema: "Accounting",
                table: "Accounts",
                columns: new[] { "CompanyId", "TemplateKey" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountingJournals_CompanyId_Code",
                schema: "Accounting",
                table: "AccountingJournals",
                columns: new[] { "CompanyId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountingJournals",
                schema: "Accounting");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_CompanyId_TemplateKey",
                schema: "Accounting",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsSystemAccount",
                schema: "Accounting",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "Accounting",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "TemplateKey",
                schema: "Accounting",
                table: "Accounts");
        }
    }
}
