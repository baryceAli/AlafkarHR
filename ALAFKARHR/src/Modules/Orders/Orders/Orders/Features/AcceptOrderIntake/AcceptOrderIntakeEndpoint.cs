using Orders.Contracts.Orders.Features.AcceptOrderIntake;

namespace Orders.Orders.Features.AcceptOrderIntake;

public record AcceptOrderIntakeResponse(Guid Id, Guid SalesOrderId);

public class AcceptOrderIntakeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/orders/intakes/{id}/accept", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new AcceptOrderIntakeCommand(id));
            return Results.Ok(result.Adapt<AcceptOrderIntakeResponse>());
        })
        .WithName("AcceptOrderIntake")
        .Produces<AcceptOrderIntakeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(PermissionList.OrderIntakePermissions.Accept);
    }
}
