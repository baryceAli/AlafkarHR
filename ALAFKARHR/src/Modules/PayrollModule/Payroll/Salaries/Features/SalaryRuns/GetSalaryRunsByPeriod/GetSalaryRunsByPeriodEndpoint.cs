namespace Payroll.Salaries.Features.SalaryRuns.GetSalaryRunsByPeriod;

public record GetSalaryRunsByPeriodResponse(List<SalaryRunPeriodDto> SalaryRunList);

public class GetSalaryRunsByPeriodEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/payroll/salary-runs/company/{companyId}/period", async (
            Guid companyId,
            int month,
            int year,
            ISender sender) =>
        {
            var result = await sender.Send(new GetSalaryRunsByPeriodQuery(companyId, month, year));
            return Results.Ok(result.Adapt<GetSalaryRunsByPeriodResponse>());
        })
            .WithName("GetSalaryRunsByPeriod")
            .Produces<GetSalaryRunsByPeriodResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Get Salary Runs By Period")
            .WithDescription("Gets generated salary runs for a company month and year")
            .RequireAuthorization(PermissionList.SalaryRunPermissions.View);
    }
}
