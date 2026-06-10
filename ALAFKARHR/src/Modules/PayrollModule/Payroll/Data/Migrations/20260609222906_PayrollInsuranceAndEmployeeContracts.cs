using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payroll.Data.Migrations
{
    /// <inheritdoc />
    public partial class PayrollInsuranceAndEmployeeContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InsuranceAmount",
                schema: "Payroll",
                table: "SalaryRuns",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InsurancePercentage",
                schema: "Payroll",
                table: "SalaryRuns",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                schema: "Payroll",
                table: "SalaryRuns",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                schema: "Payroll",
                table: "SalaryRuns",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxableAmount",
                schema: "Payroll",
                table: "SalaryRuns",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "Payroll",
                table: "EmployeeContracts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveFrom",
                schema: "Payroll",
                table: "EmployeeContracts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "Payroll",
                table: "EmployeeContracts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "InsurancePercentage",
                schema: "Payroll",
                table: "Contracts",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPercentage",
                schema: "Payroll",
                table: "Contracts",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsTaxable",
                schema: "Payroll",
                table: "Components",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContracts_CompanyId_EmployeeId_IsActive",
                schema: "Payroll",
                table: "EmployeeContracts",
                columns: new[] { "CompanyId", "EmployeeId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmployeeContracts_CompanyId_EmployeeId_IsActive",
                schema: "Payroll",
                table: "EmployeeContracts");

            migrationBuilder.DropColumn(
                name: "InsuranceAmount",
                schema: "Payroll",
                table: "SalaryRuns");

            migrationBuilder.DropColumn(
                name: "InsurancePercentage",
                schema: "Payroll",
                table: "SalaryRuns");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                schema: "Payroll",
                table: "SalaryRuns");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                schema: "Payroll",
                table: "SalaryRuns");

            migrationBuilder.DropColumn(
                name: "TaxableAmount",
                schema: "Payroll",
                table: "SalaryRuns");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "Payroll",
                table: "EmployeeContracts");

            migrationBuilder.DropColumn(
                name: "EffectiveFrom",
                schema: "Payroll",
                table: "EmployeeContracts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "Payroll",
                table: "EmployeeContracts");

            migrationBuilder.DropColumn(
                name: "InsurancePercentage",
                schema: "Payroll",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TaxPercentage",
                schema: "Payroll",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "IsTaxable",
                schema: "Payroll",
                table: "Components");
        }
    }
}
