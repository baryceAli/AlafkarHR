namespace SharedWithUI.Auth.Dtos;

public record RegisterDto
    (
        Guid Id,
        string UserName,
        string Email,
        string PhoneNumber,
        string Password,
        UserType UserType,
        Guid CompanyId,
        Guid? EmployeeId
    );



