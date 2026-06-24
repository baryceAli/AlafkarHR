using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Recruitment.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Recruitment.Services;

public interface IRecruitmentService
{
    Task<ApiResult<List<StaffingPlanDto>>> GetStaffingPlansAsync(Guid companyId, int? year = null);
    Task<ApiResult<RecruitmentActionResultDto>> CreateStaffingPlanAsync(UpsertStaffingPlanDto plan);
    Task<ApiResult<RecruitmentActionResultDto>> UpdateStaffingPlanAsync(UpsertStaffingPlanDto plan);
    Task<ApiResult<RecruitmentActionResultDto>> DeleteStaffingPlanAsync(Guid id);

    Task<ApiResult<List<JobRequisitionDto>>> GetJobRequisitionsAsync(Guid companyId, RecruitmentRequestStatus? status = null);
    Task<ApiResult<RecruitmentActionResultDto>> CreateJobRequisitionAsync(UpsertJobRequisitionDto requisition);
    Task<ApiResult<RecruitmentActionResultDto>> UpdateJobRequisitionAsync(UpsertJobRequisitionDto requisition);
    Task<ApiResult<RecruitmentActionResultDto>> OpenJobRequisitionAsync(Guid id);
    Task<ApiResult<RecruitmentActionResultDto>> CancelJobRequisitionAsync(Guid id);
    Task<ApiResult<RecruitmentActionResultDto>> CloseJobRequisitionAsync(Guid id);

    Task<ApiResult<List<ApplicantDto>>> GetApplicantsAsync(Guid companyId, Guid? jobRequisitionId = null, RecruitmentRequestStatus? status = null);
    Task<ApiResult<RecruitmentActionResultDto>> CreateApplicantAsync(UpsertApplicantDto applicant);
    Task<ApiResult<RecruitmentActionResultDto>> UpdateApplicantAsync(UpsertApplicantDto applicant);
    Task<ApiResult<RecruitmentActionResultDto>> MoveApplicantAsync(Guid id, RecruitmentRequestStatus status);

    Task<ApiResult<List<InterviewFeedbackDto>>> GetInterviewFeedbackAsync(Guid applicantId);
    Task<ApiResult<RecruitmentActionResultDto>> CreateInterviewFeedbackAsync(UpsertInterviewFeedbackDto feedback);
    Task<ApiResult<RecruitmentActionResultDto>> UpdateInterviewFeedbackAsync(UpsertInterviewFeedbackDto feedback);
    Task<ApiResult<RecruitmentActionResultDto>> DeleteInterviewFeedbackAsync(Guid id);

    Task<ApiResult<List<JobOfferDto>>> GetJobOffersAsync(Guid companyId, Guid? applicantId = null);
    Task<ApiResult<RecruitmentActionResultDto>> CreateJobOfferAsync(UpsertJobOfferDto offer);
    Task<ApiResult<RecruitmentActionResultDto>> UpdateJobOfferAsync(UpsertJobOfferDto offer);
    Task<ApiResult<RecruitmentActionResultDto>> AcceptJobOfferAsync(Guid id);
    Task<ApiResult<RecruitmentActionResultDto>> RejectJobOfferAsync(Guid id);
    Task<ApiResult<RecruitmentActionResultDto>> MarkOfferEmployeeCreatedAsync(Guid id, Guid employeeId);
}
