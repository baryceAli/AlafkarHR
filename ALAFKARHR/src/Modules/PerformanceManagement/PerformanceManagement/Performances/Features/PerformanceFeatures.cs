using static PerformanceManagement.Performances.Features.PerformanceFeatureHelpers;

namespace PerformanceManagement.Performances.Features;

public record ListCyclesQuery(Guid CompanyId) : IQuery<ListCyclesResult>;
public record ListCyclesResult(List<AppraisalCycleDto> CycleList);
public record UpsertCycleCommand(UpsertAppraisalCycleDto Cycle) : ICommand<PerformanceActionResultDto>;
public record CycleActionCommand(Guid Id, string Action) : ICommand<PerformanceActionResultDto>;

public record ListGoalDefinitionsQuery(Guid CompanyId) : IQuery<ListGoalDefinitionsResult>;
public record ListGoalDefinitionsResult(List<GoalDefinitionDto> GoalList);
public record UpsertGoalDefinitionCommand(UpsertGoalDefinitionDto Goal) : ICommand<PerformanceActionResultDto>;
public record DeleteGoalDefinitionCommand(Guid Id) : ICommand<PerformanceActionResultDto>;

public record ListCompetenciesQuery(Guid CompanyId) : IQuery<ListCompetenciesResult>;
public record ListCompetenciesResult(List<CompetencyDto> CompetencyList);
public record UpsertCompetencyCommand(UpsertCompetencyDto Competency) : ICommand<PerformanceActionResultDto>;
public record DeleteCompetencyCommand(Guid Id) : ICommand<PerformanceActionResultDto>;

public record ListEmployeeGoalsQuery(Guid EmployeeId, Guid CycleId) : IQuery<ListEmployeeGoalsResult>;
public record ListEmployeeGoalsResult(List<EmployeeGoalReviewDto> EmployeeGoalList);
public record UpsertEmployeeGoalCommand(UpsertEmployeeGoalDto EmployeeGoal) : ICommand<PerformanceActionResultDto>;
public record UpdateEmployeeGoalAchievementCommand(UpdateEmployeeGoalAchievementDto Achievement) : ICommand<PerformanceActionResultDto>;

public record ListEmployeeCompetencyScoresQuery(Guid EmployeeId, Guid CycleId) : IQuery<ListEmployeeCompetencyScoresResult>;
public record ListEmployeeCompetencyScoresResult(List<EmployeeCompetencyScoreDto> CompetencyScoreList);
public record UpsertEmployeeCompetencyScoreCommand(UpsertEmployeeCompetencyScoreDto CompetencyScore) : ICommand<PerformanceActionResultDto>;

public record ListEvaluationsQuery(Guid CompanyId, Guid? CycleId, Guid? EmployeeId) : IQuery<ListEvaluationsResult>;
public record ListEvaluationsResult(List<EmployeeAppraisalDto> EvaluationList);
public record CreateEvaluationCommand(CreateEmployeeAppraisalDto Evaluation) : ICommand<PerformanceActionResultDto>;
public record EvaluationActionCommand(Guid Id, string Action, string? EmployeeFeedback, string? ManagerFeedback) : ICommand<PerformanceActionResultDto>;

internal static class PerformanceMapper
{
    public static AppraisalCycleDto ToDto(PerformanceCycle cycle) => new()
    {
        Id = cycle.Id,
        CompanyId = cycle.CompanyId,
        Name = cycle.Name,
        StartDate = cycle.StartDate,
        EndDate = cycle.EndDate,
        IsActive = cycle.IsActive,
        IsClosed = cycle.IsClosed,
        IsCancelled = cycle.IsCancelled,
        Status = cycle.IsCancelled ? PerformanceWorkflowStatus.Cancelled
            : cycle.IsClosed ? PerformanceWorkflowStatus.Closed
            : cycle.IsActive ? PerformanceWorkflowStatus.InProgress
            : PerformanceWorkflowStatus.Draft,
        StatusLabel = cycle.IsCancelled ? "Cancelled" : cycle.IsClosed ? "Closed" : cycle.IsActive ? "In Progress" : "Draft"
    };

