using Shared.DDD;
using static Training.Training.Features.TrainingFeatureHelpers;

namespace Training.Training.Features;

public record ListTrainingProgramsQuery(Guid CompanyId) : IQuery<ListTrainingProgramsResult>;
public record ListTrainingProgramsResult(List<TrainingProgramDto> TrainingProgramList);
public record UpsertTrainingProgramCommand(UpsertTrainingProgramDto Program) : ICommand<TrainingActionResultDto>;
public record DeleteTrainingProgramCommand(Guid Id) : ICommand<TrainingActionResultDto>;

public record ListTrainingEventsQuery(Guid CompanyId, Guid? ProgramId, TrainingEventStatus? Status) : IQuery<ListTrainingEventsResult>;
public record ListTrainingEventsResult(List<TrainingEventDto> TrainingEventList);
public record UpsertTrainingEventCommand(UpsertTrainingEventDto Event) : ICommand<TrainingActionResultDto>;
public record TrainingEventActionCommand(Guid Id, string Action) : ICommand<TrainingActionResultDto>;

public record ListTrainingAttendeesQuery(Guid TrainingEventId) : IQuery<ListTrainingAttendeesResult>;
public record ListTrainingAttendeesResult(List<TrainingAttendeeDto> TrainingAttendeeList);
public record UpsertTrainingAttendeeCommand(UpsertTrainingAttendeeDto Attendee) : ICommand<TrainingActionResultDto>;
public record DeleteTrainingAttendeeCommand(Guid Id) : ICommand<TrainingActionResultDto>;
public record MarkTrainingAttendanceCommand(Guid Id, bool Attended) : ICommand<TrainingActionResultDto>;
public record RecordTrainingResultCommand(Guid Id, TrainingAttendeeResultDto Result) : ICommand<TrainingActionResultDto>;
public record LinkTrainingCertificateCommand(Guid Id, TrainingCertificateLinkDto Certificate) : ICommand<TrainingActionResultDto>;

internal static class TrainingMapper
{
    public static TrainingProgramDto ToDto(TrainingProgram item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        Name = item.Name,
        Category = item.Category,
        Provider = item.Provider,
        Objective = item.Objective,
        Description = item.Description,
        CreatedAt = item.CreatedAt
    };

    public static TrainingEventDto ToDto(TrainingEvent item, TrainingProgram? program = null, int attendeeCount = 0) => new()
    {
        Id = item.Id,
        ProgramId = item.ProgramId,
        CompanyId = item.CompanyId,
        ProgramName = program?.Name,
        Title = item.Title,
        StartAt = item.StartAt,
        EndAt = item.EndAt,
        Capacity = item.Capacity,
        Status = item.Status,
        StatusLabel = item.Status.ToString(),
        AttendeeCount = attendeeCount,
        CreatedAt = item.CreatedAt
    };

    public static TrainingAttendeeDto ToDto(TrainingAttendee item) => new()
    {
        Id = item.Id,
        TrainingEventId = item.TrainingEventId,
        EmployeeId = item.EmployeeId,
        EmployeeName = item.EmployeeId.ToString("N")[..8],
        Attended = item.Attended,
        Passed = item.Passed,
        Status = item.Status,
        StatusLabel = item.Status.ToString(),
        Score = item.Score,
        Feedback = item.Feedback,
        CertificateName = item.CertificateName,
        CertificateIssuer = item.CertificateIssuer,
        CertificateIssuedAt = item.CertificateIssuedAt,
        CertificateExpiresAt = item.CertificateExpiresAt,
        CertificationId = item.CertificationId,
        CreatedAt = item.CreatedAt
    };
}

public class ListTrainingProgramsHandler(TrainingDbContext dbContext) : IQueryHandler<ListTrainingProgramsQuery, ListTrainingProgramsResult>
{
    public async Task<ListTrainingProgramsResult> Handle(ListTrainingProgramsQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.TrainingPrograms.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return new(data.Select(TrainingMapper.ToDto).ToList());
    }
}

public class UpsertTrainingProgramHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertTrainingProgramCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(UpsertTrainingProgramCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.Program;
        var item = dto.Id == Guid.Empty
            ? TrainingProgram.Create(Guid.NewGuid(), dto.CompanyId, dto.Name, dto.Category, dto.Provider, dto.Objective, dto.Description, userId)
            : await dbContext.TrainingPrograms.FirstAsync(x => x.Id == dto.Id, cancellationToken);

        if (dto.Id == Guid.Empty) await dbContext.TrainingPrograms.AddAsync(item, cancellationToken);
        else item.Update(dto.Name, dto.Category, dto.Provider, dto.Objective, dto.Description, userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Saved", "Training program saved");
    }
}

public class DeleteTrainingProgramHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteTrainingProgramCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(DeleteTrainingProgramCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.TrainingPrograms.FirstAsync(x => x.Id == request.Id, cancellationToken);
        SoftDelete(item, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Deleted", "Training program deleted");
    }
}

