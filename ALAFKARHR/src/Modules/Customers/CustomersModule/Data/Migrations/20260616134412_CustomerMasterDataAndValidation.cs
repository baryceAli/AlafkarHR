using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomersModule.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomerMasterDataAndValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerPricingProfiles_CustomerId_PriceListId_EffectiveFrom",
                schema: "Customer",
                table: "CustomerPricingProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CustomerGroups_Name",
                schema: "Customer",
                table: "CustomerGroups");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableCredit",
                schema: "Customer",
                table: "Customers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegistrationNumber",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreditHoldReason",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreditStatus",
                schema: "Customer",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentTerm",
                schema: "Customer",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "VatNumber",
                schema: "Customer",
                table: "Customers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameEng",
                schema: "Customer",
                table: "CustomerGroups",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultBilling",
                schema: "Customer",
                table: "CustomerAddresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CompanyId_CustomerCode",
                schema: "Customer",
                table: "Customers",
                columns: new[] { "CompanyId", "CustomerCode" },
                unique: true,
                filter: "[CustomerCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPricingProfiles_CompanyId_CustomerId_PriceListId_EffectiveFrom",
                schema: "Customer",
                table: "CustomerPricingProfiles",
                columns: new[] { "CompanyId", "CustomerId", "PriceListId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroups_CompanyId_Name",
                schema: "Customer",
                table: "CustomerGroups",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroups_CompanyId_NameEng",
                schema: "Customer",
                table: "CustomerGroups",
                columns: new[] { "CompanyId", "NameEng" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_CompanyId_CustomerCode",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPricingProfiles_CompanyId_CustomerId_PriceListId_EffectiveFrom",
                schema: "Customer",
                table: "CustomerPricingProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CustomerGroups_CompanyId_Name",
                schema: "Customer",
                table: "CustomerGroups");

            migrationBuilder.DropIndex(
                name: "IX_CustomerGroups_CompanyId_NameEng",
                schema: "Customer",
                table: "CustomerGroups");

            migrationBuilder.DropColumn(
                name: "AvailableCredit",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CommercialRegistrationNumber",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreditHoldReason",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreditStatus",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerCode",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PaymentTerm",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VatNumber",
                schema: "Customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsDefaultBilling",
                schema: "Customer",
                table: "CustomerAddresses");

            migrationBuilder.AlterColumn<string>(
                name: "NameEng",
                schema: "Customer",
                table: "CustomerGroups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPricingProfiles_CustomerId_PriceListId_EffectiveFrom",
                schema: "Customer",
                table: "CustomerPricingProfiles",
                columns: new[] { "CustomerId", "PriceListId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerGroups_Name",
                schema: "Customer",
                table: "CustomerGroups",
                column: "Name",
                unique: true);
        }
    }
}
