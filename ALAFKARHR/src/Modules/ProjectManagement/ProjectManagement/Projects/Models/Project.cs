namespace ProjectManagement.Projects.Models;

public class Project : Aggregate<Guid>
{
    private readonly List<ProjectCustomer> _customers = [];
    private readonly List<ProjectDeliverable> _deliverables = [];
    private readonly List<ProjectDistributionSchedule> _distributionSchedules = [];
    private readonly List<ProjectMaterialRequirement> _materialRequirements = [];
    private readonly List<ProjectResource> _resources = [];
    private readonly List<ProjectExpense> _expenses = [];
    private readonly List<ProjectHandoff> _handoffs = [];
    private readonly List<ProjectTaskLink> _taskLinks = [];

    public string ProjectNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public Guid CompanyId { get; private set; }
    public Guid? BranchId { get; private set; }
    public ProjectType Type { get; private set; }
    public ProjectStatus Status { get; private set; }
    public Guid? ManagerUserId { get; private set; }
    public string? ManagerName { get; private set; }
    public Guid? SourceOrderId { get; private set; }
    public string? SourceOrderNumber { get; private set; }
    public string? SourceOrderType { get; private set; }
    public DateTime PlannedStartDate { get; private set; }
    public DateTime PlannedEndDate { get; private set; }
    public DateTime? ActualStartDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<ProjectCustomer> Customers => _customers;
    public IReadOnlyCollection<ProjectDeliverable> Deliverables => _deliverables;
    public IReadOnlyCollection<ProjectDistributionSchedule> DistributionSchedules => _distributionSchedules;
    public IReadOnlyCollection<ProjectMaterialRequirement> MaterialRequirements => _materialRequirements;
    public IReadOnlyCollection<ProjectResource> Resources => _resources;
    public IReadOnlyCollection<ProjectExpense> Expenses => _expenses;
    public IReadOnlyCollection<ProjectHandoff> Handoffs => _handoffs;
    public IReadOnlyCollection<ProjectTaskLink> TaskLinks => _taskLinks;

    private Project() { }

