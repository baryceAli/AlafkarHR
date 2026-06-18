namespace Contracts.Contracts.Features.Templates;

public record GetContractTemplatesQuery(Guid? CompanyId, string? ContractType, int PageIndex, int PageSize, string? SearchText)
    : IQuery<GetContractTemplatesResult>;

public record GetContractTemplatesResult(PaginatedResult<ContractTemplateDto> Templates);

public record GetContractTemplateByIdQuery(Guid Id) : IQuery<GetContractTemplateByIdResult>;
public record GetContractTemplateByIdResult(ContractTemplateDto Template);

public record CreateContractTemplateCommand(ContractTemplateDto Template) : ICommand<CreateContractTemplateResult>;
public record CreateContractTemplateResult(Guid Id);

public record UpdateContractTemplateCommand(Guid Id, ContractTemplateDto Template) : ICommand<UpdateContractTemplateResult>;
public record UpdateContractTemplateResult(bool IsSuccess);

public record DeleteContractTemplateCommand(Guid Id) : ICommand<DeleteContractTemplateResult>;
public record DeleteContractTemplateResult(bool IsSuccess);

public record UploadContractTemplateFileCommand(Guid Id, IFormFile File) : ICommand<UploadContractTemplateFileResult>;
public record UploadContractTemplateFileResult(string FilePath);
