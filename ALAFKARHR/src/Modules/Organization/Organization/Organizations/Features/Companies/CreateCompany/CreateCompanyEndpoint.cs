namespace Organization.Organizations.Features.Companies.CreateCompany;

public record CreateCompanyRequest(CompanyDto Company);
public record CreateCompanyResponse(CompanyDto CreatedCompany);

public class CreateCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}", async (CreateCompanyRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<CreateCompanyCommand>());
            return Results.Created(
                $"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/{result.CreatedCompany.Id}", 
                result.Adapt<CreateCompanyResponse>());
        })
            .WithName("CreateCompany")
            .Produces<CreateCompanyResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("CreateCompany")
            .WithDescription("CreateCompany")
            .RequireAuthorization(PermissionList.CompanyPermissions.Create);
    }
}
