using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace SuppliersModule.Suppliers.Features.Suppliers.GetSuppliersByCompanyId;

public record GetSuppliersByCompanyIdResponse(PaginatedResult<SupplierDto> SupplierList);

public class GetSuppliersByCompanyIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/suppliers/supplier/company/{companyId}",
            async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetSuppliersByCompanyIdQuery(companyId, request));
                return Results.Ok(result.Adapt<GetSuppliersByCompanyIdResponse>());
            })
            .WithName("GetSuppliersByCompanyId")
            .Produces<GetSuppliersByCompanyIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetSuppliersByCompanyId")
            .WithDescription("GetSuppliersByCompanyId")
            .RequireAuthorization(PermissionList.SupplierPermissions.View);
    }
}
