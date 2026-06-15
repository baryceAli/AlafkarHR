using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Payroll.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Payroll.Services;

public class PayrollService : BaseApiService, IPayrollService
{
    private readonly string _path;

    public PayrollService(HttpClient http, ApiConfig apiConfig) : base(http)
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
}
