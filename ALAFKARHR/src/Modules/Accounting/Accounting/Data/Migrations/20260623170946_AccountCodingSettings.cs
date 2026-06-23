using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Data.Migrations
{
    /// <inheritdoc />
    public partial class AccountCodingSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountCodingSettings",
                schema: "Accounting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetRootCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LiabilityRootCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EquityRootCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RevenueRootCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ExpenseRootCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChildGroupSuffixLength = table.Column<int>(type: "int", nullable: false),
                    ChildLedgerSuffixLength = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AccountCodingSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCodingSettings_CompanyId",
                schema: "Accounting",
                table: "AccountCodingSettings",
                column: "CompanyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountCodingSettings",
                schema: "Accounting");
        }
    }
}
