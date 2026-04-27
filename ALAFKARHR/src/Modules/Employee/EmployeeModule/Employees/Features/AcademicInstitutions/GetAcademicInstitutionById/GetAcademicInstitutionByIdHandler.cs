using EmployeeModule.Employees.Models;
using Shared.Contracts.CQRS;
using Shared.Exceptions;

namespace EmployeeModule.Employees.Features.AcademicInstitutions.GetAcademicInstitutionById;

public record GetAcademicInstitutionByIdQuery(Guid Id):IQuery<GetAcademicInstitutionByIdResult>;
public record GetAcademicInstitutionByIdResult(AcademicInstitutionDto AcademicInstitution);
public class GetAcademicInstitutionByIdHandler(EmployeeDbContext dbContext)
    : IQueryHandler<GetAcademicInstitutionByIdQuery, GetAcademicInstitutionByIdResult>
{
    public async Task<GetAcademicInstitutionByIdResult> Handle(GetAcademicInstitutionByIdQuery request, CancellationToken cancellationToken)
    {
        var academic=await dbContext.AcademicInstitutions.AsNoTracking().FirstOrDefaultAsync(a=> a.Id==request.Id&&a.IsDeleted==false,cancellationToken);
        if (academic is null)
            throw new NotFoundException($"Academic Institution not found: {request.Id}");

        return new GetAcademicInstitutionByIdResult(academic.Adapt<AcademicInstitutionDto>());
    }
}