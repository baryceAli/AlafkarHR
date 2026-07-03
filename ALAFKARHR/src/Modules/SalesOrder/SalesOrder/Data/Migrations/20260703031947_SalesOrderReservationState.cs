using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesOrderReservationState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReservedQuantity",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                schema: "SalesOrder",
                table: "SalesOrderLines");
        }
    }
}
