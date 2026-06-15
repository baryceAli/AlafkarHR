using Auth.Contracts.Features.ResetCompanyAdminPassword;

namespace Organization.Organizations.Features.Companies.ResetChildCompanyAdminPassword;

public record ResetChildCompanyAdminPasswordCommand(Guid CompanyId, string TemporaryPassword) : ICommand<ResetChildCompanyAdminPasswordResult>;
public record ResetChildCompanyAdminPasswordResult(bool IsSuccess);

public class ResetChildCompanyAdminPasswordCommandValidator : AbstractValidator<ResetChildCompanyAdminPasswordCommand>
{
    public ResetChildCompanyAdminPasswordCommandValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.TemporaryPassword).NotEmpty();
    }
}

public class ResetChildCompanyAdminPasswordHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<ResetChildCompanyAdminPasswordCommand, ResetChildCompanyAdminPasswordResult>
{
    public async Task<ResetChildCompanyAdminPasswordResult> Handle(ResetChildCompanyAdminPasswordCommand request, CancellationToken cancellationToken)
    {
        var parentCompanyId = await ResolveParentCompanyIdAsync(cancellationToken);
        var childExists = await dbContext.Companies
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.CompanyId && x.ParentCompanyId == parentCompanyId, cancellationToken);

        if (!childExists)
            throw new NotFoundException($"Child company not found: {request.CompanyId}");

        var result = await sender.Send(new ResetCompanyAdminPasswordCommand(request.CompanyId, request.TemporaryPassword), cancellationToken);
        return new ResetChildCompanyAdminPasswordResult(result.IsSuccess);
    }

    private async Task<Guid> ResolveParentCompanyIdAsync(CancellationToken cancellationToken)
    {
        var companyIdValue = httpContextAccessor.HttpContext?.User?.FindFirst("company_id")?.Value;
        if (!Guid.TryParse(companyIdValue, out var companyId))
            throw new UnauthorizedAccessException("Current user is not linked to a company");

        var company = await dbContext.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Current user's company was not found");

        if (company.ParentCompanyId.HasValue)
            throw new UnauthorizedAccessException("Child companies cannot manage child companies");

        return companyId;
    }
}
