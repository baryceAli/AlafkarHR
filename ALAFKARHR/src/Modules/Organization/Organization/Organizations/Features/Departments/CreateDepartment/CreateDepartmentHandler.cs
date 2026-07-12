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

        var administrationId = request.Department.AdministrationId
            ?? throw new BadRequestException("Administration is required");

        var administration = await dbContext.Administrations.FindAsync([administrationId]);
        if (administration is null)
            throw new NotFoundException($"Administration not found: {administrationId}");

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
            administrationId,
            request.Department.HeadOfDepartment,
            request.Department.CompanyId,
            request.Department.IsActive,
            request.Department.ParentDepartmentId,
            request.Department.Location,
            request.Department.Longitude,
            request.Department.Latitude,
            request.Department.AllowedRadiusMeters,
            userId);

        await dbContext.Departments.AddAsync(department, cancellationToken);
        await dbContext.SaveChangesAsync();

        return new CreateDepartmentResult(department.Adapt<DepartmentDto>());

    }
}
