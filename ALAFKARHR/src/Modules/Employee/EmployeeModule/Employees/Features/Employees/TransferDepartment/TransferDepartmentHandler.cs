using FluentValidation;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using System.Security.Claims;
using System.Windows.Input;

namespace EmployeeModule.Employees.Features.Employees.TransferDepartment;

public record TransferDepartmentCommand(TransferDepartmentDto TransferDepartment) : ICommand<TransferDepartmentRresult>;
public record TransferDepartmentRresult(bool IsSuccess);

public class TransferDepartmentCommandValidator : AbstractValidator<TransferDepartmentCommand>
{
    public TransferDepartmentCommandValidator()
    {
        RuleFor(c => c.TransferDepartment).NotEmpty().WithMessage("Transfer department is required");
        RuleFor(c => c.TransferDepartment.branchId).NotEmpty().WithMessage("Branch is required");
        //RuleFor(c => c.TransferDepartment.).NotEmpty().WithMessage("Branch is required");
    }
}
public class TransferDepartmentHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<TransferDepartmentCommand, TransferDepartmentRresult>
{
    public async Task<TransferDepartmentRresult> Handle(TransferDepartmentCommand request, CancellationToken cancellationToken)
    {
        //validate branch, administration and department

        var employee = await dbContext.Employees.FirstOrDefaultAsync(e=>e.Id== request.TransferDepartment.Id, cancellationToken);
        if (employee is null)
            throw new NotFoundException($"Employee not found: {request.TransferDepartment.Id}");
        await EmployeeModule.Employees.Features.Employees.EmployeeBranchScope.EnsureCanMutateAsync(sender, employee.CompanyId, employee.BranchId, cancellationToken);
        await EmployeeModule.Employees.Features.Employees.EmployeeBranchScope.EnsureCanMutateAsync(sender, employee.CompanyId, request.TransferDepartment.branchId, cancellationToken);

        var placement = await sender.Send(new ValidateOrganizationPlacementQuery(
            employee.CompanyId,
            request.TransferDepartment.branchId,
            request.TransferDepartment.administrationId,
            request.TransferDepartment.departmentId), cancellationToken);
        if (!placement.IsValid)
            throw new BadRequestException(placement.Message ?? "Invalid organization placement.");

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        employee.TransferDepartment(
            request.TransferDepartment.branchId, 
            request.TransferDepartment.administrationId, 
            request.TransferDepartment.departmentId, 
            userId);

        await EmployeeModule.Employees.Features.Employees.EmployeeBranchScope.EnsureLinkedUserBranchAccessAsync(
            sender,
            employee.CompanyId,
            employee.LinkedUserId,
            request.TransferDepartment.branchId,
            makeDefault: false,
            cancellationToken);

        await dbContext.SaveChangesAsync();
        return new TransferDepartmentRresult(true);
    }
}
