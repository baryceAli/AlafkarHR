using Shared.DDD;
using static Recruitment.Recruitment.Features.RecruitmentFeatureHelpers;

namespace Recruitment.Recruitment.Features;

public record ListStaffingPlansQuery(Guid CompanyId, int? Year) : IQuery<ListStaffingPlansResult>;
public record ListStaffingPlansResult(List<StaffingPlanDto> StaffingPlanList);
public record UpsertStaffingPlanCommand(UpsertStaffingPlanDto StaffingPlan) : ICommand<RecruitmentActionResultDto>;
public record DeleteStaffingPlanCommand(Guid Id) : ICommand<RecruitmentActionResultDto>;

public record ListJobRequisitionsQuery(Guid CompanyId, RecruitmentRequestStatus? Status) : IQuery<ListJobRequisitionsResult>;
public record ListJobRequisitionsResult(List<JobRequisitionDto> JobRequisitionList);
public record UpsertJobRequisitionCommand(UpsertJobRequisitionDto JobRequisition) : ICommand<RecruitmentActionResultDto>;
public record JobRequisitionActionCommand(Guid Id, string Action) : ICommand<RecruitmentActionResultDto>;

public record ListApplicantsQuery(Guid CompanyId, Guid? JobRequisitionId, RecruitmentRequestStatus? Status) : IQuery<ListApplicantsResult>;
public record ListApplicantsResult(List<ApplicantDto> ApplicantList);
public record UpsertApplicantCommand(UpsertApplicantDto Applicant) : ICommand<RecruitmentActionResultDto>;
public record ApplicantActionCommand(Guid Id, RecruitmentRequestStatus Status) : ICommand<RecruitmentActionResultDto>;

public record ListInterviewFeedbackQuery(Guid ApplicantId) : IQuery<ListInterviewFeedbackResult>;
public record ListInterviewFeedbackResult(List<InterviewFeedbackDto> InterviewFeedbackList);
public record UpsertInterviewFeedbackCommand(UpsertInterviewFeedbackDto InterviewFeedback) : ICommand<RecruitmentActionResultDto>;
public record DeleteInterviewFeedbackCommand(Guid Id) : ICommand<RecruitmentActionResultDto>;

public record ListJobOffersQuery(Guid CompanyId, Guid? ApplicantId) : IQuery<ListJobOffersResult>;
public record ListJobOffersResult(List<JobOfferDto> JobOfferList);
public record UpsertJobOfferCommand(UpsertJobOfferDto JobOffer) : ICommand<RecruitmentActionResultDto>;
public record JobOfferActionCommand(Guid Id, string Action, Guid? EmployeeId) : ICommand<RecruitmentActionResultDto>;

internal static class RecruitmentMapper
{
    public static StaffingPlanDto ToDto(StaffingPlan item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        Year = item.Year,
        DepartmentId = item.DepartmentId,
        PositionId = item.PositionId,
        PlannedHeadcount = item.PlannedHeadcount,
        Notes = item.Notes
    };

    public static JobRequisitionDto ToDto(JobRequisition item) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        DepartmentId = item.DepartmentId,
        PositionId = item.PositionId,
        Title = item.Title,
        Openings = item.Openings,
        Status = item.Status,
        StatusLabel = item.Status.ToString(),
        RequestedAt = item.RequestedAt
    };

    public static ApplicantDto ToDto(Applicant item, JobRequisition? requisition = null) => new()
    {
        Id = item.Id,
        CompanyId = item.CompanyId,
        JobRequisitionId = item.JobRequisitionId,
        JobRequisitionTitle = requisition?.Title,
        FullName = item.FullName,
        Email = item.Email,
        Phone = item.Phone,
        Status = item.Status,
        StatusLabel = item.Status.ToString()
    };

    public static InterviewFeedbackDto ToDto(InterviewFeedback item) => new()
    {
        Id = item.Id,
        ApplicantId = item.ApplicantId,
        InterviewerEmployeeId = item.InterviewerEmployeeId,
        InterviewerEmployeeName = item.InterviewerEmployeeId.ToString("N")[..8],
        InterviewAt = item.InterviewAt,
        Rating = item.Rating,
        Feedback = item.Feedback
    };

    public static JobOfferDto ToDto(JobOffer item, Applicant? applicant = null) => new()
    {
        Id = item.Id,
        ApplicantId = item.ApplicantId,
        ApplicantName = applicant?.FullName,
        OfferDate = item.OfferDate,
        AcceptedAt = item.AcceptedAt,
        RejectedAt = item.RejectedAt,
        ProposedSalary = item.ProposedSalary,
        CreatedEmployeeId = item.CreatedEmployeeId,
        IsAccepted = item.AcceptedAt.HasValue,
        IsRejected = item.RejectedAt.HasValue,
        StatusLabel = item.CreatedEmployeeId.HasValue ? "Employee Created" : item.AcceptedAt.HasValue ? "Accepted" : item.RejectedAt.HasValue ? "Rejected" : "Draft"
    };
}

