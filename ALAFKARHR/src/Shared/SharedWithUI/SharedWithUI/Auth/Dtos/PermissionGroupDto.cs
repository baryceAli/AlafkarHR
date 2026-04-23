namespace SharedWithUI.Auth.Dtos;

public class PermissionGroupDto
{
    public string Group { get; set; }
    public List<PermissionEntityDto> Entities { get; set; } = new();
}