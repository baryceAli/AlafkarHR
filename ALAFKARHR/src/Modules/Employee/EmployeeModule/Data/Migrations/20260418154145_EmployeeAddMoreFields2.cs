using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeAddMoreFields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstNameEng",
                schema: "Employee",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastNameEng",
                schema: "Employee",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleNameEng",
                schema: "Employee",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstNameEng",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastNameEng",
                schema: "Employee",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "MiddleNameEng",
                schema: "Employee",
                table: "Employees");
        }
    }
}