public class ListStaffingPlansHandler(RecruitmentDbContext dbContext) : IQueryHandler<ListStaffingPlansQuery, ListStaffingPlansResult>
{
    public async Task<ListStaffingPlansResult> Handle(ListStaffingPlansQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.StaffingPlans.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (request.Year.HasValue) query = query.Where(x => x.Year == request.Year.Value);
        var data = await query.OrderByDescending(x => x.Year).ToListAsync(cancellationToken);
        return new(data.Select(RecruitmentMapper.ToDto).ToList());
    }
}

public class UpsertStaffingPlanHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertStaffingPlanCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(UpsertStaffingPlanCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.StaffingPlan;
        var item = dto.Id == Guid.Empty
            ? StaffingPlan.Create(Guid.NewGuid(), dto.CompanyId, dto.Year, dto.DepartmentId, dto.PositionId, dto.PlannedHeadcount, dto.Notes, userId)
            : await dbContext.StaffingPlans.FirstAsync(x => x.Id == dto.Id, cancellationToken);
        if (dto.Id == Guid.Empty) await dbContext.StaffingPlans.AddAsync(item, cancellationToken);
        else item.Update(dto.Year, dto.DepartmentId, dto.PositionId, dto.PlannedHeadcount, dto.Notes, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Saved", "Staffing plan saved");
    }
}

public class DeleteStaffingPlanHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteStaffingPlanCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(DeleteStaffingPlanCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.StaffingPlans.FirstAsync(x => x.Id == request.Id, cancellationToken);
        SoftDelete(item, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Deleted", "Staffing plan deleted");
    }
}

public class ListJobRequisitionsHandler(RecruitmentDbContext dbContext) : IQueryHandler<ListJobRequisitionsQuery, ListJobRequisitionsResult>
{
    public async Task<ListJobRequisitionsResult> Handle(ListJobRequisitionsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.JobRequisitions.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
        var data = await query.OrderByDescending(x => x.RequestedAt).ToListAsync(cancellationToken);
        return new(data.Select(RecruitmentMapper.ToDto).ToList());
    }
}

public class UpsertJobRequisitionHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertJobRequisitionCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(UpsertJobRequisitionCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.JobRequisition;
        var item = dto.Id == Guid.Empty
            ? JobRequisition.Create(Guid.NewGuid(), dto.CompanyId, dto.DepartmentId, dto.PositionId, dto.Title, dto.Openings, dto.RequestedAt, userId)
            : await dbContext.JobRequisitions.FirstAsync(x => x.Id == dto.Id, cancellationToken);
        if (dto.Id == Guid.Empty) await dbContext.JobRequisitions.AddAsync(item, cancellationToken);
        else item.Update(dto.DepartmentId, dto.PositionId, dto.Title, dto.Openings, dto.RequestedAt, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Job requisition saved");
    }
}

public class JobRequisitionActionHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<JobRequisitionActionCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(JobRequisitionActionCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.JobRequisitions.FirstAsync(x => x.Id == request.Id, cancellationToken);
        var userId = CurrentUser(httpContextAccessor);
        switch (request.Action.ToLowerInvariant())
        {
            case "open": item.Open(userId); break;
            case "cancel": item.Cancel(userId); break;
            case "close": item.Close(userId); break;
            default: throw new InvalidOperationException("Unsupported requisition action");
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Job requisition updated");
    }
}

