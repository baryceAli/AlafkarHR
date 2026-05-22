namespace SuppliersModule.Suppliers.Features.Suppliers.UpdateSupplier;

public record UpdateSupplierRequest(SupplierDto Supplier);
public record UpdateSupplierResponse(bool IsSuccess);

public class UpdateSupplierEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/suppliers/supplier", async (UpdateSupplierRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateSupplierCommand>());
            return Results.Ok(result.Adapt<UpdateSupplierResponse>());
        })
            .WithName("UpdateSupplier")
            .Produces<UpdateSupplierResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateSupplier")
            .WithDescription("UpdateSupplier")
            .RequireAuthorization(PermissionList.SupplierPermissions.Edit);
    }
}
