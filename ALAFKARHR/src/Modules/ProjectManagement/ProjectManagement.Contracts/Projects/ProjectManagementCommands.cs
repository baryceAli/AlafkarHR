using Shared.Contracts.CQRS;
using SharedWithUI.ProjectManagement.Dtos;

namespace ProjectManagement.Contracts.Projects;

public record CreateProjectCommand(ProjectDto Project) : ICommand<CreateProjectResult>;
public record CreateProjectResult(Guid Id, string ProjectNumber);

