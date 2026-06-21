using AlAfkarERP.Shared.Dtos;
using AlAfkarERP.Shared.Services;
using AlAfkarERP.Shared.Utilities;
using SharedWithUI.MediaCenter.Dtos;
using System.Net.Http.Json;

namespace AlAfkarERP.Shared.Pages.Features.MediaCenter.Services;

public class MediaCenterService : BaseApiService, IMediaCenterService
{
    private readonly string path;

    public MediaCenterService(HttpClient http, ITokenService tokenService, ApiConfig apiConfig) : base(http, tokenService, apiConfig)
    {
        path = $"api/{apiConfig.Version}/mediacenter";
    }

    public Task<ApiResult<PaginatedResult<MediaActivityTypeDto>>> GetActivityTypesAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, bool activeOnly = true)
    {
        var url = $"{path}/activity-types?pageIndex={pageIndex}&pageSize={pageSize}&activeOnly={activeOnly}&searchText={Uri.EscapeDataString(searchText ?? string.Empty)}";
        if (companyId.HasValue) url += $"&companyId={companyId}";
        return SendAsync<PaginatedResult<MediaActivityTypeDto>>(new HttpRequestMessage(HttpMethod.Get, url), "activityTypes");
    }

    public Task<ApiResult<CreateResponseDto>> CreateActivityTypeAsync(MediaActivityTypeDto activityType)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/activity-types") { Content = JsonContent.Create(new { ActivityType = activityType }) }, null);

    public Task<ApiResult<MediaActivityTypeDto>> GetActivityTypeByIdAsync(Guid id)
        => SendAsync<MediaActivityTypeDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/activity-types/{id}"), "activityType");

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateActivityTypeAsync(MediaActivityTypeDto activityType)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/activity-types/{activityType.Id}") { Content = JsonContent.Create(new { ActivityType = activityType }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteActivityTypeAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/activity-types/{id}"), null);

    public Task<ApiResult<PaginatedResult<MediaActivityDto>>> GetActivitiesAsync(MediaActivityFilter filter)
    {
        var url = $"{path}/activities?pageIndex={filter.PageIndex}&pageSize={filter.PageSize}&searchText={Uri.EscapeDataString(filter.SearchText ?? string.Empty)}";
        if (filter.CompanyId.HasValue) url += $"&companyId={filter.CompanyId}";
        if (filter.ActivityTypeId.HasValue) url += $"&activityTypeId={filter.ActivityTypeId}";
        if (filter.ProjectId.HasValue) url += $"&projectId={filter.ProjectId}";
        if (filter.CustomerId.HasValue) url += $"&customerId={filter.CustomerId}";
        if (filter.ProjectCustomerId.HasValue) url += $"&projectCustomerId={filter.ProjectCustomerId}";
        if (filter.PlaceId.HasValue) url += $"&placeId={filter.PlaceId}";
        if (filter.AllocationId.HasValue) url += $"&allocationId={filter.AllocationId}";
        if (filter.FromDate.HasValue) url += $"&fromDate={Uri.EscapeDataString(filter.FromDate.Value.ToString("O"))}";
        if (filter.ToDate.HasValue) url += $"&toDate={Uri.EscapeDataString(filter.ToDate.Value.ToString("O"))}";
        if (filter.MediaKind.HasValue) url += $"&mediaKind={filter.MediaKind}";
        return SendAsync<PaginatedResult<MediaActivityDto>>(new HttpRequestMessage(HttpMethod.Get, url), "activities");
    }

    public Task<ApiResult<MediaActivityDto>> GetActivityByIdAsync(Guid id)
        => SendAsync<MediaActivityDto>(new HttpRequestMessage(HttpMethod.Get, $"{path}/activities/{id}"), "activity");

    public Task<ApiResult<CreateResponseDto>> CreateActivityAsync(SaveMediaActivityDto activity)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/activities") { Content = JsonContent.Create(new { Activity = activity }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateActivityAsync(SaveMediaActivityDto activity)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/activities/{activity.Id}") { Content = JsonContent.Create(new { Activity = activity }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteActivityAsync(Guid id)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/activities/{id}"), null);

    public Task<ApiResult<CreateResponseDto>> AddMediaAsync(Guid activityId, AddMediaActivityMediaDto media)
        => SendAsync<CreateResponseDto>(new HttpRequestMessage(HttpMethod.Post, $"{path}/activities/{activityId}/media") { Content = JsonContent.Create(new { Media = media }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> UpdateMediaAsync(Guid activityId, UpdateMediaActivityMediaDto media)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Put, $"{path}/activities/{activityId}/media/{media.Id}") { Content = JsonContent.Create(new { Media = media }) }, null);

    public Task<ApiResult<UpdateDeleteResponseDto>> DeleteMediaAsync(Guid activityId, Guid mediaId)
        => SendAsync<UpdateDeleteResponseDto>(new HttpRequestMessage(HttpMethod.Delete, $"{path}/activities/{activityId}/media/{mediaId}"), null);
}
