namespace Pricing.Pricings.Features.PriceLists.GetPriceListsByCompany;

public record GetPriceListsByCompanyResponse(PaginatedResult<PriceListDto> PriceList);

public class GetPriceListsByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/pricing/priceLists/company/{companyId}", async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetPriceListsByCompanyQuery(companyId, request));
            return Results.Ok(result.Adapt<GetPriceListsByCompanyResponse>());
        })
        .WithName("GetPriceListsByCompany")
        .Produces<GetPriceListsByCompanyResponse>(StatusCodes.Status200OK)
        .WithSummary("Get price lists by company")
        .WithDescription("Get price lists by company")
        .RequireAuthorization(PermissionList.PricingPermissions.View);
    }
}
