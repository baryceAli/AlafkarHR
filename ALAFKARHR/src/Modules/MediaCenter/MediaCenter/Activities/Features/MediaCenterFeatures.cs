namespace MediaCenter.Activities.Features;

public record GetMediaActivityTypesQuery(Guid? CompanyId, string? SearchText, bool ActiveOnly, PaginationRequest Pagination) : IQuery<GetMediaActivityTypesResult>;
public record GetMediaActivityTypesResult(PaginatedResult<MediaActivityTypeDto> ActivityTypes);
public record GetMediaActivityTypesResponse(PaginatedResult<MediaActivityTypeDto> ActivityTypes);
public record GetMediaActivityTypeByIdQuery(Guid Id) : IQuery<GetMediaActivityTypeByIdResult>;
public record GetMediaActivityTypeByIdResult(MediaActivityTypeDto ActivityType);
public record SaveMediaActivityTypeRequest(MediaActivityTypeDto ActivityType);
public record CreateMediaActivityTypeCommand(MediaActivityTypeDto ActivityType) : ICommand<CreateMediaEntityResult>;
public record UpdateMediaActivityTypeCommand(Guid Id, MediaActivityTypeDto ActivityType) : ICommand<UpdateMediaEntityResult>;
public record DeleteMediaActivityTypeCommand(Guid Id) : ICommand<UpdateMediaEntityResult>;

public record GetMediaActivitiesQuery(
    Guid? CompanyId,
    string? SearchText,
    Guid? ActivityTypeId,
    string? RelatedType,
    string? RelatedText,
    DateTime? FromDate,
    DateTime? ToDate,
    MediaKind? MediaKind,
    PaginationRequest Pagination) : IQuery<GetMediaActivitiesResult>;

public record GetMediaActivitiesResult(PaginatedResult<MediaActivityDto> Activities);
public record GetMediaActivitiesResponse(PaginatedResult<MediaActivityDto> Activities);
public record GetMediaActivityByIdQuery(Guid Id) : IQuery<GetMediaActivityByIdResult>;
public record GetMediaActivityByIdResult(MediaActivityDto Activity);
public record SaveMediaActivityRequest(SaveMediaActivityDto Activity);
public record CreateMediaActivityCommand(SaveMediaActivityDto Activity) : ICommand<CreateMediaEntityResult>;
public record UpdateMediaActivityCommand(Guid Id, SaveMediaActivityDto Activity) : ICommand<UpdateMediaEntityResult>;
public record DeleteMediaActivityCommand(Guid Id) : ICommand<UpdateMediaEntityResult>;
public record AddMediaActivityMediaRequest(AddMediaActivityMediaDto Media);
public record AddMediaActivityMediaCommand(Guid ActivityId, AddMediaActivityMediaDto Media) : ICommand<CreateMediaEntityResult>;
public record UpdateMediaActivityMediaRequest(UpdateMediaActivityMediaDto Media);
public record UpdateMediaActivityMediaCommand(Guid ActivityId, Guid MediaId, UpdateMediaActivityMediaDto Media) : ICommand<UpdateMediaEntityResult>;
public record DeleteMediaActivityMediaCommand(Guid ActivityId, Guid MediaId) : ICommand<UpdateMediaEntityResult>;
public record CreateMediaEntityResult(Guid Id);
public record UpdateMediaEntityResult(bool IsSuccess);

public class MediaActivityTypeValidator : AbstractValidator<MediaActivityTypeDto>
{
    public MediaActivityTypeValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(180);
        RuleFor(x => x.NameEng).MaximumLength(180);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

public class SaveMediaActivityValidator : AbstractValidator<SaveMediaActivityDto>
{
    public SaveMediaActivityValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.ActivityTypeId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(250);
        RuleFor(x => x.TitleEng).MaximumLength(250);
        RuleFor(x => x.LocationText).MaximumLength(500);
        RuleFor(x => x.Notes).MaximumLength(1000);
        RuleForEach(x => x.RelatedRecords).ChildRules(record =>
        {
            record.RuleFor(x => x.RelatedType).NotEmpty().MaximumLength(120);
            record.RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(250);
            record.RuleFor(x => x.Notes).MaximumLength(500);
        });
    }
}

