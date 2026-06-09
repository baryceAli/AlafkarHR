using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeAllowedRadiusMeters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllowedRadiusMeters",
                schema: "Employee",
                table: "Employees",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedRadiusMeters",
                schema: "Employee",
                table: "Employees");
        }
    }
}
