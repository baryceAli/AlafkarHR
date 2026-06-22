using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalesOrder.Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesOrderErpUpgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InvoicingPolicy",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SalespersonId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceQuotationId",
                schema: "SalesOrder",
                table: "SalesOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BulkDiscountAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BulkDiscountRate",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CouponDiscountAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CouponDiscountType",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CouponDiscountValue",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponStatus",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerDiscountAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerDiscountRate",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalUnitAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsManualPriceOverride",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PriceOverrideAt",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceOverrideBy",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceSource",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PriceSourceId",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PromotionUnitPrice",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReturnedQuantity",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SourceUnitPrice",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxableAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "SalesDeliveryNotes",
                schema: "SalesOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesDeliveryNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesQuotations",
                schema: "SalesOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceListId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalespersonId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QuotationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Terms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalesOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConvertedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesQuotations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesReturns",
                schema: "SalesOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SalesOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AccountingDocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreateCreditNote = table.Column<bool>(type: "bit", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReturns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesSettings",
                schema: "SalesOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoicingPolicy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesDeliveryNoteLines",
                schema: "SalesOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    SalesOrderLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductNameEng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkuCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesDeliveryNoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesDeliveryNoteLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesDeliveryNoteLines_SalesDeliveryNotes_SalesDeliveryNoteId",
                        column: x => x.SalesDeliveryNoteId,
                        principalSchema: "SalesOrder",
                        principalTable: "SalesDeliveryNotes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalesQuotationLines",
                schema: "SalesOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductNameEng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkuCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceSource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceSourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromotionUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BulkDiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BulkDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerDiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponDiscountType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CouponDiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CouponDiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxableAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalUnitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalesQuotationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesQuotationLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesQuotationLines_SalesQuotations_SalesQuotationId",
                        column: x => x.SalesQuotationId,
                        principalSchema: "SalesOrder",
                        principalTable: "SalesQuotations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SalesReturnLines",
                schema: "SalesOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    SalesOrderLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryNoteLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductSkuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductNameEng = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SkuCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitOfMeasureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesReturnId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesReturnLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesReturnLines_SalesReturns_SalesReturnId",
                        column: x => x.SalesReturnId,
                        principalSchema: "SalesOrder",
                        principalTable: "SalesReturns",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalesDeliveryNoteLines_SalesDeliveryNoteId",
                schema: "SalesOrder",
                table: "SalesDeliveryNoteLines",
                column: "SalesDeliveryNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesQuotationLines_SalesQuotationId",
                schema: "SalesOrder",
                table: "SalesQuotationLines",
                column: "SalesQuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesReturnLines_SalesReturnId",
                schema: "SalesOrder",
                table: "SalesReturnLines",
                column: "SalesReturnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalesDeliveryNoteLines",
                schema: "SalesOrder");

            migrationBuilder.DropTable(
                name: "SalesQuotationLines",
                schema: "SalesOrder");

            migrationBuilder.DropTable(
                name: "SalesReturnLines",
                schema: "SalesOrder");

            migrationBuilder.DropTable(
                name: "SalesSettings",
                schema: "SalesOrder");

            migrationBuilder.DropTable(
                name: "SalesDeliveryNotes",
                schema: "SalesOrder");

            migrationBuilder.DropTable(
                name: "SalesQuotations",
                schema: "SalesOrder");

            migrationBuilder.DropTable(
                name: "SalesReturns",
                schema: "SalesOrder");

            migrationBuilder.DropColumn(
                name: "InvoicingPolicy",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "SalespersonId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "SourceQuotationId",
                schema: "SalesOrder",
                table: "SalesOrders");

            migrationBuilder.DropColumn(
                name: "BulkDiscountAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "BulkDiscountRate",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "CouponDiscountAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "CouponDiscountType",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "CouponDiscountValue",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "CouponStatus",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "CustomerDiscountAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "CustomerDiscountRate",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "FinalUnitAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "IsManualPriceOverride",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "PriceOverrideAt",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "PriceOverrideBy",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "PriceSource",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "PriceSourceId",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "PromotionUnitPrice",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "ReturnedQuantity",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "SourceUnitPrice",
                schema: "SalesOrder",
                table: "SalesOrderLines");

            migrationBuilder.DropColumn(
                name: "TaxableAmount",
                schema: "SalesOrder",
                table: "SalesOrderLines");
        }
    }
}
