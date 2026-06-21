using EmployeeModule.Employees.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.CQRS;
using Shared.Exceptions;
using Shared.Pagination;
using SharedWithUI.Employees.Enums;
using System.Security.Claims;

namespace EmployeeModule.Employees.Features.Teams;

public record GetEmployeeTeamsQuery(Guid CompanyId, EmployeeTeamCategory? Category, bool? IsActive, PaginationRequest PaginationRequest) : IQuery<GetEmployeeTeamsResult>;
public record GetEmployeeTeamsResult(PaginatedResult<EmployeeTeamDto> TeamList);

public record GetEmployeeTeamByIdQuery(Guid Id) : IQuery<GetEmployeeTeamByIdResult>;
public record GetEmployeeTeamByIdResult(EmployeeTeamDto Team);

public record SaveEmployeeTeamRequest(EmployeeTeamDto Team);
public record CreateEmployeeTeamCommand(EmployeeTeamDto Team) : ICommand<CreateEmployeeTeamResult>;
public record CreateEmployeeTeamResult(EmployeeTeamDto CreatedTeam);

public record UpdateEmployeeTeamCommand(Guid Id, EmployeeTeamDto Team) : ICommand<UpdateEmployeeTeamResult>;
public record UpdateEmployeeTeamResult(bool IsSuccess);

public record DeleteEmployeeTeamCommand(Guid Id) : ICommand<DeleteEmployeeTeamResult>;
public record DeleteEmployeeTeamResult(bool IsSuccess);

public class EmployeeTeamListQuery
{
    public Guid CompanyId { get; set; }
    public EmployeeTeamCategory? Category { get; set; }
    public bool? IsActive { get; set; }
}

public record GetEmployeeTeamsResponse(PaginatedResult<EmployeeTeamDto> TeamList);
public record GetEmployeeTeamByIdResponse(EmployeeTeamDto Team);
public record CreateEmployeeTeamResponse(EmployeeTeamDto CreatedTeam);
public record UpdateEmployeeTeamResponse(bool IsSuccess);
public record DeleteEmployeeTeamResponse(bool IsSuccess);

public class EmployeeTeamEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet($"{Utils.URL_PATTERN}/Teams", async ([AsParameters] EmployeeTeamListQuery query, [AsParameters] PaginationRequest pagination, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeTeamsQuery(query.CompanyId, query.Category, query.IsActive, pagination));
            return Results.Ok(result.Adapt<GetEmployeeTeamsResponse>());
        })
            .WithName("GetEmployeeTeams")
            .Produces<GetEmployeeTeamsResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.TeamPermissions.View);

        app.MapGet($"{Utils.URL_PATTERN}/Teams/project", async ([FromQuery] Guid companyId, ISender sender) =>
        {
            var pagination = new PaginationRequest(0, 1000, string.Empty);
            var result = await sender.Send(new GetEmployeeTeamsQuery(companyId, EmployeeTeamCategory.Projects, true, pagination));
            return Results.Ok(result.Adapt<GetEmployeeTeamsResponse>());
        })
            .WithName("GetProjectEmployeeTeams")
            .Produces<GetEmployeeTeamsResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.ProjectManagementPermissions.Budget);

        app.MapGet($"{Utils.URL_PATTERN}/Teams/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeTeamByIdQuery(id));
            return Results.Ok(result.Adapt<GetEmployeeTeamByIdResponse>());
        })
            .WithName("GetEmployeeTeamById")
            .Produces<GetEmployeeTeamByIdResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.TeamPermissions.View);

        app.MapPost($"{Utils.URL_PATTERN}/Teams", async (SaveEmployeeTeamRequest request, ISender sender) =>
        {
            var result = await sender.Send(new CreateEmployeeTeamCommand(request.Team));
            return Results.Created($"{Utils.URL_PATTERN}/Teams/{result.CreatedTeam.Id}", result.Adapt<CreateEmployeeTeamResponse>());
        })
            .WithName("CreateEmployeeTeam")
            .Produces<CreateEmployeeTeamResponse>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.TeamPermissions.Create);

        app.MapPost($"{Utils.URL_PATTERN}/Teams/project", async (SaveEmployeeTeamRequest request, ISender sender) =>
        {
            request.Team.Category = EmployeeTeamCategory.Projects;
            request.Team.IsActive = true;
            var result = await sender.Send(new CreateEmployeeTeamCommand(request.Team));
            return Results.Created($"{Utils.URL_PATTERN}/Teams/{result.CreatedTeam.Id}", result.Adapt<CreateEmployeeTeamResponse>());
        })
            .WithName("CreateProjectEmployeeTeam")
            .Produces<CreateEmployeeTeamResponse>(StatusCodes.Status201Created)
            .RequireAuthorization(PermissionList.ProjectManagementPermissions.Budget);

        app.MapPut($"{Utils.URL_PATTERN}/Teams/{{id:guid}}", async (Guid id, SaveEmployeeTeamRequest request, ISender sender) =>
        {
            var result = await sender.Send(new UpdateEmployeeTeamCommand(id, request.Team));
            return Results.Ok(result.Adapt<UpdateEmployeeTeamResponse>());
        })
            .WithName("UpdateEmployeeTeam")
            .Produces<UpdateEmployeeTeamResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.TeamPermissions.Edit);

        app.MapDelete($"{Utils.URL_PATTERN}/Teams/{{id:guid}}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteEmployeeTeamCommand(id));
            return Results.Ok(result.Adapt<DeleteEmployeeTeamResponse>());
        })
            .WithName("DeleteEmployeeTeam")
            .Produces<DeleteEmployeeTeamResponse>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.TeamPermissions.Delete);
    }
}