public class ListApplicantsHandler(RecruitmentDbContext dbContext) : IQueryHandler<ListApplicantsQuery, ListApplicantsResult>
{
    public async Task<ListApplicantsResult> Handle(ListApplicantsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Applicants.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (request.JobRequisitionId.HasValue && request.JobRequisitionId.Value != Guid.Empty) query = query.Where(x => x.JobRequisitionId == request.JobRequisitionId.Value);
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
        var applicants = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);
        var requisitions = await dbContext.JobRequisitions.AsNoTracking()
            .Where(x => applicants.Select(a => a.JobRequisitionId).Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        return new(applicants.Select(x => RecruitmentMapper.ToDto(x, x.JobRequisitionId.HasValue ? requisitions.GetValueOrDefault(x.JobRequisitionId.Value) : null)).ToList());
    }
}

public class UpsertApplicantHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertApplicantCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(UpsertApplicantCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.Applicant;
        var item = dto.Id == Guid.Empty
            ? Applicant.Create(Guid.NewGuid(), dto.CompanyId, dto.JobRequisitionId, dto.FullName, dto.Email, dto.Phone, userId)
            : await dbContext.Applicants.FirstAsync(x => x.Id == dto.Id, cancellationToken);
        if (dto.Id == Guid.Empty) await dbContext.Applicants.AddAsync(item, cancellationToken);
        else item.Update(dto.JobRequisitionId, dto.FullName, dto.Email, dto.Phone, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Applicant saved");
    }
}

public class ApplicantActionHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<ApplicantActionCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(ApplicantActionCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.Applicants.FirstAsync(x => x.Id == request.Id, cancellationToken);
        item.MoveTo(request.Status, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, item.Status.ToString(), "Applicant status updated");
    }
}

public class ListInterviewFeedbackHandler(RecruitmentDbContext dbContext) : IQueryHandler<ListInterviewFeedbackQuery, ListInterviewFeedbackResult>
{
    public async Task<ListInterviewFeedbackResult> Handle(ListInterviewFeedbackQuery request, CancellationToken cancellationToken)
    {
        var data = await dbContext.InterviewFeedbacks.AsNoTracking()
            .Where(x => x.ApplicantId == request.ApplicantId)
            .OrderByDescending(x => x.InterviewAt)
            .ToListAsync(cancellationToken);
        return new(data.Select(RecruitmentMapper.ToDto).ToList());
    }
}

public class UpsertInterviewFeedbackHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertInterviewFeedbackCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(UpsertInterviewFeedbackCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.InterviewFeedback;
        var item = dto.Id == Guid.Empty
            ? InterviewFeedback.Create(Guid.NewGuid(), dto.ApplicantId, dto.InterviewerEmployeeId, dto.InterviewAt, dto.Rating, dto.Feedback, userId)
            : await dbContext.InterviewFeedbacks.FirstAsync(x => x.Id == dto.Id, cancellationToken);
        if (dto.Id == Guid.Empty) await dbContext.InterviewFeedbacks.AddAsync(item, cancellationToken);
        else item.Update(dto.InterviewerEmployeeId, dto.InterviewAt, dto.Rating, dto.Feedback, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Saved", "Interview feedback saved");
    }
}

public class DeleteInterviewFeedbackHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteInterviewFeedbackCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(DeleteInterviewFeedbackCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.InterviewFeedbacks.FirstAsync(x => x.Id == request.Id, cancellationToken);
        SoftDelete(item, CurrentUser(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Deleted", "Interview feedback deleted");
    }
}

public class ListJobOffersHandler(RecruitmentDbContext dbContext) : IQueryHandler<ListJobOffersQuery, ListJobOffersResult>
{
    public async Task<ListJobOffersResult> Handle(ListJobOffersQuery request, CancellationToken cancellationToken)
    {
        var applicantsQuery = dbContext.Applicants.AsNoTracking().Where(x => x.CompanyId == request.CompanyId);
        if (request.ApplicantId.HasValue && request.ApplicantId.Value != Guid.Empty) applicantsQuery = applicantsQuery.Where(x => x.Id == request.ApplicantId.Value);
        var applicants = await applicantsQuery.ToDictionaryAsync(x => x.Id, cancellationToken);
        var applicantIds = applicants.Keys.ToList();
        var offers = await dbContext.JobOffers.AsNoTracking()
            .Where(x => applicantIds.Contains(x.ApplicantId))
            .OrderByDescending(x => x.OfferDate)
            .ToListAsync(cancellationToken);
        return new(offers.Select(x => RecruitmentMapper.ToDto(x, applicants.GetValueOrDefault(x.ApplicantId))).ToList());
    }
}

public class UpsertJobOfferHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpsertJobOfferCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(UpsertJobOfferCommand request, CancellationToken cancellationToken)
    {
        var userId = CurrentUser(httpContextAccessor);
        var dto = request.JobOffer;
        var item = dto.Id == Guid.Empty
            ? JobOffer.Create(Guid.NewGuid(), dto.ApplicantId, dto.OfferDate, dto.ProposedSalary, userId)
            : await dbContext.JobOffers.FirstAsync(x => x.Id == dto.Id, cancellationToken);
        if (dto.Id == Guid.Empty) await dbContext.JobOffers.AddAsync(item, cancellationToken);
        else item.Update(dto.OfferDate, dto.ProposedSalary, userId);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, "Saved", "Job offer saved");
    }
}

