using Shared.SaveImages;

namespace Organization.Organizations.Features.Companies.UpdateCompany;

public record UpdateCompanyCommand(CompanyDto Company) : ICommand<UpdateCompanyResult>;
public record UpdateCompanyResult(bool IsSuccess);

public class UpdateCompanyHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateCompanyCommand, UpdateCompanyResult>
{
    public async Task<UpdateCompanyResult> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await dbContext.Companies.FirstOrDefaultAsync(x => x.Id == request.Company.Id, cancellationToken);
        if (company is null)
            throw new NotFoundException($"Company not found: {request.Company.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value
                        ?? throw new UnauthorizedAccessException("User not authenticated");

        if (request.Company.ParentCompanyId == request.Company.Id)
            throw new Exception("A company cannot be its own parent");

        if (request.Company.ParentCompanyId.HasValue)
        {
            var parentCompany = await dbContext.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Company.ParentCompanyId.Value, cancellationToken);

            if (parentCompany is null)
                throw new NotFoundException($"Parent company not found: {request.Company.ParentCompanyId.Value}");

            if (await WouldCreateCycleAsync(company.Id, request.Company.ParentCompanyId.Value, cancellationToken))
                throw new Exception("Company hierarchy cannot contain circular ownership");
        }

        string finalLogoPath = company.Logo;
        var incomingLogo = request.Company.Logo;

        if (!string.IsNullOrWhiteSpace(incomingLogo))
        {
            if (SaveImages.IsBase64Image(incomingLogo))
            {
                string[] pathSegments = ["wwwroot", "Images", "Companies"];
                finalLogoPath = SaveImages.SaveBase64Image($"{company.Id}", pathSegments, incomingLogo);
            }
            else
            {
                finalLogoPath = incomingLogo;
            }
        }

        company.Update(
            request.Company.Name,
            request.Company.NameEng,
            finalLogoPath,
            request.Company.HqLocation,
            request.Company.HqLongitude,
            request.Company.HqLatitude,
            request.Company.VatNo,
            request.Company.CurrencyId ?? company.CurrencyId,
            userId);
        company.UpdateParentCompany(request.Company.ParentCompanyId, userId);

        await dbContext.SaveChangesAsync();
        return new UpdateCompanyResult(true);
    }

    private async Task<bool> WouldCreateCycleAsync(Guid companyId, Guid parentCompanyId, CancellationToken cancellationToken)
    {
        var currentParentId = parentCompanyId;

        while (true)
        {
            if (currentParentId == companyId)
                return true;

            var nextParentId = await dbContext.Companies
                .AsNoTracking()
                .Where(x => x.Id == currentParentId)
                .Select(x => x.ParentCompanyId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!nextParentId.HasValue)
                return false;

            currentParentId = nextParentId.Value;
        }
    }
}
