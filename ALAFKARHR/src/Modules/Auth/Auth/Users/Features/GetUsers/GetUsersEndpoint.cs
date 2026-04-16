using Auth.Contracts.Dtos;
using Microsoft.AspNetCore.Mvc;
using Shared.Pagination;

namespace Auth.Users.Features.GetUsers;


public record GetUsersResponse(PaginatedResult<UserDto> UserList);
public class GetUsersEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/auth/users", async ([AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetUsersQuery(request));
            return Results.Ok(result.Adapt<GetUsersResult>());
        })
            .WithName("GetUsers")
            .Produces<GetUsersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("GetUsers")
            .WithDescription("GetUsers");
    }
}
