using MediatR;

namespace Catalog.Products.Features.Variants.UpdateVariant;

public record UpdateVariantRequest(VariantDto Variant);
public record UpdateVariantResponse(bool IsSuccess);

public class UpdateVariantEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/catalog/variants", async (UpdateVariantRequest request, ISender sender) =>
        {
            var command = request.Adapt<UpdateVariantCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<UpdateVariantResponse>();
            return Results.Ok(response);
        })
            .WithName("UpdateVariant")
            .Produces<UpdateVariantResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .WithSummary("Update Variant")
            .WithDescription("Update Variant")
            .RequireAuthorization($"{PermissionList.VariantPermissions.Edit}");
    }
}
