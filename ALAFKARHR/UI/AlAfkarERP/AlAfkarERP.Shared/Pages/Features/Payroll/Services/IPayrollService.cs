using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Payroll.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Payroll.Services;

public interface IPayrollService
{
    Task<ApiResult<PaginatedResult<ComponentDto>>> GetComponentsByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = "");
    Task<ApiResult<CreatePayrollResponseDto>> CreateComponentAsync(ComponentDto component);
    Task<ApiResult<CreatePayrollResponseDto>> UpdateComponentAsync(ComponentDto component);
    Task<ApiResult<DeletePayrollResponseDto>> DeleteComponentAsync(Guid componentId);
    Task<ApiResult<PaginatedResult<ContractDto>>> GetContractsByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = "");
    Task<ApiResult<CreatePayrollResponseDto>> CreateContractAsync(ContractDto contract);
    Task<ApiResult<CreatePayrollResponseDto>> UpdateContractAsync(ContractDto contract);
    Task<ApiResult<DeletePayrollResponseDto>> DeleteContractAsync(Guid contractId);
    Task<ApiResult<ContractDto>> GetContractByIdAsync(Guid id);
    Task<ApiResult<EmployeeContractDto>> AssignEmployeeContractAsync(EmployeeContractDto employeeContract);
    Task<ApiResult<PaginatedResult<EmployeeContractDto>>> GetEmployeeContractsByCompanyAsync(Guid companyId, int pageIndex, int pageSize);
    Task<ApiResult<CreatePayrollResponseDto>> CreateSalaryRunAsync(CreateSalaryRunDto salaryRun);
    Task<ApiResult<SalaryRunDto>> CalculateSalaryRunAsync(Guid salaryRunId);
    Task<ApiResult<ApproveSalaryRunDto>> ApproveSalaryRunAsync(Guid salaryRunId);
    Task<ApiResult<ApproveSalaryRunDto>> UndoSalaryRunAsync(Guid salaryRunId);
    Task<ApiResult<List<SalaryRunDto>>> GetSalaryRunsByPeriodAsync(Guid companyId, int salaryMonth, int salaryYear);
    Task<ApiResult<CommitSalaryRunsPeriodDto>> CommitSalaryRunsPeriodAsync(Guid companyId, int salaryMonth, int salaryYear);
    Task<ApiResult<UndoSalaryRunsPeriodDto>> UndoSalaryRunsPeriodAsync(Guid companyId, int salaryMonth, int salaryYear);
    Task<ApiResult<SalaryRunDto>> GetSalaryRunByIdAsync(Guid id);
}

public class CreatePayrollResponseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Message { get; set; }
}

public class CreateSalaryRunDto
{
    public Guid EmployeeId { get; set; }
    public Guid ContractId { get; set; }
    public int SalaryMonth { get; set; }
    public int SalaryYear { get; set; }
    public decimal BaseSalary { get; set; }
}

public class ApproveSalaryRunDto
{
    public Guid SalaryRunId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class CommitSalaryRunsPeriodDto
{
    public int CommittedCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class UndoSalaryRunsPeriodDto
{
    public int DeletedCount { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DeletePayrollResponseDto
{
    public bool IsSuccess { get; set; }
}
