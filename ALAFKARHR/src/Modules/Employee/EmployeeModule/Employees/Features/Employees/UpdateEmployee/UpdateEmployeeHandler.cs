using Auth.Contracts.Features.UpdateUserName;
using EmployeeModule.Employees.Models;
using FluentValidation;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using Shared.SaveImages;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Transactions;

namespace EmployeeModule.Employees.Features.Employees.UpdateEmployee;

public record UpdateEmployeeCommand(EmployeeDto Employee) : ICommand<UpdateEmployeeResult>;
public record UpdateEmployeeResult(bool IsSuccess);

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(e => e.Employee.FirstName).NotEmpty().WithMessage("FirstName is required");
        RuleFor(e => e.Employee.MiddleName).NotEmpty().WithMessage("MiddleName is required");
        RuleFor(e => e.Employee.LastName).NotEmpty().WithMessage("LastName is required");
        RuleFor(e => e.Employee.Phone).NotEmpty().WithMessage("Phone is required");
        RuleFor(e => e.Employee.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(e => e.Employee.Code).NotEmpty().WithMessage("Code is required");
        RuleFor(e => e.Employee.HireDate).NotEmpty().WithMessage("HireDate is required");
        RuleFor(e => e.Employee.DateOfBirth).NotEmpty().WithMessage("DateOfBirth is required");
        RuleFor(e => e.Employee.EmployeeNo).NotEmpty().WithMessage("EmployeeNo is required");
        RuleFor(e => e.Employee.NationalId).NotEmpty().WithMessage("NationalId is required");
        RuleFor(e => e.Employee.PositionId).NotEmpty().WithMessage("Position is required");
        RuleFor(e => e.Employee.AttendanceType).IsInEnum().WithMessage("AttendanceType is invalid");
        RuleFor(e => e.Employee.AllowedRadiusMeters)
            .GreaterThan(0)
            .When(e => e.Employee.AllowedRadiusMeters.HasValue)
            .WithMessage("Allowed radius must be greater than 0");
    }
}

public class UpdateEmployeeHandler(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor, ISender sender)
    : ICommandHandler<UpdateEmployeeCommand, UpdateEmployeeResult>
{
    public async Task<UpdateEmployeeResult> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var position = await dbContext.Positions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.Employee.PositionId, cancellationToken);
        if (position is null)
            throw new NotFoundException($"Position not found: {request.Employee.PositionId}");

        var employee = await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == request.Employee.Id, cancellationToken);
        if (employee is null)
            throw new NotFoundException($"Employee not found: {request.Employee.Id}");
        await EmployeeModule.Employees.Features.Employees.EmployeeBranchScope.EnsureCanMutateAsync(sender, employee.CompanyId, employee.BranchId, cancellationToken);

        var userId = httpContextAccessor.HttpContext?
                        .User?
                        .FindFirst(ClaimTypes.NameIdentifier)?
                        .Value ??
                        throw new UnauthorizedAccessException("User is not authenticated");

        var oldCode = employee.Code;
        var newCode = request.Employee.Code.Trim();

        string finalImagePath = employee.PhotoUrl ?? string.Empty;
        var incomingImage = request.Employee.PhotoUrl;

        if (!string.IsNullOrWhiteSpace(incomingImage))
        {
            if (SaveImages.IsBase64Image(incomingImage))
            {
                string[] pathSegments = ["wwwroot", "Images", "Employees"];
                finalImagePath = SaveImages.SaveBase64Image($"{employee.Id}", pathSegments, request.Employee.PhotoUrl);
            }
        }

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        };

        using var transaction = new TransactionScope(
            TransactionScopeOption.Required,
            transactionOptions,
            TransactionScopeAsyncFlowOption.Enabled);

        if (!string.Equals(NormalizeUserName(oldCode), NormalizeUserName(newCode), StringComparison.Ordinal))
        {
            await sender.Send(new UpdateUserNameCommand(employee.CompanyId, oldCode, newCode), cancellationToken);
            employee.ChangeCode(newCode, userId);
        }

        employee.Update(
            request.Employee.EmployeeNo,
            request.Employee.FirstName,
            request.Employee.FirstNameEng,
            request.Employee.MiddleName,
            request.Employee.MiddleNameEng,
            request.Employee.LastName,
            request.Employee.LastNameEng,
            finalImagePath,
            request.Employee.Email,
            request.Employee.Phone,
            request.Employee.Address,
            request.Employee.MaritalStatus,
            request.Employee.EmploymentType,
            request.Employee.AttendanceType,
            request.Employee.AllowedRadiusMeters,
            request.Employee.Qualification,
            request.Employee.SpecializationId.Value,
            request.Employee.AcademicInstituteId.Value,
            request.Employee.GraduationYear,
            request.Employee.ManagerEmployeeId,
            request.Employee.Grade,
            request.Employee.WorkLocation,
            request.Employee.LinkedUserId,
            userId);

        employee.ChangePosition(request.Employee.PositionId!.Value, userId);

        await dbContext.SaveChangesAsync(cancellationToken);

        transaction.Complete();

        return new UpdateEmployeeResult(true);
    }

    private static string NormalizeUserName(string? userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return string.Empty;
        }

        var normalized = userName.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);
            if (category is UnicodeCategory.Control
                or UnicodeCategory.Format
                or UnicodeCategory.NonSpacingMark
                or UnicodeCategory.SpacingCombiningMark
                or UnicodeCategory.EnclosingMark)
            {
                continue;
            }

            builder.Append(character);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
