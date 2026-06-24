using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceCorrectionCurrentSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentCheckInUtc",
                schema: "Attendance",
                table: "AttendanceCorrections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CurrentCheckOutUtc",
                schema: "Attendance",
                table: "AttendanceCorrections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentSessionStatus",
                schema: "Attendance",
                table: "AttendanceCorrections",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCheckInUtc",
                schema: "Attendance",
                table: "AttendanceCorrections");

            migrationBuilder.DropColumn(
                name: "CurrentCheckOutUtc",
                schema: "Attendance",
                table: "AttendanceCorrections");

            migrationBuilder.DropColumn(
                name: "CurrentSessionStatus",
                schema: "Attendance",
                table: "AttendanceCorrections");
        }
    }
}
