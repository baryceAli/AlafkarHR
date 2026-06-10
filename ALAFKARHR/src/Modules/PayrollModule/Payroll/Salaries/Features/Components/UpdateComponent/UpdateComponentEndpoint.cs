namespace Payroll.Salaries.Features.Components.UpdateComponent;

public record UpdateComponentRequest(ComponentDto Component);
public record UpdateComponentResponse(Guid Id, string Name);

public class UpdateComponentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/payroll/components/{id}", async (Guid id, UpdateComponentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateComponentCommand(id, request.Component));
            return Results.Ok(result.Adapt<UpdateComponentResponse>());
        })
            .WithName("UpdatePayrollComponent")
            .Produces<UpdateComponentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Payroll Component")
            .WithDescription("Updates an existing payroll allowance or deduction component")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.Edit);
    }
}
