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
            userId);

        await dbContext.SaveChangesAsync();
        return new UpdateCompanyResult(true);
    }
}
