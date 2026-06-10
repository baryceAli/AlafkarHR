namespace Payroll.Salaries.Features.Components.CreateComponent;

public record CreateComponentRequest(ComponentDto Component);
public record CreateComponentResponse(Guid Id, string Name);

public class CreateComponentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/payroll/components", async (CreateComponentRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateComponentCommand>());
            return Results.Created($"/api/v1/payroll/components/{result.Id}", result.Adapt<CreateComponentResponse>());
        })
            .WithName("CreatePayrollComponent")
            .Produces<CreateComponentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Create Payroll Component")
            .WithDescription("Creates a payroll allowance or deduction component")
            .RequireAuthorization(PermissionList.PayrollContractPermissions.Create);
    }
}
