using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedWithUI.GeneralSettings.Dtos;
using SharedWithUI.Permissions;

namespace GeneralSettings.GeneralSettings.Features.Currencies.UpdateCurrency;

public record UpdateCurrencyRequest(CurrencyDto Currency);
public record UpdateCurrencyResponse(CurrencyDto Currency);

public class UpdateCurrencyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/Settings/company/{companyId}/currencies/{currencyId}",
            async ([FromRoute] Guid companyId, [FromRoute] Guid currencyId, UpdateCurrencyRequest request, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new UpdateCurrencyCommand(companyId, currencyId, request.Currency));
                return Results.Ok(result.Adapt<UpdateCurrencyResponse>());
            })
            .WithName("UpdateCurrency")
            .Produces<UpdateCurrencyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Currency")
            .WithDescription("Update Currency")
            .RequireAuthorization(PermissionList.SystemSettingsPermissions.Edit);
    }
}
