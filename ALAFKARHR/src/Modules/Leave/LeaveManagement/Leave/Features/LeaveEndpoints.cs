using EmployeeModule.Contracts.Employees.Features.GetEmployeeAttendanceProfile;
using LeaveManagement.Leave.Features.EmergencyLeaves;
using LeaveManagement.Leave.Features.LeaveCore;
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
public record UpsertLeaveTypeRequest(UpsertLeaveTypeDto LeaveType);
public record UpsertLeavePeriodRequest(UpsertLeavePeriodDto LeavePeriod);
public record UpsertLeavePolicyRequest(UpsertLeavePolicyDto LeavePolicy);
public record UpsertLeavePolicyAssignmentRequest(UpsertLeavePolicyAssignmentDto Assignment);
public record GenerateLeaveAllocationsRequest(GenerateLeaveAllocationsDto Request);
public record UpsertLeaveApplicationRequest(UpsertLeaveApplicationDto Application);
public record ReviewLeaveApplicationRequest(ReviewLeaveApplicationDto Review);
public record CreateLeaveLedgerAdjustmentRequest(CreateLeaveLedgerAdjustmentDto Adjustment);
public record CreateLeaveEncashmentRequest(CreateLeaveEncashmentDto Encashment);

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

        group.MapGet("/my-emergency-leaves", GetMyEmergencyLeaves)
            .WithName("GetMyLeaveEmergencyLeaveRequests")
            .Produces<GetEmergencyLeaveRequestsResult>(StatusCodes.Status200OK)
            .WithSummary("Get signed-in employee emergency leave requests")
            .RequireAuthorization(PermissionList.LeavePermissions.RequestEmergencyLeave);

        group.MapGet("/employee-emergency-leaves", GetEmployeeEmergencyLeaves)
            .WithName("GetEmployeeLeaveEmergencyLeaveRequests")
            .Produces<GetEmergencyLeaveRequestsResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get employee emergency leave requests for leave reporting/profile views")
            .RequireAuthorization(PermissionList.LeavePermissions.ViewLeaveReports);

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

        group.MapGet("/leave-types", GetLeaveTypes)
            .WithName("GetLeaveTypes")
            .Produces<GetLeaveTypesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.View);

        group.MapPost("/leave-types", UpsertLeaveType)
            .WithName("UpsertLeaveType")
            .Produces<UpsertLeaveTypeResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Create);

        group.MapPut("/leave-types/{id:guid}", UpsertLeaveType)
            .WithName("UpdateLeaveType")
            .Produces<UpsertLeaveTypeResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Edit);

        group.MapDelete("/leave-types/{id:guid}", DeleteLeaveType)
            .WithName("DeleteLeaveType")
            .Produces<DeleteLeaveTypeResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Delete);

        group.MapGet("/leave-periods", GetLeavePeriods)
            .WithName("GetLeavePeriods")
            .Produces<GetLeavePeriodsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.View);

        group.MapPost("/leave-periods", UpsertLeavePeriod)
            .WithName("UpsertLeavePeriod")
            .Produces<UpsertLeavePeriodResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Create);

        group.MapPut("/leave-periods/{id:guid}", UpsertLeavePeriod)
            .WithName("UpdateLeavePeriod")
            .Produces<UpsertLeavePeriodResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Edit);

        group.MapDelete("/leave-periods/{id:guid}", DeleteLeavePeriod)
            .WithName("DeleteLeavePeriod")
            .Produces<DeleteLeavePeriodResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Delete);

        group.MapGet("/leave-policies", GetLeavePolicies)
            .WithName("GetLeavePolicies")
            .Produces<GetLeavePoliciesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.View);

        group.MapPost("/leave-policies", UpsertLeavePolicy)
            .WithName("UpsertLeavePolicy")
            .Produces<UpsertLeavePolicyResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Create);

        group.MapPut("/leave-policies/{id:guid}", UpsertLeavePolicy)
            .WithName("UpdateLeavePolicy")
            .Produces<UpsertLeavePolicyResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Edit);

        group.MapDelete("/leave-policies/{id:guid}", DeleteLeavePolicy)
            .WithName("DeleteLeavePolicy")
            .Produces<DeleteLeavePolicyResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Delete);

        group.MapGet("/leave-policy-assignments", GetLeavePolicyAssignments)
            .WithName("GetLeavePolicyAssignments")
            .Produces<GetLeavePolicyAssignmentsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.View);

        group.MapPost("/leave-policy-assignments", UpsertLeavePolicyAssignment)
            .WithName("UpsertLeavePolicyAssignment")
            .Produces<UpsertLeavePolicyAssignmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Assign);

        group.MapPut("/leave-policy-assignments/{id:guid}", UpsertLeavePolicyAssignment)
            .WithName("UpdateLeavePolicyAssignment")
            .Produces<UpsertLeavePolicyAssignmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Assign);

        group.MapDelete("/leave-policy-assignments/{id:guid}", DeleteLeavePolicyAssignment)
            .WithName("DeleteLeavePolicyAssignment")
            .Produces<DeleteLeavePolicyAssignmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Assign);

        group.MapPost("/leave-allocations/generate", GenerateLeaveAllocations)
            .WithName("GenerateLeaveAllocations")
            .Produces<GenerateLeaveAllocationsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeavePolicyPermissions.Assign);

        group.MapGet("/leave-applications", GetLeaveApplications)
            .WithName("GetLeaveApplications")
            .Produces<GetLeaveApplicationsResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveApplicationPermissions.View);

        group.MapPost("/leave-applications", UpsertLeaveApplication)
            .WithName("UpsertLeaveApplication")
            .Produces<UpsertLeaveApplicationResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveApplicationPermissions.Create);

        group.MapPut("/leave-applications/{id:guid}", UpsertLeaveApplication)
            .WithName("UpdateLeaveApplication")
            .Produces<UpsertLeaveApplicationResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveApplicationPermissions.Edit);

        group.MapPost("/leave-applications/{id:guid}/submit", SubmitLeaveApplication)
            .WithName("SubmitLeaveApplication")
            .Produces<LeaveApplicationActionResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveApplicationPermissions.Edit);

        group.MapPost("/leave-applications/review", ReviewLeaveApplication)
            .WithName("ReviewLeaveApplication")
            .Produces<LeaveApplicationActionResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveApplicationPermissions.Approve);

        group.MapPost("/leave-applications/{id:guid}/cancel", CancelLeaveApplication)
            .WithName("CancelLeaveApplication")
            .Produces<LeaveApplicationActionResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveApplicationPermissions.Cancel);

        group.MapGet("/leave-ledger", GetLeaveLedgerEntries)
            .WithName("GetLeaveLedgerEntries")
            .Produces<GetLeaveLedgerEntriesResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveLedgerPermissions.View);

        group.MapPost("/leave-ledger/adjustments", CreateLeaveLedgerAdjustment)
            .WithName("CreateLeaveLedgerAdjustment")
            .Produces<CreateLeaveLedgerAdjustmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveLedgerPermissions.Adjust);

        group.MapPost("/leave-ledger/encashments", CreateLeaveEncashment)
            .WithName("CreateLeaveEncashment")
            .Produces<CreateLeaveEncashmentResult>(StatusCodes.Status200OK)
            .RequireAuthorization(PermissionList.LeaveLedgerPermissions.Encash);
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

    private static async Task<Ok<GetEmergencyLeaveRequestsResult>> GetMyEmergencyLeaves(
        [FromQuery] Guid companyId,
        [FromQuery] AttendanceExceptionStatus? status,
        [AsParameters] PaginationRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        var employeeId = await ResolveSignedInEmployeeIdAsync(user, sender);
        var employee = await sender.Send(new GetEmployeeAttendanceProfileQuery(employeeId));
        var result = await sender.Send(new GetEmployeeEmergencyLeaveRequestsQuery(employee.CompanyId, employee.EmployeeId, status, request));
        return TypedResults.Ok(result);
    }

    private static async Task<Ok<GetEmergencyLeaveRequestsResult>> GetEmployeeEmergencyLeaves(
        [FromQuery] Guid companyId,
        [FromQuery] Guid employeeId,
        [FromQuery] AttendanceExceptionStatus? status,
        [AsParameters] PaginationRequest request,
        ISender sender)
    {
        if (companyId == Guid.Empty || employeeId == Guid.Empty)
        {
            throw new BadRequestException("Company and employee are required.");
        }

        var result = await sender.Send(new GetEmployeeEmergencyLeaveRequestsQuery(companyId, employeeId, status, request));
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

    private static async Task<Ok<GetLeaveTypesResult>> GetLeaveTypes([FromQuery] Guid companyId, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetLeaveTypesQuery(companyId)));

    private static async Task<Ok<UpsertLeaveTypeResult>> UpsertLeaveType(
        Guid? id,
        [FromBody] UpsertLeaveTypeRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        request.LeaveType.Id = id ?? request.LeaveType.Id;
        return TypedResults.Ok(await sender.Send(new UpsertLeaveTypeCommand(request.LeaveType, UserId(user))));
    }

    private static async Task<Ok<DeleteLeaveTypeResult>> DeleteLeaveType(Guid id, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new DeleteLeaveTypeCommand(id, UserId(user))));

    private static async Task<Ok<GetLeavePeriodsResult>> GetLeavePeriods([FromQuery] Guid companyId, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetLeavePeriodsQuery(companyId)));

    private static async Task<Ok<UpsertLeavePeriodResult>> UpsertLeavePeriod(
        Guid? id,
        [FromBody] UpsertLeavePeriodRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        request.LeavePeriod.Id = id ?? request.LeavePeriod.Id;
        return TypedResults.Ok(await sender.Send(new UpsertLeavePeriodCommand(request.LeavePeriod, UserId(user))));
    }

    private static async Task<Ok<DeleteLeavePeriodResult>> DeleteLeavePeriod(Guid id, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new DeleteLeavePeriodCommand(id, UserId(user))));

    private static async Task<Ok<GetLeavePoliciesResult>> GetLeavePolicies([FromQuery] Guid companyId, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetLeavePoliciesQuery(companyId)));

    private static async Task<Ok<UpsertLeavePolicyResult>> UpsertLeavePolicy(
        Guid? id,
        [FromBody] UpsertLeavePolicyRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        request.LeavePolicy.Id = id ?? request.LeavePolicy.Id;
        return TypedResults.Ok(await sender.Send(new UpsertLeavePolicyCommand(request.LeavePolicy, UserId(user))));
    }

    private static async Task<Ok<DeleteLeavePolicyResult>> DeleteLeavePolicy(Guid id, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new DeleteLeavePolicyCommand(id, UserId(user))));

    private static async Task<Ok<GetLeavePolicyAssignmentsResult>> GetLeavePolicyAssignments([FromQuery] Guid companyId, ISender sender)
        => TypedResults.Ok(await sender.Send(new GetLeavePolicyAssignmentsQuery(companyId)));

    private static async Task<Ok<UpsertLeavePolicyAssignmentResult>> UpsertLeavePolicyAssignment(
        Guid? id,
        [FromBody] UpsertLeavePolicyAssignmentRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        request.Assignment.Id = id ?? request.Assignment.Id;
        return TypedResults.Ok(await sender.Send(new UpsertLeavePolicyAssignmentCommand(request.Assignment, UserId(user))));
    }

    private static async Task<Ok<DeleteLeavePolicyAssignmentResult>> DeleteLeavePolicyAssignment(Guid id, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new DeleteLeavePolicyAssignmentCommand(id, UserId(user))));

    private static async Task<Ok<GenerateLeaveAllocationsResult>> GenerateLeaveAllocations(
        [FromBody] GenerateLeaveAllocationsRequest request,
        ClaimsPrincipal user,
        ISender sender)
        => TypedResults.Ok(await sender.Send(new GenerateLeaveAllocationsCommand(request.Request, UserId(user))));

    private static async Task<Ok<GetLeaveApplicationsResult>> GetLeaveApplications(
        [FromQuery] Guid companyId,
        [FromQuery] Guid? employeeId,
        [FromQuery] LeaveApplicationStatus? status,
        ISender sender)
        => TypedResults.Ok(await sender.Send(new GetLeaveApplicationsQuery(companyId, employeeId, status)));

    private static async Task<Ok<UpsertLeaveApplicationResult>> UpsertLeaveApplication(
        Guid? id,
        [FromBody] UpsertLeaveApplicationRequest request,
        ClaimsPrincipal user,
        ISender sender)
    {
        request.Application.Id = id ?? request.Application.Id;
        return TypedResults.Ok(await sender.Send(new UpsertLeaveApplicationCommand(request.Application, UserId(user))));
    }

    private static async Task<Ok<LeaveApplicationActionResult>> SubmitLeaveApplication(Guid id, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new SubmitLeaveApplicationCommand(id, UserId(user))));

    private static async Task<Ok<LeaveApplicationActionResult>> ReviewLeaveApplication(
        [FromBody] ReviewLeaveApplicationRequest request,
        ClaimsPrincipal user,
        ISender sender)
        => TypedResults.Ok(await sender.Send(new ReviewLeaveApplicationCommand(
            request.Review,
            UserId(user) ?? throw new UnauthorizedAccessException("User is not authenticated"))));

    private static async Task<Ok<LeaveApplicationActionResult>> CancelLeaveApplication(Guid id, ClaimsPrincipal user, ISender sender)
        => TypedResults.Ok(await sender.Send(new CancelLeaveApplicationCommand(id, UserId(user))));

    private static async Task<Ok<GetLeaveLedgerEntriesResult>> GetLeaveLedgerEntries(
        [FromQuery] Guid companyId,
        [FromQuery] Guid? employeeId,
        [FromQuery] Guid? leaveTypeId,
        [FromQuery] Guid? leavePeriodId,
        ISender sender)
        => TypedResults.Ok(await sender.Send(new GetLeaveLedgerEntriesQuery(companyId, employeeId, leaveTypeId, leavePeriodId)));

    private static async Task<Ok<CreateLeaveLedgerAdjustmentResult>> CreateLeaveLedgerAdjustment(
        [FromBody] CreateLeaveLedgerAdjustmentRequest request,
        ClaimsPrincipal user,
        ISender sender)
        => TypedResults.Ok(await sender.Send(new CreateLeaveLedgerAdjustmentCommand(request.Adjustment, UserId(user))));

    private static async Task<Ok<CreateLeaveEncashmentResult>> CreateLeaveEncashment(
        [FromBody] CreateLeaveEncashmentRequest request,
        ClaimsPrincipal user,
        ISender sender)
        => TypedResults.Ok(await sender.Send(new CreateLeaveEncashmentCommand(request.Encashment, UserId(user))));

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

    private static string? UserId(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier);
}
