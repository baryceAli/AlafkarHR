using Payroll.Salaries.Features.Contracts.CreateContract;

namespace Payroll.Salaries.Features.Contracts.UpdateContract;

public record UpdateContractRequest(
    Guid Id,
    string Name,
    string NameEng,
    string? Description,
    List<ContractItemDto> ContractItems);

public record UpdateContractResponse(Guid Id, string Name);

public class UpdateContractEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/payroll/contracts/{id}", async (Guid id, UpdateContractRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateContractCommand(
                id,
                request.Name,
                request.NameEng,
                request.Description,
                request.ContractItems));

            return Results.Ok(result.Adapt<UpdateContractResponse>());
        })
            .WithName("UpdateContract")
            .Produces<UpdateContractResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Salary Contract")
            .WithDescription("Updates an existing salary contract");
    }
}
