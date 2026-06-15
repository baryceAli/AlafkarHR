namespace Organization.Organizations.Features.Companies.ResetChildCompanyAdminPassword;

public record ResetChildCompanyAdminPasswordRequest(string TemporaryPassword);
public record ResetChildCompanyAdminPasswordResponse(bool IsSuccess);

public class ResetChildCompanyAdminPasswordEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost($"{Utils.ROUTE_PATTERN}/{Utils.CompanyEndpoint}/child-companies/{{companyId:guid}}/admin/reset-password", async ([FromRoute] Guid companyId, ResetChildCompanyAdminPasswordRequest request, ISender sender) =>
        {
            var result = await sender.Send(new ResetChildCompanyAdminPasswordCommand(companyId, request.TemporaryPassword));
            return Results.Ok(result.Adapt<ResetChildCompanyAdminPasswordResponse>());
        })
            .WithName("ResetChildCompanyAdminPassword")
            .Produces<ResetChildCompanyAdminPasswordResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Reset child company admin password")
            .WithDescription("Reset child company admin password")
            .RequireAuthorization(PermissionList.CompanyPermissions.ResetChildAdminPassword);
    }
}
