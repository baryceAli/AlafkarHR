using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using SharedWithUI.Training.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.Training.Services;

public class TrainingService : BaseApiService, ITrainingService
{
    private readonly string _path;

    public TrainingService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        _path = $"api/{apiConfig.Version}/training";
    }

    public async Task<ApiResult<List<TrainingProgramDto>>> GetProgramsAsync(Guid companyId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/programs/company/{companyId}");
        return await SendAsync<List<TrainingProgramDto>>(request, "trainingProgramList");
    }

    public async Task<ApiResult<TrainingActionResultDto>> CreateProgramAsync(UpsertTrainingProgramDto program)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/programs") { Content = JsonContent.Create(program) };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<TrainingActionResultDto>> UpdateProgramAsync(UpsertTrainingProgramDto program)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/programs/{program.Id}") { Content = JsonContent.Create(program) };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<TrainingActionResultDto>> DeleteProgramAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/programs/{id}");
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<List<TrainingEventDto>>> GetEventsAsync(Guid companyId, Guid? programId = null, TrainingEventStatus? status = null)
    {
        var query = new List<string>();
        if (programId.HasValue && programId.Value != Guid.Empty) query.Add($"programId={programId.Value}");
        if (status.HasValue) query.Add($"status={status.Value}");
        var suffix = query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/events/company/{companyId}{suffix}");
        return await SendAsync<List<TrainingEventDto>>(request, "trainingEventList");
    }

    public async Task<ApiResult<TrainingActionResultDto>> CreateEventAsync(UpsertTrainingEventDto trainingEvent)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/events") { Content = JsonContent.Create(trainingEvent) };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<TrainingActionResultDto>> UpdateEventAsync(UpsertTrainingEventDto trainingEvent)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/events/{trainingEvent.Id}") { Content = JsonContent.Create(trainingEvent) };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public Task<ApiResult<TrainingActionResultDto>> OpenEventAsync(Guid id) => PostActionAsync($"events/{id}/open");
    public Task<ApiResult<TrainingActionResultDto>> StartEventAsync(Guid id) => PostActionAsync($"events/{id}/start");
    public Task<ApiResult<TrainingActionResultDto>> CompleteEventAsync(Guid id) => PostActionAsync($"events/{id}/complete");
    public Task<ApiResult<TrainingActionResultDto>> CancelEventAsync(Guid id) => PostActionAsync($"events/{id}/cancel");

    public async Task<ApiResult<List<TrainingAttendeeDto>>> GetAttendeesAsync(Guid trainingEventId)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_path}/events/{trainingEventId}/attendees");
        return await SendAsync<List<TrainingAttendeeDto>>(request, "trainingAttendeeList");
    }

    public async Task<ApiResult<TrainingActionResultDto>> CreateAttendeeAsync(UpsertTrainingAttendeeDto attendee)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/attendees") { Content = JsonContent.Create(attendee) };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<TrainingActionResultDto>> UpdateAttendeeAsync(UpsertTrainingAttendeeDto attendee)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"{_path}/attendees/{attendee.Id}") { Content = JsonContent.Create(attendee) };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<TrainingActionResultDto>> DeleteAttendeeAsync(Guid id)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, $"{_path}/attendees/{id}");
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<TrainingActionResultDto>> MarkAttendanceAsync(Guid id, bool attended)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/attendees/{id}/attendance")
        {
            Content = JsonContent.Create(new TrainingAttendeeResultDto { Attended = attended })
        };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<TrainingActionResultDto>> RecordResultAsync(Guid id, TrainingAttendeeResultDto result)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/attendees/{id}/result") { Content = JsonContent.Create(result) };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    public async Task<ApiResult<TrainingActionResultDto>> LinkCertificateAsync(Guid id, TrainingCertificateLinkDto certificate)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/attendees/{id}/certificate") { Content = JsonContent.Create(certificate) };
        return await SendAsync<TrainingActionResultDto>(request, null);
    }

    private async Task<ApiResult<TrainingActionResultDto>> PostActionAsync(string relativePath)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_path}/{relativePath}");
        return await SendAsync<TrainingActionResultDto>(request, null);
    }
}
