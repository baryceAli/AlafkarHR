namespace Pricing.Pricings.Features.PriceLists.UpdatePriceList;

public record UpdatePriceListRequest(PriceListDto PriceList);
public record UpdatePriceListResponse(bool IsSuccess);

public class UpdatePriceListEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/pricing/priceLists", async (UpdatePriceListRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdatePriceListCommand(request.PriceList));
            return Results.Ok(result.Adapt<UpdatePriceListResponse>());
        })
        .WithName("UpdatePriceList")
        .Produces<UpdatePriceListResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Update price list")
        .WithDescription("Update price list")
        .RequireAuthorization(PermissionList.PricingPermissions.Edit);
    }
}
