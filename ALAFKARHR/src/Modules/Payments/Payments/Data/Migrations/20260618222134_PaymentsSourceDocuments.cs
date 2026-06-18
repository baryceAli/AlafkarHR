using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payments.Data.Migrations
{
    /// <inheritdoc />
    public partial class PaymentsSourceDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SourceDocumentId",
                schema: "Payments",
                table: "PaymentAttempts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SourceDocumentNumber",
                schema: "Payments",
                table: "PaymentAttempts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceType",
                schema: "Payments",
                table: "PaymentAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_SourceType_SourceDocumentId",
                schema: "Payments",
                table: "PaymentAttempts",
                columns: new[] { "SourceType", "SourceDocumentId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentAttempts_SourceType_SourceDocumentId",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "SourceDocumentId",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "SourceDocumentNumber",
                schema: "Payments",
                table: "PaymentAttempts");

            migrationBuilder.DropColumn(
                name: "SourceType",
                schema: "Payments",
                table: "PaymentAttempts");
        }
    }
}
