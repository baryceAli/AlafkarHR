using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrganizationBranchStoreRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Specialization",
                schema: "Organization",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "UserBranchRoleAssignments",
                schema: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_UserBranchRoleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBranchRoleAssignments_Branches_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "Organization",
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBranchRoleAssignments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Organization",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CompanyId_Specialization",
                schema: "Organization",
                table: "Branches",
                columns: new[] { "CompanyId", "Specialization" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchRoleAssignments_BranchId",
                schema: "Organization",
                table: "UserBranchRoleAssignments",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchRoleAssignments_CompanyId_UserId",
                schema: "Organization",
                table: "UserBranchRoleAssignments",
                columns: new[] { "CompanyId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBranchRoleAssignments_CompanyId_UserId_BranchId_TemplateKey",
                schema: "Organization",
                table: "UserBranchRoleAssignments",
                columns: new[] { "CompanyId", "UserId", "BranchId", "TemplateKey" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBranchRoleAssignments",
                schema: "Organization");

            migrationBuilder.DropIndex(
                name: "IX_Branches_CompanyId_Specialization",
                schema: "Organization",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "Specialization",
                schema: "Organization",
                table: "Branches");
        }
    }
}
