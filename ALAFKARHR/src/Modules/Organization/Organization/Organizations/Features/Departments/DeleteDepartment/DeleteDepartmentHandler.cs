namespace Organization.Organizations.Features.Departments.DeleteDepartment;


public record DeleteDepartmentCommand(Guid Id) : ICommand<DeleteDepartmentResult>;
public record DeleteDepartmentResult(bool IsSuccess);
public class DeleteDepartmentHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<DeleteDepartmentCommand, DeleteDepartmentResult>
{
    public async Task<DeleteDepartmentResult> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var department= await dbContext.Departments.FirstOrDefaultAsync(x=> x.Id == request.Id, cancellationToken);
        if (department is null)
            throw new NotFoundException($"Department not found: {request.Id}");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        department.Remove(userId);
        return new DeleteDepartmentResult(true);

    }
}
