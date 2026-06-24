using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class ShiftBasedAttendanceCalendarSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceBreakPolicies",
                schema: "Attendance");

            migrationBuilder.DropColumn(
                name: "FridayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "FridayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "FridayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "MondayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "MondayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "MondayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "SaturdayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "SaturdayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "SaturdayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "SundayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "SundayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "SundayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "ThursdayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "ThursdayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "ThursdayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "TuesdayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "TuesdayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "TuesdayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "WednesdayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "WednesdayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "WednesdayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakEndTime",
                schema: "Attendance",
                table: "Shifts",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BreakMode",
                schema: "Attendance",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "BreakStartTime",
                schema: "Attendance",
                table: "Shifts",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBreakPaid",
                schema: "Attendance",
                table: "Shifts",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakEndTime",
                schema: "Attendance",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "BreakMode",
                schema: "Attendance",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "BreakStartTime",
                schema: "Attendance",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "IsBreakPaid",
                schema: "Attendance",
                table: "Shifts");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FridayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FridayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FridayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MondayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MondayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "MondayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "SaturdayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SaturdayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "SaturdayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "SundayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SundayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "SundayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ThursdayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ThursdayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ThursdayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TuesdayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TuesdayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TuesdayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WednesdayEndTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WednesdayIsWorkingDay",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WednesdayStartTime",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "time",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendanceBreakPolicies",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdministrationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllowedDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    BreakEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    BreakMode = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BreakStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Scope = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceBreakPolicies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceBreakPolicies_CompanyId_Scope_EmployeeId_DepartmentId_AdministrationId",
                schema: "Attendance",
                table: "AttendanceBreakPolicies",
                columns: new[] { "CompanyId", "Scope", "EmployeeId", "DepartmentId", "AdministrationId" });
        }
    }
}
