namespace Organization.Organizations.Features.Companies.CreateCompany;

public record CreateCompanyCommand(CompanyDto Company) : ICommand<CreateCompanyResult>;
public record CreateCompanyResult(CompanyDto CreatedCompany);

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Company.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Company.NameEng).NotEmpty().WithMessage("NameEng is required");
        RuleFor(x => x.Company.Code).NotEmpty().WithMessage("Code is required");
        RuleFor(x => x.Company.HqLocation).NotEmpty().WithMessage("HqLocation is required");
        RuleFor(x => x.Company.VatNo).NotEmpty().WithMessage("VatNo is required");
    }
}
public class CreateCompanyHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateCompanyCommand, CreateCompanyResult>
{
    public async Task<CreateCompanyResult> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
         .User?
         .FindFirst(ClaimTypes.NameIdentifier)?
         .Value
         ?? throw new UnauthorizedAccessException("User not authenticated");

        var company =Models.Company.Create(
            Guid.NewGuid(),
            request.Company.Name,
            request.Company.NameEng,
            request.Company.Logo,
            request.Company.HqLocation,
            request.Company.HqLongitude,
            request.Company.HqLatitude,
            request.Company.VatNo,
            request.Company.Code,
            request.Company.Currency,
            request.Company.Email,
            request.Company.Phone,
            request.Company.TimeZone,
            userId
            );

        await dbContext.Companies.AddAsync(company, cancellationToken);
        await dbContext.SaveChangesAsync();
        return new CreateCompanyResult(company.Adapt<CompanyDto>());


    }
}
