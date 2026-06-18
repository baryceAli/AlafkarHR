namespace Fleet.Features;

public record CreateFleetVehicleDocumentRequest(CreateFleetVehicleDocumentDto Document);
public record UpdateFleetVehicleDocumentRequest(UpdateFleetVehicleDocumentDto Document);
public record RenewFleetVehicleDocumentRequest(RenewFleetVehicleDocumentDto Renewal);
public record CreateFleetVehicleDocumentCommand(CreateFleetVehicleDocumentDto Document) : ICommand<CreateFleetVehicleDocumentResult>;
public record UpdateFleetVehicleDocumentCommand(UpdateFleetVehicleDocumentDto Document) : ICommand<FleetActionResult>;
public record DeleteFleetVehicleDocumentCommand(Guid Id) : ICommand<FleetActionResult>;
public record RenewFleetVehicleDocumentCommand(Guid Id, RenewFleetVehicleDocumentDto Renewal) : ICommand<FleetActionResult>;
public record GetFleetVehicleDocumentsQuery(PaginationRequest PaginationRequest, Guid? VehicleId, FleetDocumentType? DocumentType, FleetDocumentStatus? Status) : IQuery<GetFleetVehicleDocumentsResult>;
public record CreateFleetVehicleDocumentResult(Guid Id);
public record GetFleetVehicleDocumentsResult(PaginatedResult<FleetVehicleDocumentDto> Documents);

public class FleetVehicleDocumentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/fleet/documents", async (int PageIndex, int PageSize, string? searchText, Guid? vehicleId, FleetDocumentType? documentType, FleetDocumentStatus? status, ISender sender) =>
        {
            var result = await sender.Send(new GetFleetVehicleDocumentsQuery(new PaginationRequest(PageIndex, PageSize, searchText), vehicleId, documentType, status));
            return Results.Ok(result);
        })
        .WithName("GetFleetVehicleDocuments")
        .Produces<GetFleetVehicleDocumentsResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleDocumentPermissions.View);

        app.MapPost("/api/v1/fleet/documents", async (CreateFleetVehicleDocumentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateFleetVehicleDocumentCommand(request.Document));
            return Results.Created($"/api/v1/fleet/documents/{result.Id}", result);
        })
        .WithName("CreateFleetVehicleDocument")
        .Produces<CreateFleetVehicleDocumentResult>(StatusCodes.Status201Created)
        .RequireAuthorization(PermissionList.FleetVehicleDocumentPermissions.Create);

        app.MapPut("/api/v1/fleet/documents", async (UpdateFleetVehicleDocumentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateFleetVehicleDocumentCommand(request.Document));
            return Results.Ok(result);
        })
        .WithName("UpdateFleetVehicleDocument")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleDocumentPermissions.Edit);

        app.MapPut("/api/v1/fleet/documents/{id:guid}/renew", async (Guid id, RenewFleetVehicleDocumentRequest request, ISender sender) =>
        {
            var result = await sender.Send(new RenewFleetVehicleDocumentCommand(id, request.Renewal));
            return Results.Ok(result);
        })
        .WithName("RenewFleetVehicleDocument")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleDocumentPermissions.Renew);

        app.MapDelete("/api/v1/fleet/documents/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteFleetVehicleDocumentCommand(id));
            return Results.Ok(result);
        })
        .WithName("DeleteFleetVehicleDocument")
        .Produces<FleetActionResult>(StatusCodes.Status200OK)
        .RequireAuthorization(PermissionList.FleetVehicleDocumentPermissions.Delete);
    }
}

public class GetFleetVehicleDocumentsHandler(FleetDbContext dbContext)
    : IQueryHandler<GetFleetVehicleDocumentsQuery, GetFleetVehicleDocumentsResult>
{
    public async Task<GetFleetVehicleDocumentsResult> Handle(GetFleetVehicleDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.VehicleDocuments.Include(x => x.Vehicle).AsNoTracking().AsQueryable();
        if (request.VehicleId.HasValue)
            query = query.Where(x => x.VehicleId == request.VehicleId.Value);
        if (request.DocumentType.HasValue)
            query = query.Where(x => x.DocumentType == request.DocumentType.Value);
        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);
        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.ToLower();
            query = query.Where(x => x.Vehicle.Name.ToLower().Contains(search) || x.Vehicle.PlateNumber.ToLower().Contains(search) || x.DocumentNumber.ToLower().Contains(search));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var documents = await query.OrderBy(x => x.ExpiryDate)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetFleetVehicleDocumentsResult(new PaginatedResult<FleetVehicleDocumentDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            documents.Select(FleetFeatureHelpers.ToDto).ToList()));
    }
}

public class CreateFleetVehicleDocumentHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateFleetVehicleDocumentCommand, CreateFleetVehicleDocumentResult>
{
    public async Task<CreateFleetVehicleDocumentResult> Handle(CreateFleetVehicleDocumentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        await FleetFeatureHelpers.EnsureVehicleAsync(dbContext, request.Document.VehicleId, cancellationToken);
        var document = FleetVehicleDocument.Create(request.Document, currentUserId);
        dbContext.VehicleDocuments.Add(document);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateFleetVehicleDocumentResult(document.Id);
    }
}

public class UpdateFleetVehicleDocumentHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateFleetVehicleDocumentCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(UpdateFleetVehicleDocumentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var document = await dbContext.VehicleDocuments.FirstOrDefaultAsync(x => x.Id == request.Document.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle document", request.Document.Id);
        document.Update(request.Document, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class RenewFleetVehicleDocumentHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RenewFleetVehicleDocumentCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(RenewFleetVehicleDocumentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var document = await dbContext.VehicleDocuments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle document", request.Id);
        document.Renew(request.Renewal, currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}

public class DeleteFleetVehicleDocumentHandler(FleetDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteFleetVehicleDocumentCommand, FleetActionResult>
{
    public async Task<FleetActionResult> Handle(DeleteFleetVehicleDocumentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = FleetFeatureHelpers.GetCurrentUserId(httpContextAccessor);
        var document = await dbContext.VehicleDocuments.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Fleet vehicle document", request.Id);
        document.Remove(currentUserId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new FleetActionResult(true);
    }
}
