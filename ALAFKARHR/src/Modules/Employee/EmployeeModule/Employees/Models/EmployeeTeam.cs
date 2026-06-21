using Shared.DDD;
using SharedWithUI.Employees.Enums;

namespace EmployeeModule.Employees.Models;

public class EmployeeTeam : Entity<Guid>
{
    private readonly List<EmployeeTeamMember> _members = [];

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? NameEng { get; private set; }
    public EmployeeTeamCategory Category { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }
    public Guid? CreatedForProjectId { get; private set; }
    public IReadOnlyCollection<EmployeeTeamMember> Members => _members;

    private EmployeeTeam() { }

    public static EmployeeTeam Create(EmployeeTeamDto dto, string createdBy)
    {
        return new EmployeeTeam
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            NameEng = Normalize(dto.NameEng),
            Category = dto.Category,
            IsActive = dto.IsActive,
            Notes = Normalize(dto.Notes),
            CreatedForProjectId = dto.CreatedForProjectId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(EmployeeTeamDto dto, string modifiedBy)
    {
        CompanyId = dto.CompanyId;
        Name = dto.Name.Trim();
        NameEng = Normalize(dto.NameEng);
        Category = dto.Category;
        IsActive = dto.IsActive;
        Notes = Normalize(dto.Notes);
        CreatedForProjectId = dto.CreatedForProjectId;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void ReplaceMembers(IEnumerable<EmployeeTeamMember> members)
    {
        _members.Clear();
        _members.AddRange(members);
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        IsActive = false;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class EmployeeTeamMember : Entity<Guid>
{
    public Guid TeamId { get; private set; }
    public Guid EmployeeId { get; private set; }
    public string EmployeeName { get; private set; } = string.Empty;
    public string? EmployeeNameEng { get; private set; }
    public string? EmployeeNo { get; private set; }

    private EmployeeTeamMember() { }

    public static EmployeeTeamMember Create(Guid teamId, EmployeeTeamMemberDto dto, string createdBy)
    {
        return new EmployeeTeamMember
        {
            Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
            TeamId = teamId,
            EmployeeId = dto.EmployeeId,
            EmployeeName = dto.EmployeeName.Trim(),
            EmployeeNameEng = Normalize(dto.EmployeeNameEng),
            EmployeeNo = Normalize(dto.EmployeeNo),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
