namespace Shared.Setup;

public interface ISetupReadinessContributor
{
    string ModuleKey => GetType().Name;
    IReadOnlyCollection<SetupTrackDefinition> Tracks => [];
    IReadOnlyCollection<SetupStepDefinition> Steps => [];

    Task<IReadOnlyCollection<SetupReadinessResult>> EvaluateAsync(
        Guid companyId,
        CancellationToken cancellationToken);
}

public interface ISetupCountryPackContributor : ISetupReadinessContributor
{
    IReadOnlyCollection<string> ApplicableCountryCodes { get; }
}

public sealed record SetupReadinessResult(
    string StepKey,
    bool IsComplete,
    string? DetailEn = null,
    string? DetailAr = null,
    bool AttentionRequired = false,
    bool IsApplicable = true);

public sealed record SetupTrackDefinition(
    string Key,
    string TitleEn,
    string TitleAr,
    string DescriptionEn,
    string DescriptionAr,
    string Icon,
    int Order,
    bool IsOptional,
    string? RequiredBusinessLineKey = null,
    IReadOnlyCollection<string>? DependencyTrackKeys = null);

public sealed record SetupStepDefinition(
    string Key,
    string TrackKey,
    string TitleEn,
    string TitleAr,
    string DescriptionEn,
    string DescriptionAr,
    string Icon,
    string Route,
    string RequiredPermission,
    int Order,
    bool IsOptional,
    IReadOnlyCollection<string>? DependencyStepKeys = null);
