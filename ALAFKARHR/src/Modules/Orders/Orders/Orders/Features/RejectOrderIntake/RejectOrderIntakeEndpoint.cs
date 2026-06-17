namespace Orders.Orders.Features.RejectOrderIntake;

public record RejectOrderIntakeRequest(string Reason);
public record RejectOrderIntakeResponse(bool IsSuccess);

public class RejectOrderIntakeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/orders/intakes/{id}/reject", async (Guid id, RejectOrderIntakeRequest request, ISender sender) =>
        {
            var result = await sender.Send(new RejectOrderIntakeCommand(id, request.Reason));
            return Results.Ok(result.Adapt<RejectOrderIntakeResponse>());
        })
        .WithName("RejectOrderIntake")
        .Produces<RejectOrderIntakeResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization(PermissionList.OrderIntakePermissions.Reject);
    }
}
