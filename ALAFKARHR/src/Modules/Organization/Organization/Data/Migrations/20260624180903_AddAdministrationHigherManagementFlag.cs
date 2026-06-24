using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministrationHigherManagementFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHigherManagement",
                schema: "Organization",
                table: "Administrations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHigherManagement",
                schema: "Organization",
                table: "Administrations");
        }
    }
}
