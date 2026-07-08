using SharedWithUI.Catalog.SKUGenerator;
using Catalog.Products.Features.Products.AddProductSku;

namespace Catalog.Products.Features.Products.GenerateVariantMatrix;

public record GenerateVariantMatrixCommand(ProductSkuVariantMatrixRequest Request)
    : ICommand<GenerateVariantMatrixResult>;

public record GenerateVariantMatrixResult(ProductSkuVariantMatrixResultDto Result);

public class GenerateVariantMatrixCommandValidator : AbstractValidator<GenerateVariantMatrixCommand>
{
    public GenerateVariantMatrixCommandValidator()
    {
        RuleFor(x => x.Request.ProductId).NotEmpty().WithMessage("Product is required");
        RuleFor(x => x.Request.BrandId).NotEmpty().WithMessage("Brand is required");
        RuleFor(x => x.Request.UnitId).NotEmpty().WithMessage("Unit is required");
        RuleFor(x => x.Request.Price).GreaterThan(0).WithMessage("Price must be greater than zero");
        RuleFor(x => x.Request.Calories).NotNull().GreaterThan(0).WithMessage("Calories must be greater than zero");
        RuleFor(x => x.Request.Options).NotEmpty().WithMessage("At least one option is required");
    }
}

public class GenerateVariantMatrixHandler(CatalogDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    : ICommandHandler<GenerateVariantMatrixCommand, GenerateVariantMatrixResult>
{
    public async Task<GenerateVariantMatrixResult> Handle(GenerateVariantMatrixCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var userId = CatalogUserContext.GetUserId(httpContextAccessor);
        var companyId = CatalogUserContext.GetCompanyId(httpContextAccessor);

        var product = await dbContext.Products.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.CompanyId == companyId && x.IsActive, cancellationToken)
            ?? throw new Exception($"Product not found: {request.ProductId}");

        await CatalogOwnershipGuard.EnsureBrandAsync(dbContext, request.BrandId, companyId, cancellationToken);
        await CatalogOwnershipGuard.EnsureUnitAsync(dbContext, request.UnitId, companyId, cancellationToken);

        var packageAssignments = request.PackageAssignments
            .Where(x => x.ProductPackageId != Guid.Empty)
            .ToList();

        if (request.PackageId.HasValue
            && request.PackageId.Value != Guid.Empty
            && packageAssignments.All(x => x.ProductPackageId != request.PackageId.Value))
        {
            packageAssignments.Add(new ProductSkuPackageDto
            {
                ProductPackageId = request.PackageId.Value,
                Quantity = 1,
                SalesEnabled = true,
                PurchaseEnabled = true,
                IsActive = true
            });
        }

        await CatalogOwnershipGuard.EnsureSkuPackageAssignmentsAsync(
            dbContext,
            packageAssignments,
            request.UnitId,
            companyId,
            null,
            cancellationToken);

        var requestedPairs = request.Options
            .Where(option => option.VariantId != Guid.Empty)
            .Select(option => new VariantMatrixOptionDto
            {
                VariantId = option.VariantId,
                VariantValueIds = option.VariantValueIds
                    .Where(valueId => valueId != Guid.Empty)
                    .Distinct()
                    .ToList()
            })
            .Where(option => option.VariantValueIds.Any())
            .ToList();

        if (!requestedPairs.Any())
            throw new Exception("At least one option value is required.");

        var variantIds = requestedPairs.Select(x => x.VariantId).Distinct().ToList();
        var valueIds = requestedPairs.SelectMany(x => x.VariantValueIds).Distinct().ToList();

        var variants = await dbContext.Variants.AsNoTracking()
            .Where(x => x.CompanyId == companyId && variantIds.Contains(x.Id) && x.IsActive)
            .ToListAsync(cancellationToken);

        var variantValues = await dbContext.VariantValues.AsNoTracking()
            .Where(x => valueIds.Contains(x.Id) && x.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var option in requestedPairs)
        {
            var variant = variants.FirstOrDefault(x => x.Id == option.VariantId)
                ?? throw new Exception($"Variant not found: {option.VariantId}");

            if (variant.CreationMode == VariantCreationMode.Never)
                throw new Exception("Never-creation variants cannot generate SKU matrix rows.");

            foreach (var valueId in option.VariantValueIds)
            {
                if (!variantValues.Any(x => x.Id == valueId && x.VariantId == option.VariantId))
                    throw new Exception($"Variant value not found: {valueId}");
            }
        }

        var productionType = request.ProductionType == default
            ? SkuProductionType.PurchasedRawMaterial
            : request.ProductionType;
        var trackingMode = AddProductSkuHandler.ResolveTrackingMode(product.ProductType, productionType, request.TrackingMode);
        AddProductSkuHandler.ValidateProductTypeCapabilities(product.ProductType, productionType, trackingMode);

        var brand = await dbContext.Brands.AsNoTracking()
            .FirstAsync(x => x.Id == request.BrandId && x.CompanyId == companyId && x.IsActive, cancellationToken);

        var variantNames = variants.ToDictionary(x => x.Id, x => x.Name);
        var variantNamesEng = variants.ToDictionary(x => x.Id, x => x.NameEng);
        var valueNames = variantValues.ToDictionary(x => x.Id, x => x.Value);
        var valueNamesEng = variantValues.ToDictionary(x => x.Id, x => x.ValueEng);
        var primaryPackageId = packageAssignments.Select(x => (Guid?)x.ProductPackageId).FirstOrDefault() ?? request.PackageId;

        var createdSkuIds = new List<Guid>();
        var skippedCount = 0;

        foreach (var combination in BuildCombinations(requestedPairs, 0, []))
        {
            var variantDtos = combination
                .Select(pair => new ProductSkuVariantDto
                {
                    VariantId = pair.VariantId,
                    VariantValueId = pair.VariantValueId
                })
                .ToList();

            if (await HasExistingCombinationAsync(companyId, request.ProductId, request.BrandId, primaryPackageId, variantDtos, cancellationToken))
            {
                skippedCount++;
                continue;
            }

            var skuContext = new SkuBuildContext(
                request.ProductId,
                request.BrandId,
                primaryPackageId,
                combination.Select(x => (x.VariantId, x.VariantValueId)).ToList());

            var skuId = Guid.NewGuid();
            var sku = ProductSku.Create(
                skuId,
                request.ProductId,
                request.BrandId,
                request.UnitId!.Value,
                primaryPackageId,
                BuildSkuName(product.Name, combination, valueNames),
                BuildSkuName(product.NameEng, combination, valueNamesEng),
                ProductSkuGenerator.GenerateSkuCode(skuContext, variantNames, valueNames, product.Name, brand.Name),
                ProductSkuGenerator.GenerateSkuCode(skuContext, variantNamesEng, valueNamesEng, product.NameEng, brand.NameEng),
                ProductSkuGenerator.BuildSkuKey(skuContext),
                null,
                string.Empty,
                request.Price,
                request.Calories,
                productionType,
                trackingMode,
                request.ExpirationDate,
                request.ShelfLifeDays,
                request.RemovalTimeDays,
                request.AlertTimeDays,
                request.GalleryImageUrls,
                request.ShowOnStore,
                request.IsSellable,
                request.IsPurchasable,
                request.IsAssetTrackable,
                companyId,
                userId);

            foreach (var variant in variantDtos)
            {
                sku.AddVariant(variant.VariantId, variant.VariantValueId, userId);
            }

            sku.SetPackages(packageAssignments, userId);
            dbContext.ProductSkus.Add(sku);
            createdSkuIds.Add(skuId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new GenerateVariantMatrixResult(new ProductSkuVariantMatrixResultDto
        {
            CreatedCount = createdSkuIds.Count,
            SkippedCount = skippedCount,
            CreatedSkuIds = createdSkuIds
        });
    }

    private async Task<bool> HasExistingCombinationAsync(
        Guid companyId,
        Guid productId,
        Guid brandId,
        Guid? packageId,
        IReadOnlyCollection<ProductSkuVariantDto> variants,
        CancellationToken cancellationToken)
    {
        var requestedPairs = variants
            .Select(x => $"{x.VariantId:N}:{x.VariantValueId:N}")
            .OrderBy(x => x)
            .ToArray();

        var candidates = await dbContext.ProductSkus
            .AsNoTracking()
            .Include(x => x.Variants)
            .Where(x => x.CompanyId == companyId
                && x.ProductId == productId
                && x.BrandId == brandId
                && x.PackageId == packageId
                && x.IsActive)
            .ToListAsync(cancellationToken);

        return candidates.Any(candidate => candidate.Variants
            .Where(x => !x.IsDeleted)
            .Select(x => $"{x.VariantId:N}:{x.VariantValueId:N}")
            .OrderBy(x => x)
            .SequenceEqual(requestedPairs));
    }

    private static IEnumerable<List<(Guid VariantId, Guid VariantValueId)>> BuildCombinations(
        IReadOnlyList<VariantMatrixOptionDto> options,
        int index,
        List<(Guid VariantId, Guid VariantValueId)> current)
    {
        if (index >= options.Count)
        {
            yield return current.ToList();
            yield break;
        }

        var option = options[index];
        foreach (var valueId in option.VariantValueIds)
        {
            current.Add((option.VariantId, valueId));
            foreach (var combination in BuildCombinations(options, index + 1, current))
                yield return combination;
            current.RemoveAt(current.Count - 1);
        }
    }

    private static string BuildSkuName(
        string productName,
        IEnumerable<(Guid VariantId, Guid VariantValueId)> combination,
        IReadOnlyDictionary<Guid, string> valueNames)
    {
        var values = combination
            .Select(pair => valueNames.TryGetValue(pair.VariantValueId, out var value) ? value : string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value));

        return $"{productName} - {string.Join(" / ", values)}";
    }
}
