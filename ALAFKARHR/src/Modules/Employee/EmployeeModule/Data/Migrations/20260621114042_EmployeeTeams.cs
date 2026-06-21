using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeTeams",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEng = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Category = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedForProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_EmployeeTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeTeamMembers",
                schema: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    EmployeeNameEng = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EmployeeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_EmployeeTeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeTeamMembers_EmployeeTeams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "Employee",
                        principalTable: "EmployeeTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTeamMembers_EmployeeId",
                schema: "Employee",
                table: "EmployeeTeamMembers",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTeamMembers_TeamId_EmployeeId",
                schema: "Employee",
                table: "EmployeeTeamMembers",
                columns: new[] { "TeamId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTeams_CompanyId_Category_IsActive",
                schema: "Employee",
                table: "EmployeeTeams",
                columns: new[] { "CompanyId", "Category", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeTeams_CreatedForProjectId",
                schema: "Employee",
                table: "EmployeeTeams",
                column: "CreatedForProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeTeamMembers",
                schema: "Employee");

            migrationBuilder.DropTable(
                name: "EmployeeTeams",
                schema: "Employee");
        }
    }
}
