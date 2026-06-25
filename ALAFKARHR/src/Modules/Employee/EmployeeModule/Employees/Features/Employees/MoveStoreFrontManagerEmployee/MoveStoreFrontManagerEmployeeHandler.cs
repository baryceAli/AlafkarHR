using EmployeeModule.Contracts.Employees.Features.MoveStoreFrontManagerEmployee;

namespace EmployeeModule.Employees.Features.Employees.MoveStoreFrontManagerEmployee;

public class MoveStoreFrontManagerEmployeeHandler(EmployeeDbContext dbContext, ISender sender)
    : ICommandHandler<MoveStoreFrontManagerEmployeeCommand, MoveStoreFrontManagerEmployeeResult>
{
    public async Task<MoveStoreFrontManagerEmployeeResult> Handle(MoveStoreFrontManagerEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (request.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (request.EmployeeId == Guid.Empty)
            throw new BadRequestException("Employee is required.");
        if (request.BranchId == Guid.Empty)
            throw new BadRequestException("Branch is required.");

        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(x => x.Id == request.EmployeeId && x.CompanyId == request.CompanyId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee not found: {request.EmployeeId}");

        if (request.RequireActive && !employee.IsActive)
            throw new BadRequestException("Store manager employee must be active.");

        if (request.RequireLinkedUser && (!employee.LinkedUserId.HasValue || employee.LinkedUserId.Value == Guid.Empty))
            throw new BadRequestException("Store manager employee must be linked to an application user.");

        var placement = await sender.Send(new ValidateOrganizationPlacementQuery(
            request.CompanyId,
            request.BranchId,
            null,
            null), cancellationToken);
        if (!placement.IsValid)
            throw new BadRequestException(placement.Message ?? "Invalid store manager branch placement.");

        employee.TransferDepartment(request.BranchId, null, null, request.UserId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new MoveStoreFrontManagerEmployeeResult(
            employee.Id,
            employee.CompanyId,
            employee.BranchId,
            employee.LinkedUserId,
            employee.FullName,
            employee.FullNameEng,
            employee.IsActive);
    }
}
