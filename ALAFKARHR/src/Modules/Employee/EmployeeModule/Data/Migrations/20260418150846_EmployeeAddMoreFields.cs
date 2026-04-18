using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeAddMoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.AddColumn<Guid>(
                name: "AcademicInstituteId",
                schema: "Employee",
                table: "Employees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "Employee",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EmploymentType",
                schema: "Employee",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GraduationYear",
                schema: "Employee",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                schema: "Employee",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                schema: "Employee",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Qualification",
                schema: "Employee",
                table: "Employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "SpecializationId",
                schema: "Employee",
                table: "Employees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                schema: "Employee",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Phone",
                schema: "Employee",
                table: "Employees",
                column: "Phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_Email",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_Phone",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "AcademicInstituteId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EmploymentType",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "GraduationYear",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Nationality",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Qualification",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "SpecializationId",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                schema: "Employee",
                table: "Employees",
                column: "Email");
        }
    }
}
