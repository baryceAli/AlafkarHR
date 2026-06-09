using Payroll.Salaries.Features.Contracts.CreateContract;

namespace Payroll.Salaries.Features.Contracts.GetContractById;

public record GetContractByIdResponse(
    Guid Id,
    string Name,
    string NameEng,
    string? Description,
    Guid CompanyId,
    List<ContractItemDto> Items);

public class GetContractByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/payroll/contracts/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetContractByIdQuery(id));
            return Results.Ok(result.Adapt<GetContractByIdResponse>());
        })
            .WithName("GetContractById")
            .Produces<GetContractByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Salary Contract")
            .WithDescription("Gets a salary contract by ID")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.View);
    }
}
