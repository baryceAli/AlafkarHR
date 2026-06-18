namespace Fleet.Features;

internal static class FleetFeatureHelpers
{
    public static Guid GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException("User is not authorized.");

        return Guid.Parse(value);
    }

    public static FleetVehicleDto ToDto(FleetVehicle vehicle)
    {
        return new FleetVehicleDto
        {
            Id = vehicle.Id,
            VehicleCode = vehicle.VehicleCode,
            PlateNumber = vehicle.PlateNumber,
            Name = vehicle.Name,
            NameEng = vehicle.NameEng,
            Make = vehicle.Make,
            Model = vehicle.Model,
            Year = vehicle.Year,
            Color = vehicle.Color,
            Vin = vehicle.Vin,
            EngineNumber = vehicle.EngineNumber,
            VehicleType = vehicle.VehicleType,
            Status = vehicle.Status,
            OwnershipType = vehicle.OwnershipType,
            CompanyId = vehicle.CompanyId,
            BranchId = vehicle.BranchId,
            MaintenanceAssetId = vehicle.MaintenanceAssetId,
            PurchaseDate = vehicle.PurchaseDate,
            PurchaseCost = vehicle.PurchaseCost,
            WarrantyEndDate = vehicle.WarrantyEndDate,
            SupplierId = vehicle.SupplierId,
            RentalContractId = vehicle.RentalContractId,
            RentalStartDate = vehicle.RentalStartDate,
            RentalEndDate = vehicle.RentalEndDate,
            MonthlyRent = vehicle.MonthlyRent,
            DailyRent = vehicle.DailyRent,
            DepositAmount = vehicle.DepositAmount,
            AllowedKilometers = vehicle.AllowedKilometers,
            ExcessKilometerRate = vehicle.ExcessKilometerRate,
            CurrentOdometer = vehicle.CurrentOdometer,
            FuelType = vehicle.FuelType,
            FuelCapacity = vehicle.FuelCapacity,
            DefaultDriverEmployeeId = vehicle.DefaultDriverEmployeeId,
            Notes = vehicle.Notes,
            CreatedAt = vehicle.CreatedAt
        };
    }

    public static FleetVehicleAssignmentDto ToDto(FleetVehicleAssignment assignment)
    {
        return new FleetVehicleAssignmentDto
        {
            Id = assignment.Id,
            VehicleId = assignment.VehicleId,
            VehicleName = assignment.Vehicle?.Name ?? string.Empty,
            PlateNumber = assignment.Vehicle?.PlateNumber ?? string.Empty,
            EmployeeId = assignment.EmployeeId,
            UserId = assignment.UserId,
            BranchId = assignment.BranchId,
            DepartmentId = assignment.DepartmentId,
            StartDate = assignment.StartDate,
            EndDate = assignment.EndDate,
            Purpose = assignment.Purpose,
            OdometerOut = assignment.OdometerOut,
            OdometerIn = assignment.OdometerIn,
            FuelLevelOut = assignment.FuelLevelOut,
            FuelLevelIn = assignment.FuelLevelIn,
            Status = assignment.Status
        };
    }

    public static FleetVehicleDocumentDto ToDto(FleetVehicleDocument document)
    {
        return new FleetVehicleDocumentDto
        {
            Id = document.Id,
            VehicleId = document.VehicleId,
            VehicleName = document.Vehicle?.Name ?? string.Empty,
            DocumentType = document.DocumentType,
            DocumentNumber = document.DocumentNumber,
            IssueDate = document.IssueDate,
            ExpiryDate = document.ExpiryDate,
            RenewalCost = document.RenewalCost,
            SupplierId = document.SupplierId,
            FileName = document.FileName,
            FilePath = document.FilePath,
            ContentType = document.ContentType,
            FileSize = document.FileSize,
            Status = document.Status,
            Notes = document.Notes
        };
    }

    public static FleetVehicleExpenseDto ToDto(FleetVehicleExpense expense)
    {
        return new FleetVehicleExpenseDto
        {
            Id = expense.Id,
            VehicleId = expense.VehicleId,
            VehicleName = expense.Vehicle?.Name ?? string.Empty,
            PlateNumber = expense.Vehicle?.PlateNumber ?? string.Empty,
            ExpenseDate = expense.ExpenseDate,
            Category = expense.Category,
            Amount = expense.Amount,
            CurrencyCode = expense.CurrencyCode,
            SupplierId = expense.SupplierId,
            VendorName = expense.VendorName,
            Odometer = expense.Odometer,
            Quantity = expense.Quantity,
            UnitPrice = expense.UnitPrice,
            Notes = expense.Notes,
            FileName = expense.FileName,
            FilePath = expense.FilePath,
            MaintenanceWorkOrderId = expense.MaintenanceWorkOrderId,
            ContractId = expense.ContractId,
            ProcurementDocumentId = expense.ProcurementDocumentId,
            PaymentReferenceId = expense.PaymentReferenceId,
            ApprovalStatus = expense.ApprovalStatus
        };
    }

    public static FleetVehicleServiceRuleDto ToDto(FleetVehicleServiceRule rule)
    {
        return new FleetVehicleServiceRuleDto
        {
            Id = rule.Id,
            VehicleId = rule.VehicleId,
            VehicleName = rule.Vehicle?.Name ?? string.Empty,
            ServiceType = rule.ServiceType,
            IntervalKilometers = rule.IntervalKilometers,
            IntervalDays = rule.IntervalDays,
            LastServiceOdometer = rule.LastServiceOdometer,
            LastServiceDate = rule.LastServiceDate,
            NextDueOdometer = rule.NextDueOdometer,
            NextDueDate = rule.NextDueDate,
            IsActive = rule.IsActive,
            IsDue = rule.Vehicle is not null && rule.IsDue(rule.Vehicle.CurrentOdometer, DateTime.UtcNow),
            Notes = rule.Notes
        };
    }

    public static async Task EnsureVehicleAsync(FleetDbContext dbContext, Guid vehicleId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Vehicles.AnyAsync(x => x.Id == vehicleId, cancellationToken);
        if (!exists)
            throw new NotFoundException("Fleet vehicle", vehicleId);
    }
}
