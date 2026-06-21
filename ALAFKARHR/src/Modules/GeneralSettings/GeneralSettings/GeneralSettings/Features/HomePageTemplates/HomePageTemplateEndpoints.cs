using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.HomePageTemplates;

public record GetHomePageTemplateResponse(HomePageTemplateDto HomePage);
public record UpdateHomePageActiveTemplateRequest(UpdateHomePageActiveTemplateDto ActiveTemplate);
public record UpdateHomePageTemplateContentRequest(List<HomePageContentItemDto> ContentItems);

public class HomePageTemplateEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/Settings/public/homepage",
            async ([FromQuery] Guid companyId, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetPublicHomePageTemplateQuery(companyId));
                return Results.Ok(result.Adapt<GetHomePageTemplateResponse>());
            })
            .WithName("GetPublicHomePageTemplate")
            .Produces<GetHomePageTemplateResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

        app.MapGet("/api/v1/Settings/company/{companyId}/homepage",
            async ([FromRoute] Guid companyId, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new GetCompanyHomePageTemplateQuery(companyId));
                return Results.Ok(result.Adapt<GetHomePageTemplateResponse>());
            })
            .WithName("GetCompanyHomePageTemplate")
            .Produces<GetHomePageTemplateResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.SystemSettingsPermissions.View);

        app.MapPut("/api/v1/Settings/company/{companyId}/homepage/active-template",
            async ([FromRoute] Guid companyId, UpdateHomePageActiveTemplateRequest request, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new UpdateHomePageActiveTemplateCommand(companyId, request.ActiveTemplate.ActiveTemplateKey));
                return Results.Ok(result.Adapt<GetHomePageTemplateResponse>());
            })
            .WithName("UpdateHomePageActiveTemplate")
            .Produces<GetHomePageTemplateResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.SystemSettingsPermissions.Edit);

        app.MapPut("/api/v1/Settings/company/{companyId}/homepage/{templateKey}/content",
            async ([FromRoute] Guid companyId, [FromRoute] string templateKey, UpdateHomePageTemplateContentRequest request, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new UpdateHomePageTemplateContentCommand(companyId, templateKey, request.ContentItems));
                return Results.Ok(result.Adapt<GetHomePageTemplateResponse>());
            })
            .WithName("UpdateHomePageTemplateContent")
            .Produces<GetHomePageTemplateResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.SystemSettingsPermissions.Edit);
    }
}

