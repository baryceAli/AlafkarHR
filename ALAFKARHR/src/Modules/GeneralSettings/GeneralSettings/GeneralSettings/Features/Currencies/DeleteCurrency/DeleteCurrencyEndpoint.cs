using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedWithUI.Permissions;

namespace GeneralSettings.GeneralSettings.Features.Currencies.DeleteCurrency;

public record DeleteCurrencyResponse(bool IsSuccess);

public class DeleteCurrencyEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/v1/Settings/company/{companyId}/currencies/{currencyId}",
            async ([FromRoute] Guid companyId, [FromRoute] Guid currencyId, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new DeleteCurrencyCommand(companyId, currencyId));
                return Results.Ok(result.Adapt<DeleteCurrencyResponse>());
            })
            .WithName("DeleteCurrency")
            .Produces<DeleteCurrencyResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Currency")
            .WithDescription("Delete Currency")
            .RequireAuthorization(PermissionList.SystemSettingsPermissions.Edit);
    }
}
