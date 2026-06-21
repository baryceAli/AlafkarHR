namespace MediaCenter.Activities.Models;

public class MediaActivityType : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private MediaActivityType() { }

    public static MediaActivityType Create(MediaActivityTypeDto dto, string createdBy)
    {
        Validate(dto);
        return new MediaActivityType
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            NameEng = string.IsNullOrWhiteSpace(dto.NameEng) ? dto.Name.Trim() : dto.NameEng.Trim(),
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(MediaActivityTypeDto dto, string modifiedBy)
    {
        Validate(dto);
        Name = dto.Name.Trim();
        NameEng = string.IsNullOrWhiteSpace(dto.NameEng) ? dto.Name.Trim() : dto.NameEng.Trim();
        Description = dto.Description;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static void Validate(MediaActivityTypeDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("Activity type name is required.");
    }
}
