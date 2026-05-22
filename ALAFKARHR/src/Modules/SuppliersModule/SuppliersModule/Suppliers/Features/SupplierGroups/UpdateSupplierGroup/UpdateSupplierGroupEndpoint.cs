namespace SuppliersModule.Suppliers.Features.SupplierGroups.UpdateSupplierGroup;

public record UpdateSupplierGroupRequest(SupplierGroupDto SupplierGroup);
public record UpdateSupplierGroupResponse(bool IsSuccess);

public class UpdateSupplierGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/suppliers/supplier-group", async (UpdateSupplierGroupRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateSupplierGroupCommand>());
            return Results.Ok(result.Adapt<UpdateSupplierGroupResponse>());
        })
            .WithName("UpdateSupplierGroup")
            .Produces<UpdateSupplierGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("UpdateSupplierGroup")
            .WithDescription("UpdateSupplierGroup")
            .RequireAuthorization(PermissionList.SupplierGroupPermissions.Edit);
    }
}
