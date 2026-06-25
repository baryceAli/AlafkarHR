namespace Organization.Organizations.Features.OrganizationPlacement;

public class ValidateOrganizationPlacementHandler(OrganizationDbContext dbContext)
    : IQueryHandler<ValidateOrganizationPlacementQuery, ValidateOrganizationPlacementResult>
{
    public async Task<ValidateOrganizationPlacementResult> Handle(
        ValidateOrganizationPlacementQuery request,
        CancellationToken cancellationToken)
    {
        if (request.DepartmentId.HasValue && !request.AdministrationId.HasValue)
        {
            return Invalid("Department requires an administration.");
        }

        if (!request.AdministrationId.HasValue)
        {
            return Valid();
        }

        var administration = await dbContext.Administrations
            .AsNoTracking()
            .Where(x => x.Id == request.AdministrationId.Value)
            .Select(x => new { x.Id, x.CompanyId, x.BranchId })
            .FirstOrDefaultAsync(cancellationToken);

        if (administration is null)
        {
            return Invalid("Administration was not found.");
        }

        if (administration.CompanyId != request.CompanyId)
        {
            return Invalid("Administration does not belong to the selected company.");
        }

        if (request.BranchId.HasValue
            && administration.BranchId.HasValue
            && administration.BranchId.Value != request.BranchId.Value)
        {
            return Invalid("Administration does not belong to the selected branch.");
        }

        if (!request.DepartmentId.HasValue)
        {
            return Valid();
        }

        var department = await dbContext.Departments
            .AsNoTracking()
            .Where(x => x.Id == request.DepartmentId.Value)
            .Select(x => new { x.Id, x.CompanyId, x.AdministrationId })
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            return Invalid("Department was not found.");
        }

        if (department.CompanyId != request.CompanyId)
        {
            return Invalid("Department does not belong to the selected company.");
        }

        if (department.AdministrationId != administration.Id)
        {
            return Invalid("Department does not belong to the selected administration.");
        }

        return Valid();
    }

    private static ValidateOrganizationPlacementResult Valid() => new(true, null);
    private static ValidateOrganizationPlacementResult Invalid(string message) => new(false, message);
}
