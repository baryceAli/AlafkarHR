using EmployeeModule.Employees.Models;
using FluentValidation;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using SharedWithUI.HRCore.Dtos;
using System.Security.Claims;

namespace EmployeeModule.Employees.Features.HrCore;

public record GetEmployeeLifecycleEventsQuery(Guid EmployeeId) : IQuery<GetEmployeeLifecycleEventsResult>;
public record GetEmployeeLifecycleEventsResult(List<HrLifecycleEventDto> Events);
public record SaveHrLifecycleEventRequest(HrLifecycleEventDto Event);
public record SaveHrLifecycleEventResponse(HrLifecycleEventDto Event);
public record CreateHrLifecycleEventCommand(Guid EmployeeId, HrLifecycleEventDto Event) : ICommand<SaveHrLifecycleEventResponse>;
public record UpdateHrLifecycleEventCommand(Guid EmployeeId, Guid EventId, HrLifecycleEventDto Event) : ICommand<SaveHrLifecycleEventResponse>;
public record DeleteHrLifecycleEventCommand(Guid EmployeeId, Guid EventId) : ICommand<UpdateDeleteResponse>;
public record TransitionHrLifecycleEventCommand(Guid EmployeeId, Guid EventId, string Transition) : ICommand<SaveHrLifecycleEventResponse>;

public record GetEmployeeEmergencyContactsQuery(Guid EmployeeId) : IQuery<GetEmployeeEmergencyContactsResult>;
public record GetEmployeeEmergencyContactsResult(List<EmployeeEmergencyContactDto> Contacts);
public record SaveEmployeeEmergencyContactRequest(EmployeeEmergencyContactDto Contact);
public record SaveEmployeeEmergencyContactResponse(EmployeeEmergencyContactDto Contact);
public record CreateEmployeeEmergencyContactCommand(Guid EmployeeId, EmployeeEmergencyContactDto Contact) : ICommand<SaveEmployeeEmergencyContactResponse>;
public record UpdateEmployeeEmergencyContactCommand(Guid EmployeeId, Guid ContactId, EmployeeEmergencyContactDto Contact) : ICommand<SaveEmployeeEmergencyContactResponse>;
public record DeleteEmployeeEmergencyContactCommand(Guid EmployeeId, Guid ContactId) : ICommand<UpdateDeleteResponse>;

public record GetEmployeeDocumentLinksQuery(Guid EmployeeId) : IQuery<GetEmployeeDocumentLinksResult>;
public record GetEmployeeDocumentLinksResult(List<EmployeeDocumentLinkDto> Documents);
public record SaveEmployeeDocumentLinkRequest(EmployeeDocumentLinkDto Document);
public record SaveEmployeeDocumentLinkResponse(EmployeeDocumentLinkDto Document);
public record CreateEmployeeDocumentLinkCommand(Guid EmployeeId, EmployeeDocumentLinkDto Document) : ICommand<SaveEmployeeDocumentLinkResponse>;
public record UpdateEmployeeDocumentLinkCommand(Guid EmployeeId, Guid DocumentId, EmployeeDocumentLinkDto Document) : ICommand<SaveEmployeeDocumentLinkResponse>;
public record DeleteEmployeeDocumentLinkCommand(Guid EmployeeId, Guid DocumentId) : ICommand<UpdateDeleteResponse>;

public record GetEmployeeSkillsQuery(Guid EmployeeId) : IQuery<GetEmployeeSkillsResult>;
public record GetEmployeeSkillsResult(List<EmployeeSkillDto> Skills);
public record SaveEmployeeSkillRequest(EmployeeSkillDto Skill);
public record SaveEmployeeSkillResponse(EmployeeSkillDto Skill);
public record CreateEmployeeSkillCommand(Guid EmployeeId, EmployeeSkillDto Skill) : ICommand<SaveEmployeeSkillResponse>;
public record UpdateEmployeeSkillCommand(Guid EmployeeId, Guid SkillId, EmployeeSkillDto Skill) : ICommand<SaveEmployeeSkillResponse>;
public record DeleteEmployeeSkillCommand(Guid EmployeeId, Guid SkillId) : ICommand<UpdateDeleteResponse>;

