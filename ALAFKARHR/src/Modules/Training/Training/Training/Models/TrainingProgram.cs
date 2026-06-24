using Shared.DDD;

namespace Training.Training.Models;

public class TrainingProgram : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Category { get; private set; }
    public string? Provider { get; private set; }
    public string? Objective { get; private set; }
    public string? Description { get; private set; }

    private TrainingProgram() { }

    public static TrainingProgram Create(Guid id, Guid companyId, string name, string? category, string? provider, string? objective, string? description, string userId)
    {
        var item = new TrainingProgram { Id = id, CompanyId = companyId };
        item.Update(name, category, provider, objective, description, userId);
        item.CreatedAt = DateTime.UtcNow;
        item.CreatedBy = userId;
        return item;
    }

    public void Update(string name, string? category, string? provider, string? objective, string? description, string userId)
    {
        Name = string.IsNullOrWhiteSpace(name) ? throw new InvalidOperationException("Training program name is required.") : name.Trim();
        Category = Normalize(category);
        Provider = Normalize(provider);
        Objective = Normalize(objective);
        Description = Normalize(description);
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
