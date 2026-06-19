using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Pagination;
using SharedWithUI.GeneralSettings.Dtos;
using SharedWithUI.Permissions;

namespace GeneralSettings.GeneralSettings.Features.Currencies.GetAvailableCurrencies;

public record GetAvailableCurrenciesResponse(PaginatedResult<CurrencyDto> CurrencyList);

public class GetAvailableCurrenciesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/Settings/currencies/available", async ([AsParameters] PaginationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new GetAvailableCurrenciesQuery(request));
            return Results.Ok(result.Adapt<GetAvailableCurrenciesResponse>());
        })
            .WithName("GetAvailableCurrencies")
            .Produces<GetAvailableCurrenciesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Available Currencies")
            .WithDescription("Get currencies available to platform parent-company administration")
            .RequireAuthorization(PermissionList.ParentCompanyPermissions.View);
    }
}
