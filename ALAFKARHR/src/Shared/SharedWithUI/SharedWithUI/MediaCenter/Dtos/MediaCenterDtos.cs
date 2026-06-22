using SharedWithUI.MediaCenter.Enums;
using System.ComponentModel.DataAnnotations;

namespace SharedWithUI.MediaCenter.Dtos;

public class MediaActivityTypeDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string NameEng { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class MediaActivityDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ActivityTypeId { get; set; }
    public string ActivityTypeName { get; set; } = string.Empty;
    public string? ActivityTypeNameEng { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string TitleEng { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow.Date;
    public TimeOnly? ActivityTime { get; set; }
    public string? LocationText { get; set; }
    public string? Notes { get; set; }
    public int MediaCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<MediaActivityRelatedRecordDto> RelatedRecords { get; set; } = [];
    public List<MediaActivityMediaDto> Media { get; set; } = [];
}

public class SaveMediaActivityDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ActivityTypeId { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string TitleEng { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; } = DateTime.UtcNow.Date;
    public TimeOnly? ActivityTime { get; set; }
    public string? LocationText { get; set; }
    public string? Notes { get; set; }
    public List<MediaActivityRelatedRecordDto> RelatedRecords { get; set; } = [];
}

public class MediaActivityRelatedRecordDto
{
    public Guid Id { get; set; }
    public string RelatedType { get; set; } = string.Empty;
    public Guid? RelatedRecordId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class MediaActivityMediaDto
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public MediaKind MediaKind { get; set; } = MediaKind.Image;
    public string? Caption { get; set; }
    public DateTime? CapturedAt { get; set; }
    public Guid? UploadedByUserId { get; set; }
    public bool IsPrimary { get; set; }
}

public class AddMediaActivityMediaDto
{
    public Guid DocumentId { get; set; }
    public MediaKind MediaKind { get; set; } = MediaKind.Image;
    public string? Caption { get; set; }
    public DateTime? CapturedAt { get; set; }
    public bool IsPrimary { get; set; }
}

public class UpdateMediaActivityMediaDto
{
    public Guid Id { get; set; }
    public MediaKind MediaKind { get; set; } = MediaKind.Image;
    public string? Caption { get; set; }
    public DateTime? CapturedAt { get; set; }
    public bool IsPrimary { get; set; }
}