    public static GoalDefinitionDto ToDto(GoalDefinition goal) => new()
    {
        Id = goal.Id,
        CompanyId = goal.CompanyId,
        Name = goal.Name,
        Code = goal.Code,
        Weight = goal.Weight
    };

    public static CompetencyDto ToDto(Competency competency) => new()
    {
        Id = competency.Id,
        CompanyId = competency.CompanyId,
        Name = competency.Name,
        Weight = competency.Weight
    };

    public static EmployeeGoalReviewDto ToDto(EmployeeGoal employeeGoal, GoalDefinition? goal = null) => new()
    {
        Id = employeeGoal.Id,
        EmployeeId = employeeGoal.EmployeeId,
        EmployeeName = employeeGoal.EmployeeId.ToString("N")[..8],
        CycleId = employeeGoal.PerformanceCycleId,
        GoalDefinitionId = employeeGoal.GoalDefinitionId,
        Goal = goal?.Name ?? employeeGoal.GoalDefinitionId.ToString("N")[..8],
        TargetValue = employeeGoal.TargetValue,
        ActualValue = employeeGoal.AchievedValue,
        Weight = employeeGoal.Weight,
        Score = employeeGoal.GetScore()
    };

    public static EmployeeCompetencyScoreDto ToDto(EmployeeCompetencyScore score, Competency? competency = null) => new()
    {
        Id = score.Id,
        EmployeeId = score.EmployeeId,
        EmployeeName = score.EmployeeId.ToString("N")[..8],
        CycleId = score.PerformanceCycleId,
        CompetencyId = score.CompetencyId,
        CompetencyName = competency?.Name,
        Score = score.Score,
        Weight = score.Weight,
        WeightedScore = score.Score * (score.Weight / 100m)
    };

    public static EmployeeAppraisalDto ToDto(PerformanceEvaluation evaluation, PerformanceCycle? cycle = null) => new()
    {
        Id = evaluation.Id,
        CompanyId = evaluation.CompanyId,
        EmployeeId = evaluation.EmployeeId,
        EmployeeName = evaluation.EmployeeId.ToString("N")[..8],
        CycleId = evaluation.PerformanceCycleId,
        CycleName = cycle?.Name,
        Status = evaluation.Status == EvaluationStatus.Approved ? PerformanceWorkflowStatus.Approved
            : evaluation.Status == EvaluationStatus.Submitted ? PerformanceWorkflowStatus.PendingApproval
            : evaluation.Status == EvaluationStatus.Rejected ? PerformanceWorkflowStatus.Cancelled
            : PerformanceWorkflowStatus.Draft,
        StatusLabel = evaluation.Status.ToString(),
        KpiScore = evaluation.KpiScore,
        CompetencyScore = evaluation.CompetencyScore,
        FinalScore = evaluation.FinalScore,
        Rating = evaluation.Rating.ToString(),
        ManagerFeedback = evaluation.ManagerComment,
        EmployeeFeedback = evaluation.EmployeeComment
    };
}

public class ListCyclesHandler(PerformanceDbContext dbContext) : IQueryHandler<ListCyclesQuery, ListCyclesResult>
{
    public async Task<ListCyclesResult> Handle(ListCyclesQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.PerformanceCycles.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderByDescending(x => x.StartDate)
            .ToListAsync(cancellationToken);
        return new(data.Select(PerformanceMapper.ToDto).ToList());
    }
}

