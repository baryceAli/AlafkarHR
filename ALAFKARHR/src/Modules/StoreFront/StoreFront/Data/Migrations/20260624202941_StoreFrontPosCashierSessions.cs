using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreFront.Data.Migrations
{
    /// <inheritdoc />
    public partial class StoreFrontPosCashierSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PosCashierSessions",
                schema: "StoreFront",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreFrontId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CashierUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CashAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OpeningAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpectedCashAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CashSalesAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CardSalesAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentCount = table.Column<int>(type: "int", nullable: false),
                    CountedCashAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    VarianceAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCashierSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PosCashierSessionTransfers",
                schema: "StoreFront",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreFrontId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToCashAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCashierSessionTransfers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashierSessions_CompanyId_BranchId_StoreFrontId",
                schema: "StoreFront",
                table: "PosCashierSessions",
                columns: new[] { "CompanyId", "BranchId", "StoreFrontId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashierSessions_StoreFrontId_CashierUserId_Status",
                schema: "StoreFront",
                table: "PosCashierSessions",
                columns: new[] { "StoreFrontId", "CashierUserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashierSessionTransfers_CompanyId_BranchId_StoreFrontId",
                schema: "StoreFront",
                table: "PosCashierSessionTransfers",
                columns: new[] { "CompanyId", "BranchId", "StoreFrontId" });

            migrationBuilder.CreateIndex(
                name: "IX_PosCashierSessionTransfers_FromSessionId",
                schema: "StoreFront",
                table: "PosCashierSessionTransfers",
                column: "FromSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_PosCashierSessionTransfers_ToSessionId",
                schema: "StoreFront",
                table: "PosCashierSessionTransfers",
                column: "ToSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PosCashierSessions",
                schema: "StoreFront");

            migrationBuilder.DropTable(
                name: "PosCashierSessionTransfers",
                schema: "StoreFront");
        }
    }
}
