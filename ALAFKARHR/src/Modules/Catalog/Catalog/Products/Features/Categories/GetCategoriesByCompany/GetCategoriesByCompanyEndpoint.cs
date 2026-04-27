using Catalog.Products.Features.Categories.GetCategories;
using MediatR;

namespace Catalog.Products.Features.Categories.GetCategoriesByCompany;


public record GetCategoriesByCompanyResponse(PaginatedResult<CategoryDto> CategoryList);
public class GetCategoriesByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/catalog/categories/company/{companyId}", async ([FromRoute]Guid companyId,[AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetCategoriesByCompanyQuery(companyId,request));

            var response = result.Adapt<GetCategoriesByCompanyResponse>();

            return Results.Ok(response);
        })
            .WithName("GetCategoriesByCompany")
            .Produces<GetCategoriesByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Categories By Company")
            .WithDescription("Get Categories By Company");
    }
}
