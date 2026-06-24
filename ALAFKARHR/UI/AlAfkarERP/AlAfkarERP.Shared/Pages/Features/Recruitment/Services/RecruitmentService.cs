using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Recruitment.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Recruitment.Services;

public class RecruitmentService : BaseApiService, IRecruitmentService
{
    private readonly string _path;

    public RecruitmentService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/recruitment";
    }

    public async Task<ApiResult<List<StaffingPlanDto>>> GetStaffingPlansAsync(Guid companyId, int? year = null)
    {
        var suffix = year.HasValue ? $"?year={year.Value}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/staffing-plans/company/{companyId}{suffix}");
        return await SendAsync<List<StaffingPlanDto>>(request, "staffingPlanList");
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> CreateStaffingPlanAsync(UpsertStaffingPlanDto plan)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/staffing-plans") { Content = JsonContent.Create(plan) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> UpdateStaffingPlanAsync(UpsertStaffingPlanDto plan)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/staffing-plans/{plan.Id}") { Content = JsonContent.Create(plan) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> DeleteStaffingPlanAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/staffing-plans/{id}");
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<JobRequisitionDto>>> GetJobRequisitionsAsync(Guid companyId, RecruitmentRequestStatus? status = null)
    {
        var suffix = status.HasValue ? $"?status={status.Value}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/job-requisitions/company/{companyId}{suffix}");
        return await SendAsync<List<JobRequisitionDto>>(request, "jobRequisitionList");
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> CreateJobRequisitionAsync(UpsertJobRequisitionDto requisition)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/job-requisitions") { Content = JsonContent.Create(requisition) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> UpdateJobRequisitionAsync(UpsertJobRequisitionDto requisition)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/job-requisitions/{requisition.Id}") { Content = JsonContent.Create(requisition) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public Task<ApiResult<RecruitmentActionResultDto>> OpenJobRequisitionAsync(Guid id) => PostActionAsync($"job-requisitions/{id}/open");
    public Task<ApiResult<RecruitmentActionResultDto>> CancelJobRequisitionAsync(Guid id) => PostActionAsync($"job-requisitions/{id}/cancel");
    public Task<ApiResult<RecruitmentActionResultDto>> CloseJobRequisitionAsync(Guid id) => PostActionAsync($"job-requisitions/{id}/close");

    public async Task<ApiResult<List<ApplicantDto>>> GetApplicantsAsync(Guid companyId, Guid? jobRequisitionId = null, RecruitmentRequestStatus? status = null)
    {
        var query = new List<string>();
        if (jobRequisitionId.HasValue && jobRequisitionId.Value != Guid.Empty) query.Add($"jobRequisitionId={jobRequisitionId.Value}");
        if (status.HasValue) query.Add($"status={status.Value}");
        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/applicants/company/{companyId}{suffix}");
        return await SendAsync<List<ApplicantDto>>(request, "applicantList");
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> CreateApplicantAsync(UpsertApplicantDto applicant)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/applicants") { Content = JsonContent.Create(applicant) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> UpdateApplicantAsync(UpsertApplicantDto applicant)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/applicants/{applicant.Id}") { Content = JsonContent.Create(applicant) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> MoveApplicantAsync(Guid id, RecruitmentRequestStatus status)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/applicants/{id}/status")
        {
            Content = JsonContent.Create(new RecruitmentStatusActionDto { Status = status })
        };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<InterviewFeedbackDto>>> GetInterviewFeedbackAsync(Guid applicantId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/applicants/{applicantId}/interviews");
        return await SendAsync<List<InterviewFeedbackDto>>(request, "interviewFeedbackList");
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> CreateInterviewFeedbackAsync(UpsertInterviewFeedbackDto feedback)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/interviews") { Content = JsonContent.Create(feedback) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> UpdateInterviewFeedbackAsync(UpsertInterviewFeedbackDto feedback)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/interviews/{feedback.Id}") { Content = JsonContent.Create(feedback) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> DeleteInterviewFeedbackAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/interviews/{id}");
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<JobOfferDto>>> GetJobOffersAsync(Guid companyId, Guid? applicantId = null)
    {
        var suffix = applicantId.HasValue && applicantId.Value != Guid.Empty ? $"?applicantId={applicantId.Value}" : string.Empty;
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/job-offers/company/{companyId}{suffix}");
        return await SendAsync<List<JobOfferDto>>(request, "jobOfferList");
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> CreateJobOfferAsync(UpsertJobOfferDto offer)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/job-offers") { Content = JsonContent.Create(offer) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public async Task<ApiResult<RecruitmentActionResultDto>> UpdateJobOfferAsync(UpsertJobOfferDto offer)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/job-offers/{offer.Id}") { Content = JsonContent.Create(offer) };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    public Task<ApiResult<RecruitmentActionResultDto>> AcceptJobOfferAsync(Guid id) => PostActionAsync($"job-offers/{id}/accept");
    public Task<ApiResult<RecruitmentActionResultDto>> RejectJobOfferAsync(Guid id) => PostActionAsync($"job-offers/{id}/reject");

    public async Task<ApiResult<RecruitmentActionResultDto>> MarkOfferEmployeeCreatedAsync(Guid id, Guid employeeId)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/job-offers/{id}/mark-employee-created")
        {
            Content = JsonContent.Create(new RecruitmentHireActionDto { EmployeeId = employeeId })
        };
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }

    private async Task<ApiResult<RecruitmentActionResultDto>> PostActionAsync(string relativePath)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{relativePath}");
        return await SendAsync<RecruitmentActionResultDto>(request, null);
    }
}
