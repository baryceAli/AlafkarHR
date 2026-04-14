namespace Organization.Organizations.Features.Departments.GetDepartmentById;


public record GetDepartmentByIdQuery(Guid Id) : IQuery<GetDepartmentByIdResult>;
public record GetDepartmentByIdResult(DepartmentDto Department);
public class GetDepartmentByIdHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetDepartmentByIdQuery, GetDepartmentByIdResult>
{
    public async Task<GetDepartmentByIdResult> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        var department=await dbContext.Departments.FirstOrDefaultAsync(x=> x.Id == request.Id, cancellationToken);
        if (department is null)
            throw new NotFoundException($"Administrator is not found: {request.Id}");

        return new GetDepartmentByIdResult(department.Adapt<DepartmentDto>());
    }
}
