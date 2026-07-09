using Shared.DDD;

namespace SalesOrder.Orders.Models;

public class SalesQuotationTemplate : Aggregate<Guid>
{
    private readonly List<SalesQuotationTemplateLine> _lines = [];

    private SalesQuotationTemplate() { }

    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int ValidityDays { get; private set; }
    public string? Notes { get; private set; }
    public string? Terms { get; private set; }
    public bool RequiresCustomerSignature { get; private set; }
    public bool RequiresOnlinePayment { get; private set; }
    public decimal DownPaymentAmount { get; private set; }
    public decimal DownPaymentPercent { get; private set; }
    public bool IsProForma { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<SalesQuotationTemplateLine> Lines => _lines;

    public static SalesQuotationTemplate Create(SalesQuotationTemplateDto dto, string userId)
    {
        var template = new SalesQuotationTemplate
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };
        template.Apply(dto, userId);
        return template;
    }

    public void Update(SalesQuotationTemplateDto dto, string userId) => Apply(dto, userId);

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public SalesQuotationTemplateDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        Name = Name,
        Description = Description,
        ValidityDays = ValidityDays,
        Notes = Notes,
        Terms = Terms,
        RequiresCustomerSignature = RequiresCustomerSignature,
        RequiresOnlinePayment = RequiresOnlinePayment,
        DownPaymentAmount = DownPaymentAmount,
        DownPaymentPercent = DownPaymentPercent,
        IsProForma = IsProForma,
        IsActive = IsActive,
        Lines = _lines.OrderBy(x => x.LineNumber).Select(x => x.ToDto()).ToList()
    };

    private void Apply(SalesQuotationTemplateDto dto, string userId)
    {
        if (dto.CompanyId == Guid.Empty)
            throw new Exception("Company is required.");

        CompanyId = dto.CompanyId;
        Name = dto.Name.Trim();
        Description = dto.Description;
        ValidityDays = dto.ValidityDays;
        Notes = dto.Notes;
        Terms = dto.Terms;
        RequiresCustomerSignature = dto.RequiresCustomerSignature;
        RequiresOnlinePayment = dto.RequiresOnlinePayment;
        DownPaymentAmount = dto.DownPaymentAmount;
        DownPaymentPercent = dto.DownPaymentPercent;
        IsProForma = dto.IsProForma;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
        ReplaceLines(dto.Lines, userId);
    }

    private void ReplaceLines(List<SalesQuotationTemplateLineDto> lines, string userId)
    {
        _lines.Clear();

        var lineNumber = 1;
        foreach (var line in lines)
            _lines.Add(SalesQuotationTemplateLine.Create(lineNumber++, line, userId));
    }
}

public class SalesQuotationTemplateLine : Entity<Guid>
{
    private SalesQuotationTemplateLine() { }

    public int LineNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductNameEng { get; private set; } = string.Empty;
    public string SkuCode { get; private set; } = string.Empty;
    public Guid UnitOfMeasureId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountRate { get; private set; }
    public decimal TaxRate { get; private set; }
    public string? Notes { get; private set; }
    public bool IsOptional { get; private set; }

    public static SalesQuotationTemplateLine Create(int lineNumber, SalesQuotationTemplateLineDto dto, string userId) => new()
    {
        Id = Guid.NewGuid(),
        LineNumber = lineNumber,
        ProductId = dto.ProductId,
        ProductSkuId = dto.ProductSkuId,
        ProductName = dto.ProductName,
        ProductNameEng = dto.ProductNameEng,
        SkuCode = dto.SkuCode,
        UnitOfMeasureId = dto.UnitOfMeasureId,
        Quantity = dto.Quantity,
        UnitPrice = dto.UnitPrice,
        DiscountRate = dto.DiscountRate,
        TaxRate = dto.TaxRate,
        Notes = dto.Notes,
        IsOptional = dto.IsOptional,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = userId
    };

    public SalesQuotationTemplateLineDto ToDto() => new()
    {
        Id = Id,
        LineNumber = LineNumber,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductName = ProductName,
        ProductNameEng = ProductNameEng,
        SkuCode = SkuCode,
        UnitOfMeasureId = UnitOfMeasureId,
        Quantity = Quantity,
        UnitPrice = UnitPrice,
        DiscountRate = DiscountRate,
        TaxRate = TaxRate,
        Notes = Notes,
        IsOptional = IsOptional
    };
}
