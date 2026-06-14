using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceConfigurationChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "WeekendDays",
                schema: "Attendance",
                table: "AttendanceConfigurations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Friday,Saturday");

            migrationBuilder.AddColumn<int>(
                name: "BreakMode",
                schema: "Attendance",
                table: "AttendanceBreakPolicies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "WeekendDays",
                schema: "Attendance",
                table: "AttendanceConfigurations");

            migrationBuilder.DropColumn(
                name: "BreakMode",
                schema: "Attendance",
                table: "AttendanceBreakPolicies");
        }
    }
}
