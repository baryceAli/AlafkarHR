using AlAfkarERP.Shared.Dtos;
using SharedWithUI.MediaCenter.Dtos;
using SharedWithUI.MediaCenter.Enums;

namespace AlAfkarERP.Shared.Pages.Features.MediaCenter.Services;

public interface IMediaCenterService
{
    Task<ApiResult<PaginatedResult<MediaActivityTypeDto>>> GetActivityTypesAsync(int pageIndex, int pageSize, Guid? companyId = null, string? searchText = null, bool activeOnly = true);
    Task<ApiResult<MediaActivityTypeDto>> GetActivityTypeByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateActivityTypeAsync(MediaActivityTypeDto activityType);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateActivityTypeAsync(MediaActivityTypeDto activityType);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteActivityTypeAsync(Guid id);
    Task<ApiResult<PaginatedResult<MediaActivityDto>>> GetActivitiesAsync(MediaActivityFilter filter);
    Task<ApiResult<MediaActivityDto>> GetActivityByIdAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> CreateActivityAsync(SaveMediaActivityDto activity);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateActivityAsync(SaveMediaActivityDto activity);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteActivityAsync(Guid id);
    Task<ApiResult<CreateResponseDto>> AddMediaAsync(Guid activityId, AddMediaActivityMediaDto media);
    Task<ApiResult<UpdateDeleteResponseDto>> UpdateMediaAsync(Guid activityId, UpdateMediaActivityMediaDto media);
    Task<ApiResult<UpdateDeleteResponseDto>> DeleteMediaAsync(Guid activityId, Guid mediaId);
}

public class MediaActivityFilter
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; } = 20;
    public Guid? CompanyId { get; set; }
    public string? SearchText { get; set; }
    public Guid? ActivityTypeId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ProjectCustomerId { get; set; }
    public Guid? PlaceId { get; set; }
    public Guid? AllocationId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public MediaKind? MediaKind { get; set; }
}
