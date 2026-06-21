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
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public Guid? DistributionPlaceId { get; set; }
    public string? PlaceName { get; set; }
    public string? FreeTextLocation { get; set; }
    public string? Notes { get; set; }
    public int MediaCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<MediaActivityCustomerDto> Customers { get; set; } = [];
    public List<MediaActivityAllocationDto> Allocations { get; set; } = [];
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
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public Guid? DistributionPlaceId { get; set; }
    public string? PlaceName { get; set; }
    public string? FreeTextLocation { get; set; }
    public string? Notes { get; set; }
    public List<MediaActivityCustomerDto> Customers { get; set; } = [];
    public List<MediaActivityAllocationDto> Allocations { get; set; } = [];
}

public class MediaActivityCustomerDto
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerNameEng { get; set; }
    public Guid? ProjectCustomerId { get; set; }
    public string? ProjectCustomerName { get; set; }
}

public class MediaActivityAllocationDto
{
    public Guid Id { get; set; }
    public Guid ProjectDistributionAllocationId { get; set; }
    public DateTime DistributionDate { get; set; }
    public Guid? ProjectCustomerId { get; set; }
    public string? CustomerName { get; set; }
    public Guid? DeliverableId { get; set; }
    public string? DeliverableName { get; set; }
    public Guid? DistributionPlaceId { get; set; }
    public string? PlaceName { get; set; }
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
