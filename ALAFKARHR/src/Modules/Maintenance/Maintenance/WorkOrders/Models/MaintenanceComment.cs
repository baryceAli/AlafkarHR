namespace Maintenance.WorkOrders.Models;

public class MaintenanceComment : Entity<Guid>
{
    public Guid WorkOrderId { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public Guid CreatedByUserId { get; private set; }

    private MaintenanceComment()
    {
    }

    public static MaintenanceComment Create(Guid workOrderId, string comment, Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new BadRequestException("Comment is required.");

        return new MaintenanceComment
        {
            Id = Guid.NewGuid(),
            WorkOrderId = workOrderId,
            Comment = comment.Trim(),
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdByUserId.ToString()
        };
    }
}
