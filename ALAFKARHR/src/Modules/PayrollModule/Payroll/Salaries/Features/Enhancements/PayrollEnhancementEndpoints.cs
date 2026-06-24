namespace Payroll.Salaries.Features.Enhancements;

public record SalaryStructureRequest(SalaryStructureUpsertDto Structure);
public record SalaryStructureAssignmentRequest(SalaryStructureAssignmentUpsertDto Assignment);
public record PayrollPeriodRequest(PayrollPeriodUpsertDto Period);
public record PayrollEntryRequest(PayrollEntryCreateDto Entry);
public record PayrollInputRequest(PayrollInputUpsertDto Input);
public record EndAssignmentRequest(DateTime EffectiveTo);

public class PayrollEnhancementEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var payroll = app.MapGroup("/api/v1/payroll");

        payroll.MapGet("/salary-structures/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new ListSalaryStructuresQuery(companyId))))
            .WithName("ListSalaryStructures")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.View);

        payroll.MapPost("/salary-structures", async (SalaryStructureRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertSalaryStructureCommand(request.Structure))))
            .WithName("CreateSalaryStructure")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.Create);

        payroll.MapPut("/salary-structures/{id:guid}", async (Guid id, SalaryStructureRequest request, ISender sender) =>
        {
            request.Structure.Id = id;
            return Results.Ok(await sender.Send(new UpsertSalaryStructureCommand(request.Structure)));
        })
            .WithName("UpdateSalaryStructure")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.Edit);

        payroll.MapPost("/salary-structures/{id:guid}/activate", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new SetSalaryStructureStatusCommand(id, true))))
            .WithName("ActivateSalaryStructure")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.Edit);

        payroll.MapPost("/salary-structures/{id:guid}/deactivate", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new SetSalaryStructureStatusCommand(id, false))))
            .WithName("DeactivateSalaryStructure")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.Edit);

        payroll.MapGet("/salary-structure-assignments/company/{companyId:guid}", async (Guid companyId, Guid? employeeId, ISender sender) =>
            Results.Ok(await sender.Send(new ListSalaryStructureAssignmentsQuery(companyId, employeeId))))
            .WithName("ListSalaryStructureAssignments")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.View);

        payroll.MapPost("/salary-structure-assignments", async (SalaryStructureAssignmentRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertSalaryStructureAssignmentCommand(request.Assignment))))
            .WithName("CreateSalaryStructureAssignment")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.Create);

        payroll.MapPut("/salary-structure-assignments/{id:guid}", async (Guid id, SalaryStructureAssignmentRequest request, ISender sender) =>
        {
            request.Assignment.Id = id;
            return Results.Ok(await sender.Send(new UpsertSalaryStructureAssignmentCommand(request.Assignment)));
        })
            .WithName("UpdateSalaryStructureAssignment")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.Edit);

        payroll.MapPost("/salary-structure-assignments/{id:guid}/end", async (Guid id, EndAssignmentRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new EndSalaryStructureAssignmentCommand(id, request.EffectiveTo))))
            .WithName("EndSalaryStructureAssignment")
            .RequireAuthorization(PermissionList.PayrollStructurePermissions.Edit);

        payroll.MapGet("/payroll-periods/company/{companyId:guid}", async (Guid companyId, ISender sender) =>
            Results.Ok(await sender.Send(new ListPayrollPeriodsQuery(companyId))))
            .WithName("ListPayrollPeriods")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapPost("/payroll-periods", async (PayrollPeriodRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertPayrollPeriodCommand(request.Period))))
            .WithName("CreatePayrollPeriod")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPut("/payroll-periods/{id:guid}", async (Guid id, PayrollPeriodRequest request, ISender sender) =>
        {
            request.Period.Id = id;
            return Results.Ok(await sender.Send(new UpsertPayrollPeriodCommand(request.Period)));
        })
            .WithName("UpdatePayrollPeriod")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPost("/payroll-periods/{id:guid}/open", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new SetPayrollPeriodStatusCommand(id, PayrollPeriodStatus.Open))))
            .WithName("OpenPayrollPeriod")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPost("/payroll-periods/{id:guid}/close", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new SetPayrollPeriodStatusCommand(id, PayrollPeriodStatus.Closed))))
            .WithName("ClosePayrollPeriod")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapPost("/payroll-periods/{id:guid}/reopen", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new SetPayrollPeriodStatusCommand(id, PayrollPeriodStatus.Open))))
            .WithName("ReopenPayrollPeriod")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapGet("/payroll-entries/company/{companyId:guid}", async (Guid companyId, Guid? payrollPeriodId, ISender sender) =>
            Results.Ok(await sender.Send(new ListPayrollEntriesQuery(companyId, payrollPeriodId))))
            .WithName("ListPayrollEntries")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapPost("/payroll-entries", async (PayrollEntryRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new CreatePayrollEntryCommand(request.Entry))))
            .WithName("CreatePayrollEntry")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPost("/payroll-entries/{id:guid}/generate", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayrollEntryActionCommand(id, "generate"))))
            .WithName("GeneratePayrollEntryPayslips")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPost("/payroll-entries/{id:guid}/approve", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayrollEntryActionCommand(id, "approve"))))
            .WithName("ApprovePayrollEntry")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapPost("/payroll-entries/{id:guid}/close", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayrollEntryActionCommand(id, "close"))))
            .WithName("ClosePayrollEntry")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapPost("/payroll-entries/{id:guid}/reopen", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayrollEntryActionCommand(id, "reopen"))))
            .WithName("ReopenPayrollEntry")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapPost("/payroll-entries/{id:guid}/cancel", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayrollEntryActionCommand(id, "cancel"))))
            .WithName("CancelPayrollEntry")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapGet("/payslips/company/{companyId:guid}", async (Guid companyId, Guid? payrollEntryId, Guid? employeeId, ISender sender) =>
            Results.Ok(await sender.Send(new ListPayslipsQuery(companyId, payrollEntryId, employeeId))))
            .WithName("ListPayslips")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapGet("/payslips/{id:guid}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new GetPayslipQuery(id))))
            .WithName("GetPayslip")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapPost("/payslips/{id:guid}/recalculate", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayslipActionCommand(id, "recalculate"))))
            .WithName("RecalculatePayslip")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPost("/payslips/{id:guid}/approve", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayslipActionCommand(id, "approve"))))
            .WithName("ApprovePayslip")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapPost("/payslips/{id:guid}/mark-paid", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayslipActionCommand(id, "paid"))))
            .WithName("MarkPayslipPaid")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapPost("/payslips/{id:guid}/cancel", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new PayslipActionCommand(id, "cancel"))))
            .WithName("CancelPayslip")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Approve);

        payroll.MapGet("/payroll-inputs/company/{companyId:guid}", async (Guid companyId, Guid? payrollPeriodId, Guid? employeeId, ISender sender) =>
            Results.Ok(await sender.Send(new ListPayrollInputsQuery(companyId, payrollPeriodId, employeeId))))
            .WithName("ListPayrollInputs")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.View);

        payroll.MapPost("/payroll-inputs", async (PayrollInputRequest request, ISender sender) =>
            Results.Ok(await sender.Send(new UpsertPayrollInputCommand(request.Input))))
            .WithName("CreatePayrollInput")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapPut("/payroll-inputs/{id:guid}", async (Guid id, PayrollInputRequest request, ISender sender) =>
        {
            request.Input.Id = id;
            return Results.Ok(await sender.Send(new UpsertPayrollInputCommand(request.Input)));
        })
            .WithName("UpdatePayrollInput")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);

        payroll.MapDelete("/payroll-inputs/{id:guid}", async (Guid id, ISender sender) =>
            Results.Ok(await sender.Send(new DeletePayrollInputCommand(id))))
            .WithName("DeletePayrollInput")
            .RequireAuthorization(PermissionList.PayrollPayslipPermissions.Generate);
    }
}
