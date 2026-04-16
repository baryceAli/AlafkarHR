using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace EmployeeModule.Employees.Features.Employees.GetEmployeesByBranchId;



public record GetEmployeesByBranchIdResponse(PaginatedResult<EmployeeDto> EmployeeList);
public class GetEmployeesByBranchIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/Branch" + "/branchId", async ([FromRoute] Guid branchId, [AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeesByBranchIdQuery(branchId, request));
            return Results.Ok(result.Adapt<GetEmployeesByBranchIdResponse>());
        })
            .WithName("GetEmployeesByBranchId")
            .Produces<GetEmployeesByBranchIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetEmployeesByBranchId")
            .WithDescription("GetEmployeesByBranchId");
    }
}