    public static Project Create(ProjectDto dto, string projectNumber, string createdBy)
    {
        ValidateDates(dto.PlannedStartDate, dto.PlannedEndDate);
        return new Project
        {
            Id = Guid.NewGuid(),
            ProjectNumber = projectNumber,
            Name = dto.Name.Trim(),
            NameEng = string.IsNullOrWhiteSpace(dto.NameEng) ? dto.Name.Trim() : dto.NameEng.Trim(),
            CompanyId = dto.CompanyId,
            BranchId = dto.BranchId,
            Type = dto.Type,
            Status = ProjectStatus.Draft,
            ManagerUserId = dto.ManagerUserId,
            ManagerName = dto.ManagerName,
            SourceOrderId = dto.SourceOrderId,
            SourceOrderNumber = dto.SourceOrderNumber,
            SourceOrderType = dto.SourceOrderType,
            PlannedStartDate = dto.PlannedStartDate.Date,
            PlannedEndDate = dto.PlannedEndDate.Date,
            ActualStartDate = dto.ActualStartDate,
            ActualEndDate = dto.ActualEndDate,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(ProjectDto dto, string modifiedBy)
    {
        ValidateDates(dto.PlannedStartDate, dto.PlannedEndDate);
        Name = dto.Name.Trim();
        NameEng = string.IsNullOrWhiteSpace(dto.NameEng) ? dto.Name.Trim() : dto.NameEng.Trim();
        BranchId = dto.BranchId;
        Type = dto.Type;
        ManagerUserId = dto.ManagerUserId;
        ManagerName = dto.ManagerName;
        SourceOrderId = dto.SourceOrderId;
        SourceOrderNumber = dto.SourceOrderNumber;
        SourceOrderType = dto.SourceOrderType;
        PlannedStartDate = dto.PlannedStartDate.Date;
        PlannedEndDate = dto.PlannedEndDate.Date;
        ActualStartDate = dto.ActualStartDate;
        ActualEndDate = dto.ActualEndDate;
        Notes = dto.Notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void ChangeStatus(ProjectStatus status, string modifiedBy)
    {
        Status = status;
        if (status == ProjectStatus.Active && ActualStartDate is null)
            ActualStartDate = DateTime.UtcNow;
        if (status == ProjectStatus.Completed && ActualEndDate is null)
            ActualEndDate = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Remove(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    private static void ValidateDates(DateTime plannedStartDate, DateTime plannedEndDate)
    {
        if (plannedEndDate.Date < plannedStartDate.Date)
            throw new BadRequestException("Project planned end date cannot be before the planned start date.");
    }
}

public class ProjectCustomer : Entity<Guid>
{
    private readonly List<ProjectCustomerProductPlan> _productPlans = [];

    public Guid ProjectId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string? CustomerNameEng { get; private set; }
    public Guid? SourceOrderId { get; private set; }
    public string? SourceOrderNumber { get; private set; }
    public decimal ContractedQuantity { get; private set; }
    public decimal ContractedAmount { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<ProjectCustomerProductPlan> ProductPlans => _productPlans;

    private ProjectCustomer() { }

    public static ProjectCustomer Create(Guid projectId, ProjectCustomerDto dto, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(dto.CustomerName))
            throw new BadRequestException("Customer name is required.");

        return new ProjectCustomer
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            CustomerId = dto.CustomerId,
            CustomerName = dto.CustomerName.Trim(),
            CustomerNameEng = dto.CustomerNameEng,
            SourceOrderId = dto.SourceOrderId,
            SourceOrderNumber = dto.SourceOrderNumber,
            ContractedQuantity = dto.ContractedQuantity,
            ContractedAmount = dto.ContractedAmount,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(ProjectCustomerDto dto, string modifiedBy)
    {
        CustomerId = dto.CustomerId;
        CustomerName = dto.CustomerName.Trim();
        CustomerNameEng = dto.CustomerNameEng;
        SourceOrderId = dto.SourceOrderId;
        SourceOrderNumber = dto.SourceOrderNumber;
        ContractedQuantity = dto.ContractedQuantity;
        ContractedAmount = dto.ContractedAmount;
        Notes = dto.Notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}

public class ProjectCustomerProductPlan : Entity<Guid>
{
    public Guid ProjectId { get; private set; }
    public Guid ProjectCustomerId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? ProductId { get; private set; }
    public string ProductSkuName { get; private set; } = string.Empty;
    public string? ProductSkuNameEng { get; private set; }
    public string? SkuCode { get; private set; }
    public string? SkuCodeEng { get; private set; }
    public Guid? ProductPackageId { get; private set; }
    public string? PackageName { get; private set; }
    public string? PackageNameEng { get; private set; }
    public decimal Quantity { get; private set; }
    public string? Notes { get; private set; }

    private ProjectCustomerProductPlan() { }

    public static ProjectCustomerProductPlan Create(Guid projectId, Guid projectCustomerId, ProjectCustomerProductPlanDto dto, string createdBy)
    {
        if (dto.ProductSkuId == Guid.Empty)
            throw new BadRequestException("Product SKU is required.");
        if (dto.Quantity <= 0)
            throw new BadRequestException("Quantity must be greater than zero.");
        if (string.IsNullOrWhiteSpace(dto.ProductSkuName))
            throw new BadRequestException("Product SKU name is required.");

        return new ProjectCustomerProductPlan
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ProjectCustomerId = projectCustomerId,
            ProductSkuId = dto.ProductSkuId,
            ProductId = dto.ProductId,
            ProductSkuName = dto.ProductSkuName.Trim(),
            ProductSkuNameEng = dto.ProductSkuNameEng,
            SkuCode = dto.SkuCode,
            SkuCodeEng = dto.SkuCodeEng,
            ProductPackageId = dto.ProductPackageId,
            PackageName = dto.PackageName,
            PackageNameEng = dto.PackageNameEng,
            Quantity = dto.Quantity,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}

public class ProjectDeliverable : Entity<Guid>
{
    public Guid ProjectId { get; private set; }
    public Guid ProductSkuId { get; private set; }
    public Guid? ProductId { get; private set; }
    public string ProductSkuName { get; private set; } = string.Empty;
    public string? ProductSkuNameEng { get; private set; }
    public MealHandlingType HandlingType { get; private set; }
    public decimal OrderedQuantity { get; private set; }
    public decimal PlannedQuantity { get; private set; }
    public decimal ProducedQuantity { get; private set; }
    public decimal ShippedQuantity { get; private set; }
    public string? Notes { get; private set; }

    private ProjectDeliverable() { }

    public static ProjectDeliverable Create(Guid projectId, ProjectDeliverableDto dto, string createdBy)
    {
        if (dto.ProductSkuId == Guid.Empty)
            throw new BadRequestException("Deliverable SKU is required.");
        if (dto.PlannedQuantity <= 0)
            throw new BadRequestException("Planned quantity must be greater than zero.");

        return new ProjectDeliverable
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ProductSkuId = dto.ProductSkuId,
            ProductId = dto.ProductId,
            ProductSkuName = dto.ProductSkuName.Trim(),
            ProductSkuNameEng = dto.ProductSkuNameEng,
            HandlingType = dto.HandlingType,
            OrderedQuantity = dto.OrderedQuantity,
            PlannedQuantity = dto.PlannedQuantity,
            ProducedQuantity = dto.ProducedQuantity,
            ShippedQuantity = dto.ShippedQuantity,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void AddProduced(decimal quantity, string modifiedBy)
    {
        ProducedQuantity += quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void AddShipped(decimal quantity, string modifiedBy)
    {
        ShippedQuantity += quantity;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}

public class DistributionPlace : Entity<Guid>
{
    public Guid CompanyId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string NameEng { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public decimal? Latitude { get; private set; }
    public decimal? Longitude { get; private set; }
    public string? ContactName { get; private set; }
    public string? ContactPhone { get; private set; }
    public bool IsActive { get; private set; } = true;

    private DistributionPlace() { }

    public static DistributionPlace Create(DistributionPlaceDto dto, string createdBy)
    {
        return new DistributionPlace
        {
            Id = Guid.NewGuid(),
            CompanyId = dto.CompanyId,
            Name = dto.Name.Trim(),
            NameEng = string.IsNullOrWhiteSpace(dto.NameEng) ? dto.Name.Trim() : dto.NameEng.Trim(),
            Address = dto.Address,
            City = dto.City,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            ContactName = dto.ContactName,
            ContactPhone = dto.ContactPhone,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void Update(DistributionPlaceDto dto, string modifiedBy)
    {
        Name = dto.Name.Trim();
        NameEng = string.IsNullOrWhiteSpace(dto.NameEng) ? dto.Name.Trim() : dto.NameEng.Trim();
        Address = dto.Address;
        City = dto.City;
        Latitude = dto.Latitude;
        Longitude = dto.Longitude;
        ContactName = dto.ContactName;
        ContactPhone = dto.ContactPhone;
        IsActive = dto.IsActive;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}

public class ProjectDistributionSchedule : Entity<Guid>
{
    private readonly List<ProjectDistributionAllocation> _allocations = [];

    public Guid ProjectId { get; private set; }
    public DateTime DistributionDate { get; private set; }
    public TimeOnly? WindowStart { get; private set; }
    public TimeOnly? WindowEnd { get; private set; }
    public DistributionStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<ProjectDistributionAllocation> Allocations => _allocations;

    private ProjectDistributionSchedule() { }

    public static ProjectDistributionSchedule Create(Guid projectId, ProjectDistributionScheduleDto dto, string createdBy)
    {
        return new ProjectDistributionSchedule
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            DistributionDate = dto.DistributionDate.Date,
            WindowStart = dto.WindowStart,
            WindowEnd = dto.WindowEnd,
            Status = dto.Status,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }
}

public class ProjectDistributionAllocation : Entity<Guid>
{
    public Guid ProjectId { get; private set; }
    public Guid ScheduleId { get; private set; }
    public DateTime DistributionDate { get; private set; }
    public Guid ProjectCustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public Guid DeliverableId { get; private set; }
    public string DeliverableName { get; private set; } = string.Empty;
    public Guid DistributionPlaceId { get; private set; }
    public string PlaceName { get; private set; } = string.Empty;
    public decimal PlannedQuantity { get; private set; }
    public decimal ShippedQuantity { get; private set; }
    public decimal DeliveredQuantity { get; private set; }
    public decimal ActualQuantity { get; private set; }
    public string? Notes { get; private set; }

    private ProjectDistributionAllocation() { }

    public static ProjectDistributionAllocation Create(
        Guid projectId,
        Guid scheduleId,
        DateTime distributionDate,
        ProjectDistributionAllocationDto dto,
        string createdBy)
    {
        if (dto.PlannedQuantity <= 0)
            throw new BadRequestException("Planned quantity must be greater than zero.");

        return new ProjectDistributionAllocation
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ScheduleId = scheduleId,
            DistributionDate = distributionDate.Date,
            ProjectCustomerId = dto.ProjectCustomerId,
            CustomerName = dto.CustomerName,
            DeliverableId = dto.DeliverableId,
            DeliverableName = dto.DeliverableName,
            DistributionPlaceId = dto.DistributionPlaceId,
            PlaceName = dto.PlaceName,
            PlannedQuantity = dto.PlannedQuantity,
            ShippedQuantity = dto.ShippedQuantity,
            DeliveredQuantity = dto.DeliveredQuantity,
            ActualQuantity = dto.ActualQuantity == 0 ? dto.PlannedQuantity : dto.ActualQuantity,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
    }

    public void RecordActuals(decimal shippedQuantity, decimal deliveredQuantity, decimal actualQuantity, string? notes, string modifiedBy)
    {
        ShippedQuantity = shippedQuantity;
        DeliveredQuantity = deliveredQuantity;
        ActualQuantity = actualQuantity;
        Notes = notes;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }
}
