using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payroll.Data.Migrations
{
    /// <inheritdoc />
    public partial class SaudiPayrollWpsEosPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountingJournalEntryId",
                schema: "Payroll",
                table: "PayrollEntries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountingJournalNumber",
                schema: "Payroll",
                table: "PayrollEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountingPostedAt",
                schema: "Payroll",
                table: "PayrollEntries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountingPostedBy",
                schema: "Payroll",
                table: "PayrollEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EosProvisionSnapshots",
                schema: "Payroll",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ServiceEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrossPayBasis = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProvisionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_EosProvisionSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PayrollImportedWorkEntries",
                schema: "Payroll",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceWorkEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_PayrollImportedWorkEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaudiPayrollInfos",
                schema: "Payroll",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: true),
                    BankCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GosiNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GosiEmployeePercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    GosiEmployerPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    EosBasicSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EosServiceStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncludeInWps = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_SaudiPayrollInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WpsBatches",
                schema: "Payroll",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayrollEntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmployeeCount = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExportedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExportedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_WpsBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WpsBatchRows",
                schema: "Payroll",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WpsBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PayslipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NetAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_WpsBatchRows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WpsBatchRows_WpsBatches_WpsBatchId",
                        column: x => x.WpsBatchId,
                        principalSchema: "Payroll",
                        principalTable: "WpsBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEntries_AccountingJournalEntryId",
                schema: "Payroll",
                table: "PayrollEntries",
                column: "AccountingJournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_EosProvisionSnapshots_CompanyId_EmployeeId_PayrollPeriodId",
                schema: "Payroll",
                table: "EosProvisionSnapshots",
                columns: new[] { "CompanyId", "EmployeeId", "PayrollPeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollImportedWorkEntries_CompanyId_EmployeeId_PayrollPeriodId",
                schema: "Payroll",
                table: "PayrollImportedWorkEntries",
                columns: new[] { "CompanyId", "EmployeeId", "PayrollPeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollImportedWorkEntries_SourceWorkEntryId",
                schema: "Payroll",
                table: "PayrollImportedWorkEntries",
                column: "SourceWorkEntryId",
                unique: true,
                filter: "[SourceWorkEntryId] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SaudiPayrollInfos_CompanyId_EmployeeId",
                schema: "Payroll",
                table: "SaudiPayrollInfos",
                columns: new[] { "CompanyId", "EmployeeId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SaudiPayrollInfos_CompanyId_IncludeInWps",
                schema: "Payroll",
                table: "SaudiPayrollInfos",
                columns: new[] { "CompanyId", "IncludeInWps" });

            migrationBuilder.CreateIndex(
                name: "IX_WpsBatches_CompanyId_BatchNumber",
                schema: "Payroll",
                table: "WpsBatches",
                columns: new[] { "CompanyId", "BatchNumber" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_WpsBatches_CompanyId_PayrollPeriodId",
                schema: "Payroll",
                table: "WpsBatches",
                columns: new[] { "CompanyId", "PayrollPeriodId" });

            migrationBuilder.CreateIndex(
                name: "IX_WpsBatchRows_WpsBatchId_EmployeeId",
                schema: "Payroll",
                table: "WpsBatchRows",
                columns: new[] { "WpsBatchId", "EmployeeId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EosProvisionSnapshots",
                schema: "Payroll");

            migrationBuilder.DropTable(
                name: "PayrollImportedWorkEntries",
                schema: "Payroll");

            migrationBuilder.DropTable(
                name: "SaudiPayrollInfos",
                schema: "Payroll");

            migrationBuilder.DropTable(
                name: "WpsBatchRows",
                schema: "Payroll");

            migrationBuilder.DropTable(
                name: "WpsBatches",
                schema: "Payroll");

            migrationBuilder.DropIndex(
                name: "IX_PayrollEntries_AccountingJournalEntryId",
                schema: "Payroll",
                table: "PayrollEntries");

            migrationBuilder.DropColumn(
                name: "AccountingJournalEntryId",
                schema: "Payroll",
                table: "PayrollEntries");

            migrationBuilder.DropColumn(
                name: "AccountingJournalNumber",
                schema: "Payroll",
                table: "PayrollEntries");

            migrationBuilder.DropColumn(
                name: "AccountingPostedAt",
                schema: "Payroll",
                table: "PayrollEntries");

            migrationBuilder.DropColumn(
                name: "AccountingPostedBy",
                schema: "Payroll",
                table: "PayrollEntries");
        }
    }
}
