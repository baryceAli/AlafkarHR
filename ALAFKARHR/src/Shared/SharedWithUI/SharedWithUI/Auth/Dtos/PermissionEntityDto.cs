namespace SharedWithUI.Auth.Dtos;

public class PermissionEntityDto
{
    public string Entity { get; set; }
    public List<string> Actions { get; set; } = new();
}