public class ListTrainingEventsHandler(TrainingDbContext dbContext) : IQueryHandler<ListTrainingEventsQuery, ListTrainingEventsResult>
{
    public async Task<ListTrainingEventsResult> Handle(ListTrainingEventsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.TrainingEvents.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (request.ProgramId.HasValue && request.ProgramId.Value != Guid.Empty) query = query.Where(x => x.ProgramId == request.ProgramId.Value);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);

        var events = await query.OrderByDescending(x => x.StartAt).ToListAsync(cancellationToken);
        var programIds = events.Select(x => x.ProgramId).Distinct().ToList();
        var programs = await dbContext.TrainingPrograms.AsNoTracking()
            .Where(x => programIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        var eventIds = events.Select(x => x.Id).ToList();
        var attendeeCounts = await dbContext.TrainingAttendees.AsNoTracking()
            .Where(x => eventIds.Contains(x.TrainingEventId))
            .GroupBy(x => x.TrainingEventId)
            .Select(x => new { EventId = x.Key, Count = x.Count() })
            .ToDictionaryAsync(x => x.EventId, x => x.Count, cancellationToken);

        return new(events.Select(x => TrainingMapper.ToDto(x, programs.GetValueOrDefault(x.ProgramId), attendeeCounts.GetValueOrDefault(x.Id))).ToList());
    }
}

public class UpsertTrainingEventHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertTrainingEventCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(UpsertTrainingEventCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.Event;
        var item = dto.Id == Guid.Empty
            ? TrainingEvent.Create(Guid.NewGuid(), dto.CompanyId, dto.ProgramId, dto.Title, dto.StartAt, dto.EndAt, dto.Capacity, userId)
            : await dbContext.TrainingEvents.FirstAsync(x => x.Id == dto.Id, cancellationToken);

        if (dto.Id == Guid.Empty) await dbContext.TrainingEvents.AddAsync(item, cancellationToken);
        else item.Update(dto.ProgramId, dto.Title, dto.StartAt, dto.EndAt, dto.Capacity, userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Training event saved");
    }
}

public class TrainingEventActionHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<TrainingEventActionCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(TrainingEventActionCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.TrainingEvents.FirstAsync(x => x.Id == request.Id, cancellationToken);
        var userId = CurrentUser(httpContextAccessor);
        switch (request.Action.ToLowerInvariant())
        {
            case "open": item.Open(userId); break;
            case "start": item.Start(userId); break;
            case "complete": item.Complete(userId); break;
            case "cancel": item.Cancel(userId); break;
            default: throw new InvalidOperationException("Unsupported training event action");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Training event updated");
    }
}

