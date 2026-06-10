namespace Payroll.Salaries.Features.EmployeeContracts.AssignEmployeeContract;

public class AssignEmployeeContractHandler(PayrollDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<AssignEmployeeContractCommand, EmployeeContractDto>
{
    public async Task<EmployeeContractDto> Handle(AssignEmployeeContractCommand request, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value ??
                    throw new UnauthorizedAccessException("User is not authenticated");

        var dto = request.EmployeeContract;

        var contractExists = await dbContext.Contracts
            .AnyAsync(x => x.Id == dto.ContractId && x.CompanyId == dto.CompanyId && !x.IsDeleted, cancellationToken);

        if (!contractExists)
            throw new KeyNotFoundException($"Contract with ID {dto.ContractId} not found");

        var activeAssignments = await dbContext.EmployeeContracts
            .Where(x => x.CompanyId == dto.CompanyId && x.EmployeeId == dto.EmployeeId && x.IsActive && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var assignment in activeAssignments)
        {
            assignment.Deactivate(userId);
        }

        var employeeContract = EmployeeContract.Assign(
            Guid.NewGuid(),
            dto.EmployeeId,
            dto.ContractId,
            dto.CompanyId,
            dto.EffectiveFrom == default ? DateTime.Today : dto.EffectiveFrom,
            userId);

        await dbContext.EmployeeContracts.AddAsync(employeeContract, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new EmployeeContractDto
        {
            Id = employeeContract.Id,
            EmployeeId = employeeContract.EmployeeId,
            ContractId = employeeContract.ContractId,
            CompanyId = employeeContract.CompanyId,
            EffectiveFrom = employeeContract.EffectiveFrom,
            IsActive = employeeContract.IsActive
        };
    }
}