public record GetEmployeeCertificationsQuery(Guid EmployeeId) : IQuery<GetEmployeeCertificationsResult>;
public record GetEmployeeCertificationsResult(List<EmployeeCertificationDto> Certifications);
public record SaveEmployeeCertificationRequest(EmployeeCertificationDto Certification);
public record SaveEmployeeCertificationResponse(EmployeeCertificationDto Certification);
public record CreateEmployeeCertificationCommand(Guid EmployeeId, EmployeeCertificationDto Certification) : ICommand<SaveEmployeeCertificationResponse>;
public record UpdateEmployeeCertificationCommand(Guid EmployeeId, Guid CertificationId, EmployeeCertificationDto Certification) : ICommand<SaveEmployeeCertificationResponse>;
public record DeleteEmployeeCertificationCommand(Guid EmployeeId, Guid CertificationId) : ICommand<UpdateDeleteResponse>;

public record UpdateDeleteResponse(bool IsSuccess);

public class HrLifecycleEventValidator : AbstractValidator<HrLifecycleEventDto>
{
    public HrLifecycleEventValidator()
    {
        RuleFor(x => x.EventType).IsInEnum();
        RuleFor(x => x.EffectiveDate).NotEmpty();
        RuleFor(x => x.ToBranchId)
            .NotEmpty()
            .When(x => x.EventType == HrLifecycleEventType.Transfer)
            .WithMessage("Transfer lifecycle events require a target branch.");
        RuleFor(x => x.ToPositionId)
            .NotEmpty()
            .When(x => x.EventType == HrLifecycleEventType.Promotion)
            .WithMessage("Promotion lifecycle events require a target position.");
        RuleFor(x => x.Reason)
            .NotEmpty()
            .When(x => x.EventType == HrLifecycleEventType.Separation)
            .WithMessage("Separation lifecycle events require a reason.");
    }
}

