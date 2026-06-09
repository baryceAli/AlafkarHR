using AttendanceDomain.Attendance.Models;
using FluentValidation;

namespace AttendanceDomain.Attendance.Features.Shifts;

public record CreateShiftCommand(CreateShiftDto Shift) : ICommand<CreateShiftResult>;
public record CreateShiftResult(ShiftDto Shift);

public class CreateShiftCommandValidator : AbstractValidator<CreateShiftCommand>
{
    public CreateShiftCommandValidator()
    {
        RuleFor(x => x.Shift.CompanyId)
            .NotEmpty()
            .WithMessage("Company is required.");

        RuleFor(x => x.Shift.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Shift.LateAfterMinutes)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Shift.ProhibitCheckInAfterMinutes)
            .GreaterThanOrEqualTo(x => x.Shift.LateAfterMinutes)
            .WithMessage("Check-in prohibit time must be after or equal to late time.");

        RuleFor(x => x.Shift.GracePeriodMinutes)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Shift.BreakMinutes)
            .GreaterThanOrEqualTo(0);
    }
}

public class CreateShiftHandler(AttendanceDbContext dbContext)
    : ICommandHandler<CreateShiftCommand, CreateShiftResult>
{
    public async Task<CreateShiftResult> Handle(CreateShiftCommand request, CancellationToken cancellationToken)
    {
        var duplicateExists = await dbContext.Shifts.AnyAsync(
            x => x.CompanyId == request.Shift.CompanyId
                && x.Name.ToLower() == request.Shift.Name.Trim().ToLower(),
            cancellationToken);

        if (duplicateExists)
        {
            throw new BadRequestException("A shift with the same name already exists for this company.");
        }

        var shift = Shift.Create(
            Guid.NewGuid(),
            request.Shift.Name.Trim(),
            request.Shift.StartTime,
            request.Shift.EndTime,
            request.Shift.GracePeriodMinutes,
            request.Shift.LateAfterMinutes,
            request.Shift.ProhibitCheckInAfterMinutes,
            request.Shift.BreakMinutes,
            request.Shift.CompanyId);

        await dbContext.Shifts.AddAsync(shift, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateShiftResult(shift.Adapt<ShiftDto>());
    }
}