public class UpsertCycleHandler(PerformanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertCycleCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(UpsertCycleCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.Cycle;
        var cycle = dto.Id == Guid.Empty
            ? PerformanceCycle.Create(Guid.NewGuid(), dto.Name, dto.StartDate, dto.EndDate, dto.CompanyId)
            : await dbContext.PerformanceCycles.FirstAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);

        if (dto.Id == Guid.Empty)
        {
            cycle.CreatedAt = DateTime.UtcNow;
            cycle.CreatedBy = userId;
            await dbContext.PerformanceCycles.AddAsync(cycle, cancellationToken);
        }
        else
        {
            cycle.Update(dto.Name, dto.StartDate, dto.EndDate, userId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(cycle.Id, "Saved", "Performance cycle saved");
    }
}

public class CycleActionHandler(PerformanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CycleActionCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(CycleActionCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var cycle = await dbContext.PerformanceCycles.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        switch (request.Action.ToLowerInvariant())
        {
            case "start": cycle.Start(userId); break;
            case "close": cycle.Close(userId); break;
            case "cancel": cycle.Cancel(userId); break;
            default: throw new InvalidOperationException("Unsupported cycle action");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(cycle.Id, request.Action, "Performance cycle updated");
    }
}

public class ListGoalDefinitionsHandler(PerformanceDbContext dbContext) : IQueryHandler<ListGoalDefinitionsQuery, ListGoalDefinitionsResult>
{
    public async Task<ListGoalDefinitionsResult> Handle(ListGoalDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.GoalDefinitions.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderBy(x => x.Code)
            .ToListAsync(cancellationToken);
        return new(data.Select(PerformanceMapper.ToDto).ToList());
    }
}

public class UpsertGoalDefinitionHandler(PerformanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertGoalDefinitionCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(UpsertGoalDefinitionCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.Goal;
        var goal = dto.Id == Guid.Empty
            ? GoalDefinition.Create(Guid.NewGuid(), dto.CompanyId, dto.Name, dto.Code, dto.Weight, userId)
            : await dbContext.GoalDefinitions.FirstAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);
        if (dto.Id == Guid.Empty)
            await dbContext.GoalDefinitions.AddAsync(goal, cancellationToken);
        else
            goal.Update(dto.Name, dto.Code, dto.Weight, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(goal.Id, "Saved", "Goal definition saved");
    }
}

public class DeleteGoalDefinitionHandler(PerformanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteGoalDefinitionCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(DeleteGoalDefinitionCommand request, CancellationToken cancellationToken)
    {
        var goal = await dbContext.GoalDefinitions.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        SoftDelete(goal, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(goal.Id, "Deleted", "Goal definition deleted");
    }
}

public class ListCompetenciesHandler(PerformanceDbContext dbContext) : IQueryHandler<ListCompetenciesQuery, ListCompetenciesResult>
{
    public async Task<ListCompetenciesResult> Handle(ListCompetenciesQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.Competencies.AsNoTracking()
            .Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return new(data.Select(PerformanceMapper.ToDto).ToList());
    }
}

public class UpsertCompetencyHandler(PerformanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertCompetencyCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(UpsertCompetencyCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.Competency;
        var competency = dto.Id == Guid.Empty
            ? Competency.Create(Guid.NewGuid(), dto.CompanyId, dto.Name, dto.Weight, userId)
            : await dbContext.Competencies.FirstAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken);
        if (dto.Id == Guid.Empty)
            await dbContext.Competencies.AddAsync(competency, cancellationToken);
        else
            competency.Update(dto.Name, dto.Weight, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(competency.Id, "Saved", "Competency saved");
    }
}

public class DeleteCompetencyHandler(PerformanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteCompetencyCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(DeleteCompetencyCommand request, CancellationToken cancellationToken)
    {
        var competency = await dbContext.Competencies.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        SoftDelete(competency, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(competency.Id, "Deleted", "Competency deleted");
    }
}

public class ListEmployeeGoalsHandler(PerformanceDbContext dbContext) : IQueryHandler<ListEmployeeGoalsQuery, ListEmployeeGoalsResult>
{
    public async Task<ListEmployeeGoalsResult> Handle(ListEmployeeGoalsQuery request, CancellationToken cancellationToken)
    {
        var goals = await dbContext.EmployeeGoals.AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && x.PerformanceCycleId == request.CycleId)
            .ToListAsync(cancellationToken);
        var definitions = await dbContext.GoalDefinitions.AsNoTracking()
            .Where(x => goals.Select(g => g.GoalDefinitionId).Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        return new(goals.Select(x => PerformanceMapper.ToDto(x, definitions.GetValueOrDefault(x.GoalDefinitionId))).ToList());
    }
}

public class UpsertEmployeeGoalHandler(PerformanceDbContext dbContext)
    : ICommandHandler<UpsertEmployeeGoalCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(UpsertEmployeeGoalCommand request, CancellationToken cancellationToken)
    {
        await EnsureCycleEditable(request.EmployeeGoal.CycleId, cancellationToken);
        var dto = request.EmployeeGoal;
        var item = dto.Id == Guid.Empty
            ? await dbContext.EmployeeGoals.FirstOrDefaultAsync(x => x.EmployeeId == dto.EmployeeId && x.PerformanceCycleId == dto.CycleId && x.GoalDefinitionId == dto.GoalDefinitionId, cancellationToken)
            : await dbContext.EmployeeGoals.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
        if (item is null)
        {
            item = EmployeeGoal.Create(Guid.NewGuid(), dto.EmployeeId, dto.GoalDefinitionId, dto.CycleId, dto.TargetValue, dto.Weight);
            await dbContext.EmployeeGoals.AddAsync(item, cancellationToken);
        }
        else
        {
            item.Update(dto.TargetValue, dto.Weight);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Saved", "Employee goal saved");

        async Task EnsureCycleEditable(Guid cycleId, CancellationToken token)
        {
            var cycle = await dbContext.PerformanceCycles.AsNoTracking().FirstAsync(x => x.Id == cycleId && !x.IsDeleted, token);
            if (cycle.IsClosed || cycle.IsCancelled)
                throw new InvalidOperationException("Closed or cancelled cycles cannot accept score edits");
        }
    }
}

public class UpdateEmployeeGoalAchievementHandler(PerformanceDbContext dbContext)
    : ICommandHandler<UpdateEmployeeGoalAchievementCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(UpdateEmployeeGoalAchievementCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.EmployeeGoals.FirstAsync(x => x.Id == request.Achievement.EmployeeGoalId, cancellationToken);
        await EnsureCycleEditable(dbContext, item.PerformanceCycleId, cancellationToken);
        item.UpdateAchievement(request.Achievement.AchievedValue);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Updated", "Goal achievement updated");
    }
}

public class ListEmployeeCompetencyScoresHandler(PerformanceDbContext dbContext)
    : IQueryHandler<ListEmployeeCompetencyScoresQuery, ListEmployeeCompetencyScoresResult>
{
    public async Task<ListEmployeeCompetencyScoresResult> Handle(ListEmployeeCompetencyScoresQuery request, CancellationToken cancellationToken)
    {
        var scores = await dbContext.EmployeeCompetencyScores.AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && x.PerformanceCycleId == request.CycleId)
            .ToListAsync(cancellationToken);
        var competencies = await dbContext.Competencies.AsNoTracking()
            .Where(x => scores.Select(s => s.CompetencyId).Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        return new(scores.Select(x => PerformanceMapper.ToDto(x, competencies.GetValueOrDefault(x.CompetencyId))).ToList());
    }
}

public class UpsertEmployeeCompetencyScoreHandler(PerformanceDbContext dbContext)
    : ICommandHandler<UpsertEmployeeCompetencyScoreCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(UpsertEmployeeCompetencyScoreCommand request, CancellationToken cancellationToken)
    {
        await EnsureCycleEditable(dbContext, request.CompetencyScore.CycleId, cancellationToken);
        var dto = request.CompetencyScore;
        var item = dto.Id == Guid.Empty
            ? await dbContext.EmployeeCompetencyScores.FirstOrDefaultAsync(x => x.EmployeeId == dto.EmployeeId && x.PerformanceCycleId == dto.CycleId && x.CompetencyId == dto.CompetencyId, cancellationToken)
            : await dbContext.EmployeeCompetencyScores.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellationToken);
        if (item is null)
        {
            item = EmployeeCompetencyScore.Create(Guid.NewGuid(), dto.EmployeeId, dto.CompetencyId, dto.CycleId, dto.Score, dto.Weight);
            await dbContext.EmployeeCompetencyScores.AddAsync(item, cancellationToken);
        }
        else
        {
            item.Update(dto.Score, dto.Weight);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Saved", "Competency score saved");
    }
}

public class ListEvaluationsHandler(PerformanceDbContext dbContext) : IQueryHandler<ListEvaluationsQuery, ListEvaluationsResult>
{
    public async Task<ListEvaluationsResult> Handle(ListEvaluationsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.PerformanceEvaluations.AsNoTracking().Where(x => x.CompanyId == request.CompanyId && !x.IsDeleted);
        if (request.CycleId.HasValue && request.CycleId.Value != Guid.Empty) query = query.Where(x => x.PerformanceCycleId == request.CycleId.Value);
        if (request.EmployeeId.HasValue && request.EmployeeId.Value != Guid.Empty) query = query.Where(x => x.EmployeeId == request.EmployeeId.Value);
        var evaluations = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        var cycles = await dbContext.PerformanceCycles.AsNoTracking().Where(x => evaluations.Select(e => e.PerformanceCycleId).Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
        return new(evaluations.Select(x => PerformanceMapper.ToDto(x, cycles.GetValueOrDefault(x.PerformanceCycleId))).ToList());
    }
}

public class CreateEvaluationHandler(PerformanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateEvaluationCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(CreateEvaluationCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.PerformanceEvaluations.FirstOrDefaultAsync(x => x.EmployeeId == request.Evaluation.EmployeeId && x.PerformanceCycleId == request.Evaluation.CycleId && !x.IsDeleted, cancellationToken);
        if (existing is not null)
            return Result(existing.Id, existing.Status.ToString(), "Evaluation already exists");

        var evaluation = PerformanceEvaluation.Create(Guid.NewGuid(), request.Evaluation.CompanyId, request.Evaluation.EmployeeId, request.Evaluation.CycleId, CurrentUser(httpContextAccessor));
        await dbContext.PerformanceEvaluations.AddAsync(evaluation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(evaluation.Id, evaluation.Status.ToString(), "Evaluation created");
    }
}

public class EvaluationActionHandler(PerformanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<EvaluationActionCommand, PerformanceActionResultDto>
{
    public async Task<PerformanceActionResultDto> Handle(EvaluationActionCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var evaluation = await dbContext.PerformanceEvaluations.FirstAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        switch (request.Action.ToLowerInvariant())
        {
            case "recalculate":
                var goals = await dbContext.EmployeeGoals.Where(x => x.EmployeeId == evaluation.EmployeeId && x.PerformanceCycleId == evaluation.PerformanceCycleId).ToListAsync(cancellationToken);
                var competencies = await dbContext.EmployeeCompetencyScores.Where(x => x.EmployeeId == evaluation.EmployeeId && x.PerformanceCycleId == evaluation.PerformanceCycleId).ToListAsync(cancellationToken);
                evaluation.Calculate(goals, competencies);
                evaluation.ModifiedBy = userId;
                break;
            case "submit":
                evaluation.Submit(request.EmployeeFeedback ?? string.Empty, userId);
                break;
            case "review":
                evaluation.Review(request.ManagerFeedback ?? string.Empty, userId);
                break;
            case "approve":
                evaluation.Approve(request.ManagerFeedback ?? evaluation.ManagerComment ?? string.Empty, userId);
                break;
            case "cancel":
                evaluation.Cancel(userId);
                break;
            default:
                throw new InvalidOperationException("Unsupported evaluation action");
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(evaluation.Id, evaluation.Status.ToString(), "Evaluation updated");
    }
}

public class PerformanceEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var performance = app.MapGroup("/api/v1/performance");

        performance.MapGet("/cycles/company/{companyId:guid}", async (Guid companyId, ISender sender) => Results.Ok(await sender.Send(new ListCyclesQuery(companyId)))).RequireAuthorization(PermissionList.PerformancePermissions.View);
        performance.MapPost("/cycles", async (UpsertAppraisalCycleDto cycle, ISender sender) => Results.Ok(await sender.Send(new UpsertCycleCommand(cycle)))).RequireAuthorization(PermissionList.PerformancePermissions.Create);
        performance.MapPut("/cycles/{id:guid}", async (Guid id, UpsertAppraisalCycleDto cycle, ISender sender) => { cycle.Id = id; return Results.Ok(await sender.Send(new UpsertCycleCommand(cycle))); }).RequireAuthorization(PermissionList.PerformancePermissions.Edit);
        performance.MapPost("/cycles/{id:guid}/start", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new CycleActionCommand(id, "start")))).RequireAuthorization(PermissionList.PerformancePermissions.Edit);
        performance.MapPost("/cycles/{id:guid}/close", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new CycleActionCommand(id, "close")))).RequireAuthorization(PermissionList.PerformancePermissions.Approve);
        performance.MapPost("/cycles/{id:guid}/cancel", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new CycleActionCommand(id, "cancel")))).RequireAuthorization(PermissionList.PerformancePermissions.Approve);

        performance.MapGet("/goals/company/{companyId:guid}", async (Guid companyId, ISender sender) => Results.Ok(await sender.Send(new ListGoalDefinitionsQuery(companyId)))).RequireAuthorization(PermissionList.PerformancePermissions.View);
        performance.MapPost("/goals", async (UpsertGoalDefinitionDto goal, ISender sender) => Results.Ok(await sender.Send(new UpsertGoalDefinitionCommand(goal)))).RequireAuthorization(PermissionList.PerformancePermissions.Create);
        performance.MapPut("/goals/{id:guid}", async (Guid id, UpsertGoalDefinitionDto goal, ISender sender) => { goal.Id = id; return Results.Ok(await sender.Send(new UpsertGoalDefinitionCommand(goal))); }).RequireAuthorization(PermissionList.PerformancePermissions.Edit);
        performance.MapDelete("/goals/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteGoalDefinitionCommand(id)))).RequireAuthorization(PermissionList.PerformancePermissions.Edit);

        performance.MapGet("/competencies/company/{companyId:guid}", async (Guid companyId, ISender sender) => Results.Ok(await sender.Send(new ListCompetenciesQuery(companyId)))).RequireAuthorization(PermissionList.PerformancePermissions.View);
        performance.MapPost("/competencies", async (UpsertCompetencyDto competency, ISender sender) => Results.Ok(await sender.Send(new UpsertCompetencyCommand(competency)))).RequireAuthorization(PermissionList.PerformancePermissions.Create);
        performance.MapPut("/competencies/{id:guid}", async (Guid id, UpsertCompetencyDto competency, ISender sender) => { competency.Id = id; return Results.Ok(await sender.Send(new UpsertCompetencyCommand(competency))); }).RequireAuthorization(PermissionList.PerformancePermissions.Edit);
        performance.MapDelete("/competencies/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteCompetencyCommand(id)))).RequireAuthorization(PermissionList.PerformancePermissions.Edit);

        performance.MapGet("/employee-goals", async (Guid employeeId, Guid cycleId, ISender sender) => Results.Ok(await sender.Send(new ListEmployeeGoalsQuery(employeeId, cycleId)))).RequireAuthorization(PermissionList.PerformancePermissions.View);
        performance.MapPost("/employee-goals", async (UpsertEmployeeGoalDto employeeGoal, ISender sender) => Results.Ok(await sender.Send(new UpsertEmployeeGoalCommand(employeeGoal)))).RequireAuthorization(PermissionList.PerformancePermissions.Edit);
        performance.MapPost("/employee-goals/achievement", async (UpdateEmployeeGoalAchievementDto achievement, ISender sender) => Results.Ok(await sender.Send(new UpdateEmployeeGoalAchievementCommand(achievement)))).RequireAuthorization(PermissionList.PerformancePermissions.Edit);

        performance.MapGet("/competency-scores", async (Guid employeeId, Guid cycleId, ISender sender) => Results.Ok(await sender.Send(new ListEmployeeCompetencyScoresQuery(employeeId, cycleId)))).RequireAuthorization(PermissionList.PerformancePermissions.View);
        performance.MapPost("/competency-scores", async (UpsertEmployeeCompetencyScoreDto competencyScore, ISender sender) => Results.Ok(await sender.Send(new UpsertEmployeeCompetencyScoreCommand(competencyScore)))).RequireAuthorization(PermissionList.PerformancePermissions.Edit);

        performance.MapGet("/evaluations/company/{companyId:guid}", async (Guid companyId, Guid? cycleId, Guid? employeeId, ISender sender) => Results.Ok(await sender.Send(new ListEvaluationsQuery(companyId, cycleId, employeeId)))).RequireAuthorization(PermissionList.PerformancePermissions.View);
        performance.MapPost("/evaluations", async (CreateEmployeeAppraisalDto evaluation, ISender sender) => Results.Ok(await sender.Send(new CreateEvaluationCommand(evaluation)))).RequireAuthorization(PermissionList.PerformancePermissions.Create);
        performance.MapPost("/evaluations/{id:guid}/recalculate", async (Guid id, PerformanceActionDto body, ISender sender) => Results.Ok(await sender.Send(new EvaluationActionCommand(id, "recalculate", body.EmployeeFeedback, body.ManagerFeedback)))).RequireAuthorization(PermissionList.PerformancePermissions.Review);
        performance.MapPost("/evaluations/{id:guid}/submit", async (Guid id, PerformanceActionDto body, ISender sender) => Results.Ok(await sender.Send(new EvaluationActionCommand(id, "submit", body.EmployeeFeedback, body.ManagerFeedback)))).RequireAuthorization(PermissionList.PerformancePermissions.Edit);
        performance.MapPost("/evaluations/{id:guid}/review", async (Guid id, PerformanceActionDto body, ISender sender) => Results.Ok(await sender.Send(new EvaluationActionCommand(id, "review", body.EmployeeFeedback, body.ManagerFeedback)))).RequireAuthorization(PermissionList.PerformancePermissions.Review);
        performance.MapPost("/evaluations/{id:guid}/approve", async (Guid id, PerformanceActionDto body, ISender sender) => Results.Ok(await sender.Send(new EvaluationActionCommand(id, "approve", body.EmployeeFeedback, body.ManagerFeedback)))).RequireAuthorization(PermissionList.PerformancePermissions.Approve);
        performance.MapPost("/evaluations/{id:guid}/cancel", async (Guid id, PerformanceActionDto body, ISender sender) => Results.Ok(await sender.Send(new EvaluationActionCommand(id, "cancel", body.EmployeeFeedback, body.ManagerFeedback)))).RequireAuthorization(PermissionList.PerformancePermissions.Approve);
    }
}

internal static class PerformanceFeatureHelpers
{
    public static string CurrentUser(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static PerformanceActionResultDto Result(Guid id, string status, string message) => new()
    {
        Id = id,
        Status = status,
        Message = message,
        IsSuccess = true
    };

    public static void SoftDelete(Shared.DDD.Entity<Guid> entity, string userId)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.DeletedBy = userId;
    }

    public static async Task EnsureCycleEditable(PerformanceDbContext dbContext, Guid cycleId, CancellationToken cancellationToken)
    {
        var cycle = await dbContext.PerformanceCycles.AsNoTracking().FirstAsync(x => x.Id == cycleId && !x.IsDeleted, cancellationToken);
        if (cycle.IsClosed || cycle.IsCancelled)
            throw new InvalidOperationException("Closed or cancelled cycles cannot accept score edits");
    }
}
