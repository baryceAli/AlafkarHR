using Microsoft.AspNetCore.Mvc;

namespace SuppliersModule.Suppliers.Features.Suppliers.RemoveSupplier;

public record RemoveSupplierResponse(bool IsSuccess);

public class RemoveSupplierEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/suppliers/supplier/{id}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new RemoveSupplierCommand(id));
            return Results.Ok(result.Adapt<RemoveSupplierResponse>());
        })
            .WithName("RemoveSupplier")
            .Produces<RemoveSupplierResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("RemoveSupplier")
            .WithDescription("RemoveSupplier")
            .RequireAuthorization(PermissionList.SupplierPermissions.Delete);
    }
}
