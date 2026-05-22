namespace Pricing.Pricings.Features.PriceLists.CreatePriceList;

public record CreatePriceListRequest(PriceListDto PriceList);
public record CreatePriceListResponse(Guid Id);

public class CreatePriceListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/pricing/priceLists", async (CreatePriceListRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreatePriceListCommand(request.PriceList));
            return Results.Created($"/api/v1/pricing/priceLists/{result.Id}", result.Adapt<CreatePriceListResponse>());
        })
        .WithName("CreatePriceList")
        .Produces<CreatePriceListResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create price list")
        .WithDescription("Create price list")
        .RequireAuthorization(PermissionList.PricingPermissions.Create);
    }
}
