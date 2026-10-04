using GeneralSettings.Data;
using GeneralSettings.GeneralSettings.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Contracts.CQRS;
using Shared.Contracts.Organization;
using Shared.Exceptions;
using Shared.Setup;
using SharedWithUI.GeneralSettings.Dtos;
using System.Security.Claims;

namespace GeneralSettings.GeneralSettings.Features.SystemSetup;

public record GetCurrentSystemSetupQuery : IQuery<GetCurrentSystemSetupResult>;
public record GetCurrentSystemSetupResult(SetupSummaryDto Setup);
public record UpdateCurrentSystemSetupCommand(UpdateSetupProgressDto Progress) : ICommand<UpdateCurrentSystemSetupResult>;
public record UpdateCurrentSystemSetupResult(SetupSummaryDto Setup);

public sealed class SystemSetupHandler(
    GeneralSettingsDbContext dbContext,
    IEnumerable<ISetupReadinessContributor> contributors,
    IHttpContextAccessor httpContextAccessor,
    IBusinessLineEntitlementService businessLineEntitlementService,
    ILogger<SystemSetupHandler> logger)
    : IQueryHandler<GetCurrentSystemSetupQuery, GetCurrentSystemSetupResult>,
      ICommandHandler<UpdateCurrentSystemSetupCommand, UpdateCurrentSystemSetupResult>
{
    private const int SchemaVersion = 2;

    public async Task<GetCurrentSystemSetupResult> Handle(GetCurrentSystemSetupQuery request, CancellationToken cancellationToken)
    {
        var (companyId, user) = CurrentContext();
        var profile = await dbContext.CompanySetupProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        return new GetCurrentSystemSetupResult(await BuildSummaryAsync(companyId, user, profile, cancellationToken));
    }

    public async Task<UpdateCurrentSystemSetupResult> Handle(UpdateCurrentSystemSetupCommand request, CancellationToken cancellationToken)
    {
        var (companyId, user) = CurrentContext();
        var profile = await dbContext.CompanySetupProfiles.FirstOrDefaultAsync(x => x.CompanyId == companyId, cancellationToken);
        if (profile is null)
        {
            profile = CompanySetupProfile.Create(companyId, UserName(user));
            await dbContext.CompanySetupProfiles.AddAsync(profile, cancellationToken);
        }

        var beforeUpdate = await BuildSummaryAsync(companyId, user, profile, cancellationToken);
        var knownStepKeys = beforeUpdate.Tracks.SelectMany(x => x.Steps).Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var knownTrackKeys = beforeUpdate.Tracks.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var hiddenSelections = profile.GetSelectedTrackKeys().Where(x => !knownTrackKeys.Contains(x));
        var selectedTracks = SetupProgressPolicy.ExpandTrackDependencies(
                request.Progress.SelectedTrackKeys.Where(knownTrackKeys.Contains),
                ToDefinitions(beforeUpdate.Tracks))
            .Concat(hiddenSelections)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var skippedSteps = request.Progress.SkippedStepKeys.Where(knownStepKeys.Contains).ToList();
        var currentStep = knownStepKeys.Contains(request.Progress.CurrentStepKey ?? string.Empty)
            ? request.Progress.CurrentStepKey
            : profile.CurrentStepKey;

        profile.Update(currentStep, request.Progress.RemindersDismissed, selectedTracks, skippedSteps, false, UserName(user));
        await dbContext.SaveChangesAsync(cancellationToken);

        var summary = await BuildSummaryAsync(companyId, user, profile, cancellationToken);
        profile.Update(currentStep, request.Progress.RemindersDismissed, selectedTracks, skippedSteps, summary.IsComplete, UserName(user));
        await dbContext.SaveChangesAsync(cancellationToken);
        summary.CompletedAt = profile.CompletedAt;
        return new UpdateCurrentSystemSetupResult(summary);
    }

    private async Task<SetupSummaryDto> BuildSummaryAsync(
        Guid companyId,
        ClaimsPrincipal user,
        CompanySetupProfile? profile,
        CancellationToken cancellationToken)
    {
        var contributorList = contributors.ToList();
        var trackDefinitions = contributorList.SelectMany(x => x.Tracks)
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .OrderBy(x => x.Order)
            .ToList();
        var stepDefinitions = contributorList.SelectMany(x => x.Steps)
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .ToList();

        IReadOnlySet<string> licensedBusinessLines;
        try
        {
            licensedBusinessLines = await businessLineEntitlementService.GetCurrentLicensedBusinessLineKeysAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unable to resolve setup business-line licenses for company {CompanyId}.", companyId);
            licensedBusinessLines = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        trackDefinitions = trackDefinitions
            .Where(x => string.IsNullOrWhiteSpace(x.RequiredBusinessLineKey) || licensedBusinessLines.Contains(x.RequiredBusinessLineKey))
            .ToList();
        var availableTrackKeys = trackDefinitions.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);
        stepDefinitions = stepDefinitions.Where(x => availableTrackKeys.Contains(x.TrackKey)).ToList();

        var readiness = new Dictionary<string, SetupReadinessResult>(StringComparer.OrdinalIgnoreCase);
        foreach (var contributor in contributorList)
        {
            try
            {
                foreach (var result in await contributor.EvaluateAsync(companyId, cancellationToken)) readiness[result.StepKey] = result;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Setup readiness contributor {ModuleKey} failed for company {CompanyId}.", contributor.ModuleKey, companyId);
                foreach (var step in contributor.Steps.Where(x => availableTrackKeys.Contains(x.TrackKey)))
                {
                    readiness[step.Key] = new SetupReadinessResult(
                        step.Key,
                        false,
                        "Readiness could not be evaluated. Refresh or ask an administrator to review the module.",
                        "تعذر تقييم الجاهزية. حدّث الصفحة أو اطلب من المسؤول مراجعة الوحدة.",
                        true);
                }
            }
        }

        var coreReady = stepDefinitions
            .Where(x => x.TrackKey.Equals("core", StringComparison.OrdinalIgnoreCase) && !x.IsOptional && x.Key != "review-summary")
            .All(x => readiness.GetValueOrDefault(x.Key)?.IsComplete == true);
        readiness["review-summary"] = new SetupReadinessResult(
            "review-summary",
            coreReady,
            coreReady ? "All required core setup areas are ready." : "Review the incomplete core steps before finishing setup.",
            coreReady ? "جميع مجالات الإعداد الأساسي المطلوبة جاهزة." : "راجع خطوات الإعداد الأساسي غير المكتملة قبل الإنهاء.");

        var storedTracks = profile?.GetSelectedTrackKeys() ?? ["core", "compliance"];
        var selectedTracks = SetupProgressPolicy.ExpandTrackDependencies(storedTracks.Where(availableTrackKeys.Contains), trackDefinitions).ToHashSet(StringComparer.OrdinalIgnoreCase);
        selectedTracks.Add("core");
        var skippedSteps = (profile?.GetSkippedStepKeys() ?? []).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var steps = stepDefinitions
            .Select(x => ToStep(x, readiness, user, skippedSteps, selectedTracks, profile?.CompletedAt))
            .OrderBy(x => x.Order)
            .ToList();

        var selectedRequired = steps.Where(x => x.IsBlocking).ToList();
        var selectedCompleted = selectedRequired.Count(x => x.IsComplete);
        var isComplete = selectedRequired.Count > 0 && selectedCompleted == selectedRequired.Count;
        var completionPercent = SetupProgressPolicy.CompletionPercent(selectedRequired.Select(x => x.IsComplete));
        var coreSteps = steps.Where(x => x.TrackKey.Equals("core", StringComparison.OrdinalIgnoreCase) && !x.IsOptional).ToList();
        var coreCompleted = coreSteps.Count(x => x.IsComplete);
        var isCoreComplete = coreSteps.Count > 0 && coreCompleted == coreSteps.Count;
        var coreCompletionPercent = SetupProgressPolicy.CompletionPercent(coreSteps.Select(x => x.IsComplete));

        var tracks = trackDefinitions.Select(track =>
        {
            var trackSteps = steps.Where(x => x.TrackKey.Equals(track.Key, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Order).ToList();
            var required = trackSteps.Where(x => !x.IsOptional).ToList();
            var completed = required.Count(x => x.IsComplete);
            return new SetupTrackDto
            {
                Key = track.Key,
                TitleEn = track.TitleEn,
                TitleAr = track.TitleAr,
                DescriptionEn = track.DescriptionEn,
                DescriptionAr = track.DescriptionAr,
                Icon = track.Icon,
                Order = track.Order,
                IsOptional = track.IsOptional,
                IsSelected = selectedTracks.Contains(track.Key),
                IsAvailable = true,
                IsComplete = required.Count == 0 || completed == required.Count,
                CompletionPercent = required.Count == 0 ? 100 : SetupProgressPolicy.CompletionPercent(required.Select(x => x.IsComplete)),
                RequiredBusinessLineKey = track.RequiredBusinessLineKey,
                DependencyTrackKeys = track.DependencyTrackKeys?.ToList() ?? [],
                Steps = trackSteps
            };
        }).Where(x => x.Steps.Count > 0).OrderBy(x => x.Order).ToList();

        return new SetupSummaryDto
        {
            SchemaVersion = SchemaVersion,
            CurrentStepKey = profile?.CurrentStepKey ?? "company-profile",
            RemindersDismissed = profile?.RemindersDismissed ?? false,
            IsComplete = isComplete,
            CompletionPercent = completionPercent,
            IsCoreComplete = isCoreComplete,
            CoreCompletionPercent = coreCompletionPercent,
            HasAttentionRequired = steps.Any(x => selectedTracks.Contains(x.TrackKey) && x.Status == SetupStepStatus.AttentionRequired),
            ReadinessRefreshedAt = DateTime.UtcNow,
            CompletedAt = profile?.CompletedAt,
            SelectedTrackKeys = selectedTracks.OrderBy(x => x).ToList(),
            SkippedStepKeys = skippedSteps.OrderBy(x => x).ToList(),
            Tracks = tracks
        };
    }

    private static SetupStepDto ToStep(
        SetupStepDefinition definition,
        IReadOnlyDictionary<string, SetupReadinessResult> readiness,
        ClaimsPrincipal user,
        IReadOnlySet<string> skippedSteps,
        IReadOnlySet<string> selectedTracks,
        DateTime? previousCompletion)
    {
        var available = string.IsNullOrWhiteSpace(definition.RequiredPermission)
            || user.Claims.Any(x => string.Equals(x.Value, definition.RequiredPermission, StringComparison.Ordinal));
        var result = readiness.GetValueOrDefault(definition.Key);
        var applicable = result?.IsApplicable != false;
        var complete = result?.IsComplete == true;
        var skipped = definition.IsOptional && skippedSteps.Contains(definition.Key);
        var status = !applicable
            ? SetupStepStatus.NotApplicable
            : !available
            ? SetupStepStatus.Unavailable
            : complete
                ? SetupStepStatus.Complete
                : result?.AttentionRequired == true || previousCompletion.HasValue && !definition.IsOptional
                    ? SetupStepStatus.AttentionRequired
                    : definition.IsOptional || skipped
                        ? SetupStepStatus.Optional
                        : SetupStepStatus.Incomplete;

        return new SetupStepDto
        {
            Key = definition.Key,
            TrackKey = definition.TrackKey,
            TitleEn = definition.TitleEn,
            TitleAr = definition.TitleAr,
            DescriptionEn = definition.DescriptionEn,
            DescriptionAr = definition.DescriptionAr,
            Icon = definition.Icon,
            Route = definition.Route,
            RequiredPermission = definition.RequiredPermission,
            Order = definition.Order,
            IsOptional = definition.IsOptional,
            IsBlocking = selectedTracks.Contains(definition.TrackKey) && !definition.IsOptional && applicable,
            IsComplete = complete,
            IsAvailable = available,
            IsSkipped = skipped,
            Status = status,
            DetailEn = available ? result?.DetailEn : "Another administrator with the required permission must complete this step.",
            DetailAr = available ? result?.DetailAr : "يجب أن يكمل هذه الخطوة مسؤول آخر لديه الصلاحية المطلوبة.",
            DependencyStepKeys = definition.DependencyStepKeys?.ToList() ?? [],
            AvailabilityReasonEn = available ? null : $"Requires permission: {definition.RequiredPermission}",
            AvailabilityReasonAr = available ? null : $"تتطلب الصلاحية: {definition.RequiredPermission}"
        };
    }

    private (Guid CompanyId, ClaimsPrincipal User) CurrentContext()
    {
        var user = httpContextAccessor.HttpContext?.User ?? throw new BadRequestException("Authenticated user context is unavailable.");
        if (!Guid.TryParse(user.FindFirst("company_id")?.Value, out var companyId)) throw new BadRequestException("Company claim is missing.");
        return (companyId, user);
    }

    private static string UserName(ClaimsPrincipal user)
        => user.Identity?.Name ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system-setup";

    private static IEnumerable<SetupTrackDefinition> ToDefinitions(IEnumerable<SetupTrackDto> tracks)
        => tracks.Select(x => new SetupTrackDefinition(
            x.Key, x.TitleEn, x.TitleAr, x.DescriptionEn, x.DescriptionAr, x.Icon, x.Order,
            x.IsOptional, x.RequiredBusinessLineKey, x.DependencyTrackKeys));
}
