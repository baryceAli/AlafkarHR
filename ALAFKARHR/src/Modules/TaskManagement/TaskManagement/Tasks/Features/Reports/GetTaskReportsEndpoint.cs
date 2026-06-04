namespace TaskManagement.Tasks.Features.Reports;

public class GetTaskReportsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/taskmanagement/reports/summary", async (
            Guid? departmentId, string? userId, TaskWorkflowStatus? status, TaskPriority? priority, DateTime? fromDate, DateTime? toDate, ISender sender) =>
        {
            var result = await sender.Send(new GetTaskSummaryReportQuery(new TaskFilterDto
            {
                DepartmentId = departmentId,
                AssignedToUser = userId,
                Status = status,
                Priority = priority,
                FromDate = fromDate,
                ToDate = toDate
            }));
            return Results.Ok(result);
        })
        .WithName("GetTaskSummaryReport")
        .RequireAuthorization(PermissionList.TaskManagementPermissions.ViewReports);

        app.MapGet("/api/v1/taskmanagement/reports/employee-productivity", async (
            Guid? departmentId, string? userId, DateTime? fromDate, DateTime? toDate, ISender sender) =>
        {
            var result = await sender.Send(new GetEmployeeProductivityReportQuery(new TaskFilterDto
            {
                DepartmentId = departmentId,
                AssignedToUser = userId,
                FromDate = fromDate,
                ToDate = toDate
            }));
            return Results.Ok(result);
        })
        .WithName("GetEmployeeProductivityReport")
        .RequireAuthorization(PermissionList.TaskManagementPermissions.ViewReports);

        app.MapGet("/api/v1/taskmanagement/reports/department-performance", async (
            Guid? departmentId, DateTime? fromDate, DateTime? toDate, ISender sender) =>
        {
            var result = await sender.Send(new GetDepartmentPerformanceReportQuery(new TaskFilterDto
            {
                DepartmentId = departmentId,
                FromDate = fromDate,
                ToDate = toDate
            }));
            return Results.Ok(result);
        })
        .WithName("GetDepartmentPerformanceReport")
        .RequireAuthorization(PermissionList.TaskManagementPermissions.ViewReports);
    }
}
