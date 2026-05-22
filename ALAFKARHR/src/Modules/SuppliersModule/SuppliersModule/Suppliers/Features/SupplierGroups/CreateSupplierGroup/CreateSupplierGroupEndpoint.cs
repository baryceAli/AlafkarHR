namespace SuppliersModule.Suppliers.Features.SupplierGroups.CreateSupplierGroup;

public record CreateSupplierGroupRequest(SupplierGroupDto SupplierGroup);
public record CreateSupplierGroupResponse(Guid Id);

public class CreateSupplierGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/suppliers/supplier-group", async (CreateSupplierGroupRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateSupplierGroupCommand(request.SupplierGroup));
            return Results.Created($"/api/v1/suppliers/supplier-group/{result.Id}", result.Adapt<CreateSupplierGroupResponse>());
        })
            .WithName("CreateSupplierGroup")
            .Produces<CreateSupplierGroupResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateSupplierGroup")
            .WithDescription("CreateSupplierGroup")
            .RequireAuthorization(PermissionList.SupplierGroupPermissions.Create);
    }
}
