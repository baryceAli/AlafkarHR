namespace Organization.Organizations.Features.Departments.GetDepartmentsByAdministrationId;


public record GetDepartmentsByAdministrationIdQuery(Guid AdministrationId) : IQuery<GetDepartmentsByAdministrationIdResult>;
public record GetDepartmentsByAdministrationIdResult(List<DepartmentDto> DepartmentList);
public class GetDepartmentsByAdministrationIdHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetDepartmentsByAdministrationIdQuery, GetDepartmentsByAdministrationIdResult>
{
    public async Task<GetDepartmentsByAdministrationIdResult> Handle(GetDepartmentsByAdministrationIdQuery request, CancellationToken cancellationToken)
    {
        var departments = await dbContext.Departments
            .AsNoTracking()
            .Where(x => x.AdministrationId == request.AdministrationId)
            .ToListAsync(cancellationToken);

        return new GetDepartmentsByAdministrationIdResult(departments.Adapt<List<DepartmentDto>>());

    }
}
