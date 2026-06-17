using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceSessionNormalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizationNote",
                schema: "Attendance",
                table: "AttendanceSessions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NormalizationStatus",
                schema: "Attendance",
                table: "AttendanceSessions",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "NormalizedAt",
                schema: "Attendance",
                table: "AttendanceSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedBy",
                schema: "Attendance",
                table: "AttendanceSessions",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_CompanyId_NormalizationStatus",
                schema: "Attendance",
                table: "AttendanceSessions",
                columns: new[] { "CompanyId", "NormalizationStatus" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_CompanyId_NormalizationStatus",
                schema: "Attendance",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "NormalizationNote",
                schema: "Attendance",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "NormalizationStatus",
                schema: "Attendance",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "NormalizedAt",
                schema: "Attendance",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "NormalizedBy",
                schema: "Attendance",
                table: "AttendanceSessions");
        }
    }
}
