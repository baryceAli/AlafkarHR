using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catering.Data.Migrations
{
    /// <inheritdoc />
    public partial class CateringMultiContractProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CateringProjectDailyPlanId",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CateringProjectId",
                schema: "Catering",
                table: "CateringDailySchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CateringProjects",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CateringProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CateringProjectContractLinks",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_CateringProjectContractLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CateringProjectContractLinks_CateringProjects_CateringProjectId",
                        column: x => x.CateringProjectId,
                        principalSchema: "Catering",
                        principalTable: "CateringProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CateringProjectDailyPlans",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlannedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_CateringProjectDailyPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CateringProjectDailyPlans_CateringProjects_CateringProjectId",
                        column: x => x.CateringProjectId,
                        principalSchema: "Catering",
                        principalTable: "CateringProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CateringProjectSquareScopes",
                schema: "Catering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CateringProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SquareId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_CateringProjectSquareScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CateringProjectSquareScopes_CateringProjects_CateringProjectId",
                        column: x => x.CateringProjectId,
                        principalSchema: "Catering",
                        principalTable: "CateringProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CateringDailySchedules_CateringProjectDailyPlanId",
                schema: "Catering",
                table: "CateringDailySchedules",
                column: "CateringProjectDailyPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_CateringDailySchedules_CateringProjectId",
                schema: "Catering",
                table: "CateringDailySchedules",
                column: "CateringProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CateringProjectContractLinks_CateringProjectId_CateringContractId",
                schema: "Catering",
                table: "CateringProjectContractLinks",
                columns: new[] { "CateringProjectId", "CateringContractId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CateringProjectDailyPlans_CateringProjectId_ServiceDate",
                schema: "Catering",
                table: "CateringProjectDailyPlans",
                columns: new[] { "CateringProjectId", "ServiceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CateringProjects_CompanyId_ProjectName",
                schema: "Catering",
                table: "CateringProjects",
                columns: new[] { "CompanyId", "ProjectName" });

            migrationBuilder.CreateIndex(
                name: "IX_CateringProjectSquareScopes_CateringProjectId_SquareId",
                schema: "Catering",
                table: "CateringProjectSquareScopes",
                columns: new[] { "CateringProjectId", "SquareId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CateringProjectContractLinks",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringProjectDailyPlans",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringProjectSquareScopes",
                schema: "Catering");

            migrationBuilder.DropTable(
                name: "CateringProjects",
                schema: "Catering");

            migrationBuilder.DropIndex(
                name: "IX_CateringDailySchedules_CateringProjectDailyPlanId",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropIndex(
                name: "IX_CateringDailySchedules_CateringProjectId",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "CateringProjectDailyPlanId",
                schema: "Catering",
                table: "CateringDailySchedules");

            migrationBuilder.DropColumn(
                name: "CateringProjectId",
                schema: "Catering",
                table: "CateringDailySchedules");
        }
    }
}