public class JobOfferActionHandler(RecruitmentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<JobOfferActionCommand, RecruitmentActionResultDto>
{
    public async Task<RecruitmentActionResultDto> Handle(JobOfferActionCommand request, CancellationToken cancellationToken)
    {
        var item = await dbContext.JobOffers.FirstAsync(x => x.Id == request.Id, cancellationToken);
        var userId = CurrentUser(httpContextAccessor);
        switch (request.Action.ToLowerInvariant())
        {
            case "accept":
                item.Accept(userId);
                await MoveApplicant(item.ApplicantId, RecruitmentRequestStatus.Hired, userId, cancellationToken);
                break;
            case "reject":
                item.Reject(userId);
                break;
            case "mark-employee-created":
                item.MarkEmployeeCreated(request.EmployeeId ?? throw new InvalidOperationException("Employee id is required"), userId);
                break;
            default: throw new InvalidOperationException("Unsupported offer action");
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result(item.Id, request.Action, "Job offer updated");
    }

    private async Task MoveApplicant(Guid applicantId, RecruitmentRequestStatus status, string userId, CancellationToken cancellationToken)
    {
        var applicant = await dbContext.Applicants.FirstAsync(x => x.Id == applicantId, cancellationToken);
        applicant.MoveTo(status, userId);
    }
}

public class RecruitmentEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var recruitment = app.MapGroup("/api/v1/recruitment");

        recruitment.MapGet("/staffing-plans/company/{companyId:guid}", async (Guid companyId, int? year, ISender sender) => Results.Ok(await sender.Send(new ListStaffingPlansQuery(companyId, year)))).RequireAuthorization(PermissionList.RecruitmentPermissions.View);
        recruitment.MapPost("/staffing-plans", async (UpsertStaffingPlanDto plan, ISender sender) => Results.Ok(await sender.Send(new UpsertStaffingPlanCommand(plan)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Create);
        recruitment.MapPut("/staffing-plans/{id:guid}", async (Guid id, UpsertStaffingPlanDto plan, ISender sender) => { plan.Id = id; return Results.Ok(await sender.Send(new UpsertStaffingPlanCommand(plan))); }).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);
        recruitment.MapDelete("/staffing-plans/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteStaffingPlanCommand(id)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);

        recruitment.MapGet("/job-requisitions/company/{companyId:guid}", async (Guid companyId, RecruitmentRequestStatus? status, ISender sender) => Results.Ok(await sender.Send(new ListJobRequisitionsQuery(companyId, status)))).RequireAuthorization(PermissionList.RecruitmentPermissions.View);
        recruitment.MapPost("/job-requisitions", async (UpsertJobRequisitionDto requisition, ISender sender) => Results.Ok(await sender.Send(new UpsertJobRequisitionCommand(requisition)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Create);
        recruitment.MapPut("/job-requisitions/{id:guid}", async (Guid id, UpsertJobRequisitionDto requisition, ISender sender) => { requisition.Id = id; return Results.Ok(await sender.Send(new UpsertJobRequisitionCommand(requisition))); }).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);
        recruitment.MapPost("/job-requisitions/{id:guid}/open", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new JobRequisitionActionCommand(id, "open")))).RequireAuthorization(PermissionList.RecruitmentPermissions.Approve);
        recruitment.MapPost("/job-requisitions/{id:guid}/cancel", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new JobRequisitionActionCommand(id, "cancel")))).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);
        recruitment.MapPost("/job-requisitions/{id:guid}/close", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new JobRequisitionActionCommand(id, "close")))).RequireAuthorization(PermissionList.RecruitmentPermissions.Approve);

        recruitment.MapGet("/applicants/company/{companyId:guid}", async (Guid companyId, Guid? jobRequisitionId, RecruitmentRequestStatus? status, ISender sender) => Results.Ok(await sender.Send(new ListApplicantsQuery(companyId, jobRequisitionId, status)))).RequireAuthorization(PermissionList.RecruitmentPermissions.View);
        recruitment.MapPost("/applicants", async (UpsertApplicantDto applicant, ISender sender) => Results.Ok(await sender.Send(new UpsertApplicantCommand(applicant)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Create);
        recruitment.MapPut("/applicants/{id:guid}", async (Guid id, UpsertApplicantDto applicant, ISender sender) => { applicant.Id = id; return Results.Ok(await sender.Send(new UpsertApplicantCommand(applicant))); }).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);
        recruitment.MapPost("/applicants/{id:guid}/status", async (Guid id, RecruitmentStatusActionDto action, ISender sender) => Results.Ok(await sender.Send(new ApplicantActionCommand(id, action.Status)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);

        recruitment.MapGet("/applicants/{applicantId:guid}/interviews", async (Guid applicantId, ISender sender) => Results.Ok(await sender.Send(new ListInterviewFeedbackQuery(applicantId)))).RequireAuthorization(PermissionList.RecruitmentPermissions.View);
        recruitment.MapPost("/interviews", async (UpsertInterviewFeedbackDto feedback, ISender sender) => Results.Ok(await sender.Send(new UpsertInterviewFeedbackCommand(feedback)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Create);
        recruitment.MapPut("/interviews/{id:guid}", async (Guid id, UpsertInterviewFeedbackDto feedback, ISender sender) => { feedback.Id = id; return Results.Ok(await sender.Send(new UpsertInterviewFeedbackCommand(feedback))); }).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);
        recruitment.MapDelete("/interviews/{id:guid}", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new DeleteInterviewFeedbackCommand(id)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);

        recruitment.MapGet("/job-offers/company/{companyId:guid}", async (Guid companyId, Guid? applicantId, ISender sender) => Results.Ok(await sender.Send(new ListJobOffersQuery(companyId, applicantId)))).RequireAuthorization(PermissionList.RecruitmentPermissions.View);
        recruitment.MapPost("/job-offers", async (UpsertJobOfferDto offer, ISender sender) => Results.Ok(await sender.Send(new UpsertJobOfferCommand(offer)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Create);
        recruitment.MapPut("/job-offers/{id:guid}", async (Guid id, UpsertJobOfferDto offer, ISender sender) => { offer.Id = id; return Results.Ok(await sender.Send(new UpsertJobOfferCommand(offer))); }).RequireAuthorization(PermissionList.RecruitmentPermissions.Edit);
        recruitment.MapPost("/job-offers/{id:guid}/accept", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new JobOfferActionCommand(id, "accept", null)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Approve);
        recruitment.MapPost("/job-offers/{id:guid}/reject", async (Guid id, ISender sender) => Results.Ok(await sender.Send(new JobOfferActionCommand(id, "reject", null)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Approve);
        recruitment.MapPost("/job-offers/{id:guid}/mark-employee-created", async (Guid id, RecruitmentHireActionDto action, ISender sender) => Results.Ok(await sender.Send(new JobOfferActionCommand(id, "mark-employee-created", action.EmployeeId)))).RequireAuthorization(PermissionList.RecruitmentPermissions.Hire);
    }
}

internal static class RecruitmentFeatureHelpers
{
    public static string CurrentUser(IHttpContextAccessor httpContextAccessor) =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    public static RecruitmentActionResultDto Result(Guid id, string status, string message) => new()
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
