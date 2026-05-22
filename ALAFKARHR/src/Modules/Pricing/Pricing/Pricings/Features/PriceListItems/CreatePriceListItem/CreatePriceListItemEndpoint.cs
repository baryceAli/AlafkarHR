namespace Pricing.Pricings.Features.PriceListItems.CreatePriceListItem;

public record CreatePriceListItemRequest(PriceListItemDto PriceListItem);
public record CreatePriceListItemResponse(Guid Id);

public class CreatePriceListItemEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/pricing/priceListItems", async (CreatePriceListItemRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreatePriceListItemCommand(request.PriceListItem));
            return Results.Created($"/api/v1/pricing/priceListItems/{result.Id}", result.Adapt<CreatePriceListItemResponse>());
        })
        .WithName("CreatePriceListItem")
        .Produces<CreatePriceListItemResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create price list item")
        .WithDescription("Create price list item")
        .RequireAuthorization(PermissionList.PricingPermissions.Create);
    }
}
