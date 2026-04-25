using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Catalog.Products.Features.Variants.CreateVariant;


public record CreateVariantRequest(VariantDto Variant);
public record CreateVariantResponse(Guid Id);
public class CreateVariantEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/catalog/variants", async (CreateVariantRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateVariantCommand>();
            var result = await sender.Send(command);
            var response = result.Adapt<CreateVariantResponse>();
            return Results.Created($"/api/vi/catalog/variants/{response.Id}", response);
        })
            .WithName("CreateVariant")
            .Produces<CreateVariantResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Variant")
            .WithDescription("Create Variant")
            .RequireAuthorization($"{PermissionList.VariantPermissions.Create}");
    }
}
