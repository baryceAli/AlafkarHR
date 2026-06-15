using Organization.Organizations.Features.Companies.CreateCompany;

namespace Organization.Organizations.Features.Companies.CreateChildCompany;

public record CreateChildCompanyRequest(CompanyDto Company);
public record CreateChildCompanyResponse(CompanyDto CreatedCompany);

public class CreateChildCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/child-companies", async (CreateChildCompanyRequest request, IHttpContextAccessor httpContextAccessor, ISender sender) =>
        {
            var companyIdValue = httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
            if (!Guid.TryParse(companyIdValue, out var companyId))
                return Results.Unauthorized();

            request.Company.ParentCompanyId = companyId;
            var result = await sender.Send(new CreateCompanyCommand(request.Company));
            return Results.Created($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/child-companies/{result.CreatedCompany.Id}", result.Adapt<CreateChildCompanyResponse>());
        })
            .WithName("CreateChildCompany")
            .Produces<CreateChildCompanyResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Create child company")
            .WithDescription("Create child company")
            .RequireAuthorization(PermissionList.CompanyPermissions.CreateChild);
    }
}
