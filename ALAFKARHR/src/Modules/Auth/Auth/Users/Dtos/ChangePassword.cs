namespace Auth.Users.Dtos;

public record ChangePassword(string UserName, string currentPassword, string newPassword);
