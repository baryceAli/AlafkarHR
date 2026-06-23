namespace Maintenance.WorkOrders.Features;

public record UploadMaintenanceAttachmentCommand(Guid WorkOrderId, IFormFile File) : ICommand<MaintenanceCreateResult>;

public class UploadMaintenanceAttachmentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/v1/maintenance/work-orders/{id:guid}/attachments", async (Guid id, IFormFile file, ISender sender) =>
        {
            var result = await sender.Send(new UploadMaintenanceAttachmentCommand(id, file));
            return Results.Created($"/api/v1/maintenance/work-orders/{id}", result);
        })
        .WithName("UploadMaintenanceAttachment")
        .Produces<MaintenanceCreateResult>(StatusCodes.Status201Created)
        .DisableAntiforgery()
        .RequireAuthorization(PermissionList.MaintenanceWorkOrderPermissions.Edit);
    }
}

public class UploadMaintenanceAttachmentHandler(
    MaintenanceDbContext dbContext,
    IWebHostEnvironment environment,
    IHttpContextAccessor httpContextAccessor,
    ISender sender)
    : ICommandHandler<UploadMaintenanceAttachmentCommand, MaintenanceCreateResult>
{
    public async Task<MaintenanceCreateResult> Handle(UploadMaintenanceAttachmentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = MaintenanceFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        if (request.File.Length <= 0)
            throw new BadRequestException("Attachment file is required.");

        var workOrder = await dbContext.MaintenanceWorkOrders.Include(x => x.Asset).FirstOrDefaultAsync(x => x.Id == request.WorkOrderId, cancellationToken)
            ?? throw new NotFoundException("Maintenance work order", request.WorkOrderId);
        await MaintenanceFeatureHelpers.EnsureCanMutateWorkOrderAsync(sender, workOrder, cancellationToken);

        var uploadsRoot = Path.Combine(environment.ContentRootPath, "wwwroot", "uploads", "maintenance");
        Directory.CreateDirectory(uploadsRoot);
        var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(request.File.FileName)}";
        var fullPath = Path.Combine(uploadsRoot, safeFileName);

        await using (var stream = File.Create(fullPath))
        {
            await request.File.CopyToAsync(stream, cancellationToken);
        }

        var relativePath = $"/uploads/maintenance/{safeFileName}";
        var attachment = MaintenanceAttachment.Create(
            workOrder.Id,
            request.File.FileName,
            request.File.ContentType,
            relativePath,
            request.File.Length,
            currentUserId);

        workOrder.AddAttachment(attachment);
        MaintenanceFeatureHelpers.AddHistory(workOrder, "AttachmentUploaded", request.File.FileName, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new MaintenanceCreateResult(attachment.Id);
    }
}
