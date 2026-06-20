using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using Microsoft.AspNetCore.Components.Forms;
using SharedWithUI.Attendance.Enums;
using SharedWithUI.Leave.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.LeavesManagement.Services;

public interface ILeaveService
{
    Task<ApiResult<PaginatedResult<EmergencyLeaveRequestDto>>> GetEmergencyLeavesAsync(Guid companyId, int pageIndex, int pageSize, AttendanceExceptionStatus? status = null, Guid? employeeId = null);
    Task<ApiResult<string>> UploadEmergencyLeaveAttachmentAsync(IBrowserFile file);
    Task<ApiResult<EmergencyLeaveRequestDto>> CreateEmergencyLeaveAsync(CreateEmergencyLeaveRequestDto request);
    Task<ApiResult<EmergencyLeaveRequestDto>> ReviewEmergencyLeaveAsync(ReviewEmergencyLeaveRequestDto review);
    Task<ApiResult<List<EmployeeLeaveBalanceDto>>> GetLeaveBalancesAsync(Guid companyId, int year, Guid? employeeId = null);
    Task<ApiResult<EmployeeLeaveBalanceDto>> UpsertLeaveBalanceAsync(UpsertEmployeeLeaveBalanceDto balance);
    Task<ApiResult<LeaveReportDto>> GetLeaveReportAsync(LeaveReportFilterDto filter);
}

public class LeaveService : BaseApiService, ILeaveService
{
    private const long MaxAttachmentSize = 10 * 1024 * 1024;
    private readonly string path;

    public LeaveService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/leave";
    }

    public async Task<ApiResult<PaginatedResult<EmergencyLeaveRequestDto>>> GetEmergencyLeavesAsync(
        Guid companyId,
        int pageIndex,
        int pageSize,
        AttendanceExceptionStatus? status = null,
        Guid? employeeId = null)
    {
        var url = $"{path}/emergency-leaves?companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}";
        if (status.HasValue) url += $"&status={status.Value}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";

        return await SendAsync<PaginatedResult<EmergencyLeaveRequestDto>>(new HttpRequestMessage(HttpMethod.Get, url), "requestList");
    }

    public async Task<ApiResult<EmergencyLeaveRequestDto>> CreateEmergencyLeaveAsync(CreateEmergencyLeaveRequestDto request)
    {
        return await SendAsync<EmergencyLeaveRequestDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/emergency-leaves")
        {
            Content = JsonContent.Create(new { Request = request })
        }, "request");
    }

    public async Task<ApiResult<string>> UploadEmergencyLeaveAttachmentAsync(IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(file.OpenReadStream(MaxAttachmentSize));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        return await SendAsync<string>(new HttpRequestMessage(HttpMethod.Post, $"{path}/emergency-leaves/attachments")
        {
            Content = content
        }, "attachmentPath");
    }

    public async Task<ApiResult<EmergencyLeaveRequestDto>> ReviewEmergencyLeaveAsync(ReviewEmergencyLeaveRequestDto review)
    {
        return await SendAsync<EmergencyLeaveRequestDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/emergency-leaves/review")
        {
            Content = JsonContent.Create(new { Review = review })
        }, "request");
    }

    public async Task<ApiResult<List<EmployeeLeaveBalanceDto>>> GetLeaveBalancesAsync(Guid companyId, int year, Guid? employeeId = null)
    {
        var url = $"{path}/leave-balances?companyId={companyId}&year={year}";
        if (employeeId.HasValue)
        {
            url += $"&employeeId={employeeId.Value}";
        }

        return await SendAsync<List<EmployeeLeaveBalanceDto>>(new HttpRequestMessage(HttpMethod.Get, url), "balanceList");
    }

    public async Task<ApiResult<EmployeeLeaveBalanceDto>> UpsertLeaveBalanceAsync(UpsertEmployeeLeaveBalanceDto balance)
    {
        return await SendAsync<EmployeeLeaveBalanceDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-balances")
        {
            Content = JsonContent.Create(new { Balance = balance })
        }, "balance");
    }

    public async Task<ApiResult<LeaveReportDto>> GetLeaveReportAsync(LeaveReportFilterDto filter)
    {
        return await SendAsync<LeaveReportDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-reports")
        {
            Content = JsonContent.Create(new { Filter = filter })
        }, "report");
    }
}