public class HrCoreEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var employeeBase = $"{Utils.URL_PATTERN}/{Utils.Employee_Endpoint}/{{employeeId:guid}}";

        app.MapGet($"{employeeBase}/Lifecycle", async (Guid employeeId, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeLifecycleEventsQuery(employeeId));
            return Results.Ok(result);
        })
            .WithName("GetEmployeeLifecycleEvents")
            .Produces<GetEmployeeLifecycleEventsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.View);

        app.MapPost($"{employeeBase}/Lifecycle", async (Guid employeeId, SaveHrLifecycleEventRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateHrLifecycleEventCommand(employeeId, request.Event));
            return Results.Created($"{employeeBase}/Lifecycle/{result.Event.Id}", result);
        })
            .WithName("CreateEmployeeLifecycleEvent")
            .Produces<SaveHrLifecycleEventResponse>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.Create);

        app.MapPut($"{employeeBase}/Lifecycle/{{eventId:guid}}", async (Guid employeeId, Guid eventId, SaveHrLifecycleEventRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateHrLifecycleEventCommand(employeeId, eventId, request.Event));
            return Results.Ok(result);
        })
            .WithName("UpdateEmployeeLifecycleEvent")
            .Produces<SaveHrLifecycleEventResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.Edit);

        app.MapPost($"{employeeBase}/Lifecycle/{{eventId:guid}}/{{transition}}", async (Guid employeeId, Guid eventId, string transition, ISender sender) =>
        {
            var result = await sender.Send(new TransitionHrLifecycleEventCommand(employeeId, eventId, transition));
            return Results.Ok(result);
        })
            .WithName("TransitionEmployeeLifecycleEvent")
            .Produces<SaveHrLifecycleEventResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.Approve);

        app.MapDelete($"{employeeBase}/Lifecycle/{{eventId:guid}}", async (Guid employeeId, Guid eventId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteHrLifecycleEventCommand(employeeId, eventId));
            return Results.Ok(result);
        })
            .WithName("DeleteEmployeeLifecycleEvent")
            .Produces<UpdateDeleteResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.Delete);

        app.MapGet($"{employeeBase}/EmergencyContacts", async (Guid employeeId, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeEmergencyContactsQuery(employeeId));
            return Results.Ok(result);
        })
            .WithName("GetEmployeeEmergencyContacts")
            .Produces<GetEmployeeEmergencyContactsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.View);

        app.MapPost($"{employeeBase}/EmergencyContacts", async (Guid employeeId, SaveEmployeeEmergencyContactRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateEmployeeEmergencyContactCommand(employeeId, request.Contact));
            return Results.Created($"{employeeBase}/EmergencyContacts/{result.Contact.Id}", result);
        })
            .WithName("CreateEmployeeEmergencyContact")
            .Produces<SaveEmployeeEmergencyContactResponse>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.Create);

        app.MapPut($"{employeeBase}/EmergencyContacts/{{contactId:guid}}", async (Guid employeeId, Guid contactId, SaveEmployeeEmergencyContactRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateEmployeeEmergencyContactCommand(employeeId, contactId, request.Contact));
            return Results.Ok(result);
        })
            .WithName("UpdateEmployeeEmergencyContact")
            .Produces<SaveEmployeeEmergencyContactResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.Edit);

        app.MapDelete($"{employeeBase}/EmergencyContacts/{{contactId:guid}}", async (Guid employeeId, Guid contactId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteEmployeeEmergencyContactCommand(employeeId, contactId));
            return Results.Ok(result);
        })
            .WithName("DeleteEmployeeEmergencyContact")
            .Produces<UpdateDeleteResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeLifecyclePermissions.Delete);

        app.MapGet($"{employeeBase}/Documents", async (Guid employeeId, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeDocumentLinksQuery(employeeId));
            return Results.Ok(result);
        })
            .WithName("GetEmployeeDocumentLinks")
            .Produces<GetEmployeeDocumentLinksResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeDocumentPermissions.View);

        app.MapPost($"{employeeBase}/Documents", async (Guid employeeId, SaveEmployeeDocumentLinkRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateEmployeeDocumentLinkCommand(employeeId, request.Document));
            return Results.Created($"{employeeBase}/Documents/{result.Document.Id}", result);
        })
            .WithName("CreateEmployeeDocumentLink")
            .Produces<SaveEmployeeDocumentLinkResponse>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.EmployeeDocumentPermissions.Create);

        app.MapPut($"{employeeBase}/Documents/{{documentId:guid}}", async (Guid employeeId, Guid documentId, SaveEmployeeDocumentLinkRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateEmployeeDocumentLinkCommand(employeeId, documentId, request.Document));
            return Results.Ok(result);
        })
            .WithName("UpdateEmployeeDocumentLink")
            .Produces<SaveEmployeeDocumentLinkResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeDocumentPermissions.Edit);

        app.MapDelete($"{employeeBase}/Documents/{{documentId:guid}}", async (Guid employeeId, Guid documentId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteEmployeeDocumentLinkCommand(employeeId, documentId));
            return Results.Ok(result);
        })
            .WithName("DeleteEmployeeDocumentLink")
            .Produces<UpdateDeleteResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeDocumentPermissions.Delete);

        app.MapGet($"{employeeBase}/Skills", async (Guid employeeId, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeSkillsQuery(employeeId));
            return Results.Ok(result);
        })
            .WithName("GetEmployeeSkills")
            .Produces<GetEmployeeSkillsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeSkillPermissions.View);

        app.MapPost($"{employeeBase}/Skills", async (Guid employeeId, SaveEmployeeSkillRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateEmployeeSkillCommand(employeeId, request.Skill));
            return Results.Created($"{employeeBase}/Skills/{result.Skill.Id}", result);
        })
            .WithName("CreateEmployeeSkill")
            .Produces<SaveEmployeeSkillResponse>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.EmployeeSkillPermissions.Create);

        app.MapPut($"{employeeBase}/Skills/{{skillId:guid}}", async (Guid employeeId, Guid skillId, SaveEmployeeSkillRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateEmployeeSkillCommand(employeeId, skillId, request.Skill));
            return Results.Ok(result);
        })
            .WithName("UpdateEmployeeSkill")
            .Produces<SaveEmployeeSkillResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeSkillPermissions.Edit);

        app.MapDelete($"{employeeBase}/Skills/{{skillId:guid}}", async (Guid employeeId, Guid skillId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteEmployeeSkillCommand(employeeId, skillId));
            return Results.Ok(result);
        })
            .WithName("DeleteEmployeeSkill")
            .Produces<UpdateDeleteResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeSkillPermissions.Delete);

        app.MapGet($"{employeeBase}/Certifications", async (Guid employeeId, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeCertificationsQuery(employeeId));
            return Results.Ok(result);
        })
            .WithName("GetEmployeeCertifications")
            .Produces<GetEmployeeCertificationsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeSkillPermissions.View);

        app.MapPost($"{employeeBase}/Certifications", async (Guid employeeId, SaveEmployeeCertificationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateEmployeeCertificationCommand(employeeId, request.Certification));
            return Results.Created($"{employeeBase}/Certifications/{result.Certification.Id}", result);
        })
            .WithName("CreateEmployeeCertification")
            .Produces<SaveEmployeeCertificationResponse>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.EmployeeSkillPermissions.Create);

        app.MapPut($"{employeeBase}/Certifications/{{certificationId:guid}}", async (Guid employeeId, Guid certificationId, SaveEmployeeCertificationRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateEmployeeCertificationCommand(employeeId, certificationId, request.Certification));
            return Results.Ok(result);
        })
            .WithName("UpdateEmployeeCertification")
            .Produces<SaveEmployeeCertificationResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeSkillPermissions.Edit);

        app.MapDelete($"{employeeBase}/Certifications/{{certificationId:guid}}", async (Guid employeeId, Guid certificationId, ISender sender) =>
        {
            var result = await sender.Send(new DeleteEmployeeCertificationCommand(employeeId, certificationId));
            return Results.Ok(result);
        })
            .WithName("DeleteEmployeeCertification")
            .Produces<UpdateDeleteResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.EmployeeSkillPermissions.Delete);
    }
}

