using AlAfkarERP.Shared.Dtos;
using SharedWithUI.Training.Dtos;

namespace AlAfkarERP.Shared.Pages.Features.Training.Services;

public interface ITrainingService
{
    Task<ApiResult<List<TrainingProgramDto>>> GetProgramsAsync(Guid companyId);
    Task<ApiResult<TrainingActionResultDto>> CreateProgramAsync(UpsertTrainingProgramDto program);
    Task<ApiResult<TrainingActionResultDto>> UpdateProgramAsync(UpsertTrainingProgramDto program);
    Task<ApiResult<TrainingActionResultDto>> DeleteProgramAsync(Guid id);

    Task<ApiResult<List<TrainingEventDto>>> GetEventsAsync(Guid companyId, Guid? programId = null, TrainingEventStatus? status = null);
    Task<ApiResult<TrainingActionResultDto>> CreateEventAsync(UpsertTrainingEventDto trainingEvent);
    Task<ApiResult<TrainingActionResultDto>> UpdateEventAsync(UpsertTrainingEventDto trainingEvent);
    Task<ApiResult<TrainingActionResultDto>> OpenEventAsync(Guid id);
    Task<ApiResult<TrainingActionResultDto>> StartEventAsync(Guid id);
    Task<ApiResult<TrainingActionResultDto>> CompleteEventAsync(Guid id);
    Task<ApiResult<TrainingActionResultDto>> CancelEventAsync(Guid id);

    Task<ApiResult<List<TrainingAttendeeDto>>> GetAttendeesAsync(Guid trainingEventId);
    Task<ApiResult<TrainingActionResultDto>> CreateAttendeeAsync(UpsertTrainingAttendeeDto attendee);
    Task<ApiResult<TrainingActionResultDto>> UpdateAttendeeAsync(UpsertTrainingAttendeeDto attendee);
    Task<ApiResult<TrainingActionResultDto>> DeleteAttendeeAsync(Guid id);
    Task<ApiResult<TrainingActionResultDto>> MarkAttendanceAsync(Guid id, bool attended);
    Task<ApiResult<TrainingActionResultDto>> RecordResultAsync(Guid id, TrainingAttendeeResultDto result);
    Task<ApiResult<TrainingActionResultDto>> LinkCertificateAsync(Guid id, TrainingCertificateLinkDto certificate);
}
