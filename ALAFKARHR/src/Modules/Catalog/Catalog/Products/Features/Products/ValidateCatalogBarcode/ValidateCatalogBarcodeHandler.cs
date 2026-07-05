namespace Catalog.Products.Features.Products.ValidateCatalogBarcode;

public record ValidateCatalogBarcodeQuery(
    Guid CompanyId,
    string Barcode,
    Guid? ExcludeSkuId,
    Guid? ExcludeSkuPackageId) : IQuery<ValidateCatalogBarcodeResult>;

public record ValidateCatalogBarcodeResult(CatalogBarcodeValidationResultDto Validation);

public class ValidateCatalogBarcodeHandler(CatalogDbContext dbContext)
    : IQueryHandler<ValidateCatalogBarcodeQuery, ValidateCatalogBarcodeResult>
{
    public async Task<ValidateCatalogBarcodeResult> Handle(ValidateCatalogBarcodeQuery request, CancellationToken cancellationToken)
    {
        var validation = await CatalogOwnershipGuard.GetBarcodeValidationResultAsync(
            dbContext,
            request.CompanyId,
            request.Barcode,
            request.ExcludeSkuId,
            request.ExcludeSkuPackageId,
            cancellationToken);

        return new ValidateCatalogBarcodeResult(validation);
    }
}
