using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Pagination;
using SharedWithUI.GeneralSettings.Dtos;
using SharedWithUI.Permissions;

namespace GeneralSettings.GeneralSettings.Features.Currencies.GetCurrencies;

public record GetCurrenciesResponse(PaginatedResult<CurrencyDto> CurrencyList);
public class GetCurrenciesEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/Settings/company/{companyId}", async ([FromRoute] Guid companyId, [AsParameters] PaginationRequest request, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetCurrenciesQuery(companyId, request));
            var ss=result.Adapt<GetCurrenciesResponse>();
            return Results.Ok(result.Adapt<GetCurrenciesResponse>());
        })
            .WithName("GetCurrencies")
            .Produces<GetCurrenciesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Currencies")
            .WithDescription("Get Currencies");
            //.RequireAuthorization(PermissionList.g)
    }
}
