namespace Payroll.Salaries.Features.Contracts.DeleteContract;

public record DeleteContractResponse(bool IsSuccess);

public class DeleteContractEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/payroll/contracts/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteContractCommand(id));
            return Results.Ok(result.Adapt<DeleteContractResponse>());
        })
            .WithName("DeletePayrollContract")
            .Produces<DeleteContractResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Payroll Contract")
            .WithDescription("Soft deletes a payroll contract")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.Delete);
    }
}
