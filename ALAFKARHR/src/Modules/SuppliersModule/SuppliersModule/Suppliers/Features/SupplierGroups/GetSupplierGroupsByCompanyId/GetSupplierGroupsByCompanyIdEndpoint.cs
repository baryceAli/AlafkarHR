using Microsoft.AspNetCore.Mvc;

namespace SuppliersModule.Suppliers.Features.SupplierGroups.GetSupplierGroupsByCompanyId;

public record GetSupplierGroupsByCompanyIdResponse(List<SupplierGroupDto> SupplierGroups);

public class GetSupplierGroupsByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/suppliers/supplier-group/company/{companyId}", async ([FromRoute] Guid companyId, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetSupplierGroupsByCompanyIdQuery(companyId));
            return Results.Ok(result.Adapt<GetSupplierGroupsByCompanyIdResponse>());
        })
            .WithName("GetSupplierGroupsByCompanyId")
            .Produces<GetSupplierGroupsByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetSupplierGroupsByCompanyId")
            .WithDescription("GetSupplierGroupsByCompanyId")
            .RequireAuthorization(PermissionList.SupplierGroupPermissions.View);
    }
}
