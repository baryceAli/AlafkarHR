using EmployeeModule.Employees.Config;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeesByCompanyId;


public record GetEmployeesByCompanyIdResponse(PaginatedResult<EmployeeDto> EmployeeList);
public class GetEmployeesByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/company" + "/{companyId}", async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeesByCompanyIdQuery(companyId, request));
            return Results.Ok(result.Adapt<GetEmployeesByCompanyIdResponse>());
        })
            .WithName("GetEmployeesByCompanyId")
            .Produces<GetEmployeesByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetEmployeesByCompanyId")
            .WithDescription("GetEmployeesByCompanyId");

    }
}
