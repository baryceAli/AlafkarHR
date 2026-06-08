using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedWithUI.GeneralSettings.Dtos;
using SharedWithUI.Permissions;

namespace GeneralSettings.GeneralSettings.Features.CompanySettings.UpdateCompanySetting;

public record UpdateCompanySettingRequest(CompanySettingDto CompanySetting);
public record UpdateCompanySettingResponse(CompanySettingDto CompanySetting);

public class UpdateCompanySettingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/v1/Settings/company/{companyId}/setting",
            async ([FromRoute] Guid companyId, UpdateCompanySettingRequest request, [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new UpdateCompanySettingCommand(companyId, request.CompanySetting));
                return Results.Ok(result.Adapt<UpdateCompanySettingResponse>());
            })
            .WithName("UpdateCompanySetting")
            .Produces<UpdateCompanySettingResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithSummary("Update Company Setting")
            .WithDescription("Update Company Setting")
            .RequireAuthorization(PermissionList.SystemSettingsPermissions.Edit);
    }
}
