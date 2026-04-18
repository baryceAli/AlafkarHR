using EmployeeModule.Employees.Config;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeesByAdministrationId;


public record GetEmployeesByAdministrationIdResponse(Guid AdministrationId,PaginatedResult<EmployeeDto> EmployeeList);
public class GetEmployeesByAdministrationIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/administration" + "/{administrationId}", async ([FromRoute] Guid administrationId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeesByAdministrationIdQuery(administrationId, request));
            return Results.Ok(result.Adapt<GetEmployeesByAdministrationIdResponse>());
        })
            .WithName("GetEmployeesByAdministrationId")
            .Produces<GetEmployeesByAdministrationIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetEmployeesByAdministrationId")
            .WithDescription("GetEmployeesByAdministrationId");
    }
}
