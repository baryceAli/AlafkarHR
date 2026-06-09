using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceDomain.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScopedShiftAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Attendance",
                table: "EmployeeShifts",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                schema: "Attendance",
                table: "EmployeeShifts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AdministrationId",
                schema: "Attendance",
                table: "EmployeeShifts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                schema: "Attendance",
                table: "EmployeeShifts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Scope",
                schema: "Attendance",
                table: "EmployeeShifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShifts_AdministrationId_IsActive_EffectiveFrom",
                schema: "Attendance",
                table: "EmployeeShifts",
                columns: new[] { "AdministrationId", "IsActive", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShifts_CompanyId_Scope_IsActive_EffectiveFrom",
                schema: "Attendance",
                table: "EmployeeShifts",
                columns: new[] { "CompanyId", "Scope", "IsActive", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShifts_DepartmentId_IsActive_EffectiveFrom",
                schema: "Attendance",
                table: "EmployeeShifts",
                columns: new[] { "DepartmentId", "IsActive", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeShifts_EmployeeId_IsActive_EffectiveFrom",
                schema: "Attendance",
                table: "EmployeeShifts",
                columns: new[] { "EmployeeId", "IsActive", "EffectiveFrom" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeShifts_AdministrationId_IsActive_EffectiveFrom",
                schema: "Attendance",
                table: "EmployeeShifts");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeShifts_CompanyId_Scope_IsActive_EffectiveFrom",
                schema: "Attendance",
                table: "EmployeeShifts");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeShifts_DepartmentId_IsActive_EffectiveFrom",
                schema: "Attendance",
                table: "EmployeeShifts");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeShifts_EmployeeId_IsActive_EffectiveFrom",
                schema: "Attendance",
                table: "EmployeeShifts");

            migrationBuilder.DropColumn(
                name: "AdministrationId",
                schema: "Attendance",
                table: "EmployeeShifts");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "Attendance",
                table: "EmployeeShifts");

            migrationBuilder.DropColumn(
                name: "Scope",
                schema: "Attendance",
                table: "EmployeeShifts");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "Attendance",
                table: "EmployeeShifts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                schema: "Attendance",
                table: "EmployeeShifts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
