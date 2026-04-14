namespace Organization.Organizations.Features.Departments.UpdateDepartment;


public record UpdateDepartmentCommand(DepartmentDto Department) : ICommand<UpdateDepartmentResult>;
public record UpdateDepartmentResult(bool IsSuccess);
public class UpdateDepartmentHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateDepartmentCommand, UpdateDepartmentResult>
{
    public async Task<UpdateDepartmentResult> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments.FirstOrDefaultAsync(x => x.Id == request.Department.Id, cancellationToken);

        if (department is null)
            throw new NotFoundException($"Department not found: {request.Department.Id}");


        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        department.Update(
            request.Department.Name,
            request.Department.NameEng,
            userId);

        await dbContext.SaveChangesAsync();
        return new UpdateDepartmentResult(true);
    }
}
