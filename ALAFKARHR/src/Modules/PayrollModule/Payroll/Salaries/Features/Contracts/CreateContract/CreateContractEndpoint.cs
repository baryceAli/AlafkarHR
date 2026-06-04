
namespace Payroll.Salaries.Features.Contracts.CreateContract;

public record CreateContractRequest(
    string Name,
    string NameEng,
    string? Description,
    Guid CompanyId,
    List<ContractItemDto> ContractItems);

public record CreateContractResponse(Guid Id, string Name);

public class CreateContractEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/contracts", async (CreateContractRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateContractCommand>());
            return Results.Created($"/api/v1/payroll/contracts/{result.Id}", result.Adapt<CreateContractResponse>());
        })
            .WithName("CreateContract")
            .Produces<CreateContractResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Create Salary Contract")
            .WithDescription("Creates a new salary contract with associated components");
    }
}
