namespace MediaCenter.Activities.Models;

public class MediaActivity : Aggregate<Guid>
{
    private readonly List<MediaActivityRelatedRecord> _relatedRecords = [];
    private readonly List<MediaActivityMedia> _media = [];

    public Guid CompanyId { get; private set; }
    public Guid ActivityTypeId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string TitleEng { get; private set; } = string.Empty;
    public DateTime ActivityDate { get; private set; }
    public TimeOnly? ActivityTime { get; private set; }
    public string? LocationText { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<MediaActivityRelatedRecord> RelatedRecords => _relatedRecords;
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
            LocationText = dto.LocationText,
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
        LocationText = dto.LocationText;
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

public class MediaActivityRelatedRecord : Entity<Guid>
{
    public Guid MediaActivityId { get; private set; }
    public string RelatedType { get; private set; } = string.Empty;
    public Guid? RelatedRecordId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string? Notes { get; private set; }

    private MediaActivityRelatedRecord() { }

    public static MediaActivityRelatedRecord Create(Guid mediaActivityId, MediaActivityRelatedRecordDto dto, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(dto.RelatedType))
            throw new BadRequestException("Related record type is required.");
        if (string.IsNullOrWhiteSpace(dto.DisplayName))
            throw new BadRequestException("Related record display name is required.");

        return new MediaActivityRelatedRecord
        {
            Id = Guid.NewGuid(),
            MediaActivityId = mediaActivityId,
            RelatedType = dto.RelatedType.Trim(),
            RelatedRecordId = dto.RelatedRecordId,
            DisplayName = dto.DisplayName.Trim(),
            Notes = dto.Notes,
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
