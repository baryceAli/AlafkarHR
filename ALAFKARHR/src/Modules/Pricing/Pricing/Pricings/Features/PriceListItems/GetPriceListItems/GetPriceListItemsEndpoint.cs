namespace Pricing.Pricings.Features.PriceListItems.GetPriceListItems;

public record GetPriceListItemsResponse(List<PriceListItemDto> Items);

public class GetPriceListItemsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/pricing/priceLists/{priceListId}/items", async ([FromRoute] Guid priceListId, ISender sender) =>
        {
            var result = await sender.Send(new GetPriceListItemsQuery(priceListId));
            return Results.Ok(result.Adapt<GetPriceListItemsResponse>());
        })
        .WithName("GetPriceListItems")
        .Produces<GetPriceListItemsResponse>(StatusCodes.Status200OK)
        .WithSummary("Get price list items")
        .WithDescription("Get price list items")
        .RequireAuthorization(PermissionList.PricingPermissions.View);
    }
}
