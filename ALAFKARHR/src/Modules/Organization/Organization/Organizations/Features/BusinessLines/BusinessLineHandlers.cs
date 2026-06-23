namespace Organization.Organizations.Features.BusinessLines;

public record GetBusinessLinesQuery(bool IncludeInactive) : IQuery<GetBusinessLinesResult>;
public record GetBusinessLinesResult(List<BusinessLineDto> BusinessLines);
public record CreateBusinessLineCommand(BusinessLineDto BusinessLine) : ICommand<CreateBusinessLineResult>;
public record CreateBusinessLineResult(BusinessLineDto BusinessLine);
public record UpdateBusinessLineCommand(BusinessLineDto BusinessLine) : ICommand<UpdateBusinessLineResult>;
public record UpdateBusinessLineResult(bool IsSuccess);
public record SetBusinessLineStatusCommand(Guid Id, bool IsActive) : ICommand<SetBusinessLineStatusResult>;
public record SetBusinessLineStatusResult(bool IsSuccess);

public class BusinessLineValidator : AbstractValidator<BusinessLineDto>
{
    public BusinessLineValidator()
    {
        RuleFor(x => x.Key).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Icon).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.ActivationPolicy).IsInEnum();
    }
}

public class CreateBusinessLineCommandValidator : AbstractValidator<CreateBusinessLineCommand>
{
    public CreateBusinessLineCommandValidator()
    {
        RuleFor(x => x.BusinessLine).SetValidator(new BusinessLineValidator());
    }
}

public class UpdateBusinessLineCommandValidator : AbstractValidator<UpdateBusinessLineCommand>
{
    public UpdateBusinessLineCommandValidator()
    {
        RuleFor(x => x.BusinessLine.Id).NotEmpty();
        RuleFor(x => x.BusinessLine).SetValidator(new BusinessLineValidator());
    }
}

public class BusinessLineQueryHandler(OrganizationDbContext dbContext)
    : IQueryHandler<GetBusinessLinesQuery, GetBusinessLinesResult>
{
    public async Task<GetBusinessLinesResult> Handle(GetBusinessLinesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.BusinessLines.AsNoTracking();
        if (!request.IncludeInactive)
        {
            query = query.Where(x => x.IsActive);
        }

        var businessLines = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return new GetBusinessLinesResult(businessLines.Select(ToDto).ToList());
    }

    public static BusinessLineDto ToDto(BusinessLine businessLine) => new()
    {
        Id = businessLine.Id,
        Key = businessLine.Key,
        Name = businessLine.Name,
        NameAr = businessLine.NameAr,
        Icon = businessLine.Icon,
        Description = businessLine.Description,
        IsActive = businessLine.IsActive,
        DisplayOrder = businessLine.DisplayOrder,
        ActivationPolicy = businessLine.ActivationPolicy
    };

    public static LicensedBusinessLineDto ToLicensedDto(BusinessLine businessLine) => new()
    {
        BusinessLineId = businessLine.Id,
        Key = businessLine.Key,
        Name = businessLine.Name,
        NameAr = businessLine.NameAr,
        Icon = businessLine.Icon,
        ActivationPolicy = businessLine.ActivationPolicy
    };
}

public class BusinessLineCommandHandler(OrganizationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<CreateBusinessLineCommand, CreateBusinessLineResult>,
      ICommandHandler<UpdateBusinessLineCommand, UpdateBusinessLineResult>,
      ICommandHandler<SetBusinessLineStatusCommand, SetBusinessLineStatusResult>
{
    public async Task<CreateBusinessLineResult> Handle(CreateBusinessLineCommand request, CancellationToken cancellationToken)
    {
        var normalizedKey = BusinessLine.NormalizeKey(request.BusinessLine.Key);
        var keyExists = await dbContext.BusinessLines.AnyAsync(x => x.Key == normalizedKey, cancellationToken);
        if (keyExists)
            throw new InvalidOperationException("Business line key already exists");

        var businessLine = BusinessLine.Create(
            Guid.NewGuid(),
            request.BusinessLine.Key,
            request.BusinessLine.Name,
            request.BusinessLine.NameAr,
            request.BusinessLine.Icon,
            request.BusinessLine.Description,
            request.BusinessLine.DisplayOrder,
            request.BusinessLine.ActivationPolicy,
            GetUserId());

        await dbContext.BusinessLines.AddAsync(businessLine, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateBusinessLineResult(BusinessLineQueryHandler.ToDto(businessLine));
    }

    public async Task<UpdateBusinessLineResult> Handle(UpdateBusinessLineCommand request, CancellationToken cancellationToken)
    {
        var businessLine = await FindAsync(request.BusinessLine.Id, cancellationToken);
        var normalizedKey = BusinessLine.NormalizeKey(request.BusinessLine.Key);
        var keyExists = await dbContext.BusinessLines
            .AnyAsync(x => x.Id != request.BusinessLine.Id && x.Key == normalizedKey, cancellationToken);
        if (keyExists)
            throw new InvalidOperationException("Business line key already exists");

        businessLine.Update(
            request.BusinessLine.Key,
            request.BusinessLine.Name,
            request.BusinessLine.NameAr,
            request.BusinessLine.Icon,
            request.BusinessLine.Description,
            request.BusinessLine.DisplayOrder,
            request.BusinessLine.ActivationPolicy,
            GetUserId());

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateBusinessLineResult(true);
    }

    public async Task<SetBusinessLineStatusResult> Handle(SetBusinessLineStatusCommand request, CancellationToken cancellationToken)
    {
        var businessLine = await FindAsync(request.Id, cancellationToken);
        businessLine.SetActive(request.IsActive, GetUserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SetBusinessLineStatusResult(true);
    }

    private async Task<BusinessLine> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.BusinessLines.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
        ?? throw new NotFoundException($"Business line not found: {id}");

    private string GetUserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("User not authenticated");
}
