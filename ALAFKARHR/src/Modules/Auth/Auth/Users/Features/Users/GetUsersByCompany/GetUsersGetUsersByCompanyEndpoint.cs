using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace Auth.Users.Features.Users.GetUsersByCompany;


public record GetUsersGetUsersByCompanyResponse(PaginatedResult<UserDto> UserList);
public class GetUsersGetUsersByCompanyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/Users/company/{companyId}", async ([FromRoute]Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetUsersGetUsersByCompanyQuery(companyId,request));
            return Results.Ok(result.Adapt<GetUsersGetUsersByCompanyResult>());
        })
            .WithName("GetUsers")
            .Produces<GetUsersGetUsersByCompanyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetUsers")
            .WithDescription("GetUsers");
    }
}
