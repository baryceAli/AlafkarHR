using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catering.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCateringCalorieCompliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CaloriesPerUnit",
                schema: "Catering",
                table: "MealComponents",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCalories",
                schema: "Catering",
                table: "MealComponents",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMealCaloriesRequired",
                schema: "Catering",
                table: "CateringContracts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxMealCalories",
                schema: "Catering",
                table: "CateringContracts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinMealCalories",
                schema: "Catering",
                table: "CateringContracts",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesPerUnit",
                schema: "Catering",
                table: "MealComponents");

            migrationBuilder.DropColumn(
                name: "TotalCalories",
                schema: "Catering",
                table: "MealComponents");

            migrationBuilder.DropColumn(
                name: "IsMealCaloriesRequired",
                schema: "Catering",
                table: "CateringContracts");

            migrationBuilder.DropColumn(
                name: "MaxMealCalories",
                schema: "Catering",
                table: "CateringContracts");

            migrationBuilder.DropColumn(
                name: "MinMealCalories",
                schema: "Catering",
                table: "CateringContracts");
        }
    }
}
