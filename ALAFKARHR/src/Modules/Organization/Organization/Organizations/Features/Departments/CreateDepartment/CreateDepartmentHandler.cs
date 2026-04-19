namespace Organization.Organizations.Features.Departments.CreateDepartment;


public record CreateDepartmentCommand(DepartmentDto Department) : ICommand<CreateDepartmentResult>;
public record CreateDepartmentResult(DepartmentDto CreatedDepartment);
public class CreateDepartmentHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateDepartmentCommand, CreateDepartmentResult>
{
    public async Task<CreateDepartmentResult> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {

        var company = await dbContext.Companies.FindAsync([request.Department.CompanyId]);
        if (company is null)
            throw new NotFoundException($"Company not found: {request.Department.CompanyId}");

        var administration = await dbContext.Administrations.FindAsync([request.Department.AdministrationId]);
        if (administration is null)
            throw new NotFoundException($"Administration not found: {request.Department.AdministrationId}");

        //if(request.Department.HeadOfDepartment.HasValue)
        //{
            
        //}

        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var department = Department.Create(
            Guid.NewGuid(),
            request.Department.Name,
            request.Department.NameEng,
            request.Department.Code,
            request.Department.AdministrationId.Value,
            request.Department.HeadOfDepartment,
            request.Department.CompanyId,
            request.Department.IsActive,
            request.Department.HeadOfDepartment,
            userId);

        await dbContext.Departments.AddAsync(department, cancellationToken);
        await dbContext.SaveChangesAsync();

        return new CreateDepartmentResult(department.Adapt<DepartmentDto>());

    }
}