public class HrCoreHandlers(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor) :
    IQueryHandler<GetEmployeeLifecycleEventsQuery, GetEmployeeLifecycleEventsResult>,
    ICommandHandler<CreateHrLifecycleEventCommand, SaveHrLifecycleEventResponse>,
    ICommandHandler<UpdateHrLifecycleEventCommand, SaveHrLifecycleEventResponse>,
    ICommandHandler<DeleteHrLifecycleEventCommand, UpdateDeleteResponse>,
    ICommandHandler<TransitionHrLifecycleEventCommand, SaveHrLifecycleEventResponse>,
    IQueryHandler<GetEmployeeEmergencyContactsQuery, GetEmployeeEmergencyContactsResult>,
    ICommandHandler<CreateEmployeeEmergencyContactCommand, SaveEmployeeEmergencyContactResponse>,
    ICommandHandler<UpdateEmployeeEmergencyContactCommand, SaveEmployeeEmergencyContactResponse>,
    ICommandHandler<DeleteEmployeeEmergencyContactCommand, UpdateDeleteResponse>,
    IQueryHandler<GetEmployeeDocumentLinksQuery, GetEmployeeDocumentLinksResult>,
    ICommandHandler<CreateEmployeeDocumentLinkCommand, SaveEmployeeDocumentLinkResponse>,
    ICommandHandler<UpdateEmployeeDocumentLinkCommand, SaveEmployeeDocumentLinkResponse>,
    ICommandHandler<DeleteEmployeeDocumentLinkCommand, UpdateDeleteResponse>,
    IQueryHandler<GetEmployeeSkillsQuery, GetEmployeeSkillsResult>,
    ICommandHandler<CreateEmployeeSkillCommand, SaveEmployeeSkillResponse>,
    ICommandHandler<UpdateEmployeeSkillCommand, SaveEmployeeSkillResponse>,
    ICommandHandler<DeleteEmployeeSkillCommand, UpdateDeleteResponse>,
    IQueryHandler<GetEmployeeCertificationsQuery, GetEmployeeCertificationsResult>,
    ICommandHandler<CreateEmployeeCertificationCommand, SaveEmployeeCertificationResponse>,
    ICommandHandler<UpdateEmployeeCertificationCommand, SaveEmployeeCertificationResponse>,
    ICommandHandler<DeleteEmployeeCertificationCommand, UpdateDeleteResponse>
{
    public async Task<GetEmployeeLifecycleEventsResult> Handle(GetEmployeeLifecycleEventsQuery request, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var events = await dbContext.HrLifecycleEvents
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && !x.IsDeleted)
            .OrderByDescending(x => x.EffectiveDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return new GetEmployeeLifecycleEventsResult(events.Select(x => ToDto(x, employee)).ToList());
    }

    public async Task<SaveHrLifecycleEventResponse> Handle(CreateHrLifecycleEventCommand request, CancellationToken cancellationToken)
    {
        var validator = new HrLifecycleEventValidator();
        await validator.ValidateAndThrowAsync(request.Event, cancellationToken);
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = HrLifecycleEvent.Create(request.Event, employee, UserId());
        await dbContext.HrLifecycleEvents.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveHrLifecycleEventResponse(ToDto(entity, employee));
    }

    public async Task<SaveHrLifecycleEventResponse> Handle(UpdateHrLifecycleEventCommand request, CancellationToken cancellationToken)
    {
        var validator = new HrLifecycleEventValidator();
        await validator.ValidateAndThrowAsync(request.Event, cancellationToken);
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = await GetLifecycleEventAsync(request.EmployeeId, request.EventId, cancellationToken);
        entity.Update(request.Event, employee, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveHrLifecycleEventResponse(ToDto(entity, employee));
    }

    public async Task<UpdateDeleteResponse> Handle(DeleteHrLifecycleEventCommand request, CancellationToken cancellationToken)
    {
        var entity = await GetLifecycleEventAsync(request.EmployeeId, request.EventId, cancellationToken);
        entity.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateDeleteResponse(true);
    }

    public async Task<SaveHrLifecycleEventResponse> Handle(TransitionHrLifecycleEventCommand request, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = await GetLifecycleEventAsync(request.EmployeeId, request.EventId, cancellationToken);
        var userId = UserId();

        switch (request.Transition.Trim().ToLowerInvariant())
        {
            case "submit":
                entity.Submit(userId);
                break;
            case "approve":
                entity.Approve(userId);
                break;
            case "complete":
                entity.Complete(employee, userId);
                break;
            case "cancel":
                entity.Cancel(userId);
                break;
            default:
                throw new BadRequestException("Unsupported lifecycle transition.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveHrLifecycleEventResponse(ToDto(entity, employee));
    }

    public async Task<GetEmployeeEmergencyContactsResult> Handle(GetEmployeeEmergencyContactsQuery request, CancellationToken cancellationToken)
    {
        await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var contacts = await dbContext.EmployeeEmergencyContacts
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && !x.IsDeleted)
            .OrderByDescending(x => x.IsPrimary)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return new GetEmployeeEmergencyContactsResult(contacts.Adapt<List<EmployeeEmergencyContactDto>>());
    }

    public async Task<SaveEmployeeEmergencyContactResponse> Handle(CreateEmployeeEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = EmployeeEmergencyContact.Create(request.Contact, employee, UserId());
        await ClearPrimaryContactsAsync(employee.Id, entity.IsPrimary, cancellationToken);
        await dbContext.EmployeeEmergencyContacts.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveEmployeeEmergencyContactResponse(entity.Adapt<EmployeeEmergencyContactDto>());
    }

    public async Task<SaveEmployeeEmergencyContactResponse> Handle(UpdateEmployeeEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = await dbContext.EmployeeEmergencyContacts.FirstOrDefaultAsync(x => x.Id == request.ContactId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Emergency contact not found: {request.ContactId}");
        entity.Update(request.Contact, UserId());
        await ClearPrimaryContactsAsync(request.EmployeeId, entity.IsPrimary, cancellationToken, entity.Id);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveEmployeeEmergencyContactResponse(entity.Adapt<EmployeeEmergencyContactDto>());
    }

    public async Task<UpdateDeleteResponse> Handle(DeleteEmployeeEmergencyContactCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.EmployeeEmergencyContacts.FirstOrDefaultAsync(x => x.Id == request.ContactId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Emergency contact not found: {request.ContactId}");
        entity.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateDeleteResponse(true);
    }

    public async Task<GetEmployeeDocumentLinksResult> Handle(GetEmployeeDocumentLinksQuery request, CancellationToken cancellationToken)
    {
        await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var documents = await dbContext.EmployeeDocumentLinks
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && !x.IsDeleted)
            .OrderBy(x => x.ExpiryDate ?? DateTime.MaxValue)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);
        return new GetEmployeeDocumentLinksResult(documents.Adapt<List<EmployeeDocumentLinkDto>>());
    }

    public async Task<SaveEmployeeDocumentLinkResponse> Handle(CreateEmployeeDocumentLinkCommand request, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = EmployeeDocumentLink.Create(request.Document, employee, UserId());
        await dbContext.EmployeeDocumentLinks.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveEmployeeDocumentLinkResponse(entity.Adapt<EmployeeDocumentLinkDto>());
    }

    public async Task<SaveEmployeeDocumentLinkResponse> Handle(UpdateEmployeeDocumentLinkCommand request, CancellationToken cancellationToken)
    {
        await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = await dbContext.EmployeeDocumentLinks.FirstOrDefaultAsync(x => x.Id == request.DocumentId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee document link not found: {request.DocumentId}");
        entity.Update(request.Document, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveEmployeeDocumentLinkResponse(entity.Adapt<EmployeeDocumentLinkDto>());
    }

    public async Task<UpdateDeleteResponse> Handle(DeleteEmployeeDocumentLinkCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.EmployeeDocumentLinks.FirstOrDefaultAsync(x => x.Id == request.DocumentId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee document link not found: {request.DocumentId}");
        entity.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateDeleteResponse(true);
    }

    public async Task<GetEmployeeSkillsResult> Handle(GetEmployeeSkillsQuery request, CancellationToken cancellationToken)
    {
        await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var skills = await dbContext.EmployeeSkills
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && !x.IsDeleted)
            .OrderBy(x => x.Category)
            .ThenBy(x => x.SkillName)
            .ToListAsync(cancellationToken);
        return new GetEmployeeSkillsResult(skills.Adapt<List<EmployeeSkillDto>>());
    }

    public async Task<SaveEmployeeSkillResponse> Handle(CreateEmployeeSkillCommand request, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = EmployeeSkill.Create(request.Skill, employee, UserId());
        await dbContext.EmployeeSkills.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveEmployeeSkillResponse(entity.Adapt<EmployeeSkillDto>());
    }

    public async Task<SaveEmployeeSkillResponse> Handle(UpdateEmployeeSkillCommand request, CancellationToken cancellationToken)
    {
        await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = await dbContext.EmployeeSkills.FirstOrDefaultAsync(x => x.Id == request.SkillId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee skill not found: {request.SkillId}");
        entity.Update(request.Skill, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveEmployeeSkillResponse(entity.Adapt<EmployeeSkillDto>());
    }

    public async Task<UpdateDeleteResponse> Handle(DeleteEmployeeSkillCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.EmployeeSkills.FirstOrDefaultAsync(x => x.Id == request.SkillId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee skill not found: {request.SkillId}");
        entity.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateDeleteResponse(true);
    }

    public async Task<GetEmployeeCertificationsResult> Handle(GetEmployeeCertificationsQuery request, CancellationToken cancellationToken)
    {
        await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var certifications = await dbContext.EmployeeCertifications
            .AsNoTracking()
            .Where(x => x.EmployeeId == request.EmployeeId && !x.IsDeleted)
            .OrderBy(x => x.ExpiresAt ?? DateTime.MaxValue)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
        return new GetEmployeeCertificationsResult(certifications.Adapt<List<EmployeeCertificationDto>>());
    }

    public async Task<SaveEmployeeCertificationResponse> Handle(CreateEmployeeCertificationCommand request, CancellationToken cancellationToken)
    {
        var employee = await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = EmployeeCertification.Create(request.Certification, employee, UserId());
        await dbContext.EmployeeCertifications.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveEmployeeCertificationResponse(entity.Adapt<EmployeeCertificationDto>());
    }

    public async Task<SaveEmployeeCertificationResponse> Handle(UpdateEmployeeCertificationCommand request, CancellationToken cancellationToken)
    {
        await GetEmployeeAsync(request.EmployeeId, cancellationToken);
        var entity = await dbContext.EmployeeCertifications.FirstOrDefaultAsync(x => x.Id == request.CertificationId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee certification not found: {request.CertificationId}");
        entity.Update(request.Certification, UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new SaveEmployeeCertificationResponse(entity.Adapt<EmployeeCertificationDto>());
    }

    public async Task<UpdateDeleteResponse> Handle(DeleteEmployeeCertificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.EmployeeCertifications.FirstOrDefaultAsync(x => x.Id == request.CertificationId && x.EmployeeId == request.EmployeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee certification not found: {request.CertificationId}");
        entity.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateDeleteResponse(true);
    }

    private async Task<Employee> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken)
        => await dbContext.Employees.FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee not found: {employeeId}");

    private async Task<HrLifecycleEvent> GetLifecycleEventAsync(Guid employeeId, Guid eventId, CancellationToken cancellationToken)
        => await dbContext.HrLifecycleEvents.FirstOrDefaultAsync(x => x.Id == eventId && x.EmployeeId == employeeId && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Lifecycle event not found: {eventId}");

    private async Task ClearPrimaryContactsAsync(Guid employeeId, bool isPrimary, CancellationToken cancellationToken, Guid? exceptContactId = null)
    {
        if (!isPrimary)
            return;

        var contacts = await dbContext.EmployeeEmergencyContacts
            .Where(x => x.EmployeeId == employeeId && x.IsPrimary && !x.IsDeleted && (!exceptContactId.HasValue || x.Id != exceptContactId.Value))
            .ToListAsync(cancellationToken);

        foreach (var contact in contacts)
        {
            contact.Update(new EmployeeEmergencyContactDto
            {
                Name = contact.Name,
                Relationship = contact.Relationship,
                Phone = contact.Phone,
                Email = contact.Email,
                IsPrimary = false
            }, UserId());
        }
    }

    private string UserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("User is not authenticated");

    private static HrLifecycleEventDto ToDto(HrLifecycleEvent entity, Employee employee)
    {
        var dto = entity.Adapt<HrLifecycleEventDto>();
        dto.EmployeeName = employee.FullName;
        dto.EmployeeNameEng = employee.FullNameEng;
        dto.EmployeeNo = employee.EmployeeNo;
        return dto;
    }
}
