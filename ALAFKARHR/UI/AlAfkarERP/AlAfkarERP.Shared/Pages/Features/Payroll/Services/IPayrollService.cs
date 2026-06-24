using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Payroll.Dtos;
using SharedWithUI.Payroll.Enums;

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
    Task<ApiResult<PaginatedResult<EmployeeLoanDto>>> GetEmployeeLoansByCompanyAsync(Guid companyId, int pageIndex, int pageSize, Guid? employeeId = null, EmployeeLoanStatus? status = null, string? searchText = "");
    Task<ApiResult<EmployeeLoanActionDto>> CreateEmployeeLoanAsync(CreateEmployeeLoanDto employeeLoan);
    Task<ApiResult<EmployeeLoanActionDto>> UpdateEmployeeLoanAsync(UpdateEmployeeLoanDto employeeLoan);
    Task<ApiResult<EmployeeLoanActionDto>> ApproveEmployeeLoanAsync(Guid employeeLoanId);
    Task<ApiResult<EmployeeLoanActionDto>> CancelEmployeeLoanAsync(Guid employeeLoanId);
    Task<ApiResult<List<SalaryStructureDto>>> GetSalaryStructuresAsync(Guid companyId);
    Task<ApiResult<PayrollActionResultDto>> CreateSalaryStructureAsync(SalaryStructureUpsertDto structure);
    Task<ApiResult<PayrollActionResultDto>> UpdateSalaryStructureAsync(SalaryStructureUpsertDto structure);
    Task<ApiResult<PayrollActionResultDto>> ActivateSalaryStructureAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> DeactivateSalaryStructureAsync(Guid id);
    Task<ApiResult<List<SalaryStructureAssignmentDto>>> GetSalaryStructureAssignmentsAsync(Guid companyId, Guid? employeeId = null);
    Task<ApiResult<PayrollActionResultDto>> CreateSalaryStructureAssignmentAsync(SalaryStructureAssignmentUpsertDto assignment);
    Task<ApiResult<PayrollActionResultDto>> UpdateSalaryStructureAssignmentAsync(SalaryStructureAssignmentUpsertDto assignment);
    Task<ApiResult<PayrollActionResultDto>> EndSalaryStructureAssignmentAsync(Guid id, DateTime effectiveTo);
    Task<ApiResult<List<PayrollPeriodDto>>> GetPayrollPeriodsAsync(Guid companyId);
    Task<ApiResult<PayrollActionResultDto>> CreatePayrollPeriodAsync(PayrollPeriodUpsertDto period);
    Task<ApiResult<PayrollActionResultDto>> UpdatePayrollPeriodAsync(PayrollPeriodUpsertDto period);
    Task<ApiResult<PayrollActionResultDto>> OpenPayrollPeriodAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> ClosePayrollPeriodAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> ReopenPayrollPeriodAsync(Guid id);
    Task<ApiResult<List<PayrollEntryDto>>> GetPayrollEntriesAsync(Guid companyId, Guid? payrollPeriodId = null);
    Task<ApiResult<PayrollActionResultDto>> CreatePayrollEntryAsync(PayrollEntryCreateDto entry);
    Task<ApiResult<PayrollActionResultDto>> GeneratePayrollEntryAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> ApprovePayrollEntryAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> ClosePayrollEntryAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> ReopenPayrollEntryAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> CancelPayrollEntryAsync(Guid id);
    Task<ApiResult<List<PayslipDto>>> GetPayslipsAsync(Guid companyId, Guid? payrollEntryId = null, Guid? employeeId = null);
    Task<ApiResult<PayslipDto>> GetPayslipAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> RecalculatePayslipAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> ApprovePayslipAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> MarkPayslipPaidAsync(Guid id);
    Task<ApiResult<PayrollActionResultDto>> CancelPayslipAsync(Guid id);
    Task<ApiResult<List<PayrollInputDto>>> GetPayrollInputsAsync(Guid companyId, Guid? payrollPeriodId = null, Guid? employeeId = null);
    Task<ApiResult<PayrollActionResultDto>> CreatePayrollInputAsync(PayrollInputUpsertDto input);
    Task<ApiResult<PayrollActionResultDto>> UpdatePayrollInputAsync(PayrollInputUpsertDto input);
    Task<ApiResult<PayrollActionResultDto>> DeletePayrollInputAsync(Guid id);
    Task<ApiResult<List<SaudiPayrollInfoDto>>> GetSaudiPayrollInfosAsync(Guid companyId);
    Task<ApiResult<SaudiPayrollInfoDto>> GetSaudiPayrollInfoAsync(Guid companyId, Guid employeeId);
    Task<ApiResult<PayrollActionResultDto>> UpsertSaudiPayrollInfoAsync(SaudiPayrollInfoUpsertDto info);
    Task<ApiResult<List<WpsBatchDto>>> GetWpsBatchesAsync(Guid companyId, Guid? payrollPeriodId = null);
    Task<ApiResult<PayrollActionResultDto>> CreateWpsBatchAsync(CreateWpsBatchDto batch);
    Task<ApiResult<PayrollActionResultDto>> MarkWpsBatchExportedAsync(Guid id);
    Task<ApiResult<List<EosProvisionSnapshotDto>>> GetEosProvisionSnapshotsAsync(Guid companyId, Guid? payrollPeriodId = null, Guid? employeeId = null);
    Task<ApiResult<PayrollActionResultDto>> CreateEosProvisionSnapshotAsync(CreateEosProvisionSnapshotDto snapshot);
    Task<ApiResult<PayrollActionResultDto>> PostPayrollEntryAccountingAsync(Guid payrollEntryId);
    Task<ApiResult<List<PayrollWorkEntryImportDto>>> GetImportedPayrollWorkEntriesAsync(Guid companyId, Guid? payrollPeriodId = null, Guid? employeeId = null);
    Task<ApiResult<PayrollActionResultDto>> ImportPayrollWorkEntryAsync(PayrollWorkEntryImportDto workEntry);
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

public class EmployeeLoanActionDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
