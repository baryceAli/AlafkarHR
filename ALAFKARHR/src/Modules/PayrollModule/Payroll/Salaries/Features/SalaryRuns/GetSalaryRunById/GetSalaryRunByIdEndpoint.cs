namespace Payroll.Salaries.Features.SalaryRuns.GetSalaryRunById;

public record GetSalaryRunByIdResponse(
    Guid Id,
    Guid EmployeeId,
    Guid ContractId,
    int SalaryMonth,
    int SalaryYear,
    decimal TotalSalary,
    decimal TotalAllowances,
    decimal TotalDeductions,
    decimal NetSalary,
    string Status);

public class GetSalaryRunByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/payroll/salary-runs/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSalaryRunByIdQuery(id));
            return Results.Ok(result.Adapt<GetSalaryRunByIdResponse>());
        })
            .WithName("GetSalaryRunById")
            .Produces<GetSalaryRunByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Salary Run")
            .WithDescription("Gets a salary run by ID")
            .RequireAuthorization(PermissionList.SalaryRunPermissions.View);
    }
}
