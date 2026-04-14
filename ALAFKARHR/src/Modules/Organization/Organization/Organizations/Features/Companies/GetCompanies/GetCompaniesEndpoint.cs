
namespace Organization.Organizations.Features.Companies.GetCompanies;

//public record GetCompaniesRequest(PaginationRequest PaginationRequest);
public record GetCompaniesResponse(PaginatedResult<CompanyDto> CompanyList);

public class GetCompaniesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}", async ([AsParameters]PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetCompaniesQuery(request));
            return Results.Ok(result.Adapt<GetCompaniesResponse>());
        })
            .WithName("GetCompanies")
            .Produces<GetCompaniesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetCompanies")
            .WithDescription("GetCompanies");
    }
}
