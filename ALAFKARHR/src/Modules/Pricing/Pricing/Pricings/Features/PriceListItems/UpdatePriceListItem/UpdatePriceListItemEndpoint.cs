namespace Pricing.Pricings.Features.PriceListItems.UpdatePriceListItem;

public record UpdatePriceListItemRequest(PriceListItemDto PriceListItem);
public record UpdatePriceListItemResponse(bool IsSuccess);

public class UpdatePriceListItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/pricing/priceListItems", async (UpdatePriceListItemRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdatePriceListItemCommand(request.PriceListItem));
            return Results.Ok(result.Adapt<UpdatePriceListItemResponse>());
        })
        .WithName("UpdatePriceListItem")
        .Produces<UpdatePriceListItemResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update price list item")
        .WithDescription("Update price list item")
        .RequireAuthorization(PermissionList.PricingPermissions.Edit);
    }
}
