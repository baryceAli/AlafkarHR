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
    Task<ApiResult<PaginatedResult<EmergencyLeaveRequestDto>>> GetMyEmergencyLeavesAsync(Guid companyId, int pageIndex, int pageSize, AttendanceExceptionStatus? status = null);
    Task<ApiResult<PaginatedResult<EmergencyLeaveRequestDto>>> GetEmployeeEmergencyLeavesAsync(Guid companyId, Guid employeeId, int pageIndex, int pageSize, AttendanceExceptionStatus? status = null);
    Task<ApiResult<string>> UploadEmergencyLeaveAttachmentAsync(IBrowserFile file);
    Task<ApiResult<string>> UploadLeaveApplicationAttachmentAsync(IBrowserFile file);
    Task<ApiResult<string>> UploadMyLeaveApplicationAttachmentAsync(IBrowserFile file);
    Task<ApiResult<EmergencyLeaveRequestDto>> CreateEmergencyLeaveAsync(CreateEmergencyLeaveRequestDto request);
    Task<ApiResult<EmergencyLeaveRequestDto>> ReviewEmergencyLeaveAsync(ReviewEmergencyLeaveRequestDto review);
    Task<ApiResult<List<EmployeeLeaveBalanceDto>>> GetLeaveBalancesAsync(Guid companyId, int year, Guid? employeeId = null);
    Task<ApiResult<EmployeeLeaveBalanceDto>> UpsertLeaveBalanceAsync(UpsertEmployeeLeaveBalanceDto balance);
    Task<ApiResult<LeaveReportDto>> GetLeaveReportAsync(LeaveReportFilterDto filter);
    Task<ApiResult<List<LeaveTypeDto>>> GetLeaveTypesAsync(Guid companyId);
    Task<ApiResult<List<LeaveTypeDto>>> GetMyLeaveTypesAsync();
    Task<ApiResult<LeaveTypeDto>> UpsertLeaveTypeAsync(UpsertLeaveTypeDto leaveType);
    Task<ApiResult<bool>> DeleteLeaveTypeAsync(Guid id);
    Task<ApiResult<List<LeavePeriodDto>>> GetLeavePeriodsAsync(Guid companyId);
    Task<ApiResult<LeavePeriodDto>> UpsertLeavePeriodAsync(UpsertLeavePeriodDto leavePeriod);
    Task<ApiResult<bool>> DeleteLeavePeriodAsync(Guid id);
    Task<ApiResult<List<LeavePolicyDto>>> GetLeavePoliciesAsync(Guid companyId);
    Task<ApiResult<LeavePolicyDto>> UpsertLeavePolicyAsync(UpsertLeavePolicyDto leavePolicy);
    Task<ApiResult<bool>> DeleteLeavePolicyAsync(Guid id);
    Task<ApiResult<List<LeavePolicyAssignmentDto>>> GetLeavePolicyAssignmentsAsync(Guid companyId);
    Task<ApiResult<LeavePolicyAssignmentDto>> UpsertLeavePolicyAssignmentAsync(UpsertLeavePolicyAssignmentDto assignment);
    Task<ApiResult<bool>> DeleteLeavePolicyAssignmentAsync(Guid id);
    Task<ApiResult<int>> GenerateLeaveAllocationsAsync(GenerateLeaveAllocationsDto request);
    Task<ApiResult<List<LeaveApplicationDto>>> GetLeaveApplicationsAsync(Guid companyId, Guid? employeeId = null, LeaveApplicationStatus? status = null);
    Task<ApiResult<List<LeaveApplicationDto>>> GetMyLeaveApplicationsAsync(LeaveApplicationStatus? status = null);
    Task<ApiResult<LeaveApplicationDto>> UpsertLeaveApplicationAsync(UpsertLeaveApplicationDto application);
    Task<ApiResult<LeaveApplicationDto>> UpsertMyLeaveApplicationAsync(UpsertLeaveApplicationDto application);
    Task<ApiResult<LeaveApplicationDto>> SubmitLeaveApplicationAsync(Guid id);
    Task<ApiResult<LeaveApplicationDto>> SubmitMyLeaveApplicationAsync(Guid id);
    Task<ApiResult<LeaveApplicationDto>> ReviewLeaveApplicationAsync(ReviewLeaveApplicationDto review);
    Task<ApiResult<LeaveApplicationDto>> CancelLeaveApplicationAsync(Guid id);
    Task<ApiResult<LeaveApplicationDto>> CancelMyLeaveApplicationAsync(Guid id);
    Task<ApiResult<List<LeaveLedgerEntryDto>>> GetLeaveLedgerEntriesAsync(Guid companyId, Guid? employeeId = null, Guid? leaveTypeId = null, Guid? leavePeriodId = null);
    Task<ApiResult<LeaveLedgerEntryDto>> CreateLeaveLedgerAdjustmentAsync(CreateLeaveLedgerAdjustmentDto adjustment);
    Task<ApiResult<(LeaveEncashmentDto? Encashment, LeaveLedgerEntryDto? Entry)>> CreateLeaveEncashmentAsync(CreateLeaveEncashmentDto encashment);
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

    public async Task<ApiResult<PaginatedResult<EmergencyLeaveRequestDto>>> GetMyEmergencyLeavesAsync(
        Guid companyId,
        int pageIndex,
        int pageSize,
        AttendanceExceptionStatus? status = null)
    {
        var url = $"{path}/my-emergency-leaves?companyId={companyId}&pageIndex={pageIndex}&pageSize={pageSize}";
        if (status.HasValue) url += $"&status={status.Value}";

        return await SendAsync<PaginatedResult<EmergencyLeaveRequestDto>>(new HttpRequestMessage(HttpMethod.Get, url), "requestList");
    }

    public async Task<ApiResult<PaginatedResult<EmergencyLeaveRequestDto>>> GetEmployeeEmergencyLeavesAsync(
        Guid companyId,
        Guid employeeId,
        int pageIndex,
        int pageSize,
        AttendanceExceptionStatus? status = null)
    {
        var url = $"{path}/employee-emergency-leaves?companyId={companyId}&employeeId={employeeId}&pageIndex={pageIndex}&pageSize={pageSize}";
        if (status.HasValue) url += $"&status={status.Value}";

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

    public async Task<ApiResult<string>> UploadLeaveApplicationAttachmentAsync(IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(file.OpenReadStream(MaxAttachmentSize));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        return await SendAsync<string>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-applications/attachments")
        {
            Content = content
        }, "attachmentPath");
    }

    public async Task<ApiResult<string>> UploadMyLeaveApplicationAttachmentAsync(IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        using var fileContent = new StreamContent(file.OpenReadStream(MaxAttachmentSize));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        return await SendAsync<string>(new HttpRequestMessage(HttpMethod.Post, $"{path}/my-leave-applications/attachments")
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

    public async Task<ApiResult<List<LeaveTypeDto>>> GetLeaveTypesAsync(Guid companyId)
        => await SendAsync<List<LeaveTypeDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/leave-types?companyId={companyId}"), "leaveTypes");

    public async Task<ApiResult<List<LeaveTypeDto>>> GetMyLeaveTypesAsync()
        => await SendAsync<List<LeaveTypeDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/my-leave-types"), "leaveTypes");

    public async Task<ApiResult<LeaveTypeDto>> UpsertLeaveTypeAsync(UpsertLeaveTypeDto leaveType)
        => await SendAsync<LeaveTypeDto>(new HttpRequestMessage(leaveType.Id.HasValue ? HttpMethod.Put : HttpMethod.Post, leaveType.Id.HasValue ? $"{path}/leave-types/{leaveType.Id}" : $"{path}/leave-types")
        {
            Content = JsonContent.Create(new { LeaveType = leaveType })
        }, "leaveType");

    public async Task<ApiResult<bool>> DeleteLeaveTypeAsync(Guid id)
        => await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/leave-types/{id}"), "success");

    public async Task<ApiResult<List<LeavePeriodDto>>> GetLeavePeriodsAsync(Guid companyId)
        => await SendAsync<List<LeavePeriodDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/leave-periods?companyId={companyId}"), "leavePeriods");

    public async Task<ApiResult<LeavePeriodDto>> UpsertLeavePeriodAsync(UpsertLeavePeriodDto leavePeriod)
        => await SendAsync<LeavePeriodDto>(new HttpRequestMessage(leavePeriod.Id.HasValue ? HttpMethod.Put : HttpMethod.Post, leavePeriod.Id.HasValue ? $"{path}/leave-periods/{leavePeriod.Id}" : $"{path}/leave-periods")
        {
            Content = JsonContent.Create(new { LeavePeriod = leavePeriod })
        }, "leavePeriod");

    public async Task<ApiResult<bool>> DeleteLeavePeriodAsync(Guid id)
        => await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/leave-periods/{id}"), "success");

    public async Task<ApiResult<List<LeavePolicyDto>>> GetLeavePoliciesAsync(Guid companyId)
        => await SendAsync<List<LeavePolicyDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/leave-policies?companyId={companyId}"), "leavePolicies");

    public async Task<ApiResult<LeavePolicyDto>> UpsertLeavePolicyAsync(UpsertLeavePolicyDto leavePolicy)
        => await SendAsync<LeavePolicyDto>(new HttpRequestMessage(leavePolicy.Id.HasValue ? HttpMethod.Put : HttpMethod.Post, leavePolicy.Id.HasValue ? $"{path}/leave-policies/{leavePolicy.Id}" : $"{path}/leave-policies")
        {
            Content = JsonContent.Create(new { LeavePolicy = leavePolicy })
        }, "leavePolicy");

    public async Task<ApiResult<bool>> DeleteLeavePolicyAsync(Guid id)
        => await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/leave-policies/{id}"), "success");

    public async Task<ApiResult<List<LeavePolicyAssignmentDto>>> GetLeavePolicyAssignmentsAsync(Guid companyId)
        => await SendAsync<List<LeavePolicyAssignmentDto>>(new HttpRequestMessage(HttpMethod.Get, $"{path}/leave-policy-assignments?companyId={companyId}"), "assignments");

    public async Task<ApiResult<LeavePolicyAssignmentDto>> UpsertLeavePolicyAssignmentAsync(UpsertLeavePolicyAssignmentDto assignment)
        => await SendAsync<LeavePolicyAssignmentDto>(new HttpRequestMessage(assignment.Id.HasValue ? HttpMethod.Put : HttpMethod.Post, assignment.Id.HasValue ? $"{path}/leave-policy-assignments/{assignment.Id}" : $"{path}/leave-policy-assignments")
        {
            Content = JsonContent.Create(new { Assignment = assignment })
        }, "assignment");

    public async Task<ApiResult<bool>> DeleteLeavePolicyAssignmentAsync(Guid id)
        => await SendAsync<bool>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/leave-policy-assignments/{id}"), "success");

    public async Task<ApiResult<int>> GenerateLeaveAllocationsAsync(GenerateLeaveAllocationsDto request)
        => await SendAsync<int>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-allocations/generate")
        {
            Content = JsonContent.Create(new { Request = request })
        }, "createdEntries");

    public async Task<ApiResult<List<LeaveApplicationDto>>> GetLeaveApplicationsAsync(Guid companyId, Guid? employeeId = null, LeaveApplicationStatus? status = null)
    {
        var url = $"{path}/leave-applications?companyId={companyId}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";
        if (status.HasValue) url += $"&status={status.Value}";
        return await SendAsync<List<LeaveApplicationDto>>(new HttpRequestMessage(HttpMethod.Get, url), "applications");
    }

    public async Task<ApiResult<List<LeaveApplicationDto>>> GetMyLeaveApplicationsAsync(LeaveApplicationStatus? status = null)
    {
        var url = $"{path}/my-leave-applications";
        if (status.HasValue) url += $"?status={status.Value}";
        return await SendAsync<List<LeaveApplicationDto>>(new HttpRequestMessage(HttpMethod.Get, url), "applications");
    }

    public async Task<ApiResult<LeaveApplicationDto>> UpsertLeaveApplicationAsync(UpsertLeaveApplicationDto application)
        => await SendAsync<LeaveApplicationDto>(new HttpRequestMessage(application.Id.HasValue ? HttpMethod.Put : HttpMethod.Post, application.Id.HasValue ? $"{path}/leave-applications/{application.Id}" : $"{path}/leave-applications")
        {
            Content = JsonContent.Create(new { Application = application })
        }, "application");

    public async Task<ApiResult<LeaveApplicationDto>> UpsertMyLeaveApplicationAsync(UpsertLeaveApplicationDto application)
        => await SendAsync<LeaveApplicationDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/my-leave-applications")
        {
            Content = JsonContent.Create(new { Application = application })
        }, "application");

    public async Task<ApiResult<LeaveApplicationDto>> SubmitLeaveApplicationAsync(Guid id)
        => await SendAsync<LeaveApplicationDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-applications/{id}/submit"), "application");

    public async Task<ApiResult<LeaveApplicationDto>> SubmitMyLeaveApplicationAsync(Guid id)
        => await SendAsync<LeaveApplicationDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/my-leave-applications/{id}/submit"), "application");

    public async Task<ApiResult<LeaveApplicationDto>> ReviewLeaveApplicationAsync(ReviewLeaveApplicationDto review)
        => await SendAsync<LeaveApplicationDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-applications/review")
        {
            Content = JsonContent.Create(new { Review = review })
        }, "application");

    public async Task<ApiResult<LeaveApplicationDto>> CancelLeaveApplicationAsync(Guid id)
        => await SendAsync<LeaveApplicationDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-applications/{id}/cancel"), "application");

    public async Task<ApiResult<LeaveApplicationDto>> CancelMyLeaveApplicationAsync(Guid id)
        => await SendAsync<LeaveApplicationDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/my-leave-applications/{id}/cancel"), "application");

    public async Task<ApiResult<List<LeaveLedgerEntryDto>>> GetLeaveLedgerEntriesAsync(Guid companyId, Guid? employeeId = null, Guid? leaveTypeId = null, Guid? leavePeriodId = null)
    {
        var url = $"{path}/leave-ledger?companyId={companyId}";
        if (employeeId.HasValue) url += $"&employeeId={employeeId.Value}";
        if (leaveTypeId.HasValue) url += $"&leaveTypeId={leaveTypeId.Value}";
        if (leavePeriodId.HasValue) url += $"&leavePeriodId={leavePeriodId.Value}";
        return await SendAsync<List<LeaveLedgerEntryDto>>(new HttpRequestMessage(HttpMethod.Get, url), "entries");
    }

    public async Task<ApiResult<LeaveLedgerEntryDto>> CreateLeaveLedgerAdjustmentAsync(CreateLeaveLedgerAdjustmentDto adjustment)
        => await SendAsync<LeaveLedgerEntryDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-ledger/adjustments")
        {
            Content = JsonContent.Create(new { Adjustment = adjustment })
        }, "entry");

    public async Task<ApiResult<(LeaveEncashmentDto? Encashment, LeaveLedgerEntryDto? Entry)>> CreateLeaveEncashmentAsync(CreateLeaveEncashmentDto encashment)
    {
        var result = await SendAsync<CreateLeaveEncashmentResponse>(new HttpRequestMessage(HttpMethod.Post, $"{path}/leave-ledger/encashments")
        {
            Content = JsonContent.Create(new { Encashment = encashment })
        }, null);

        return result.IsSuccess
            ? ApiResult<(LeaveEncashmentDto? Encashment, LeaveLedgerEntryDto? Entry)>.Success((result.Data?.Encashment, result.Data?.Entry))
            : ApiResult<(LeaveEncashmentDto? Encashment, LeaveLedgerEntryDto? Entry)>.Failure(result.Error!);
    }

    private sealed class CreateLeaveEncashmentResponse
    {
        public LeaveEncashmentDto? Encashment { get; set; }
        public LeaveLedgerEntryDto? Entry { get; set; }
    }
}
