namespace TaskManagement.Tasks.Features.GetTasks;

public class GetTasksEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/v1/taskmanagement/tasks", async (
            int PageIndex,
            int PageSize,
            string? searchText,
            string? taskNumber,
            string? title,
            string? assignedToUser,
            Guid? departmentId,
            TaskPriority? priority,
            TaskWorkflowStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            ISender sender) =>
        {
            var pagination = new PaginationRequest(PageIndex, PageSize, searchText);
            var filter = new TaskFilterDto
            {
                TaskNumber = taskNumber,
                Title = title,
                AssignedToUser = assignedToUser,
                DepartmentId = departmentId,
                Priority = priority,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate
            };
            var result = await sender.Send(new GetTasksQuery(pagination, filter));
            return Results.Ok(result);
        })
        .WithName("GetTasks")
        .Produces<GetTasksResult>(StatusCodes.Status200OK)
        .WithSummary("Get Tasks")
        .WithDescription("Get paged task list")
        .RequireAuthorization(PermissionList.TaskManagementPermissions.View);
    }
}
