using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectCustomerProductPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ContractedAmount",
                schema: "ProjectManagement",
                table: "ProjectCustomers",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "ProjectCustomerProductPlans",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductSkuName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ProductSkuNameEng = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SkuCode = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    SkuCodeEng = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ProductPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PackageName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    PackageNameEng = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_ProjectCustomerProductPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectCustomerProductPlans_ProjectCustomers_ProjectCustomerId",
                        column: x => x.ProjectCustomerId,
                        principalSchema: "ProjectManagement",
                        principalTable: "ProjectCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCustomerProductPlans_ProjectCustomerId",
                schema: "ProjectManagement",
                table: "ProjectCustomerProductPlans",
                column: "ProjectCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCustomerProductPlans_ProjectId_ProjectCustomerId_ProductSkuId",
                schema: "ProjectManagement",
                table: "ProjectCustomerProductPlans",
                columns: new[] { "ProjectId", "ProjectCustomerId", "ProductSkuId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectCustomerProductPlans",
                schema: "ProjectManagement");

            migrationBuilder.DropColumn(
                name: "ContractedAmount",
                schema: "ProjectManagement",
                table: "ProjectCustomers");
        }
    }
}
