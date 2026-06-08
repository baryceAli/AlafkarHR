using Carter;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.CompanySettings.GetCompanySetting;

public record GetCompanySettingResponse(CompanySettingDto CompanySetting);

public class GetCompanySettingEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/Settings/company/{companyId}/setting", async ([FromRoute] Guid companyId, [FromServices] ISender sender) =>
        {
            var result = await sender.Send(new GetCompanySettingQuery(companyId));
            return Results.Ok(result.Adapt<GetCompanySettingResponse>());
        })
        .WithName("GetCompanySetting")
        .Produces<GetCompanySettingResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Company Setting")
        .WithDescription("Get Company Setting");
    }
}
