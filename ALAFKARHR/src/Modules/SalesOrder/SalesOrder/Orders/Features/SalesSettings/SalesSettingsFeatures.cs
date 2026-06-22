using SalesOrder.Orders.Features;
using SalesOrder.Orders.Models;

namespace SalesOrder.Orders.Features.SalesSettings;

public record GetSalesSettingsQuery(Guid CompanyId) : IQuery<GetSalesSettingsResult>;
public record GetSalesSettingsResult(SalesSettingsDto Settings);
public record UpdateSalesSettingsCommand(SalesSettingsDto Settings) : ICommand<UpdateSalesSettingsResult>;
public record UpdateSalesSettingsResult(bool IsSuccess);

public class GetSalesSettingsHandler(SalesOrderDbContext dbContext)
    : IQueryHandler<GetSalesSettingsQuery, GetSalesSettingsResult>
{
    public async Task<GetSalesSettingsResult> Handle(GetSalesSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await dbContext.SalesSettings.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == request.CompanyId, cancellationToken);

        return new GetSalesSettingsResult(settings is null
            ? new SalesSettingsDto { CompanyId = request.CompanyId }
            : new SalesSettingsDto
            {
                Id = settings.Id,
                CompanyId = settings.CompanyId,
                InvoicingPolicy = settings.InvoicingPolicy
            });
    }
}

public class UpdateSalesSettingsHandler(SalesOrderDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateSalesSettingsCommand, UpdateSalesSettingsResult>
{
    public async Task<UpdateSalesSettingsResult> Handle(UpdateSalesSettingsCommand request, CancellationToken cancellationToken)
    {
        var userId = SalesDocumentFeatureHelpers.CurrentUser(httpContextAccessor);
        var settings = await dbContext.SalesSettings
            .FirstOrDefaultAsync(x => x.CompanyId == request.Settings.CompanyId, cancellationToken);

        if (settings is null)
        {
            settings = Models.SalesSettings.Create(request.Settings.CompanyId, request.Settings.InvoicingPolicy, userId);
            await dbContext.SalesSettings.AddAsync(settings, cancellationToken);
        }
        else
        {
            settings.Update(request.Settings.InvoicingPolicy, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateSalesSettingsResult(true);
    }
}

public record UpdateSalesSettingsRequest(SalesSettingsDto Settings);

public class SalesSettingsEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/sales/settings/company/{companyId}", async (Guid companyId, ISender sender) =>
        {
            var result = await sender.Send(new GetSalesSettingsQuery(companyId));
            return Results.Ok(result);
        })
        .WithName("GetSalesSettings")
        .Produces<GetSalesSettingsResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.View);

        app.MapPut("/api/v1/sales/settings", async (UpdateSalesSettingsRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateSalesSettingsCommand(request.Settings));
            return Results.Ok(result);
        })
        .WithName("UpdateSalesSettings")
        .Produces<UpdateSalesSettingsResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.SalesOrderPermissions.Edit);
    }
}
