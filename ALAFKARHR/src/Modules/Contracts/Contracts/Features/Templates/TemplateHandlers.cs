namespace Contracts.Contracts.Features.Templates;

public class TemplateHandlers(ContractsDbContext dbContext, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment environment) :
    IQueryHandler<GetContractTemplatesQuery, GetContractTemplatesResult>,
    IQueryHandler<GetContractTemplateByIdQuery, GetContractTemplateByIdResult>,
    ICommandHandler<CreateContractTemplateCommand, CreateContractTemplateResult>,
    ICommandHandler<UpdateContractTemplateCommand, UpdateContractTemplateResult>,
    ICommandHandler<DeleteContractTemplateCommand, DeleteContractTemplateResult>,
    ICommandHandler<UploadContractTemplateFileCommand, UploadContractTemplateFileResult>
{
    public async Task<GetContractTemplatesResult> Handle(GetContractTemplatesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.ContractTemplates.AsNoTracking().AsQueryable();
        if (request.CompanyId.HasValue)
            query = query.Where(x => x.CompanyId == request.CompanyId);
        if (!string.IsNullOrWhiteSpace(request.ContractType))
            query = query.Where(x => x.ContractType == request.ContractType);
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var search = request.SearchText.Trim();
            query = query.Where(x => x.Name.Contains(search) || x.NameEng.Contains(search) || x.ContractType.Contains(search));
        }

        var pageIndex = Math.Max(1, request.PageIndex);
        var pageSize = Math.Clamp(request.PageSize, 1, 200);
        var count = await query.CountAsync(cancellationToken);
        var data = await query.OrderByDescending(x => x.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return new GetContractTemplatesResult(new PaginatedResult<ContractTemplateDto>(pageIndex, pageSize, count, data));
    }

    public async Task<GetContractTemplateByIdResult> Handle(GetContractTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await dbContext.ContractTemplates.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract template not found: {request.Id}");
        return new GetContractTemplateByIdResult(template.ToDto());
    }

    public async Task<CreateContractTemplateResult> Handle(CreateContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = ContractTemplate.Create(request.Template, ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.ContractTemplates.AddAsync(template, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateContractTemplateResult(template.Id);
    }

    public async Task<UpdateContractTemplateResult> Handle(UpdateContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await dbContext.ContractTemplates.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract template not found: {request.Id}");
        template.Update(request.Template, ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UpdateContractTemplateResult(true);
    }

    public async Task<DeleteContractTemplateResult> Handle(DeleteContractTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await dbContext.ContractTemplates.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract template not found: {request.Id}");
        template.Remove(ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DeleteContractTemplateResult(true);
    }

    public async Task<UploadContractTemplateFileResult> Handle(UploadContractTemplateFileCommand request, CancellationToken cancellationToken)
    {
        var template = await dbContext.ContractTemplates.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Contract template not found: {request.Id}");

        var filePath = await ContractFeatureHelpers.SaveFileAsync(request.File, environment, "Templates", template.Id, ContractFeatureHelpers.AllowedDocumentContentTypes(), cancellationToken);
        template.SetFile(request.File.FileName, filePath, request.File.ContentType, request.File.Length, ContractFeatureHelpers.CurrentUserId(httpContextAccessor));
        await dbContext.SaveChangesAsync(cancellationToken);
        return new UploadContractTemplateFileResult(filePath);
    }
}
