namespace Organization.Organizations.Features.Companies.GetCompanyById;

public record GetCompanyByIdResponse(CompanyDto Company);
public class GetCompanyByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/" + "{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetCompanyByIdQuery(id));
            return Results.Ok(result.Adapt<GetCompanyByIdResponse>());
        })
            .WithName("GetCompanyById")
            .Produces<GetCompanyByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("GetCompanyById")
            .WithDescription("GetCompanyById");
    }
}
