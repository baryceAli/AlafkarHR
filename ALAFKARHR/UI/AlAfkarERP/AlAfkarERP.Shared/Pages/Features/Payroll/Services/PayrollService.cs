using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Payroll.Dtos;
using SharedWithUI.Payroll.Enums;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Payroll.Services;

public class PayrollService : BaseApiService, IPayrollService
{
    private readonly string _path;

    public PayrollService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/payroll";
    }

    public async Task<ApiResult<CreatePayrollResponseDto>> CreateComponentAsync(ComponentDto component)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/components")
        {
            Content = JsonContent.Create(new { Component = component })
        };

        return await SendAsync<CreatePayrollResponseDto>(request, null);
    }

    public async Task<ApiResult<CreatePayrollResponseDto>> UpdateComponentAsync(ComponentDto component)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/components/{component.Id}")
        {
            Content = JsonContent.Create(new { Component = component })
        };

        return await SendAsync<CreatePayrollResponseDto>(request, null);
    }

    public async Task<ApiResult<DeletePayrollResponseDto>> DeleteComponentAsync(Guid componentId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/components/{componentId}");
        return await SendAsync<DeletePayrollResponseDto>(request, null);
    }

    public async Task<ApiResult<CreatePayrollResponseDto>> CreateContractAsync(ContractDto contract)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/contracts")
        {
            Content = JsonContent.Create(new
            {
                contract.Name,
                contract.NameEng,
                contract.Description,
                contract.TaxPercentage,
                contract.InsurancePercentage,
                contract.CompanyId,
                ContractItems = contract.Items
            })
        };

        return await SendAsync<CreatePayrollResponseDto>(request, null);
    }

    public async Task<ApiResult<CreatePayrollResponseDto>> CreateSalaryRunAsync(CreateSalaryRunDto salaryRun)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-runs")
        {
            Content = JsonContent.Create(salaryRun)
        };

        return await SendAsync<CreatePayrollResponseDto>(request, null);
    }

    public async Task<ApiResult<ApproveSalaryRunDto>> ApproveSalaryRunAsync(Guid salaryRunId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-runs/{salaryRunId}/approve");
        return await SendAsync<ApproveSalaryRunDto>(request, null);
    }

    public async Task<ApiResult<ApproveSalaryRunDto>> UndoSalaryRunAsync(Guid salaryRunId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-runs/{salaryRunId}/undo");
        return await SendAsync<ApproveSalaryRunDto>(request, null);
    }

    public async Task<ApiResult<List<SalaryRunDto>>> GetSalaryRunsByPeriodAsync(Guid companyId, int salaryMonth, int salaryYear)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/salary-runs/company/{companyId}/period?month={salaryMonth}&year={salaryYear}");
        return await SendAsync<List<SalaryRunDto>>(request, "salaryRunList");
    }

    public async Task<ApiResult<CommitSalaryRunsPeriodDto>> CommitSalaryRunsPeriodAsync(Guid companyId, int salaryMonth, int salaryYear)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-runs/period/commit")
        {
            Content = JsonContent.Create(new { CompanyId = companyId, SalaryMonth = salaryMonth, SalaryYear = salaryYear })
        };

        return await SendAsync<CommitSalaryRunsPeriodDto>(request, null);
    }

    public async Task<ApiResult<UndoSalaryRunsPeriodDto>> UndoSalaryRunsPeriodAsync(Guid companyId, int salaryMonth, int salaryYear)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-runs/period/undo")
        {
            Content = JsonContent.Create(new { CompanyId = companyId, SalaryMonth = salaryMonth, SalaryYear = salaryYear })
        };

        return await SendAsync<UndoSalaryRunsPeriodDto>(request, null);
    }

    public async Task<ApiResult<SalaryRunDto>> CalculateSalaryRunAsync(Guid salaryRunId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-runs/{salaryRunId}/calculate");
        return await SendAsync<SalaryRunDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<ComponentDto>>> GetComponentsByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = "")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/components/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<ComponentDto>>(request, "componentList");
    }

    public async Task<ApiResult<PaginatedResult<ContractDto>>> GetContractsByCompanyAsync(Guid companyId, int pageIndex, int pageSize, string? searchText = "")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/contracts/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}");
        return await SendAsync<PaginatedResult<ContractDto>>(request, "contractList");
    }

    public async Task<ApiResult<ContractDto>> GetContractByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/contracts/{id}");
        return await SendAsync<ContractDto>(request, null);
    }

    public async Task<ApiResult<SalaryRunDto>> GetSalaryRunByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/salary-runs/{id}");
        return await SendAsync<SalaryRunDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<EmployeeLoanDto>>> GetEmployeeLoansByCompanyAsync(Guid companyId, int pageIndex, int pageSize, Guid? employeeId = null, EmployeeLoanStatus? status = null, string? searchText = "")
    {
        var query = $"pageIndex={pageIndex}&pageSize={pageSize}&searchText={searchText}";
        if (employeeId.HasValue && employeeId.Value != Guid.Empty)
        {
            query += $"&employeeId={employeeId.Value}";
        }

        if (status.HasValue)
        {
            query += $"&status={status.Value}";
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/loans/company/{companyId}?{query}");
        return await SendAsync<PaginatedResult<EmployeeLoanDto>>(request, "employeeLoanList");
    }

    public async Task<ApiResult<EmployeeLoanActionDto>> CreateEmployeeLoanAsync(CreateEmployeeLoanDto employeeLoan)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/loans")
        {
            Content = JsonContent.Create(new { EmployeeLoan = employeeLoan })
        };

        return await SendAsync<EmployeeLoanActionDto>(request, null);
    }

    public async Task<ApiResult<EmployeeLoanActionDto>> UpdateEmployeeLoanAsync(UpdateEmployeeLoanDto employeeLoan)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/loans/{employeeLoan.Id}")
        {
            Content = JsonContent.Create(new { EmployeeLoan = employeeLoan })
        };

        return await SendAsync<EmployeeLoanActionDto>(request, null);
    }

    public async Task<ApiResult<EmployeeLoanActionDto>> ApproveEmployeeLoanAsync(Guid employeeLoanId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/loans/{employeeLoanId}/approve");
        return await SendAsync<EmployeeLoanActionDto>(request, null);
    }

    public async Task<ApiResult<EmployeeLoanActionDto>> CancelEmployeeLoanAsync(Guid employeeLoanId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/loans/{employeeLoanId}/cancel");
        return await SendAsync<EmployeeLoanActionDto>(request, null);
    }

    public async Task<ApiResult<EmployeeContractDto>> AssignEmployeeContractAsync(EmployeeContractDto employeeContract)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/employee-contracts")
        {
            Content = JsonContent.Create(new { EmployeeContract = employeeContract })
        };

        return await SendAsync<EmployeeContractDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<EmployeeContractDto>>> GetEmployeeContractsByCompanyAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/employee-contracts/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeContractDto>>(request, "employeeContractList");
    }

    public async Task<ApiResult<CreatePayrollResponseDto>> UpdateContractAsync(ContractDto contract)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/contracts/{contract.Id}")
        {
            Content = JsonContent.Create(new
            {
                contract.Id,
                contract.Name,
                contract.NameEng,
                contract.Description,
                contract.TaxPercentage,
                contract.InsurancePercentage,
                ContractItems = contract.Items
            })
        };

        return await SendAsync<CreatePayrollResponseDto>(request, null);
    }

    public async Task<ApiResult<DeletePayrollResponseDto>> DeleteContractAsync(Guid contractId)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/contracts/{contractId}");
        return await SendAsync<DeletePayrollResponseDto>(request, null);
    }

    public async Task<ApiResult<List<SalaryStructureDto>>> GetSalaryStructuresAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/salary-structures/company/{companyId}");
        return await SendAsync<List<SalaryStructureDto>>(request, "structureList");
    }

    public async Task<ApiResult<PayrollActionResultDto>> CreateSalaryStructureAsync(SalaryStructureUpsertDto structure)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-structures") { Content = JsonContent.Create(new { Structure = structure }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> UpdateSalaryStructureAsync(SalaryStructureUpsertDto structure)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/salary-structures/{structure.Id}") { Content = JsonContent.Create(new { Structure = structure }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> ActivateSalaryStructureAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-structures/{id}/activate");
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> DeactivateSalaryStructureAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-structures/{id}/deactivate");
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<SalaryStructureAssignmentDto>>> GetSalaryStructureAssignmentsAsync(Guid companyId, Guid? employeeId = null)
    {
        var query = employeeId.HasValue && employeeId.Value != Guid.Empty ? $"?employeeId={employeeId.Value}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/salary-structure-assignments/company/{companyId}{query}");
        return await SendAsync<List<SalaryStructureAssignmentDto>>(request, "assignmentList");
    }

    public async Task<ApiResult<PayrollActionResultDto>> CreateSalaryStructureAssignmentAsync(SalaryStructureAssignmentUpsertDto assignment)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-structure-assignments") { Content = JsonContent.Create(new { Assignment = assignment }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> UpdateSalaryStructureAssignmentAsync(SalaryStructureAssignmentUpsertDto assignment)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/salary-structure-assignments/{assignment.Id}") { Content = JsonContent.Create(new { Assignment = assignment }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> EndSalaryStructureAssignmentAsync(Guid id, DateTime effectiveTo)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/salary-structure-assignments/{id}/end") { Content = JsonContent.Create(new { EffectiveTo = effectiveTo }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<PayrollPeriodDto>>> GetPayrollPeriodsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/payroll-periods/company/{companyId}");
        return await SendAsync<List<PayrollPeriodDto>>(request, "periodList");
    }

    public async Task<ApiResult<PayrollActionResultDto>> CreatePayrollPeriodAsync(PayrollPeriodUpsertDto period)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/payroll-periods") { Content = JsonContent.Create(new { Period = period }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> UpdatePayrollPeriodAsync(PayrollPeriodUpsertDto period)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/payroll-periods/{period.Id}") { Content = JsonContent.Create(new { Period = period }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> OpenPayrollPeriodAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/payroll-periods/{id}/open");
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> ClosePayrollPeriodAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/payroll-periods/{id}/close");
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> ReopenPayrollPeriodAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/payroll-periods/{id}/reopen");
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<PayrollEntryDto>>> GetPayrollEntriesAsync(Guid companyId, Guid? payrollPeriodId = null)
    {
        var query = payrollPeriodId.HasValue && payrollPeriodId.Value != Guid.Empty ? $"?payrollPeriodId={payrollPeriodId.Value}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/payroll-entries/company/{companyId}{query}");
        return await SendAsync<List<PayrollEntryDto>>(request, "entryList");
    }

    public async Task<ApiResult<PayrollActionResultDto>> CreatePayrollEntryAsync(PayrollEntryCreateDto entry)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/payroll-entries") { Content = JsonContent.Create(new { Entry = entry }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> GeneratePayrollEntryAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payroll-entries/{id}/generate");
    public async Task<ApiResult<PayrollActionResultDto>> ApprovePayrollEntryAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payroll-entries/{id}/approve");
    public async Task<ApiResult<PayrollActionResultDto>> ClosePayrollEntryAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payroll-entries/{id}/close");
    public async Task<ApiResult<PayrollActionResultDto>> ReopenPayrollEntryAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payroll-entries/{id}/reopen");
    public async Task<ApiResult<PayrollActionResultDto>> CancelPayrollEntryAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payroll-entries/{id}/cancel");

    public async Task<ApiResult<List<PayslipDto>>> GetPayslipsAsync(Guid companyId, Guid? payrollEntryId = null, Guid? employeeId = null)
    {
        var query = new List<string>();
        if (payrollEntryId.HasValue && payrollEntryId.Value != Guid.Empty) query.Add($"payrollEntryId={payrollEntryId.Value}");
        if (employeeId.HasValue && employeeId.Value != Guid.Empty) query.Add($"employeeId={employeeId.Value}");
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/payslips/company/{companyId}{(query.Count == 0 ? string.Empty : "?" + string.Join("&", query))}");
        return await SendAsync<List<PayslipDto>>(request, "payslipList");
    }

    public async Task<ApiResult<PayslipDto>> GetPayslipAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/payslips/{id}");
        return await SendAsync<PayslipDto>(request, "payslip");
    }

    public async Task<ApiResult<PayrollActionResultDto>> RecalculatePayslipAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payslips/{id}/recalculate");
    public async Task<ApiResult<PayrollActionResultDto>> ApprovePayslipAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payslips/{id}/approve");
    public async Task<ApiResult<PayrollActionResultDto>> MarkPayslipPaidAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payslips/{id}/mark-paid");
    public async Task<ApiResult<PayrollActionResultDto>> CancelPayslipAsync(Guid id) => await PostPayrollActionAsync($"{_path}/payslips/{id}/cancel");

    public async Task<ApiResult<List<PayrollInputDto>>> GetPayrollInputsAsync(Guid companyId, Guid? payrollPeriodId = null, Guid? employeeId = null)
    {
        var query = new List<string>();
        if (payrollPeriodId.HasValue && payrollPeriodId.Value != Guid.Empty) query.Add($"payrollPeriodId={payrollPeriodId.Value}");
        if (employeeId.HasValue && employeeId.Value != Guid.Empty) query.Add($"employeeId={employeeId.Value}");
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/payroll-inputs/company/{companyId}{(query.Count == 0 ? string.Empty : "?" + string.Join("&", query))}");
        return await SendAsync<List<PayrollInputDto>>(request, "inputList");
    }

    public async Task<ApiResult<PayrollActionResultDto>> CreatePayrollInputAsync(PayrollInputUpsertDto input)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/payroll-inputs") { Content = JsonContent.Create(new { Input = input }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> UpdatePayrollInputAsync(PayrollInputUpsertDto input)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/payroll-inputs/{input.Id}") { Content = JsonContent.Create(new { Input = input }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> DeletePayrollInputAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/payroll-inputs/{id}");
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    private async Task<ApiResult<PayrollActionResultDto>> PostPayrollActionAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<SaudiPayrollInfoDto>>> GetSaudiPayrollInfosAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/saudi-payroll/company/{companyId}");
        return await SendAsync<List<SaudiPayrollInfoDto>>(request, "saudiPayrollInfoList");
    }

    public async Task<ApiResult<SaudiPayrollInfoDto>> GetSaudiPayrollInfoAsync(Guid companyId, Guid employeeId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/saudi-payroll/company/{companyId}/employee/{employeeId}");
        return await SendAsync<SaudiPayrollInfoDto>(request, "saudiPayrollInfo");
    }

    public async Task<ApiResult<PayrollActionResultDto>> UpsertSaudiPayrollInfoAsync(SaudiPayrollInfoUpsertDto info)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/saudi-payroll") { Content = JsonContent.Create(new { SaudiPayrollInfo = info }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<WpsBatchDto>>> GetWpsBatchesAsync(Guid companyId, Guid? payrollPeriodId = null)
    {
        var query = payrollPeriodId.HasValue && payrollPeriodId.Value != Guid.Empty ? $"?payrollPeriodId={payrollPeriodId.Value}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/wps-batches/company/{companyId}{query}");
        return await SendAsync<List<WpsBatchDto>>(request, "wpsBatchList");
    }

    public async Task<ApiResult<PayrollActionResultDto>> CreateWpsBatchAsync(CreateWpsBatchDto batch)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/wps-batches") { Content = JsonContent.Create(new { Batch = batch }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> MarkWpsBatchExportedAsync(Guid id) => await PostPayrollActionAsync($"{_path}/wps-batches/{id}/mark-exported");

    public async Task<ApiResult<List<EosProvisionSnapshotDto>>> GetEosProvisionSnapshotsAsync(Guid companyId, Guid? payrollPeriodId = null, Guid? employeeId = null)
    {
        var query = new List<string>();
        if (payrollPeriodId.HasValue && payrollPeriodId.Value != Guid.Empty) query.Add($"payrollPeriodId={payrollPeriodId.Value}");
        if (employeeId.HasValue && employeeId.Value != Guid.Empty) query.Add($"employeeId={employeeId.Value}");
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/eos-snapshots/company/{companyId}{(query.Count == 0 ? string.Empty : "?" + string.Join("&", query))}");
        return await SendAsync<List<EosProvisionSnapshotDto>>(request, "eosSnapshotList");
    }

    public async Task<ApiResult<PayrollActionResultDto>> CreateEosProvisionSnapshotAsync(CreateEosProvisionSnapshotDto snapshot)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/eos-snapshots") { Content = JsonContent.Create(new { Snapshot = snapshot }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }

    public async Task<ApiResult<PayrollActionResultDto>> PostPayrollEntryAccountingAsync(Guid payrollEntryId) => await PostPayrollActionAsync($"{_path}/payroll-entries/{payrollEntryId}/post-accounting");

    public async Task<ApiResult<List<PayrollWorkEntryImportDto>>> GetImportedPayrollWorkEntriesAsync(Guid companyId, Guid? payrollPeriodId = null, Guid? employeeId = null)
    {
        var query = new List<string>();
        if (payrollPeriodId.HasValue && payrollPeriodId.Value != Guid.Empty) query.Add($"payrollPeriodId={payrollPeriodId.Value}");
        if (employeeId.HasValue && employeeId.Value != Guid.Empty) query.Add($"employeeId={employeeId.Value}");
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/payroll-work-entries/company/{companyId}{(query.Count == 0 ? string.Empty : "?" + string.Join("&", query))}");
        return await SendAsync<List<PayrollWorkEntryImportDto>>(request, "workEntryList");
    }

    public async Task<ApiResult<PayrollActionResultDto>> ImportPayrollWorkEntryAsync(PayrollWorkEntryImportDto workEntry)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/payroll-work-entries/import") { Content = JsonContent.Create(new { WorkEntry = workEntry }) };
        return await SendAsync<PayrollActionResultDto>(request, null);
    }
}
