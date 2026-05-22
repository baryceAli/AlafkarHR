namespace SuppliersModule.Suppliers.Features.Suppliers.GetSupplierById;

public record GetSupplierByIdResponse(SupplierDto Supplier);

public class GetSupplierByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/suppliers/supplier/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSupplierByIdQuery(id));
            return Results.Ok(result.Adapt<GetSupplierByIdResponse>());
        })
            .WithName("GetSupplierById")
            .Produces<GetSupplierByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetSupplierById")
            .WithDescription("GetSupplierById")
            .RequireAuthorization(PermissionList.SupplierPermissions.View);
    }
}
