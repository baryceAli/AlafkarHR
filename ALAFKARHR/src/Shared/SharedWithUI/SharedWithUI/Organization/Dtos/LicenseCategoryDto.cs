namespace SharedWithUI.Organization.Dtos;

public class LicenseCategoryDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MaxUsers { get; set; }
    public int MaxChildCompanies { get; set; }
    public int MaxBranches { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public string CurrencyCode { get; set; } = "SAR";
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
