using Microsoft.AspNetCore.Mvc;

namespace Auth.Users.Features.Users.CreateCompanyUser;

public record CreateCompanyUserRequest(CreateCompanyUserDto User);
public record CreateCompanyUserResponse(Guid UserId, int AssignedRolesCount, int BranchAssignmentsCount);

public class CreateCompanyUserEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/auth/users/company/{companyId}", async ([FromRoute] Guid companyId, CreateCompanyUserRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateCompanyUserCommand(companyId, request.User));
            return Results.Ok(result.Adapt<CreateCompanyUserResponse>());
        })
            .WithName("CreateCompanyUser")
            .Produces<CreateCompanyUserResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Create company user")
            .WithDescription("Create a system user for the current company and optionally assign initial roles and branch access.")
            .RequireAuthorization(PermissionList.UsersPermissions.Create);
    }
}
