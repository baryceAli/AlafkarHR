using Shared.DDD;

namespace LeaveManagement.Leave.Models;

public class LeavePeriod : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsClosed { get; private set; }

    private LeavePeriod() { }

    public static LeavePeriod Create(Guid id, UpsertLeavePeriodDto dto, string? userId)
    {
        Validate(dto);
        return new LeavePeriod
        {
            Id = id,
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            StartDate = UtcDateTime.Normalize(dto.StartDate).Date,
            EndDate = UtcDateTime.Normalize(dto.EndDate).Date,
            IsClosed = dto.IsClosed,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void Update(UpsertLeavePeriodDto dto, string? userId)
    {
        if (CompanyId != dto.CompanyId)
        {
            throw new BadRequestException("Leave period company cannot be changed.");
        }

        Validate(dto);
        Name = dto.Name.Trim();
        StartDate = UtcDateTime.Normalize(dto.StartDate).Date;
        EndDate = UtcDateTime.Normalize(dto.EndDate).Date;
        IsClosed = dto.IsClosed;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string? userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    private static void Validate(UpsertLeavePeriodDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new BadRequestException("Leave period name is required.");
        }

        if (UtcDateTime.Normalize(dto.EndDate).Date < UtcDateTime.Normalize(dto.StartDate).Date)
        {
            throw new BadRequestException("Leave period end date must be on or after start date.");
        }
    }
}

public class LeavePolicy : Aggregate<Guid>
{
    private readonly List<LeavePolicyLine> _lines = [];

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public IReadOnlyCollection<LeavePolicyLine> Lines => _lines;

    private LeavePolicy() { }

    public static LeavePolicy Create(Guid id, UpsertLeavePolicyDto dto, string? userId)
    {
        Validate(dto);
        var policy = new LeavePolicy
        {
            Id = id,
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            NameEng = dto.NameEng.Trim(),
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        policy.ReplaceLines(dto.Lines);
        return policy;
    }

    public void Update(UpsertLeavePolicyDto dto, string? userId)
    {
        if (CompanyId != dto.CompanyId)
        {
            throw new BadRequestException("Leave policy company cannot be changed.");
        }

        Validate(dto);
        Name = dto.Name.Trim();
        NameEng = dto.NameEng.Trim();
        IsActive = dto.IsActive;
        ReplaceLines(dto.Lines);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string? userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    private void ReplaceLines(IEnumerable<LeavePolicyLineDto> lines)
    {
        var requestedLines = lines
            .Where(x => x.LeaveTypeId != Guid.Empty)
            .ToList();
        var duplicateLeaveTypeId = requestedLines
            .GroupBy(x => x.LeaveTypeId)
            .FirstOrDefault(x => x.Count() > 1)
            ?.Key;

        if (duplicateLeaveTypeId.HasValue)
        {
            throw new BadRequestException("Leave policy cannot contain duplicate leave types.");
        }

        var requestedLeaveTypeIds = requestedLines
            .Select(x => x.LeaveTypeId)
            .ToHashSet();

        _lines.RemoveAll(x => !requestedLeaveTypeIds.Contains(x.LeaveTypeId));

        foreach (var line in requestedLines)
        {
            var existingLine = _lines.FirstOrDefault(x => x.LeaveTypeId == line.LeaveTypeId);
            if (existingLine is null)
            {
                _lines.Add(LeavePolicyLine.Create(Guid.NewGuid(), Id, line));
                continue;
            }

            existingLine.Update(line);
        }
    }

    private static void Validate(UpsertLeavePolicyDto dto)
    {
        if (dto.CompanyId == Guid.Empty)
        {
            throw new BadRequestException("Company is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.NameEng))
        {
            throw new BadRequestException("Leave policy Arabic and English names are required.");
        }
    }
}

public class LeavePolicyLine : Entity<Guid>
{
    public Guid LeavePolicyId { get; private set; }
    public Guid LeaveTypeId { get; private set; }
    public decimal AnnualAllocationDays { get; private set; }
    public bool AccruesMonthly { get; private set; }
    public bool AllowCarryForward { get; private set; }
    public decimal MaxCarryForwardDays { get; private set; }

    private LeavePolicyLine() { }

    public static LeavePolicyLine Create(Guid id, Guid leavePolicyId, LeavePolicyLineDto dto)
    {
        Validate(dto);

        return new LeavePolicyLine
        {
            Id = id,
            LeavePolicyId = leavePolicyId,
            LeaveTypeId = dto.LeaveTypeId,
            AnnualAllocationDays = dto.AnnualAllocationDays,
            AccruesMonthly = dto.AccruesMonthly,
            AllowCarryForward = dto.AllowCarryForward,
            MaxCarryForwardDays = dto.MaxCarryForwardDays
        };
    }

    public void Update(LeavePolicyLineDto dto)
    {
        if (LeaveTypeId != dto.LeaveTypeId)
        {
            throw new BadRequestException("Leave policy line leave type cannot be changed.");
        }

        Validate(dto);
        AnnualAllocationDays = dto.AnnualAllocationDays;
        AccruesMonthly = dto.AccruesMonthly;
        AllowCarryForward = dto.AllowCarryForward;
        MaxCarryForwardDays = dto.MaxCarryForwardDays;
    }

    private static void Validate(LeavePolicyLineDto dto)
    {
        if (dto.AnnualAllocationDays < 0 || dto.MaxCarryForwardDays < 0)
        {
            throw new BadRequestException("Leave policy days cannot be negative.");
        }
    }
}

public class LeavePolicyAssignment : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public Guid PolicyId { get; private set; }
    public LeavePolicyAssignmentTarget Target { get; private set; }
    public Guid? EmployeeId { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    private LeavePolicyAssignment() { }

    public static LeavePolicyAssignment Create(Guid id, UpsertLeavePolicyAssignmentDto dto, string? userId)
    {
        Validate(dto);
        return new LeavePolicyAssignment
        {
            Id = id,
            CompanyId = dto.CompanyId,
            PolicyId = dto.PolicyId,
            Target = dto.Target,
            EmployeeId = dto.Target == LeavePolicyAssignmentTarget.Employee ? dto.EmployeeId : null,
            DepartmentId = dto.Target == LeavePolicyAssignmentTarget.Department ? dto.DepartmentId : null,
            EffectiveFrom = UtcDateTime.Normalize(dto.EffectiveFrom).Date,
            EffectiveTo = dto.EffectiveTo.HasValue ? UtcDateTime.Normalize(dto.EffectiveTo.Value).Date : null,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
    }

    public void Update(UpsertLeavePolicyAssignmentDto dto, string? userId)
    {
        if (CompanyId != dto.CompanyId)
        {
            throw new BadRequestException("Leave policy assignment company cannot be changed.");
        }

        Validate(dto);
        PolicyId = dto.PolicyId;
        Target = dto.Target;
        EmployeeId = dto.Target == LeavePolicyAssignmentTarget.Employee ? dto.EmployeeId : null;
        DepartmentId = dto.Target == LeavePolicyAssignmentTarget.Department ? dto.DepartmentId : null;
        EffectiveFrom = UtcDateTime.Normalize(dto.EffectiveFrom).Date;
        EffectiveTo = dto.EffectiveTo.HasValue ? UtcDateTime.Normalize(dto.EffectiveTo.Value).Date : null;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string? userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    private static void Validate(UpsertLeavePolicyAssignmentDto dto)
    {
        if (dto.CompanyId == Guid.Empty || dto.PolicyId == Guid.Empty)
        {
            throw new BadRequestException("Company and policy are required.");
        }

        if (dto.Target == LeavePolicyAssignmentTarget.Employee && !dto.EmployeeId.HasValue)
        {
            throw new BadRequestException("Employee is required for employee policy assignments.");
        }

        if (dto.Target == LeavePolicyAssignmentTarget.Department && !dto.DepartmentId.HasValue)
        {
            throw new BadRequestException("Department is required for department policy assignments.");
        }

        if (dto.EffectiveTo.HasValue && dto.EffectiveTo.Value.Date < dto.EffectiveFrom.Date)
        {
            throw new BadRequestException("Assignment end date must be on or after effective date.");
        }
    }
}

