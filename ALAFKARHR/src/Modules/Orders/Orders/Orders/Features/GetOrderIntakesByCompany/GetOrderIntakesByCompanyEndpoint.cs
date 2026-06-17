namespace Orders.Orders.Features.GetOrderIntakesByCompany;

public record GetOrderIntakesByCompanyResponse(PaginatedResult<OrderIntakeDto> Orders);

public class GetOrderIntakesByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/orders/intakes/company/{companyId}", async (Guid companyId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetOrderIntakesByCompanyQuery(companyId, request));
            return Results.Ok(result.Adapt<GetOrderIntakesByCompanyResponse>());
        })
        .WithName("GetOrderIntakesByCompany")
        .Produces<GetOrderIntakesByCompanyResponse>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.OrderIntakePermissions.View);
    }
}
