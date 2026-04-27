using Catalog.Products.Features.Units.GetUnits;
using MediatR;

namespace Catalog.Products.Features.Units.GetUnitsByComapny;


public record GetUnitsByComapnyResponse(PaginatedResult<UnitDto> UnitList);
public class GetUnitsByComapnyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/units/company/{companyId}", async ([FromRoute]Guid companyId,[AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetUnitsByComapnyQuery(companyId,request));
            var response = result.Adapt<GetUnitsByComapnyResponse>();
            return Results.Ok(response);
        })
            .WithName("GetUnitsByComapny")
            .Produces<GetUnitsByComapnyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Units by Company")
            .WithDescription("Get Units by Company")
            .RequireAuthorization(PermissionList.UnitPermissions.View);
    }
}
