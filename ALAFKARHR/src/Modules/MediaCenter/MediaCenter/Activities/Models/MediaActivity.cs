namespace MediaCenter.Activities.Models;

public class MediaActivity : Aggregate<Guid>
{
    private readonly List<MediaActivityCustomer> _customers = [];
    private readonly List<MediaActivityAllocation> _allocations = [];
    private readonly List<MediaActivityMedia> _media = [];

    public Guid CompanyId { get; private set; }
    public Guid ActivityTypeId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string TitleEng { get; private set; } = string.Empty;
    public DateTime ActivityDate { get; private set; }
    public TimeOnly? ActivityTime { get; private set; }
    public Guid? ProjectId { get; private set; }
    public string? ProjectName { get; private set; }
    public Guid? DistributionPlaceId { get; private set; }
    public string? PlaceName { get; private set; }
    public string? FreeTextLocation { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<MediaActivityCustomer> Customers => _customers;
    public IReadOnlyCollection<MediaActivityAllocation> Allocations => _allocations;
    public IReadOnlyCollection<MediaActivityMedia> Media => _media;

    private MediaActivity() { }

    public static MediaActivity Create(SaveMediaActivityDto dto, string createdBy)
    {
        Validate(dto);
        return new MediaActivity
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            ActivityTypeId = dto.ActivityTypeId,
            Title = dto.Title.Trim(),
            TitleEng = string.IsNullOrWhiteSpace(dto.TitleEng) ? dto.Title.Trim() : dto.TitleEng.Trim(),
            ActivityDate = dto.ActivityDate.Date,
            ActivityTime = dto.ActivityTime,
            ProjectId = dto.ProjectId,
            ProjectName = dto.ProjectName,
            DistributionPlaceId = dto.DistributionPlaceId,
            PlaceName = dto.PlaceName,
            FreeTextLocation = dto.FreeTextLocation,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(SaveMediaActivityDto dto, string modifiedBy)
    {
        Validate(dto);
        ActivityTypeId = dto.ActivityTypeId;
        Title = dto.Title.Trim();
        TitleEng = string.IsNullOrWhiteSpace(dto.TitleEng) ? dto.Title.Trim() : dto.TitleEng.Trim();
        ActivityDate = dto.ActivityDate.Date;
        ActivityTime = dto.ActivityTime;
        ProjectId = dto.ProjectId;
        ProjectName = dto.ProjectName;
        DistributionPlaceId = dto.DistributionPlaceId;
        PlaceName = dto.PlaceName;
        FreeTextLocation = dto.FreeTextLocation;
        Notes = dto.Notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void AddMedia(MediaActivityMedia media) => _media.Add(media);

    private static void Validate(SaveMediaActivityDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (dto.ActivityTypeId == Guid.Empty)
            throw new BadRequestException("Activity type is required.");
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new BadRequestException("Activity title is required.");
    }
}

public class MediaActivityCustomer : Entity<Guid>
{
    public Guid MediaActivityId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public string? CustomerNameEng { get; private set; }
    public Guid? ProjectCustomerId { get; private set; }
    public string? ProjectCustomerName { get; private set; }

    private MediaActivityCustomer() { }

    public static MediaActivityCustomer Create(Guid mediaActivityId, MediaActivityCustomerDto dto, string createdBy)
    {
        if (!dto.CustomerId.HasValue && !dto.ProjectCustomerId.HasValue && string.IsNullOrWhiteSpace(dto.CustomerName))
            throw new BadRequestException("Customer or project customer is required.");

        return new MediaActivityCustomer
        {
            Id = Guid.NewGuid(),
            MediaActivityId = mediaActivityId,
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName,
            CustomerNameEng = dto.CustomerNameEng,
            ProjectCustomerId = dto.ProjectCustomerId,
            ProjectCustomerName = dto.ProjectCustomerName,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}

public class MediaActivityAllocation : Entity<Guid>
{
    public Guid MediaActivityId { get; private set; }
    public Guid ProjectDistributionAllocationId { get; private set; }
    public DateTime DistributionDate { get; private set; }
    public Guid? ProjectCustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public Guid? DeliverableId { get; private set; }
    public string? DeliverableName { get; private set; }
    public Guid? DistributionPlaceId { get; private set; }
    public string? PlaceName { get; private set; }

    private MediaActivityAllocation() { }

    public static MediaActivityAllocation Create(Guid mediaActivityId, MediaActivityAllocationDto dto, string createdBy)
    {
        if (dto.ProjectDistributionAllocationId == Guid.Empty)
            throw new BadRequestException("Project distribution allocation is required.");

        return new MediaActivityAllocation
        {
            Id = Guid.NewGuid(),
            MediaActivityId = mediaActivityId,
            ProjectDistributionAllocationId = dto.ProjectDistributionAllocationId,
            DistributionDate = dto.DistributionDate.Date,
            ProjectCustomerId = dto.ProjectCustomerId,
            CustomerName = dto.CustomerName,
            DeliverableId = dto.DeliverableId,
            DeliverableName = dto.DeliverableName,
            DistributionPlaceId = dto.DistributionPlaceId,
            PlaceName = dto.PlaceName,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}

public class MediaActivityMedia : Entity<Guid>
{
    public Guid MediaActivityId { get; private set; }
    public Guid DocumentId { get; private set; }
    public MediaKind MediaKind { get; private set; }
    public string? Caption { get; private set; }
    public DateTime? CapturedAt { get; private set; }
    public Guid? UploadedByUserId { get; private set; }
    public bool IsPrimary { get; private set; }

    private MediaActivityMedia() { }

    public static MediaActivityMedia Create(Guid mediaActivityId, AddMediaActivityMediaDto dto, Guid? uploadedByUserId, string createdBy)
    {
        if (dto.DocumentId == Guid.Empty)
            throw new BadRequestException("Document is required.");

        return new MediaActivityMedia
        {
            Id = Guid.NewGuid(),
            MediaActivityId = mediaActivityId,
            DocumentId = dto.DocumentId,
            MediaKind = dto.MediaKind,
            Caption = dto.Caption,
            CapturedAt = dto.CapturedAt,
            UploadedByUserId = uploadedByUserId,
            IsPrimary = dto.IsPrimary,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Update(UpdateMediaActivityMediaDto dto, string modifiedBy)
    {
        MediaKind = dto.MediaKind;
        Caption = dto.Caption;
        CapturedAt = dto.CapturedAt;
        IsPrimary = dto.IsPrimary;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
