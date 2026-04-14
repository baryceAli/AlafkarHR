namespace Organization.Organizations.Features.Branches.CreateBranch;


public record CreateBranchCommand(BranchDto Branch) : ICommand<CreateBranchResult>;
public record CreateBranchResult(BranchDto CreatedBranch);
public class CreateBranchHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateBranchCommand, CreateBranchResult>
{
    public async Task<CreateBranchResult> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {

        var company = await dbContext.Companies.FindAsync([request.Branch.CompanyId]);
        if (company is null)
            throw new NotFoundException($"Company not found: {request.Branch.CompanyId}");


        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ?? 
                        throw new UnauthorizedAccessException("User is not authenticated");
        
        var branch = Branch.Create(
            Guid.NewGuid(),
            request.Branch.Name,
            request.Branch.NameEng,
            request.Branch.Location,
            request.Branch.Longitude,
            request.Branch.Latitude,
            request.Branch.Code,
            request.Branch.Phone,
            request.Branch.Email,
            request.Branch.IsMainBranch,
            request.Branch.CompanyId,
            userId
            );

        await dbContext.Branches.AddAsync(branch, cancellationToken);
        await dbContext.SaveChangesAsync();
        return new CreateBranchResult(branch.Adapt<BranchDto>());

    }
}
