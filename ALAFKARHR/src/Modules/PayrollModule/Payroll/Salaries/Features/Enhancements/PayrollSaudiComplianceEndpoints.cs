namespace Payroll.Salaries.Features.Enhancements;

public record SaudiPayrollInfoRequest(SaudiPayrollInfoUpsertDto SaudiPayrollInfo);
public record CreateWpsBatchRequest(CreateWpsBatchDto Batch);
public record CreateEosSnapshotRequest(CreateEosProvisionSnapshotDto Snapshot);
public record PayrollWorkEntryImportRequest(PayrollWorkEntryImportDto WorkEntry);

public class PayrollSaudiComplianceEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var payroll = app.MapGroup("/api/v1/payroll");

        payroll.MapGet("/saudi-payroll/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new ListSaudiPayrollInfoQuery(companyId))))
            .WithName("ListSaudiPayrollInfo")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapGet("/saudi-payroll/company/{companyId:guid}/employee/{employeeId:guid}", async (Guid companyId, Guid employeeId, ISender sender) =>
            Results.Ok(await sender.Send(new GetSaudiPayrollInfoQuery(companyId, employeeId))))
            .WithName("GetSaudiPayrollInfo")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapPost("/saudi-payroll", async (SaudiPayrollInfoRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertSaudiPayrollInfoCommand(request.SaudiPayrollInfo))))
            .WithName("UpsertSaudiPayrollInfo")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapGet("/wps-batches/company/{companyId:guid}", async (Guid companyId, Guid? payrollPeriodId, ISender sender) =>
            Results.Ok(await sender.Send(new ListWpsBatchesQuery(companyId, payrollPeriodId))))
            .WithName("ListWpsBatches")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapPost("/wps-batches", async (CreateWpsBatchRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new CreateWpsBatchCommand(request.Batch))))
            .WithName("CreateWpsBatch")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPost("/wps-batches/{id:guid}/mark-exported", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new MarkWpsBatchExportedCommand(id))))
            .WithName("MarkWpsBatchExported")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Close);

        payroll.MapGet("/eos-snapshots/company/{companyId:guid}", async (Guid companyId, Guid? payrollPeriodId, Guid? employeeId, ISender sender) =>
            Results.Ok(await sender.Send(new ListEosProvisionSnapshotsQuery(companyId, payrollPeriodId, employeeId))))
            .WithName("ListEosProvisionSnapshots")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapPost("/eos-snapshots", async (CreateEosSnapshotRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new CreateEosProvisionSnapshotCommand(request.Snapshot))))
            .WithName("CreateEosProvisionSnapshot")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPost("/payroll-entries/{id:guid}/post-accounting", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PostPayrollEntryAccountingCommand(id))))
            .WithName("PostPayrollEntryAccounting")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Close);

        payroll.MapGet("/payroll-work-entries/company/{companyId:guid}", async (Guid companyId, Guid? payrollPeriodId, Guid? employeeId, ISender sender) =>
            Results.Ok(await sender.Send(new ListImportedPayrollWorkEntriesQuery(companyId, payrollPeriodId, employeeId))))
            .WithName("ListImportedPayrollWorkEntries")
            .RequireAuthorization(PermissionList.PayrollWorkEntryPermissions.View);

        payroll.MapPost("/payroll-work-entries/import", async (PayrollWorkEntryImportRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new ImportPayrollWorkEntryCommand(request.WorkEntry))))
            .WithName("ImportPayrollWorkEntry")
            .RequireAuthorization(PermissionList.PayrollWorkEntryPermissions.Import);
    }
}
