namespace Contracts.Contracts.Models;

public class ContractStatusHistory : Entity<Guid>
{
    private ContractStatusHistory()
    {
    }

    public Guid ContractId { get; private set; }
    public ContractStatus? OldStatus { get; private set; }
    public ContractStatus NewStatus { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public string ChangedBy { get; private set; } = string.Empty;

    public static ContractStatusHistory Create(Guid contractId, ContractStatus? oldStatus, ContractStatus newStatus, string action, string? notes, string userId) =>
        new()
        {
            Id = Guid.NewGuid(),
            ContractId = contractId,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Action = action,
            Notes = notes,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = userId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

    public ContractStatusHistoryDto ToDto() => new()
    {
        Id = Id,
        ContractId = ContractId,
        OldStatus = OldStatus,
        NewStatus = NewStatus,
        Action = Action,
        Notes = Notes,
        ChangedAt = ChangedAt,
        ChangedBy = ChangedBy
    };
}
