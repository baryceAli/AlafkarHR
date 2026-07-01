namespace SharedWithUI.GeneralSettings.Dtos;

public sealed record DemoDataCreateRequestDto
{
    public string CompanyCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyNameEng { get; set; } = string.Empty;
    public string? AdminUserName { get; set; }
    public string? AdminEmail { get; set; }
    public string? DisplayLabel { get; set; }
}

public sealed record DemoDataSummaryDto
{
    public string CompanyCode { get; init; } = string.Empty;
    public Guid? CompanyId { get; init; }
    public string? CompanyName { get; init; }
    public string? CompanyNameEng { get; init; }
    public string? AdminUserName { get; init; }
    public string? AdminEmail { get; init; }
    public bool Exists { get; init; }
    public bool IsRecognizedDemoTenant { get; init; }
    public bool IsProduction { get; init; }
    public bool AllowProductionActions { get; init; }
    public bool DestructiveActionsAllowed { get; init; }
    public int BranchCount { get; init; }
    public int UserCount { get; init; }
    public int EmployeeCount { get; init; }
    public int TaskCount { get; init; }
    public int ProductSkuCount { get; init; }
    public int CustomerCount { get; init; }
    public int SupplierCount { get; init; }
    public int WarehouseCount { get; init; }
    public int AccountingDocumentCount { get; init; }
    public int JournalEntryCount { get; init; }
    public string LastKnownMarker { get; init; } = string.Empty;
}

public sealed record DemoDataStatusDto
{
    public string CompanyCode { get; init; } = string.Empty;
    public Guid? CompanyId { get; init; }
    public string? CompanyName { get; init; }
    public string? CompanyNameEng { get; init; }
    public string? AdminUserName { get; init; }
    public string? AdminEmail { get; init; }
    public bool Exists { get; init; }
    public bool IsRecognizedDemoTenant { get; init; }
    public bool IsProduction { get; init; }
    public bool AllowProductionActions { get; init; }
    public bool DestructiveActionsAllowed { get; init; }
    public int BranchCount { get; init; }
    public int UserCount { get; init; }
    public int EmployeeCount { get; init; }
    public int TaskCount { get; init; }
    public int ProductSkuCount { get; init; }
    public int CustomerCount { get; init; }
    public int SupplierCount { get; init; }
    public int WarehouseCount { get; init; }
    public int AccountingDocumentCount { get; init; }
    public int JournalEntryCount { get; init; }
    public string LastKnownMarker { get; init; } = string.Empty;
}

public sealed record DemoDataOperationResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public DemoDataStatusDto Status { get; init; } = new();
}

public sealed record DemoDataConfirmationRequestDto
{
    public string CompanyCode { get; init; } = string.Empty;
}
