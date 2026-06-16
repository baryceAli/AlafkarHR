namespace Procurement.Procurement.Models;

public abstract class ProcurementDocument : Aggregate<Guid>
{
    private readonly List<ProcurementDocumentLine> _lines = new();

    protected ProcurementDocument()
    {
    }

    public ProcurementDocumentKind Kind { get; protected set; }
    public string Number { get; protected set; } = string.Empty;
    public string Status { get; protected set; } = string.Empty;
    public DateTime DocumentDate { get; protected set; }
    public Guid CompanyId { get; protected set; }
    public Guid? BranchId { get; protected set; }
    public Guid? DepartmentId { get; protected set; }
    public Guid? SupplierId { get; protected set; }
    public string? SupplierName { get; protected set; }
    public Guid? WarehouseId { get; protected set; }
    public Guid? CurrencyId { get; protected set; }
    public Guid? SourceDocumentId { get; protected set; }
    public string? SourceDocumentNumber { get; protected set; }
    public string? Notes { get; protected set; }
    public decimal Subtotal { get; protected set; }
    public decimal TaxAmount { get; protected set; }
    public decimal TotalAmount { get; protected set; }
    public IReadOnlyCollection<ProcurementDocumentLine> Lines => _lines.Where(x => !x.IsDeleted).OrderBy(x => x.LineNumber).ToList();

    protected void Initialize(ProcurementDocumentKind kind, string status, ProcurementDocumentDto dto, string userId)
    {
        Id = Guid.NewGuid();
        Kind = kind;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        CreatedBy = userId;
        Apply(dto, userId);
    }

