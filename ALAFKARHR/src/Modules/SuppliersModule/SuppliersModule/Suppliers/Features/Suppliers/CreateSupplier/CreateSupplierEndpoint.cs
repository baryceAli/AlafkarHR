namespace SuppliersModule.Suppliers.Features.Suppliers.CreateSupplier;

public record CreateSupplierRequest(SupplierDto Supplier);
public record CreateSupplierResponse(Guid Id);

public class CreateSupplierEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/suppliers/supplier", async (CreateSupplierRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateSupplierCommand(request.Supplier));
            return Results.Created($"/api/v1/suppliers/supplier/{result.Id}", result.Adapt<CreateSupplierResponse>());
        })
            .WithName("CreateSupplier")
            .Produces<CreateSupplierResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateSupplier")
            .WithDescription("CreateSupplier")
            .RequireAuthorization(PermissionList.SupplierPermissions.Create);
    }
}
