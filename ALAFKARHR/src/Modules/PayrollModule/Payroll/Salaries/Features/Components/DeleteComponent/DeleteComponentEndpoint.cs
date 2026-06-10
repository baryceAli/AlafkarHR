namespace Payroll.Salaries.Features.Components.DeleteComponent;

public record DeleteComponentResponse(bool IsSuccess);

public class DeleteComponentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/payroll/components/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteComponentCommand(id));
            return Results.Ok(result.Adapt<DeleteComponentResponse>());
        })
            .WithName("DeletePayrollComponent")
            .Produces<DeleteComponentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Payroll Component")
            .WithDescription("Soft deletes a payroll component")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.Delete);
    }
}
