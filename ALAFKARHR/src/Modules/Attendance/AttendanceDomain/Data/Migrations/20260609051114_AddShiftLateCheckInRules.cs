using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftLateCheckInRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GracePeriodMinutes",
                schema: "Attendance",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 15,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BreakMinutes",
                schema: "Attendance",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LateAfterMinutes",
                schema: "Attendance",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 15);

            migrationBuilder.AddColumn<int>(
                name: "ProhibitCheckInAfterMinutes",
                schema: "Attendance",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 120);

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                schema: "Attendance",
                table: "AttendanceSessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LateCheckInRequests",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttendanceType = table.Column<int>(type: "int", nullable: false),
                    ShiftStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedCheckInTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegisteredCheckInTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ManagerNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    AccuracyMeters = table.Column<double>(type: "float", nullable: true),
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
                    table.PrimaryKey("PK_LateCheckInRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_ShiftId",
                schema: "Attendance",
                table: "AttendanceSessions",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_LateCheckInRequests_CompanyId",
                schema: "Attendance",
                table: "LateCheckInRequests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_LateCheckInRequests_EmployeeId",
                schema: "Attendance",
                table: "LateCheckInRequests",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LateCheckInRequests_SessionId",
                schema: "Attendance",
                table: "LateCheckInRequests",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LateCheckInRequests_ShiftId",
                schema: "Attendance",
                table: "LateCheckInRequests",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_LateCheckInRequests_Status_RequestedCheckInTimeUtc",
                schema: "Attendance",
                table: "LateCheckInRequests",
                columns: new[] { "Status", "RequestedCheckInTimeUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LateCheckInRequests",
                schema: "Attendance");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_ShiftId",
                schema: "Attendance",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "LateAfterMinutes",
                schema: "Attendance",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ProhibitCheckInAfterMinutes",
                schema: "Attendance",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                schema: "Attendance",
                table: "AttendanceSessions");

            migrationBuilder.AlterColumn<int>(
                name: "GracePeriodMinutes",
                schema: "Attendance",
                table: "Shifts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 15);

            migrationBuilder.AlterColumn<int>(
                name: "BreakMinutes",
                schema: "Attendance",
                table: "Shifts",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
