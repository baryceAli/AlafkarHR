using LeaveManagement.Leave.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagement.Data.Configurations;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.ToTable("LeaveTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(40);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEng).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NegativeBalanceLimit).HasPrecision(8, 2);
        builder.HasIndex(x => new { x.CompanyId, x.Code }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class LeavePeriodConfiguration : IEntityTypeConfiguration<LeavePeriod>
{
    public void Configure(EntityTypeBuilder<LeavePeriod> builder)
    {
        builder.ToTable("LeavePeriods");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.HasIndex(x => new { x.CompanyId, x.StartDate, x.EndDate });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class LeavePolicyConfiguration : IEntityTypeConfiguration<LeavePolicy>
{
    public void Configure(EntityTypeBuilder<LeavePolicy> builder)
    {
        builder.ToTable("LeavePolicies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.NameEng).IsRequired().HasMaxLength(200);
        builder.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey(x => x.LeavePolicyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Metadata.FindNavigation(nameof(LeavePolicy.Lines))?.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(x => new { x.CompanyId, x.Name }).IsUnique().HasFilter("[IsDeleted] = 0");
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class LeavePolicyLineConfiguration : IEntityTypeConfiguration<LeavePolicyLine>
{
    public void Configure(EntityTypeBuilder<LeavePolicyLine> builder)
    {
        builder.ToTable("LeavePolicyLines");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AnnualAllocationDays).HasPrecision(8, 2);
        builder.Property(x => x.MaxCarryForwardDays).HasPrecision(8, 2);
        builder.HasIndex(x => new { x.LeavePolicyId, x.LeaveTypeId }).IsUnique();
    }
}

public class LeavePolicyAssignmentConfiguration : IEntityTypeConfiguration<LeavePolicyAssignment>
{
    public void Configure(EntityTypeBuilder<LeavePolicyAssignment> builder)
    {
        builder.ToTable("LeavePolicyAssignments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Target).HasConversion<int>();
        builder.HasIndex(x => new { x.CompanyId, x.PolicyId, x.Target, x.EmployeeId, x.DepartmentId, x.EffectiveFrom });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class LeaveApplicationConfiguration : IEntityTypeConfiguration<LeaveApplication>
{
    public void Configure(EntityTypeBuilder<LeaveApplication> builder)
    {
        builder.ToTable("LeaveApplications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TotalDays).HasPrecision(8, 2);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.Reason).HasMaxLength(1000);
        builder.Property(x => x.AttachmentPath).HasMaxLength(500);
        builder.Property(x => x.ApproverUserId).HasMaxLength(100);
        builder.Property(x => x.ApproverComment).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.Status, x.StartDate });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class LeaveLedgerEntryConfiguration : IEntityTypeConfiguration<LeaveLedgerEntry>
{
    public void Configure(EntityTypeBuilder<LeaveLedgerEntry> builder)
    {
        builder.ToTable("LeaveLedgerEntries");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EntryType).HasConversion<int>();
        builder.Property(x => x.Days).HasPrecision(8, 2);
        builder.Property(x => x.BalanceAfter).HasPrecision(8, 2);
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.LeaveTypeId, x.PostingDate });
        builder.HasIndex(x => new { x.SourceDocumentId, x.EntryType });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class LeaveEncashmentRequestConfiguration : IEntityTypeConfiguration<LeaveEncashmentRequest>
{
    public void Configure(EntityTypeBuilder<LeaveEncashmentRequest> builder)
    {
        builder.ToTable("LeaveEncashmentRequests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Days).HasPrecision(8, 2);
        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.HasIndex(x => new { x.CompanyId, x.EmployeeId, x.LeaveTypeId });
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
