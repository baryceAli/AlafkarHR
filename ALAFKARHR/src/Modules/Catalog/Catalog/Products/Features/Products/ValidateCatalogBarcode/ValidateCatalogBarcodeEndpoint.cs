using MediatR;

namespace Catalog.Products.Features.Products.ValidateCatalogBarcode;

public record ValidateCatalogBarcodeResponse(CatalogBarcodeValidationResultDto Validation);

public class ValidateCatalogBarcodeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/barcodes/validate", async (
                [FromQuery] Guid companyId,
                [FromQuery] string barcode,
                [FromQuery] Guid? excludeSkuId,
                [FromQuery] Guid? excludeSkuPackageId,
                ISender sender) =>
            {
                var result = await sender.Send(new ValidateCatalogBarcodeQuery(
                    companyId,
                    barcode,
                    excludeSkuId,
                    excludeSkuPackageId));

                return Results.Ok(new ValidateCatalogBarcodeResponse(result.Validation));
            })
            .WithName("ValidateCatalogBarcode")
            .Produces<ValidateCatalogBarcodeResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Validate catalog barcode availability")
            .RequireAuthorization(PermissionList.ProductPermissions.View);
    }
}
