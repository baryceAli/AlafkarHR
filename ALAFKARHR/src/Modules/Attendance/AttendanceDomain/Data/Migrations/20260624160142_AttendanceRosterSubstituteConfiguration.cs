using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceRosterSubstituteConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceRosterSubstituteConfigurations",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRosterVisible = table.Column<bool>(type: "bit", nullable: false),
                    IsSubstituteEligible = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AttendanceRosterSubstituteConfigurations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRosterSubstituteConfigurations_CompanyId_EmployeeId_IsDeleted",
                schema: "Attendance",
                table: "AttendanceRosterSubstituteConfigurations",
                columns: new[] { "CompanyId", "EmployeeId", "IsDeleted" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRosterSubstituteConfigurations",
                schema: "Attendance");
        }
    }
}
