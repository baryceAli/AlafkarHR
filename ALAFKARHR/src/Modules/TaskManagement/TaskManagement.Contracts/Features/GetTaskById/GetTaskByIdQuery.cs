using Shared.Contracts.CQRS;
using SharedWithUI.TaskManagement.Dtos;

namespace TaskManagement.Contracts.Features.GetTaskById;

public record GetTaskByIdQuery(Guid Id) : IQuery<GetTaskByIdResult>;
public record GetTaskByIdResult(TaskItemDto Task);
