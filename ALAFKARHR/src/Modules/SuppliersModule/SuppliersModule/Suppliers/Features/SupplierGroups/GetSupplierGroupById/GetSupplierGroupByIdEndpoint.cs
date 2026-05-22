namespace SuppliersModule.Suppliers.Features.SupplierGroups.GetSupplierGroupById;

public record GetSupplierGroupByIdResponse(SupplierGroupDto SupplierGroup);

public class GetSupplierGroupByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/suppliers/supplier-group/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetSupplierGroupByIdQuery(id));
            return Results.Ok(result.Adapt<GetSupplierGroupByIdResponse>());
        })
            .WithName("GetSupplierGroupById")
            .Produces<GetSupplierGroupByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetSupplierGroupById")
            .WithDescription("GetSupplierGroupById")
            .RequireAuthorization(PermissionList.SupplierGroupPermissions.View);
    }
}
