namespace Contracts.Contracts.Models;

public class ContractTemplate : Aggregate<Guid>
{
    private ContractTemplate()
    {
    }

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public string ContractType { get; private set; } = string.Empty;
    public string? FileName { get; private set; }
    public string? FilePath { get; private set; }
    public string? ContentType { get; private set; }
    public long FileSize { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static ContractTemplate Create(ContractTemplateDto dto, string userId)
    {
        Validate(dto);
        return new ContractTemplate
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            NameEng = dto.NameEng.Trim(),
            ContractType = dto.ContractType.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void Update(ContractTemplateDto dto, string userId)
    {
        Validate(dto);
        Name = dto.Name.Trim();
        NameEng = dto.NameEng.Trim();
        ContractType = dto.ContractType.Trim();
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void SetFile(string fileName, string filePath, string contentType, long fileSize, string userId)
    {
        FileName = fileName;
        FilePath = filePath;
        ContentType = contentType;
        FileSize = fileSize;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public ContractTemplateDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        Name = Name,
        NameEng = NameEng,
        ContractType = ContractType,
        FileName = FileName,
        FilePath = FilePath,
        ContentType = ContentType,
        FileSize = FileSize,
        IsActive = IsActive
    };

    private static void Validate(ContractTemplateDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("Template name is required.");
        if (string.IsNullOrWhiteSpace(dto.NameEng))
            throw new BadRequestException("English template name is required.");
        if (string.IsNullOrWhiteSpace(dto.ContractType))
            throw new BadRequestException("Contract type is required.");
    }
}
