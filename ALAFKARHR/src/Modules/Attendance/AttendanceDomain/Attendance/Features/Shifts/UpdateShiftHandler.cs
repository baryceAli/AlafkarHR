using FluentValidation;
using System.Security.Claims;

namespace AttendanceDomain.Attendance.Features.Shifts;

public record UpdateShiftCommand(ShiftDto Shift) : ICommand<UpdateShiftResult>;
public record UpdateShiftResult(ShiftDto Shift);

public class UpdateShiftCommandValidator : AbstractValidator<UpdateShiftCommand>
{
    public UpdateShiftCommandValidator()
    {
        RuleFor(x => x.Shift.Id)
            .NotEmpty()
            .WithMessage("Shift is required.");

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

        RuleFor(x => x.Shift.BreakEndTime)
            .GreaterThan(x => x.Shift.BreakStartTime)
            .When(x => x.Shift.BreakMode == AttendanceBreakMode.Strict
                && x.Shift.BreakStartTime.HasValue
                && x.Shift.BreakEndTime.HasValue)
            .WithMessage("Strict break end time must be after start time.");

        RuleFor(x => x.Shift)
            .Must(x => x.BreakMode != AttendanceBreakMode.Strict
                || x.BreakMinutes == 0
                || (x.BreakStartTime.HasValue && x.BreakEndTime.HasValue))
            .WithMessage("Strict break mode requires break start and end times.");
    }
}

public class UpdateShiftHandler(AttendanceDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<UpdateShiftCommand, UpdateShiftResult>
{
    public async Task<UpdateShiftResult> Handle(UpdateShiftCommand request, CancellationToken cancellationToken)
    {
        var shift = await dbContext.Shifts
            .FirstOrDefaultAsync(x => x.Id == request.Shift.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Shift", request.Shift.Id);

        if (shift.CompanyId != request.Shift.CompanyId)
        {
            throw new BadRequestException("Shift company cannot be changed.");
        }

        var name = request.Shift.Name.Trim();
        var duplicateExists = await dbContext.Shifts.AnyAsync(
            x => x.Id != request.Shift.Id
                && x.CompanyId == request.Shift.CompanyId
                && !x.IsDeleted
                && x.Name.ToLower() == name.ToLower(),
            cancellationToken);

        if (duplicateExists)
        {
            throw new BadRequestException("A shift with the same name already exists for this company.");
        }

        var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        shift.Update(
            name,
            request.Shift.StartTime,
            request.Shift.EndTime,
            request.Shift.GracePeriodMinutes,
            request.Shift.LateAfterMinutes,
            request.Shift.ProhibitCheckInAfterMinutes,
            request.Shift.BreakMinutes,
            request.Shift.BreakMode,
            request.Shift.BreakStartTime,
            request.Shift.BreakEndTime,
            request.Shift.IsBreakPaid,
            userId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateShiftResult(shift.Adapt<ShiftDto>());
    }
}
