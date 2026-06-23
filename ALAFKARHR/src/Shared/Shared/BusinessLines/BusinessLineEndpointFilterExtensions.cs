using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Contracts.Organization;

namespace Shared.BusinessLines;

public static class BusinessLineEndpointFilterExtensions
{
    public static RouteGroupBuilder RequireBusinessLine(this RouteGroupBuilder builder, string businessLineKey)
    {
        builder.AddEndpointFilter((context, next) => CheckAsync(context, next, businessLineKey));
        return builder;
    }

    private static async ValueTask<object?> CheckAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next, string businessLineKey)
    {
        var service = context.HttpContext.RequestServices.GetService(typeof(IBusinessLineEntitlementService)) as IBusinessLineEntitlementService;
        if (service is null || !await service.IsBusinessLineLicensedAsync(businessLineKey, context.HttpContext.RequestAborted))
        {
            return Results.Forbid();
        }

        return await next(context);
    }
}
