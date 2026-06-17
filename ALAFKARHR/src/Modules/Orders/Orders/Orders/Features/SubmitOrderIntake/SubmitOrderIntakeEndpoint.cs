using Orders.Contracts.Orders.Features.SubmitOrderIntake;

namespace Orders.Orders.Features.SubmitOrderIntake;

public record SubmitOrderIntakeRequest(OrderIntakeDto Order);
public record SubmitOrderIntakeResponse(Guid Id, string Number);

public class SubmitOrderIntakeEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/orders/intakes", async (SubmitOrderIntakeRequest request, ISender sender) =>
        {
            var result = await sender.Send(new SubmitOrderIntakeCommand(request.Order));
            return Results.Created($"/api/v1/orders/intakes/{result.Id}", result.Adapt<SubmitOrderIntakeResponse>());
        })
        .WithName("SubmitOrderIntake")
        .Produces<SubmitOrderIntakeResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization(PermissionList.OrderIntakePermissions.Create);
    }
}
