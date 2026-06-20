using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using LeaveManagement.Leave.Features.EmergencyLeaves;
using LeaveManagement.Leave.Features.LeaveBalances;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;
using Shared.Pagination;
using SharedWithUI.Permissions;
using System.Security.Claims;

namespace LeaveManagement.Leave.Features;

public record CreateEmergencyLeaveRequestRequest(CreateEmergencyLeaveRequestDto Request);
public record ReviewEmergencyLeaveRequestRequest(ReviewEmergencyLeaveRequestDto Review);
public record UpsertEmployeeLeaveBalanceRequest(UpsertEmployeeLeaveBalanceDto Balance);
public record GetLeaveReportRequest(LeaveReportFilterDto Filter);

public class LeaveEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/leave")
            .RequireAuthorization()
            .WithTags("Leave");

        group.MapGet("/emergency-leaves", GetEmergencyLeaves)
            .WithName("GetLeaveEmergencyLeaveRequests")
            .Produces<GetEmergencyLeaveRequestsResult>(StatusCodes.Status200OK)
            .WithSummary("Get emergency leave requests")
            .RequireAuthorization(PermissionList.LeavePermissions.ApproveEmergencyLeave);

        group.MapPost("/emergency-leaves", CreateEmergencyLeave)
            .WithName("CreateLeaveEmergencyLeaveRequest")
            .Produces<CreateEmergencyLeaveRequestResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create an emergency leave request")
            .RequireAuthorization(PermissionList.LeavePermissions.RequestEmergencyLeave);

        group.MapPost("/emergency-leaves/attachments", UploadEmergencyLeaveAttachment)
            .WithName("UploadLeaveEmergencyLeaveAttachment")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<UploadEmergencyLeaveAttachmentResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Upload an emergency leave attachment")
            .RequireAuthorization(PermissionList.LeavePermissions.RequestEmergencyLeave)
            .DisableAntiforgery();

        group.MapPost("/emergency-leaves/review", ReviewEmergencyLeave)
            .WithName("ReviewLeaveEmergencyLeaveRequest")
            .Produces<ReviewEmergencyLeaveRequestResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Approve or reject an emergency leave request")
            .RequireAuthorization(PermissionList.LeavePermissions.ApproveEmergencyLeave);

        group.MapGet("/leave-balances", GetEmployeeLeaveBalances)
            .WithName("GetLeaveEmployeeLeaveBalances")
            .Produces<GetEmployeeLeaveBalancesResult>(StatusCodes.Status200OK)
            .WithSummary("Get employee leave balances")
            .RequireAuthorization(PermissionList.LeavePermissions.ViewLeaveBalances);

        group.MapPost("/leave-balances", UpsertEmployeeLeaveBalance)
            .WithName("UpsertLeaveEmployeeLeaveBalance")
            .Produces<UpsertEmployeeLeaveBalanceResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create or update employee yearly leave balance")
            .RequireAuthorization(PermissionList.LeavePermissions.ManageLeaveBalances);

        group.MapPost("/leave-reports", GetLeaveReport)
            .WithName("GetLeaveReport")
            .Produces<GetLeaveReportResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get leave balance and usage report")
            .RequireAuthorization(PermissionList.LeavePermissions.ViewLeaveReports);
    }

    private static async Task<Ok<GetEmergencyLeaveRequestsResult>> GetEmergencyLeaves(
        [FromQuery] Guid companyId,
        [FromQuery] AttendanceExceptionStatus? status,
        [FromQuery] Guid? employeeId,
        [AsParameters] PaginationRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var reviewerEmployeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var result = await sender.Send(new GetEmergencyLeaveRequestsQuery(companyId, status, employeeId, reviewerEmployeeId, request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<CreateEmergencyLeaveRequestResult>> CreateEmergencyLeave(
        [FromBody] CreateEmergencyLeaveRequestRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var employeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(employeeId));
        request.Request.EmployeeId = employee.EmployeeId;
        request.Request.CompanyId = employee.CompanyId;

        var result = await sender.Send(new CreateEmergencyLeaveRequestCommand(request.Request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UploadEmergencyLeaveAttachmentResult>> UploadEmergencyLeaveAttachment(
        IFormFile file,
        ClaimsPrincipal user,
        ISender sender)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated");

        var result = await sender.Send(new UploadEmergencyLeaveAttachmentCommand(file, userId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<ReviewEmergencyLeaveRequestResult>> ReviewEmergencyLeave(
        [FromBody] ReviewEmergencyLeaveRequestRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var reviewedBy = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated");
        var reviewerEmployeeId = await ResolveSignedInEmployeeIdAsync(user, sender);

        var result = await sender.Send(new ReviewEmergencyLeaveRequestCommand(request.Review, reviewedBy, reviewerEmployeeId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetEmployeeLeaveBalancesResult>> GetEmployeeLeaveBalances(
        [FromQuery] Guid companyId,
        [FromQuery] int year,
        [FromQuery] Guid? employeeId,
        ISender sender)
    {
        var result = await sender.Send(new GetEmployeeLeaveBalancesQuery(companyId, year, employeeId));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<UpsertEmployeeLeaveBalanceResult>> UpsertEmployeeLeaveBalance(
        [FromBody] UpsertEmployeeLeaveBalanceRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var result = await sender.Send(new UpsertEmployeeLeaveBalanceCommand(
            request.Balance,
            user.FindFirstValue(ClaimTypes.NameIdentifier)));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetLeaveReportResult>> GetLeaveReport(
        [FromBody] GetLeaveReportRequest request,
        ISender sender)
    {
        var result = await sender.Send(new GetLeaveReportQuery(request.Filter));
        return TypedResults.Ok(result);
    }

    private static Guid? ResolveEmployeeIdClaim(ClaimsPrincipal user)
    {
        var value = FirstClaimValue(
            user,
            "employee_id",
            "employeeId",
            "EmployeeId");

        return Guid.TryParse(value, out var employeeId) ? employeeId : null;
    }

    private static async Task<Guid> ResolveSignedInEmployeeIdAsync(ClaimsPrincipal user, ISender sender)
    {
        var employeeId = ResolveEmployeeIdClaim(user);
        if (employeeId.HasValue)
        {
            return employeeId.Value;
        }

        var userName = FirstClaimValue(
            user,
            ClaimTypes.Name,
            "name",
            "unique_name",
            "preferred_username",
            "upn")
            ?? throw new UnauthorizedAccessException("The signed-in user does not have a username claim that can be matched to an employee code.");

        try
        {
            var employee = await sender.Send(new GetEmployeeAttendanceProfileByCodeQuery(userName));
            return employee.EmployeeId;
        }
        catch (NotFoundException ex)
        {
            throw new UnauthorizedAccessException("This operation requires a linked employee account.", ex);
        }
    }

    private static string? FirstClaimValue(ClaimsPrincipal user, params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var value = user.FindFirstValue(claimType);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}
