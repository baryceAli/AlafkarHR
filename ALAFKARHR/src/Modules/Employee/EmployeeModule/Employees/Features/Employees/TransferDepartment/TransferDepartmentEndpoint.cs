namespace EmployeeModule.Employees.Features.Employees.TransferDepartment;


public record TransferDepartmentRequest(TransferDepartmentDto TransferDepartment);
public record TransferDepartmentResponse(bool IsSuccess);
public class TransferDepartmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/Transfer", async (TransferDepartmentRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<TransferDepartmentRequest>());
            return Results.Ok(result.Adapt<TransferDepartmentResponse>());
        })
            .WithName("TransferDepartment")
            .Produces<TransferDepartmentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("TransferDepartment")
            .WithDescription("TransferDepartment")
            .RequireAuthorization(PermissionList.EmployeePermissions.Edit);
    }
}
