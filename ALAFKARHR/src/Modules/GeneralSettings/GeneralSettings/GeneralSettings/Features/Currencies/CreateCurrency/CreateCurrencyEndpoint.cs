using Carter;
using GeneralSettings.GeneralSettings.Features.Currencies.CreateCurrency;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedWithUI.GeneralSettings.Dtos;
using SharedWithUI.Permissions;

namespace GeneralSettings.GeneralSettings.Features.Currencies.CreateCurrency;

public record CreateCurrencyRequest(CurrencyDto Currency);
public record CreateCurrencyResponse(CurrencyDto Currency);

public class CreateCurrencyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/Settings/company/{companyId}/currencies",
            async ([FromRoute] Guid companyId, CreateCurrencyRequest request, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new CreateCurrencyCommand(companyId, request.Currency));
                return Results.Created($"/api/v1/Settings/company/{companyId}/currencies/{result.Currency.Id}", result.Adapt<CreateCurrencyResponse>());
            })
            .WithName("CreateCurrency")
            .Produces<CreateCurrencyResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Create Currency")
            .WithDescription("Create Currency")
            .RequireAuthorization(PermissionList.SystemSettingsPermissions.Edit);
    }
}
