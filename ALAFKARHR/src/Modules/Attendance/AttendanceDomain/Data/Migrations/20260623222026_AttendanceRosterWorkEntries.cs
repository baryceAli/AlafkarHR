using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceRosterWorkEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceCorrections",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CorrectedCheckInUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorrectedCheckOutUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ManagerNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppliedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceCorrections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceWorkEntries",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryType = table.Column<int>(type: "int", nullable: false),
                    Hours = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SourceDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceModule = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ApprovedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ApprovedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LockedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_AttendanceWorkEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BiometricImportBatches",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImportedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalRows = table.Column<int>(type: "int", nullable: false),
                    AcceptedRows = table.Column<int>(type: "int", nullable: false),
                    RejectedRows = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_BiometricImportBatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BiometricImportRows",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeviceEmployeeCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PunchTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCheckOut = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_BiometricImportRows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftScheduleAssignments",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_ShiftScheduleAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftSchedules",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PublishedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    LockedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
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
                    table.PrimaryKey("PK_ShiftSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftSwapRequests",
                schema: "Attendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleAssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestingEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ManagerNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_ShiftSwapRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrections_CompanyId_Status",
                schema: "Attendance",
                table: "AttendanceCorrections",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrections_EmployeeId_WorkDate",
                schema: "Attendance",
                table: "AttendanceCorrections",
                columns: new[] { "EmployeeId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceCorrections_SessionId",
                schema: "Attendance",
                table: "AttendanceCorrections",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceWorkEntries_CompanyId_Status",
                schema: "Attendance",
                table: "AttendanceWorkEntries",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceWorkEntries_CompanyId_WorkDate",
                schema: "Attendance",
                table: "AttendanceWorkEntries",
                columns: new[] { "CompanyId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceWorkEntries_EmployeeId_WorkDate",
                schema: "Attendance",
                table: "AttendanceWorkEntries",
                columns: new[] { "EmployeeId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceWorkEntries_SourceModule_SourceDocumentId",
                schema: "Attendance",
                table: "AttendanceWorkEntries",
                columns: new[] { "SourceModule", "SourceDocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_BiometricImportBatches_CompanyId_Status",
                schema: "Attendance",
                table: "BiometricImportBatches",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BiometricImportBatches_ImportedAtUtc",
                schema: "Attendance",
                table: "BiometricImportBatches",
                column: "ImportedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_BiometricImportRows_BatchId_Status",
                schema: "Attendance",
                table: "BiometricImportRows",
                columns: new[] { "BatchId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BiometricImportRows_CompanyId_PunchTimeUtc",
                schema: "Attendance",
                table: "BiometricImportRows",
                columns: new[] { "CompanyId", "PunchTimeUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BiometricImportRows_EmployeeId",
                schema: "Attendance",
                table: "BiometricImportRows",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftScheduleAssignments_CompanyId_WorkDate",
                schema: "Attendance",
                table: "ShiftScheduleAssignments",
                columns: new[] { "CompanyId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftScheduleAssignments_ScheduleId_EmployeeId_WorkDate",
                schema: "Attendance",
                table: "ShiftScheduleAssignments",
                columns: new[] { "ScheduleId", "EmployeeId", "WorkDate" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftScheduleAssignments_ShiftId",
                schema: "Attendance",
                table: "ShiftScheduleAssignments",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSchedules_CompanyId_StartDate_EndDate",
                schema: "Attendance",
                table: "ShiftSchedules",
                columns: new[] { "CompanyId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSchedules_CompanyId_Status",
                schema: "Attendance",
                table: "ShiftSchedules",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwapRequests_CompanyId_Status",
                schema: "Attendance",
                table: "ShiftSwapRequests",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwapRequests_RequestingEmployeeId_WorkDate",
                schema: "Attendance",
                table: "ShiftSwapRequests",
                columns: new[] { "RequestingEmployeeId", "WorkDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ShiftSwapRequests_TargetEmployeeId_WorkDate",
                schema: "Attendance",
                table: "ShiftSwapRequests",
                columns: new[] { "TargetEmployeeId", "WorkDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceCorrections",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "AttendanceWorkEntries",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "BiometricImportBatches",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "BiometricImportRows",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "ShiftScheduleAssignments",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "ShiftSchedules",
                schema: "Attendance");

            migrationBuilder.DropTable(
                name: "ShiftSwapRequests",
                schema: "Attendance");
        }
    }
}
