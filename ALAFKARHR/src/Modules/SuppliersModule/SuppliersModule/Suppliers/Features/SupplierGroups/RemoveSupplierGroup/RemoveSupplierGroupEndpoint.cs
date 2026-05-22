using Microsoft.AspNetCore.Mvc;

namespace SuppliersModule.Suppliers.Features.SupplierGroups.RemoveSupplierGroup;

public record RemoveSupplierGroupResponse(bool IsSuccess);

public class RemoveSupplierGroupEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/suppliers/supplier-group/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveSupplierGroupCommand(id));
            return Results.Ok(result.Adapt<RemoveSupplierGroupResponse>());
        })
            .WithName("RemoveSupplierGroup")
            .Produces<RemoveSupplierGroupResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("RemoveSupplierGroup")
            .WithDescription("RemoveSupplierGroup")
            .RequireAuthorization(PermissionList.SupplierGroupPermissions.Delete);
    }
}