    public void Update(ProcurementDocumentDto dto, string userId)
    {
        EnsureEditable();
        Apply(dto, userId);
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public void ChangeStatus(string status, string userId)
    {
        Status = status;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public ProcurementDocumentDto ToDto() =>
        new()
        {
            Id = Id,
            Kind = Kind,
            Number = Number,
            Status = Status,
            DocumentDate = DocumentDate,
            CompanyId = CompanyId,
            BranchId = BranchId,
            DepartmentId = DepartmentId,
            SupplierId = SupplierId,
            SupplierName = SupplierName,
            WarehouseId = WarehouseId,
            CurrencyId = CurrencyId,
            SourceDocumentId = SourceDocumentId,
            SourceDocumentNumber = SourceDocumentNumber,
            Notes = Notes,
            Subtotal = Subtotal,
            TaxAmount = TaxAmount,
            TotalAmount = TotalAmount,
            Lines = Lines.Select(x => x.ToDto()).ToList()
        };

    private void Apply(ProcurementDocumentDto dto, string userId)
    {
        Number = dto.Number;
        DocumentDate = dto.DocumentDate == default ? DateTime.UtcNow : dto.DocumentDate;
        CompanyId = dto.CompanyId;
        BranchId = dto.BranchId;
        DepartmentId = dto.DepartmentId;
        SupplierId = dto.SupplierId;
        SupplierName = dto.SupplierName;
        WarehouseId = dto.WarehouseId;
        CurrencyId = dto.CurrencyId;
        SourceDocumentId = dto.SourceDocumentId;
        SourceDocumentNumber = dto.SourceDocumentNumber;
        Notes = dto.Notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;

        foreach (var line in _lines.Where(x => !x.IsDeleted))
        {
            line.Remove(userId);
        }

        var nextLine = 1;
        foreach (var lineDto in dto.Lines)
        {
            _lines.Add(ProcurementDocumentLine.Create(nextLine++, lineDto, userId));
        }

        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        Subtotal = Lines.Sum(x => x.NetAmount);
        TaxAmount = Lines.Sum(x => x.TaxAmount);
        TotalAmount = Lines.Sum(x => x.TotalAmount);
    }

    private void EnsureEditable()
    {
        if (IsDeleted)
            throw new Exception("Deleted procurement document cannot be edited.");

        if (Status is "Posted" or "Closed" or "Cancelled")
            throw new Exception("Procurement document is not editable.");
    }
}

public class ProcurementDocumentLine : Entity<Guid>
{
    private ProcurementDocumentLine()
    {
    }

    public int LineNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public string ProductNameEng { get; private set; } = string.Empty;
    public string SkuCode { get; private set; } = string.Empty;
    public Guid? UnitOfMeasureId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public Guid? BatchId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal DiscountRate { get; private set; }
    public decimal TaxRate { get; private set; }
    public string? Notes { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }

    public static ProcurementDocumentLine Create(int lineNumber, ProcurementDocumentLineDto dto, string userId)
    {
        var line = new ProcurementDocumentLine
        {
            Id = Guid.NewGuid(),
            LineNumber = lineNumber,
            ProductId = dto.ProductId,
            ProductSkuId = dto.ProductSkuId,
            ProductPackageId = dto.ProductPackageId,
            ProductName = dto.ProductName,
            ProductNameEng = dto.ProductNameEng,
            SkuCode = dto.SkuCode,
            UnitOfMeasureId = dto.UnitOfMeasureId,
            WarehouseId = dto.WarehouseId,
            BatchId = dto.BatchId,
            Quantity = dto.Quantity,
            UnitCost = dto.UnitCost,
            DiscountRate = dto.DiscountRate,
            TaxRate = dto.TaxRate,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        line.Recalculate();
        return line;
    }

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public ProcurementDocumentLineDto ToDto() =>
        new()
        {
            Id = Id,
            LineNumber = LineNumber,
            ProductId = ProductId,
            ProductSkuId = ProductSkuId,
            ProductPackageId = ProductPackageId,
            ProductName = ProductName,
            ProductNameEng = ProductNameEng,
            SkuCode = SkuCode,
            UnitOfMeasureId = UnitOfMeasureId,
            WarehouseId = WarehouseId,
            BatchId = BatchId,
            Quantity = Quantity,
            UnitCost = UnitCost,
            DiscountRate = DiscountRate,
            TaxRate = TaxRate,
            Notes = Notes,
            NetAmount = NetAmount,
            TaxAmount = TaxAmount,
            TotalAmount = TotalAmount
        };

    private void Recalculate()
    {
        var gross = Quantity * UnitCost;
        var discount = gross * (DiscountRate / 100m);
        NetAmount = gross - discount;
        TaxAmount = NetAmount * (TaxRate / 100m);
        TotalAmount = NetAmount + TaxAmount;
    }
}

public class PurchaseRequest : ProcurementDocument
{
    private PurchaseRequest() { }
    public static PurchaseRequest Create(ProcurementDocumentDto dto, string userId)
    {
        var document = new PurchaseRequest();
        document.Initialize(ProcurementDocumentKind.PurchaseRequest, PurchaseRequestStatus.Draft.ToString(), dto, userId);
        return document;
    }
}

public class RequestForQuotation : ProcurementDocument
{
    private RequestForQuotation() { }
    public static RequestForQuotation Create(ProcurementDocumentDto dto, string userId)
    {
        var document = new RequestForQuotation();
        document.Initialize(ProcurementDocumentKind.RequestForQuotation, RequestForQuotationStatus.Draft.ToString(), dto, userId);
        return document;
    }
}

public class SupplierQuotation : ProcurementDocument
{
    private SupplierQuotation() { }
    public static SupplierQuotation Create(ProcurementDocumentDto dto, string userId)
    {
        var document = new SupplierQuotation();
        document.Initialize(ProcurementDocumentKind.SupplierQuotation, SupplierQuotationStatus.Received.ToString(), dto, userId);
        return document;
    }
}

public class PurchaseOrder : ProcurementDocument
{
    private PurchaseOrder() { }
    public static PurchaseOrder Create(ProcurementDocumentDto dto, string userId)
    {
        var document = new PurchaseOrder();
        document.Initialize(ProcurementDocumentKind.PurchaseOrder, PurchaseOrderStatus.Draft.ToString(), dto, userId);
        return document;
    }
}

public class GoodsReceipt : ProcurementDocument
{
    private GoodsReceipt() { }
    public static GoodsReceipt Create(ProcurementDocumentDto dto, string userId)
    {
        var document = new GoodsReceipt();
        document.Initialize(ProcurementDocumentKind.GoodsReceipt, PostedDocumentStatus.Draft.ToString(), dto, userId);
        return document;
    }
}

public class PurchaseReturn : ProcurementDocument
{
    private PurchaseReturn() { }
    public static PurchaseReturn Create(ProcurementDocumentDto dto, string userId)
    {
        var document = new PurchaseReturn();
        document.Initialize(ProcurementDocumentKind.PurchaseReturn, PostedDocumentStatus.Draft.ToString(), dto, userId);
        return document;
    }
}

public class SupplierInvoice : ProcurementDocument
{
    private SupplierInvoice() { }
    public static SupplierInvoice Create(ProcurementDocumentDto dto, string userId)
    {
        var document = new SupplierInvoice();
        document.Initialize(ProcurementDocumentKind.SupplierInvoice, SupplierInvoiceStatus.Draft.ToString(), dto, userId);
        return document;
    }
}