public class AddMediaActivityMediaValidator : AbstractValidator<AddMediaActivityMediaDto>
{
    public AddMediaActivityMediaValidator()
    {
        RuleFor(x => x.DocumentId).NotEmpty();
        RuleFor(x => x.MediaKind).IsInEnum();
        RuleFor(x => x.Caption).MaximumLength(500);
    }
}

public class UpdateMediaActivityMediaValidator : AbstractValidator<UpdateMediaActivityMediaDto>
{
    public UpdateMediaActivityMediaValidator()
    {
        RuleFor(x => x.MediaKind).IsInEnum();
        RuleFor(x => x.Caption).MaximumLength(500);
    }
}

public class MediaCenterEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/mediacenter");

        group.MapGet("/activity-types", async ([AsParameters] MediaActivityTypeListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
        {
            var result = await sender.Send(new GetMediaActivityTypesQuery(query.CompanyId, pagination.SearchText, query.ActiveOnly, pagination));
            return Results.Ok(result.Adapt<GetMediaActivityTypesResponse>());
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.Select);

        group.MapGet("/activity-types/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMediaActivityTypeByIdQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.View);

        group.MapPost("/activity-types", async (SaveMediaActivityTypeRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateMediaActivityTypeCommand(request.ActivityType));
            return Results.Created($"/api/v1/mediacenter/activity-types/{result.Id}", result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.ManageTypes);

        group.MapPut("/activity-types/{id:guid}", async (Guid id, SaveMediaActivityTypeRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateMediaActivityTypeCommand(id, request.ActivityType));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.ManageTypes);

        group.MapDelete("/activity-types/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteMediaActivityTypeCommand(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.ManageTypes);

        group.MapGet("/activities", async ([AsParameters] MediaActivityListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
        {
            var result = await sender.Send(new GetMediaActivitiesQuery(
                query.CompanyId,
                pagination.SearchText,
                query.ActivityTypeId,
                query.RelatedType,
                query.RelatedText,
                query.FromDate,
                query.ToDate,
                query.MediaKind,
                pagination));
            return Results.Ok(result.Adapt<GetMediaActivitiesResponse>());
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.View);

        group.MapGet("/activities/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMediaActivityByIdQuery(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.View);

        group.MapPost("/activities", async (SaveMediaActivityRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateMediaActivityCommand(request.Activity));
            return Results.Created($"/api/v1/mediacenter/activities/{result.Id}", result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.Create);

        group.MapPut("/activities/{id:guid}", async (Guid id, SaveMediaActivityRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateMediaActivityCommand(id, request.Activity));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.Edit);

        group.MapDelete("/activities/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteMediaActivityCommand(id));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.Delete);

        group.MapPost("/activities/{id:guid}/media", async (Guid id, AddMediaActivityMediaRequest request, ISender sender) =>
        {
            var result = await sender.Send(new AddMediaActivityMediaCommand(id, request.Media));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.Upload);

        group.MapPut("/activities/{activityId:guid}/media/{mediaId:guid}", async (Guid activityId, Guid mediaId, UpdateMediaActivityMediaRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateMediaActivityMediaCommand(activityId, mediaId, request.Media));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.Upload);

        group.MapDelete("/activities/{activityId:guid}/media/{mediaId:guid}", async (Guid activityId, Guid mediaId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteMediaActivityMediaCommand(activityId, mediaId));
            return Results.Ok(result);
        }).RequireAuthorization(PermissionList.MediaCenterPermissions.Upload);
    }
}

public class MediaCenterHandlers(MediaCenterDbContext dbContext, IHttpContextAccessor httpContextAccessor) :
    IQueryHandler<GetMediaActivityTypesQuery, GetMediaActivityTypesResult>,
    IQueryHandler<GetMediaActivityTypeByIdQuery, GetMediaActivityTypeByIdResult>,
    ICommandHandler<CreateMediaActivityTypeCommand, CreateMediaEntityResult>,
    ICommandHandler<UpdateMediaActivityTypeCommand, UpdateMediaEntityResult>,
    ICommandHandler<DeleteMediaActivityTypeCommand, UpdateMediaEntityResult>,
    IQueryHandler<GetMediaActivitiesQuery, GetMediaActivitiesResult>,
    IQueryHandler<GetMediaActivityByIdQuery, GetMediaActivityByIdResult>,
    ICommandHandler<CreateMediaActivityCommand, CreateMediaEntityResult>,
    ICommandHandler<UpdateMediaActivityCommand, UpdateMediaEntityResult>,
    ICommandHandler<DeleteMediaActivityCommand, UpdateMediaEntityResult>,
    ICommandHandler<AddMediaActivityMediaCommand, CreateMediaEntityResult>,
    ICommandHandler<UpdateMediaActivityMediaCommand, UpdateMediaEntityResult>,
    ICommandHandler<DeleteMediaActivityMediaCommand, UpdateMediaEntityResult>
{
    public async Task<GetMediaActivityTypesResult> Handle(GetMediaActivityTypesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.MediaActivityTypes.AsNoTracking();
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.ActiveOnly) query = query.Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Name.Contains(search) || x.NameEng.Contains(search) || (x.Description != null && x.Description.Contains(search)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var data = await query
            .OrderBy(x => x.Name)
            .Skip(request.Pagination.PageIndex * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new GetMediaActivityTypesResult(new PaginatedResult<MediaActivityTypeDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(ToDto)));
    }

    public async Task<GetMediaActivityTypeByIdResult> Handle(GetMediaActivityTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var activityType = await dbContext.MediaActivityTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Media activity type not found: {request.Id}");
        return new GetMediaActivityTypeByIdResult(ToDto(activityType));
    }

    public async Task<CreateMediaEntityResult> Handle(CreateMediaActivityTypeCommand request, CancellationToken cancellationToken)
    {
        var activityType = MediaActivityType.Create(request.ActivityType, UserId());
        await dbContext.MediaActivityTypes.AddAsync(activityType, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateMediaEntityResult(activityType.Id);
    }

    public async Task<UpdateMediaEntityResult> Handle(UpdateMediaActivityTypeCommand request, CancellationToken cancellationToken)
    {
        var activityType = await dbContext.MediaActivityTypes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Media activity type not found: {request.Id}");
        activityType.Update(request.ActivityType, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMediaEntityResult(true);
    }

    public async Task<UpdateMediaEntityResult> Handle(DeleteMediaActivityTypeCommand request, CancellationToken cancellationToken)
    {
        var activityType = await dbContext.MediaActivityTypes.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Media activity type not found: {request.Id}");
        activityType.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMediaEntityResult(true);
    }

    public async Task<GetMediaActivitiesResult> Handle(GetMediaActivitiesQuery request, CancellationToken cancellationToken)
    {
        var query = ActivityQuery().AsNoTracking();
        query = ApplyActivityFilters(query, request);

        var count = await query.LongCountAsync(cancellationToken);
        var data = await query
            .OrderByDescending(x => x.ActivityDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip(request.Pagination.PageIndex * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .ToListAsync(cancellationToken);

        var typeNames = await TypeNamesAsync(data.Select(x => x.ActivityTypeId), cancellationToken);
        return new GetMediaActivitiesResult(new PaginatedResult<MediaActivityDto>(request.Pagination.PageIndex, request.Pagination.PageSize, count, data.Select(x => ToDto(x, typeNames))));
    }

    public async Task<GetMediaActivityByIdResult> Handle(GetMediaActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var activity = await ActivityQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Media activity not found: {request.Id}");
        var typeNames = await TypeNamesAsync([activity.ActivityTypeId], cancellationToken);
        return new GetMediaActivityByIdResult(ToDto(activity, typeNames));
    }

    public async Task<CreateMediaEntityResult> Handle(CreateMediaActivityCommand request, CancellationToken cancellationToken)
    {
        await EnsureActivityTypeAsync(request.Activity.CompanyId, request.Activity.ActivityTypeId, cancellationToken);
        var activity = MediaActivity.Create(request.Activity, UserId());
        await dbContext.MediaActivities.AddAsync(activity, cancellationToken);
        await SyncChildrenAsync(activity.Id, request.Activity.RelatedRecords, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateMediaEntityResult(activity.Id);
    }

    public async Task<UpdateMediaEntityResult> Handle(UpdateMediaActivityCommand request, CancellationToken cancellationToken)
    {
        var activity = await dbContext.MediaActivities.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Media activity not found: {request.Id}");
        await EnsureActivityTypeAsync(activity.CompanyId, request.Activity.ActivityTypeId, cancellationToken);
        activity.Update(request.Activity, UserId());
        await SyncChildrenAsync(activity.Id, request.Activity.RelatedRecords, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMediaEntityResult(true);
    }

    public async Task<UpdateMediaEntityResult> Handle(DeleteMediaActivityCommand request, CancellationToken cancellationToken)
    {
        var activity = await dbContext.MediaActivities.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Media activity not found: {request.Id}");
        activity.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMediaEntityResult(true);
    }

    public async Task<CreateMediaEntityResult> Handle(AddMediaActivityMediaCommand request, CancellationToken cancellationToken)
    {
        var activity = await dbContext.MediaActivities
            .Include(x => x.Media)
            .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken)
            ?? throw new NotFoundException($"Media activity not found: {request.ActivityId}");
        var media = MediaActivityMedia.Create(activity.Id, request.Media, UserGuid(), UserId());
        activity.AddMedia(media);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateMediaEntityResult(media.Id);
    }

    public async Task<UpdateMediaEntityResult> Handle(UpdateMediaActivityMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await dbContext.MediaActivityMedia.FirstOrDefaultAsync(x => x.Id == request.MediaId && x.MediaActivityId == request.ActivityId, cancellationToken)
            ?? throw new NotFoundException($"Media link not found: {request.MediaId}");
        media.Update(request.Media, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMediaEntityResult(true);
    }

    public async Task<UpdateMediaEntityResult> Handle(DeleteMediaActivityMediaCommand request, CancellationToken cancellationToken)
    {
        var media = await dbContext.MediaActivityMedia.FirstOrDefaultAsync(x => x.Id == request.MediaId && x.MediaActivityId == request.ActivityId, cancellationToken)
            ?? throw new NotFoundException($"Media link not found: {request.MediaId}");
        media.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateMediaEntityResult(true);
    }

    private IQueryable<MediaActivity> ActivityQuery() =>
        dbContext.MediaActivities
            .Include(x => x.RelatedRecords)
            .Include(x => x.Media);

    private static IQueryable<MediaActivity> ApplyActivityFilters(IQueryable<MediaActivity> query, GetMediaActivitiesQuery request)
    {
        if (request.CompanyId.HasValue) query = query.Where(x => x.CompanyId == request.CompanyId);
        if (request.ActivityTypeId.HasValue) query = query.Where(x => x.ActivityTypeId == request.ActivityTypeId);
        if (!string.IsNullOrWhiteSpace(request.RelatedType))
        {
            var relatedType = request.RelatedType.Trim();
            query = query.Where(x => x.RelatedRecords.Any(r => r.RelatedType.Contains(relatedType)));
        }
        if (!string.IsNullOrWhiteSpace(request.RelatedText))
        {
            var relatedText = request.RelatedText.Trim();
            query = query.Where(x => x.RelatedRecords.Any(r =>
                r.DisplayName.Contains(relatedText) ||
                r.RelatedType.Contains(relatedText) ||
                (r.Notes != null && r.Notes.Contains(relatedText))));
        }
        if (request.FromDate.HasValue) query = query.Where(x => x.ActivityDate >= request.FromDate.Value.Date);
        if (request.ToDate.HasValue) query = query.Where(x => x.ActivityDate <= request.ToDate.Value.Date);
        if (request.MediaKind.HasValue && request.MediaKind.Value != MediaKind.All) query = query.Where(x => x.Media.Any(m => m.MediaKind == request.MediaKind));
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x =>
                x.Title.Contains(search) ||
                x.TitleEng.Contains(search) ||
                (x.LocationText != null && x.LocationText.Contains(search)) ||
                (x.Notes != null && x.Notes.Contains(search)) ||
                x.RelatedRecords.Any(r =>
                    r.RelatedType.Contains(search) ||
                    r.DisplayName.Contains(search) ||
                    (r.Notes != null && r.Notes.Contains(search))));
        }

        return query;
    }

    private async Task SyncChildrenAsync(Guid activityId, IEnumerable<MediaActivityRelatedRecordDto> relatedRecords, CancellationToken cancellationToken)
    {
        var existing = await dbContext.MediaActivityRelatedRecords.Where(x => x.MediaActivityId == activityId).ToListAsync(cancellationToken);
        dbContext.MediaActivityRelatedRecords.RemoveRange(existing);

        var next = relatedRecords.Select(x => MediaActivityRelatedRecord.Create(activityId, x, UserId())).ToList();
        await dbContext.MediaActivityRelatedRecords.AddRangeAsync(next, cancellationToken);
    }

    private async Task EnsureActivityTypeAsync(Guid companyId, Guid activityTypeId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.MediaActivityTypes.AnyAsync(x => x.Id == activityTypeId && x.CompanyId == companyId && x.IsActive, cancellationToken);
        if (!exists)
            throw new BadRequestException("Active media activity type is required.");
    }

    private async Task<Dictionary<Guid, MediaActivityTypeDto>> TypeNamesAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        var typeIds = ids.Distinct().ToList();
        return await dbContext.MediaActivityTypes.AsNoTracking()
            .Where(x => typeIds.Contains(x.Id))
            .Select(x => ToDto(x))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
    }

    private string UserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? httpContextAccessor.HttpContext?.User?.Identity?.Name
        ?? "system";

    private Guid? UserGuid()
    {
        var value = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    private static MediaActivityTypeDto ToDto(MediaActivityType activityType) => new()
    {
        Id = activityType.Id,
        CompanyId = activityType.CompanyId,
        Name = activityType.Name,
        NameEng = activityType.NameEng,
        Description = activityType.Description,
        IsActive = activityType.IsActive
    };

    private static MediaActivityDto ToDto(MediaActivity activity, IReadOnlyDictionary<Guid, MediaActivityTypeDto> activityTypes)
    {
        activityTypes.TryGetValue(activity.ActivityTypeId, out var activityType);
        return new MediaActivityDto
        {
            Id = activity.Id,
            CompanyId = activity.CompanyId,
            ActivityTypeId = activity.ActivityTypeId,
            ActivityTypeName = activityType?.Name ?? string.Empty,
            ActivityTypeNameEng = activityType?.NameEng,
            Title = activity.Title,
            TitleEng = activity.TitleEng,
            ActivityDate = activity.ActivityDate,
            ActivityTime = activity.ActivityTime,
            LocationText = activity.LocationText,
            Notes = activity.Notes,
            MediaCount = activity.Media.Count,
            CreatedAt = activity.CreatedAt,
            RelatedRecords = activity.RelatedRecords.Select(ToDto).ToList(),
            Media = activity.Media.OrderByDescending(x => x.IsPrimary).ThenByDescending(x => x.CreatedAt).Select(ToDto).ToList()
        };
    }

    private static MediaActivityRelatedRecordDto ToDto(MediaActivityRelatedRecord item) => new()
    {
        Id = item.Id,
        RelatedType = item.RelatedType,
        RelatedRecordId = item.RelatedRecordId,
        DisplayName = item.DisplayName,
        Notes = item.Notes
    };

    private static MediaActivityMediaDto ToDto(MediaActivityMedia item) => new()
    {
        Id = item.Id,
        DocumentId = item.DocumentId,
        MediaKind = item.MediaKind,
        Caption = item.Caption,
        CapturedAt = item.CapturedAt,
        UploadedByUserId = item.UploadedByUserId,
        IsPrimary = item.IsPrimary
    };
}

public class MediaActivityTypeListQuery
{
    public Guid? CompanyId { get; set; }
    public bool ActiveOnly { get; set; } = true;
}

public class MediaActivityListQuery
{
    public Guid? CompanyId { get; set; }
    public Guid? ActivityTypeId { get; set; }
    public string? RelatedType { get; set; }
    public string? RelatedText { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public MediaKind? MediaKind { get; set; }
}
