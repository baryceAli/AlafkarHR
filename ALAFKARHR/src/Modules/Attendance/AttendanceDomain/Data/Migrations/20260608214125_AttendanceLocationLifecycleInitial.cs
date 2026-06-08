using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceLocationLifecycleInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Attendance");

            migrationBuilder.CreateTable(
                name: "AttendanceCheckIns",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientCheckInId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    ArrivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceCheckIns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceDays",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckInTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    CheckOutTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    WorkedMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsLate = table.Column<bool>(type: "bit", nullable: false),
                    IsAbsent = table.Column<bool>(type: "bit", nullable: false),
                    LateMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    OvertimeMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_AttendanceDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceExceptions",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExceptionType = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ManagerNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceExceptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceLocationPings",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientPingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    AccuracyMeters = table.Column<double>(type: "float", nullable: true),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsIdle = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_AttendanceLocationPings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceLogs",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_AttendanceLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceSessions",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttendanceType = table.Column<int>(type: "int", nullable: false),
                    ShiftStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShiftEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualStartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalHours = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    TotalDistanceKm = table.Column<decimal>(type: "decimal(10,3)", nullable: false, defaultValue: 0m),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_AttendanceSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeShifts",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_EmployeeShifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    GracePeriodMinutes = table.Column<int>(type: "int", nullable: false),
                    BreakMinutes = table.Column<int>(type: "int", nullable: false),
                    IsFlexible = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCheckIns_ClientCheckInId",
                schema: "Attendance",
                table: "AttendanceCheckIns",
                column: "ClientCheckInId",
                unique: true,
                filter: "[ClientCheckInId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCheckIns_EmployeeId_ArrivedAtUtc",
                schema: "Attendance",
                table: "AttendanceCheckIns",
                columns: new[] { "EmployeeId", "ArrivedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCheckIns_SessionId",
                schema: "Attendance",
                table: "AttendanceCheckIns",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDays_CompanyId",
                schema: "Attendance",
                table: "AttendanceDays",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceDays_EmployeeId_Date",
                schema: "Attendance",
                table: "AttendanceDays",
                columns: new[] { "EmployeeId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceExceptions_EmployeeId",
                schema: "Attendance",
                table: "AttendanceExceptions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceExceptions_SessionId",
                schema: "Attendance",
                table: "AttendanceExceptions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceExceptions_Status_ExceptionType",
                schema: "Attendance",
                table: "AttendanceExceptions",
                columns: new[] { "Status", "ExceptionType" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLocationPings_ClientPingId",
                schema: "Attendance",
                table: "AttendanceLocationPings",
                column: "ClientPingId",
                unique: true,
                filter: "[ClientPingId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLocationPings_EmployeeId_RecordedAtUtc",
                schema: "Attendance",
                table: "AttendanceLocationPings",
                columns: new[] { "EmployeeId", "RecordedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLocationPings_SessionId",
                schema: "Attendance",
                table: "AttendanceLocationPings",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLogs_CompanyId",
                schema: "Attendance",
                table: "AttendanceLogs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLogs_EmployeeId",
                schema: "Attendance",
                table: "AttendanceLogs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceLogs_EmployeeId_Timestamp",
                schema: "Attendance",
                table: "AttendanceLogs",
                columns: new[] { "EmployeeId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_CompanyId_ShiftStart",
                schema: "Attendance",
                table: "AttendanceSessions",
                columns: new[] { "CompanyId", "ShiftStart" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_EmployeeId",
                schema: "Attendance",
                table: "AttendanceSessions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_EmployeeId_Status",
                schema: "Attendance",
                table: "AttendanceSessions",
                columns: new[] { "EmployeeId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_CompanyId_Name",
                schema: "Attendance",
                table: "Shifts",
                columns: new[] { "CompanyId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceCheckIns",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "AttendanceDays",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "AttendanceExceptions",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "AttendanceLocationPings",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "AttendanceLogs",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "AttendanceSessions",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "EmployeeShifts",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "Shifts",
                schema: "Attendance");
        }
    }
}
