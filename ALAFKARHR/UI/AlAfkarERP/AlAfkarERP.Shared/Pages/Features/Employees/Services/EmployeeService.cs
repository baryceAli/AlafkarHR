using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Employees.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Services;

public class EmployeeService :BaseApiService, IEmployeeService
{
    private readonly ApiConfig _apiConfig;
    private readonly string _path;
    public EmployeeService(HttpClient http, ApiConfig apiConfig) : base(http)
    {
        _apiConfig = apiConfig;
        _path = $"api/{_apiConfig.Version}/Employee/Employees";
    }

    public async Task<ApiResult<EmployeeDto>> CreateAsync(EmployeeDto employee)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Employee = employee
            })
        };
        return await SendAsync<EmployeeDto>(request, "createdEmployee");
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> ChangePositionAsync(ChangePositionDto changePosition)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/ChangePosition")
        {
            Content = JsonContent.Create(new
            {
                ChangePosition=changePosition
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> DeleteAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/{id}");
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetAsync(int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByAdministrationAsync(Guid administrationId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/administration/{administrationId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByBranchAsync(Guid branchId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/branch/{branchId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByCompanyIdAsync(Guid companyId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/company/{companyId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByDepartmentAsync(Guid departmentId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/department/{departmentId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    public async Task<ApiResult<EmployeeDto>> GetByIdAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/{id}");
        return await SendAsync<EmployeeDto>(request, "employee");
    }

    public async Task<ApiResult<PaginatedResult<EmployeeDto>>> GetByPositionAsync(Guid positionId, int pageIndex, int pageSize)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/position/{positionId}?pageIndex={pageIndex}&pageSize={pageSize}");
        return await SendAsync<PaginatedResult<EmployeeDto>>(request, "employeeList");
    }

    
    public async Task<ApiResult<UpdateDeleteResponseDto>> TerminateEmployeeAsync(TerminateEmployeeDto terminateEmployee)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/Terminate")
        {
            Content = JsonContent.Create(new
            {
                ChangePosition = terminateEmployee
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> TransferDepartmentAsync(TransferDepartmentDto transferDepartment)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/Transfer")
        {
            Content = JsonContent.Create(new
            {
                ChangePosition = transferDepartment
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }

    public async Task<ApiResult<UpdateDeleteResponseDto>> UpdateAsync(EmployeeDto employee)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}")
        {
            Content = JsonContent.Create(new
            {
                Employee = employee
            })
        };
        return await SendAsync<UpdateDeleteResponseDto>(request, null);
    }
}
