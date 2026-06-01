namespace TaskManagement.Tasks.Services;

public interface ITaskNumberGenerator
{
    Task<string> GenerateAsync(CancellationToken cancellationToken);
}
