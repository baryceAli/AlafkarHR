using Shared.DDD;



namespace LeaveManagement.Leave.Models;

public class LeaveType : Aggregate<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public bool IsPaid { get; private set; } = true;
    public bool AllowNegativeBalance { get; private set; }
    public decimal NegativeBalanceLimit { get; private set; }
    public bool RequiresAttachment { get; private set; }
    public bool IsEmergencyLeave { get; private set; }
    public bool IsActive { get; private set; } = true;

    private LeaveType() { }

    public static LeaveType Create(Guid id, UpsertLeaveTypeDto dto, string? userId)
    {
        Validate(dto);
        return new LeaveType
        {
            Id = id,
            CompanyId = dto.CompanyId,
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            NameEng = dto.NameEng.Trim(),
            IsPaid = dto.IsPaid,
            AllowNegativeBalance = dto.AllowNegativeBalance,
            NegativeBalanceLimit = dto.AllowNegativeBalance ? dto.NegativeBalanceLimit : 0,
            RequiresAttachment = dto.RequiresAttachment,
            IsEmergencyLeave = dto.IsEmergencyLeave,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void Update(UpsertLeaveTypeDto dto, string? userId)
    {
        if (CompanyId != dto.CompanyId)
        {
            throw new BadRequestException("Leave type company cannot be changed.");
        }

        Validate(dto);
        Code = dto.Code.Trim();
        Name = dto.Name.Trim();
        NameEng = dto.NameEng.Trim();
        IsPaid = dto.IsPaid;
        AllowNegativeBalance = dto.AllowNegativeBalance;
        NegativeBalanceLimit = dto.AllowNegativeBalance ? dto.NegativeBalanceLimit : 0;
        RequiresAttachment = dto.RequiresAttachment;
        IsEmergencyLeave = dto.IsEmergencyLeave;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string? userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    private static void Validate(UpsertLeaveTypeDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            throw new BadRequestException("Leave type code is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.NameEng))
        {
            throw new BadRequestException("Leave type Arabic and English names are required.");
        }

        if (dto.NegativeBalanceLimit < 0)
        {
            throw new BadRequestException("Negative balance limit cannot be negative.");
        }
    }
}