public class EmployeeTeamHandlers(EmployeeDbContext dbContext, IHttpContextAccessor httpContextAccessor) :
    IQueryHandler<GetEmployeeTeamsQuery, GetEmployeeTeamsResult>,
    IQueryHandler<GetEmployeeTeamByIdQuery, GetEmployeeTeamByIdResult>,
    ICommandHandler<CreateEmployeeTeamCommand, CreateEmployeeTeamResult>,
    ICommandHandler<UpdateEmployeeTeamCommand, UpdateEmployeeTeamResult>,
    ICommandHandler<DeleteEmployeeTeamCommand, DeleteEmployeeTeamResult>
{
    public async Task<GetEmployeeTeamsResult> Handle(GetEmployeeTeamsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.EmployeeTeams
            .AsNoTracking()
            .Include(x => x.Members)
            .Where(x => !x.IsDeleted && x.CompanyId == request.CompanyId);

        if (request.Category.HasValue)
            query = query.Where(x => x.Category == request.Category.Value);

        if (request.IsActive.HasValue)
            query = query.Where(x => x.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.PaginationRequest.SearchText))
        {
            var search = request.PaginationRequest.SearchText.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(search) || (x.NameEng != null && x.NameEng.ToLower().Contains(search)));
        }

        var count = await query.LongCountAsync(cancellationToken);
        var teams = await query
            .OrderBy(x => x.Name)
            .Skip(request.PaginationRequest.PageIndex * request.PaginationRequest.PageSize)
            .Take(request.PaginationRequest.PageSize)
            .ToListAsync(cancellationToken);

        return new GetEmployeeTeamsResult(new PaginatedResult<EmployeeTeamDto>(
            request.PaginationRequest.PageIndex,
            request.PaginationRequest.PageSize,
            count,
            teams.Select(ToDto).ToList()));
    }

    public async Task<GetEmployeeTeamByIdResult> Handle(GetEmployeeTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var team = await dbContext.EmployeeTeams
            .AsNoTracking()
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee team not found: {request.Id}");

        return new GetEmployeeTeamByIdResult(ToDto(team));
    }

    public async Task<CreateEmployeeTeamResult> Handle(CreateEmployeeTeamCommand request, CancellationToken cancellationToken)
    {
        await ValidateTeamAsync(request.Team, cancellationToken);
        var userId = UserId();
        var team = EmployeeTeam.Create(request.Team, userId);
        team.ReplaceMembers(BuildMembers(team.Id, request.Team.Members, userId));

        await dbContext.EmployeeTeams.AddAsync(team, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateEmployeeTeamResult(ToDto(team));
    }

    public async Task<UpdateEmployeeTeamResult> Handle(UpdateEmployeeTeamCommand request, CancellationToken cancellationToken)
    {
        request.Team.Id = request.Id;
        await ValidateTeamAsync(request.Team, cancellationToken);

        var team = await dbContext.EmployeeTeams
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee team not found: {request.Id}");

        var userId = UserId();
        team.Update(request.Team, userId);
        team.ReplaceMembers(BuildMembers(team.Id, request.Team.Members, userId));

        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateEmployeeTeamResult(true);
    }

    public async Task<DeleteEmployeeTeamResult> Handle(DeleteEmployeeTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await dbContext.EmployeeTeams.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException($"Employee team not found: {request.Id}");

        team.Remove(UserId());
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteEmployeeTeamResult(true);
    }

    private async Task ValidateTeamAsync(EmployeeTeamDto team, CancellationToken cancellationToken)
    {
        if (team.CompanyId == Guid.Empty)
            throw new BadRequestException("Company is required.");

        if (string.IsNullOrWhiteSpace(team.Name))
            throw new BadRequestException("Team name is required.");

        if (team.Members.Count == 0)
            throw new BadRequestException("At least one employee member is required.");

        var requestedEmployeeIds = team.Members.Select(x => x.EmployeeId).Where(x => x != Guid.Empty).Distinct().ToList();
        if (requestedEmployeeIds.Count != team.Members.Count)
            throw new BadRequestException("Employee members are invalid.");

        var employees = await dbContext.Employees
            .AsNoTracking()
            .Where(x => requestedEmployeeIds.Contains(x.Id) && !x.IsDeleted && x.CompanyId == team.CompanyId)
            .Select(x => new { x.Id, x.EmployeeNo, x.FirstName, x.MiddleName, x.LastName, x.FirstNameEng, x.MiddleNameEng, x.LastNameEng })
            .ToListAsync(cancellationToken);

        if (employees.Count != requestedEmployeeIds.Count)
            throw new BadRequestException("All team members must be valid employees in the selected company.");

        foreach (var member in team.Members)
        {
            var employee = employees.First(x => x.Id == member.EmployeeId);
            member.EmployeeName = $"{employee.FirstName} {employee.MiddleName} {employee.LastName}".Trim();
            member.EmployeeNameEng = $"{employee.FirstNameEng} {employee.MiddleNameEng} {employee.LastNameEng}".Trim();
            member.EmployeeNo = employee.EmployeeNo;
        }
    }

    private static List<EmployeeTeamMember> BuildMembers(Guid teamId, IEnumerable<EmployeeTeamMemberDto> members, string userId)
        => members.Select(member => EmployeeTeamMember.Create(teamId, member, userId)).ToList();

    private string UserId() =>
        httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? httpContextAccessor.HttpContext?.User?.Identity?.Name
        ?? "system";

    private static EmployeeTeamDto ToDto(EmployeeTeam team) => new()
    {
        Id = team.Id,
        CompanyId = team.CompanyId,
        Name = team.Name,
        NameEng = team.NameEng,
        Category = team.Category,
        IsActive = team.IsActive,
        Notes = team.Notes,
        CreatedForProjectId = team.CreatedForProjectId,
        Members = team.Members.Select(member => new EmployeeTeamMemberDto
        {
            Id = member.Id,
            TeamId = member.TeamId,
            EmployeeId = member.EmployeeId,
            EmployeeName = member.EmployeeName,
            EmployeeNameEng = member.EmployeeNameEng,
            EmployeeNo = member.EmployeeNo
        }).ToList()
    };
}
