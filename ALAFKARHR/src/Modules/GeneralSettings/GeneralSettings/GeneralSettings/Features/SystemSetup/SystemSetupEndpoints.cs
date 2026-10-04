using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedWithUI.GeneralSettings.Dtos;
using SharedWithUI.Permissions;

namespace GeneralSettings.GeneralSettings.Features.SystemSetup;

public record GetCurrentSystemSetupResponse(SetupSummaryDto Setup);
public record UpdateCurrentSystemSetupRequest(UpdateSetupProgressDto Progress);
public record UpdateCurrentSystemSetupResponse(SetupSummaryDto Setup);

public sealed class SystemSetupEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/GeneralSettings/setup/current", async (ISender sender) =>
            {
                var result = await sender.Send(new GetCurrentSystemSetupQuery());
                return Results.Ok(new GetCurrentSystemSetupResponse(result.Setup));
            })
            .WithName("GetCurrentSystemSetup")
            .Produces<GetCurrentSystemSetupResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get current company setup readiness")
            .RequireAuthorization(PermissionList.SystemSetupPermissions.View);

        app.MapPut("/api/v1/GeneralSettings/setup/current/progress", async (UpdateCurrentSystemSetupRequest request, ISender sender) =>
            {
                var result = await sender.Send(new UpdateCurrentSystemSetupCommand(request.Progress));
                return Results.Ok(new UpdateCurrentSystemSetupResponse(result.Setup));
            })
            .WithName("UpdateCurrentSystemSetupProgress")
            .Produces<UpdateCurrentSystemSetupResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update current company setup progress")
            .RequireAuthorization(PermissionList.SystemSetupPermissions.Manage);
    }
}
