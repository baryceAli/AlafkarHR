namespace Maintenance.WorkOrders.Models;

public class MaintenanceHistory : Entity<Guid>
{
    public Guid WorkOrderId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string? Details { get; private set; }
    public Guid PerformedByUserId { get; private set; }

    private MaintenanceHistory()
    {
    }

    public static MaintenanceHistory Create(Guid workOrderId, string action, string? details, Guid performedByUserId)
    {
        return new MaintenanceHistory
        {
            Id = Guid.NewGuid(),
            WorkOrderId = workOrderId,
            Action = action.Trim(),
            Details = details?.Trim(),
            PerformedByUserId = performedByUserId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = performedByUserId.ToString()
        };
    }
}
