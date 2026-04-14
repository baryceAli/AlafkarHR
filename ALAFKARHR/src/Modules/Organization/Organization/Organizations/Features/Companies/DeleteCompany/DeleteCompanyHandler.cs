namespace Organization.Organizations.Features.Companies.DeleteCompany;

public record DeleteCompanyCommand(Guid Id) : ICommand<DeleteCompanyResult>;
public record DeleteCompanyResult(bool IsSuccess);
public class DeleteCompanyHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteCompanyCommand, DeleteCompanyResult>
{
    public async Task<DeleteCompanyResult> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company=await dbContext.Companies.FirstOrDefaultAsync(x=> x.Id==request.Id, cancellationToken);

        if (company is null)
            throw new NotFoundException($"Company not found: {request.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)
                        .Value ??
                        throw new UnauthorizedAccessException("User is unauthorized");

        company.Remove(userId);
        await dbContext.SaveChangesAsync();
        return new DeleteCompanyResult(true);
    }
}
