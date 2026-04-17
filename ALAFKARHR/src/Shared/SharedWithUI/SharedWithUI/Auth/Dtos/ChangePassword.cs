namespace SharedWithUI.Auth.Dtos;

public record ChangePassword(string UserName, string currentPassword, string newPassword);
