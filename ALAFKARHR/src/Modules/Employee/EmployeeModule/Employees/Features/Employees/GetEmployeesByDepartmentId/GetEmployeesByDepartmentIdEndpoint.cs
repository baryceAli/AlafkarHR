using EmployeeModule.Employees.Config;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeesByDepartmentId;


public record GetEmployeesByDepartmentIdResponse(PaginatedResult<EmployeeDto> EmployeeList);
public class GetEmployeesByDepartmentIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/department" + "/{departmentId}", async ([FromRoute] Guid departmentId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeesByDepartmentIdQuery(departmentId, request));
            return Results.Ok(result.Adapt<GetEmployeesByDepartmentIdResult>());
        })
            .WithName("GetEmployeesByDepartmentId")
            .Produces<GetEmployeesByDepartmentIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetEmployeesByDepartmentId")
            .WithDescription("GetEmployeesByDepartmentId");
    }
}
