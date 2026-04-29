using MediatR;

namespace Catalog.Products.Features.Variants.GetVariantByCompany;


public record GetVariantByCompanyResponse(PaginatedResult<VariantDto> VariantList);
public class GetVariantByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/variants/company/{companyId}", async (Guid companyId, [AsParameters] PaginationRequest paginationRequest, ISender sender) =>
        {
            var result = await sender.Send(new GetVariantByCompanyQuery(companyId, paginationRequest));
            return Results.Ok(new GetVariantByCompanyResponse(result.VariantList));
        })
            .WithName("GetVariantByCompany")
            .Produces<GetVariantByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get a paginated list of product variants for a specific company.")
            .WithDescription("Retrieves a paginated list of product variants associated with the specified company ID. Supports pagination and optional search text.")
            .RequireAuthorization(PermissionList.VariantPermissions.View);

    }
}
