namespace Inventory.Warehouses.Models;

public class WarehouseLocation : Aggregate<Guid>
{
    private WarehouseLocation() { }

    public Guid CompanyId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public string? ParentCode { get; private set; }
    public WarehouseLocationType LocationType { get; private set; }
    public WarehouseLocationUsage LocationUsage { get; private set; } = WarehouseLocationUsage.Internal;
    public bool IsVirtual { get; private set; }
    public bool ExcludeFromPhysicalStock { get; private set; }
    public bool IsActive { get; private set; }

    public static WarehouseLocation Create(WarehouseLocationDto dto, string userId)
    {
        var location = new WarehouseLocation { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        location.Apply(dto, userId);
        return location;
    }

    public void Update(WarehouseLocationDto dto, string userId) => Apply(dto, userId);
    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public WarehouseLocationDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        WarehouseId = WarehouseId,
        Code = Code,
        Name = Name,
        NameEng = NameEng,
        ParentCode = ParentCode,
        LocationType = LocationType,
        LocationUsage = LocationUsage,
        IsVirtual = IsVirtual,
        ExcludeFromPhysicalStock = ExcludeFromPhysicalStock,
        IsActive = IsActive
    };

    private void Apply(WarehouseLocationDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        WarehouseId = dto.WarehouseId;
        Code = dto.Code;
        Name = dto.Name;
        NameEng = dto.NameEng;
        ParentCode = dto.ParentCode;
        LocationType = dto.LocationType;
        LocationUsage = dto.LocationUsage;
        IsVirtual = dto.IsVirtual || dto.LocationUsage is WarehouseLocationUsage.Transit or WarehouseLocationUsage.VirtualScrap;
        ExcludeFromPhysicalStock = dto.ExcludeFromPhysicalStock || IsVirtual;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class InventoryOperationType : Aggregate<Guid>
{
    private InventoryOperationType() { }

    public Guid CompanyId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public InventoryOperationKind OperationKind { get; private set; }
    public Guid? DefaultSourceLocationId { get; private set; }
    public Guid? DefaultDestinationLocationId { get; private set; }
    public bool IsActive { get; private set; }

    public static InventoryOperationType Create(InventoryOperationTypeDto dto, string userId)
    {
        var operationType = new InventoryOperationType { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        operationType.Apply(dto, userId);
        return operationType;
    }

    public void Update(InventoryOperationTypeDto dto, string userId) => Apply(dto, userId);

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public InventoryOperationTypeDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        WarehouseId = WarehouseId,
        Code = Code,
        Name = Name,
        NameEng = NameEng,
        OperationKind = OperationKind,
        DefaultSourceLocationId = DefaultSourceLocationId,
        DefaultDestinationLocationId = DefaultDestinationLocationId,
        IsActive = IsActive
    };

    private void Apply(InventoryOperationTypeDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        WarehouseId = dto.WarehouseId;
        Code = dto.Code;
        Name = dto.Name;
        NameEng = dto.NameEng;
        OperationKind = dto.OperationKind;
        DefaultSourceLocationId = dto.DefaultSourceLocationId;
        DefaultDestinationLocationId = dto.DefaultDestinationLocationId;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class InventoryRoute : Aggregate<Guid>
{
    private InventoryRoute() { }

    public Guid CompanyId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public InventoryRouteApplicationScope ApplicationScope { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? ProductSkuId { get; private set; }
    public Guid? ProductCategoryId { get; private set; }
    public bool IsActive { get; private set; }

    public static InventoryRoute Create(InventoryRouteDto dto, string userId)
    {
        var route = new InventoryRoute { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        route.Apply(dto, userId);
        return route;
    }

    public void Update(InventoryRouteDto dto, string userId) => Apply(dto, userId);

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public InventoryRouteDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        WarehouseId = WarehouseId,
        Code = Code,
        Name = Name,
        NameEng = NameEng,
        ApplicationScope = ApplicationScope,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductCategoryId = ProductCategoryId,
        IsActive = IsActive
    };

    private void Apply(InventoryRouteDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        WarehouseId = dto.WarehouseId;
        Code = dto.Code;
        Name = dto.Name;
        NameEng = dto.NameEng;
        ApplicationScope = dto.ApplicationScope;
        ProductId = dto.ProductId;
        ProductSkuId = dto.ProductSkuId;
        ProductCategoryId = dto.ProductCategoryId;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class InventoryRouteRule : Aggregate<Guid>
{
    private InventoryRouteRule() { }

    public Guid CompanyId { get; private set; }
    public Guid RouteId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid OperationTypeId { get; private set; }
    public InventoryRouteRuleAction Action { get; private set; }
    public Guid SourceLocationId { get; private set; }
    public Guid DestinationLocationId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? ProductSkuId { get; private set; }
    public Guid? ProductCategoryId { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; }

    public static InventoryRouteRule Create(InventoryRouteRuleDto dto, string userId)
    {
        var rule = new InventoryRouteRule { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        rule.Apply(dto, userId);
        return rule;
    }

    public void Update(InventoryRouteRuleDto dto, string userId) => Apply(dto, userId);

    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public InventoryRouteRuleDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        RouteId = RouteId,
        WarehouseId = WarehouseId,
        OperationTypeId = OperationTypeId,
        Action = Action,
        SourceLocationId = SourceLocationId,
        DestinationLocationId = DestinationLocationId,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        ProductCategoryId = ProductCategoryId,
        Priority = Priority,
        IsActive = IsActive
    };

    private void Apply(InventoryRouteRuleDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        RouteId = dto.RouteId;
        WarehouseId = dto.WarehouseId;
        OperationTypeId = dto.OperationTypeId;
        Action = dto.Action;
        SourceLocationId = dto.SourceLocationId;
        DestinationLocationId = dto.DestinationLocationId;
        ProductId = dto.ProductId;
        ProductSkuId = dto.ProductSkuId;
        ProductCategoryId = dto.ProductCategoryId;
        Priority = dto.Priority;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class PutawayRule : Aggregate<Guid>
{
    private PutawayRule() { }

    public Guid CompanyId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? ProductSkuId { get; private set; }
    public Guid? DestinationLocationId { get; private set; }
    public RemovalStrategy RemovalStrategy { get; private set; }
    public int Priority { get; private set; }
    public bool IsActive { get; private set; }

    public static PutawayRule Create(PutawayRuleDto dto, string userId)
    {
        var rule = new PutawayRule { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        rule.Apply(dto, userId);
        return rule;
    }

    public void Update(PutawayRuleDto dto, string userId) => Apply(dto, userId);
    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public PutawayRuleDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        WarehouseId = WarehouseId,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        DestinationLocationId = DestinationLocationId,
        RemovalStrategy = RemovalStrategy,
        Priority = Priority,
        IsActive = IsActive
    };

    private void Apply(PutawayRuleDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        WarehouseId = dto.WarehouseId;
        ProductId = dto.ProductId;
        ProductSkuId = dto.ProductSkuId;
        DestinationLocationId = dto.DestinationLocationId;
        RemovalStrategy = dto.RemovalStrategy;
        Priority = dto.Priority;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class QualityInspection : Aggregate<Guid>
{
    private QualityInspection() { }

    public Guid CompanyId { get; private set; }
    public Guid? SourceDocumentId { get; private set; }
    public string? SourceDocumentNumber { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? BatchId { get; private set; }
    public Guid? WarehouseId { get; private set; }
    public decimal Quantity { get; private set; }
    public QualityInspectionStatus Status { get; private set; }
    public string? ResultNotes { get; private set; }
    public DateTime InspectionDate { get; private set; }

    public static QualityInspection Create(QualityInspectionDto dto, string userId)
    {
        var inspection = new QualityInspection { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        inspection.Apply(dto, userId);
        return inspection;
    }

    public void Update(QualityInspectionDto dto, string userId) => Apply(dto, userId);
    public void Remove(string userId)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public QualityInspectionDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        SourceDocumentId = SourceDocumentId,
        SourceDocumentNumber = SourceDocumentNumber,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        BatchId = BatchId,
        WarehouseId = WarehouseId,
        Quantity = Quantity,
        Status = Status,
        ResultNotes = ResultNotes,
        InspectionDate = InspectionDate
    };

    private void Apply(QualityInspectionDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        SourceDocumentId = dto.SourceDocumentId;
        SourceDocumentNumber = dto.SourceDocumentNumber;
        ProductId = dto.ProductId;
        ProductSkuId = dto.ProductSkuId;
        BatchId = dto.BatchId;
        WarehouseId = dto.WarehouseId;
        Quantity = dto.Quantity;
        Status = dto.Status;
        ResultNotes = dto.ResultNotes;
        InspectionDate = dto.InspectionDate == default ? DateTime.UtcNow : dto.InspectionDate;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class LandedCostVoucher : Aggregate<Guid>
{
    private LandedCostVoucher() { }

    public Guid CompanyId { get; private set; }
    public Guid SourceDocumentId { get; private set; }
    public string SourceDocumentNumber { get; private set; } = string.Empty;
    public Guid? CurrencyId { get; private set; }
    public LandedCostAllocationMethod AllocationMethod { get; private set; }
    public decimal FreightAmount { get; private set; }
    public decimal CustomsAmount { get; private set; }
    public decimal HandlingAmount { get; private set; }
    public decimal OtherAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsPosted { get; private set; }
    public DateTime VoucherDate { get; private set; }

    public static LandedCostVoucher Create(LandedCostVoucherDto dto, string userId)
    {
        var voucher = new LandedCostVoucher { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, CreatedBy = userId };
        voucher.Apply(dto, userId);
        return voucher;
    }

    public void Update(LandedCostVoucherDto dto, string userId)
    {
        if (IsPosted)
            throw new InvalidOperationException("Posted landed cost vouchers cannot be edited.");
        Apply(dto, userId);
    }

    public void Post(string userId)
    {
        IsPosted = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    public void Remove(string userId)
    {
        if (IsPosted)
            throw new InvalidOperationException("Posted landed cost vouchers cannot be deleted.");
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = userId;
    }

    public LandedCostVoucherDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        SourceDocumentId = SourceDocumentId,
        SourceDocumentNumber = SourceDocumentNumber,
        CurrencyId = CurrencyId,
        AllocationMethod = AllocationMethod,
        FreightAmount = FreightAmount,
        CustomsAmount = CustomsAmount,
        HandlingAmount = HandlingAmount,
        OtherAmount = OtherAmount,
        TotalAmount = TotalAmount,
        IsPosted = IsPosted,
        VoucherDate = VoucherDate
    };

    private void Apply(LandedCostVoucherDto dto, string userId)
    {
        CompanyId = dto.CompanyId;
        SourceDocumentId = dto.SourceDocumentId;
        SourceDocumentNumber = dto.SourceDocumentNumber;
        CurrencyId = dto.CurrencyId;
        AllocationMethod = dto.AllocationMethod;
        FreightAmount = dto.FreightAmount;
        CustomsAmount = dto.CustomsAmount;
        HandlingAmount = dto.HandlingAmount;
        OtherAmount = dto.OtherAmount;
        TotalAmount = dto.TotalAmount > 0 ? dto.TotalAmount : dto.FreightAmount + dto.CustomsAmount + dto.HandlingAmount + dto.OtherAmount;
        VoucherDate = dto.VoucherDate == default ? DateTime.UtcNow : dto.VoucherDate;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }
}

public class InventoryValuationLayer : Entity<Guid>
{
    private InventoryValuationLayer() { }

    public Guid CompanyId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid? BatchId { get; private set; }
    public string SourceDocumentType { get; private set; } = string.Empty;
    public string ReferenceNumber { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitCost { get; private set; }
    public decimal TotalCost { get; private set; }
    public DateTime LayerDate { get; private set; }

    public static InventoryValuationLayer Create(InventoryValuationLayerDto dto, string userId) => new()
    {
        Id = Guid.NewGuid(),
        CompanyId = dto.CompanyId,
        ProductId = dto.ProductId,
        ProductSkuId = dto.ProductSkuId,
        WarehouseId = dto.WarehouseId,
        BatchId = dto.BatchId,
        SourceDocumentType = dto.SourceDocumentType,
        ReferenceNumber = dto.ReferenceNumber,
        Quantity = dto.Quantity,
        UnitCost = dto.UnitCost,
        TotalCost = dto.TotalCost,
        LayerDate = dto.LayerDate == default ? DateTime.UtcNow : dto.LayerDate,
        CreatedAt = DateTime.UtcNow,
        CreatedBy = userId
    };

    public static InventoryValuationLayer FromMovement(StockMovement movement, Guid companyId, string userId)
    {
        var signedQuantity = movement.MovementDirection == MovementDirection.OUT
            ? -Math.Abs(movement.NormalizedQuantity)
            : Math.Abs(movement.NormalizedQuantity);
        var signedCost = movement.MovementDirection == MovementDirection.OUT
            ? -Math.Abs(movement.TotalCost)
            : Math.Abs(movement.TotalCost);

        return Create(new InventoryValuationLayerDto
        {
            CompanyId = companyId,
            ProductId = movement.ProductId,
            ProductSkuId = movement.ProductSkuId,
            WarehouseId = movement.WarehouseId,
            BatchId = movement.BatchId,
            SourceDocumentType = movement.SourceDocumentType,
            ReferenceNumber = movement.ReferenceNumber,
            Quantity = signedQuantity,
            UnitCost = movement.UnitCost,
            TotalCost = signedCost,
            LayerDate = movement.CreatedAt ?? DateTime.UtcNow
        }, userId);
    }

    public InventoryValuationLayerDto ToDto() => new()
    {
        Id = Id,
        CompanyId = CompanyId,
        ProductId = ProductId,
        ProductSkuId = ProductSkuId,
        WarehouseId = WarehouseId,
        BatchId = BatchId,
        SourceDocumentType = SourceDocumentType,
        ReferenceNumber = ReferenceNumber,
        Quantity = Quantity,
        UnitCost = UnitCost,
        TotalCost = TotalCost,
        LayerDate = LayerDate
    };
}