public class ListTrainingAttendeesHandler(TrainingDbContext dbContext) : IQueryHandler<ListTrainingAttendeesQuery, ListTrainingAttendeesResult>
{
    public async Task<ListTrainingAttendeesResult> Handle(ListTrainingAttendeesQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.TrainingAttendees.AsNoTracking()
            .Where(x => x.TrainingEventId == request.TrainingEventId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return new(data.Select(TrainingMapper.ToDto).ToList());
    }
}

public class UpsertTrainingAttendeeHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertTrainingAttendeeCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(UpsertTrainingAttendeeCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.Attendee;
        var item = dto.Id == Guid.Empty
            ? TrainingAttendee.Create(Guid.NewGuid(), dto.TrainingEventId, dto.EmployeeId, userId)
            : await dbContext.TrainingAttendees.FirstAsync(x => x.Id == dto.Id, cancellationToken);

        if (dto.Id == Guid.Empty) await dbContext.TrainingAttendees.AddAsync(item, cancellationToken);
        else item.ChangeEmployee(dto.EmployeeId, userId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Training attendee saved");
    }
}

public class DeleteTrainingAttendeeHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteTrainingAttendeeCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(DeleteTrainingAttendeeCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.TrainingAttendees.FirstAsync(x => x.Id == request.Id, cancellationToken);
        SoftDelete(item, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Deleted", "Training attendee deleted");
    }
}

public class MarkTrainingAttendanceHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<MarkTrainingAttendanceCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(MarkTrainingAttendanceCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.TrainingAttendees.FirstAsync(x => x.Id == request.Id, cancellationToken);
        item.MarkAttended(request.Attended, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Training attendance updated");
    }
}

public class RecordTrainingResultHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<RecordTrainingResultCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(RecordTrainingResultCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.TrainingAttendees.FirstAsync(x => x.Id == request.Id, cancellationToken);
        item.RecordResult(request.Result.Attended, request.Result.Passed, request.Result.Score, request.Result.Feedback, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Training result saved");
    }
}

public class LinkTrainingCertificateHandler(TrainingDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<LinkTrainingCertificateCommand, TrainingActionResultDto>
{
    public async Task<TrainingActionResultDto> Handle(LinkTrainingCertificateCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.TrainingAttendees.FirstAsync(x => x.Id == request.Id, cancellationToken);
        item.LinkCertification(
            request.Certificate.CertificationId,
            request.Certificate.CertificateName,
            request.Certificate.CertificateIssuer,
            request.Certificate.CertificateIssuedAt,
            request.Certificate.CertificateExpiresAt,
            CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "CertificateLinked", "Training certificate linked");
    }
}

public class TrainingEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var training = app.MapGroup("/api/v1/training");

        training.MapGet("/programs/company/{companyId:guid}", async (Guid companyId, ISender sender) => Results.Ok(await sender.Send(new ListTrainingProgramsQuery(companyId)))).RequireAuthorization(PermissionList.TrainingPermissions.View);
        training.MapPost("/programs", async (UpsertTrainingProgramDto program, ISender sender) => Results.Ok(await sender.Send(new UpsertTrainingProgramCommand(program)))).RequireAuthorization(PermissionList.TrainingPermissions.Create);
        training.MapPut("/programs/{id:guid}", async (Guid id, UpsertTrainingProgramDto program, ISender sender) => { program.Id = id; return Results.Ok(await sender.Send(new UpsertTrainingProgramCommand(program))); }).RequireAuthorization(PermissionList.TrainingPermissions.Edit);
        training.MapDelete("/programs/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteTrainingProgramCommand(id)))).RequireAuthorization(PermissionList.TrainingPermissions.Edit);

        training.MapGet("/events/company/{companyId:guid}", async (Guid companyId, Guid? programId, TrainingEventStatus? status, ISender sender) => Results.Ok(await sender.Send(new ListTrainingEventsQuery(companyId, programId, status)))).RequireAuthorization(PermissionList.TrainingPermissions.View);
        training.MapPost("/events", async (UpsertTrainingEventDto trainingEvent, ISender sender) => Results.Ok(await sender.Send(new UpsertTrainingEventCommand(trainingEvent)))).RequireAuthorization(PermissionList.TrainingPermissions.Create);
        training.MapPut("/events/{id:guid}", async (Guid id, UpsertTrainingEventDto trainingEvent, ISender sender) => { trainingEvent.Id = id; return Results.Ok(await sender.Send(new UpsertTrainingEventCommand(trainingEvent))); }).RequireAuthorization(PermissionList.TrainingPermissions.Edit);
        training.MapPost("/events/{id:guid}/open", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new TrainingEventActionCommand(id, "open")))).RequireAuthorization(PermissionList.TrainingPermissions.Complete);
        training.MapPost("/events/{id:guid}/start", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new TrainingEventActionCommand(id, "start")))).RequireAuthorization(PermissionList.TrainingPermissions.Complete);
        training.MapPost("/events/{id:guid}/complete", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new TrainingEventActionCommand(id, "complete")))).RequireAuthorization(PermissionList.TrainingPermissions.Complete);
        training.MapPost("/events/{id:guid}/cancel", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new TrainingEventActionCommand(id, "cancel")))).RequireAuthorization(PermissionList.TrainingPermissions.Complete);

        training.MapGet("/events/{trainingEventId:guid}/attendees", async (Guid trainingEventId, ISender sender) => Results.Ok(await sender.Send(new ListTrainingAttendeesQuery(trainingEventId)))).RequireAuthorization(PermissionList.TrainingPermissions.View);
        training.MapPost("/attendees", async (UpsertTrainingAttendeeDto attendee, ISender sender) => Results.Ok(await sender.Send(new UpsertTrainingAttendeeCommand(attendee)))).RequireAuthorization(PermissionList.TrainingPermissions.Create);
        training.MapPut("/attendees/{id:guid}", async (Guid id, UpsertTrainingAttendeeDto attendee, ISender sender) => { attendee.Id = id; return Results.Ok(await sender.Send(new UpsertTrainingAttendeeCommand(attendee))); }).RequireAuthorization(PermissionList.TrainingPermissions.Edit);
        training.MapDelete("/attendees/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteTrainingAttendeeCommand(id)))).RequireAuthorization(PermissionList.TrainingPermissions.Edit);
        training.MapPost("/attendees/{id:guid}/attendance", async (Guid id, TrainingAttendeeResultDto result, ISender sender) => Results.Ok(await sender.Send(new MarkTrainingAttendanceCommand(id, result.Attended)))).RequireAuthorization(PermissionList.TrainingPermissions.Edit);
        training.MapPost("/attendees/{id:guid}/result", async (Guid id, TrainingAttendeeResultDto result, ISender sender) => Results.Ok(await sender.Send(new RecordTrainingResultCommand(id, result)))).RequireAuthorization(PermissionList.TrainingPermissions.Edit);
        training.MapPost("/attendees/{id:guid}/certificate", async (Guid id, TrainingCertificateLinkDto certificate, ISender sender) => Results.Ok(await sender.Send(new LinkTrainingCertificateCommand(id, certificate)))).RequireAuthorization(PermissionList.TrainingPermissions.Edit);
    }
}

internal static class TrainingFeatureHelpers
{
    public static string CurrentUser(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static TrainingActionResultDto Result(Guid id, string status, string message) => new()
    {
        Id = id,
        Status = status,
        Message = message,
        IsSuccess = true
    };

    public static void SoftDelete(Entity<Guid> entity, string userId)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
    }
}
