using EmployeeModule.Contracts.Employees.Features.GetStoreManagerEmployee;

namespace EmployeeModule.Employees.Features.Employees.GetStoreManagerEmployee;

public class GetStoreManagerEmployeeHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetStoreManagerEmployeeQuery, GetStoreManagerEmployeeResult>
{
    public async Task<GetStoreManagerEmployeeResult> Handle(GetStoreManagerEmployeeQuery request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.EmployeeId && x.CompanyId == request.CompanyId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee not found: {request.EmployeeId}");

        if (!employee.IsActive)
            throw new BadRequestException("Store manager employee must be active.");

        if (!employee.LinkedUserId.HasValue || employee.LinkedUserId.Value == Guid.Empty)
            throw new BadRequestException("Store manager employee must be linked to an application user.");

        return new GetStoreManagerEmployeeResult(
            employee.Id,
            employee.CompanyId,
            employee.LinkedUserId.Value,
            employee.FullName,
            employee.FullNameEng,
            employee.IsActive);
    }
}
