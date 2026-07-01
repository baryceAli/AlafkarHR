# User Guide: Payroll Officer

## Role Purpose

The Payroll Officer configures payroll masters, assigns payroll data to employees, runs payroll, reviews payslips, manages loans, handles WPS/Saudi payroll information, and coordinates payroll accounting posting where in scope.

## Main Pages

- Payroll components: `/Payroll/Components`
- Contracts: `/Payroll/Contracts`
- Assign contracts: `/Payroll/AssignContract`
- Salary runs: `/Payroll/SalaryRuns`
- Employee loans: `/Payroll/Loans`
- Salary structures: `/HR/PayrollStructures`
- Work entries: `/HR/WorkEntries`
- Payslips: `/HR/Payslips`
- Saudi payroll: `/HR/SaudiPayroll`

## Required Permissions

- `Payroll.Contract.*`
- `Payroll.Loan.*`
- `Payroll.SalaryRun.*`
- `Payroll.Structure.*`
- `Payroll.Payslip.*`
- `Payroll.WorkEntry.*`
- View access to employee and attendance data as approved.
- Finance/accounting permissions if payroll posting is part of the role.

## Payroll Setup Workflow

1. Confirm employee records and assignments are approved.
2. Create payroll components.
3. Create contracts.
4. Assign employee contracts.
5. Create salary structures and assignments if used.
6. Create payroll periods.
7. Enter Saudi payroll and WPS information where required.
8. Validate employee loans.

## Monthly Payroll Workflow

1. Confirm attendance work entries and leave impacts are ready.
2. Create payroll entry or salary runs for the period.
3. Generate or calculate payroll.
4. Review payroll inputs, loans, deductions, and allowances.
5. Review payslips.
6. Recalculate if corrections are required.
7. Approve payroll output.
8. Mark payslips paid where applicable.
9. Create/export WPS batch if in scope.
10. Post payroll accounting if finance integration is approved.

## Employee Loan Workflow

1. Open `/Payroll/Loans`.
2. Create loan for employee.
3. Enter loan type, amount, schedule, and notes.
4. Submit or save according to company process.
5. Approve loan if authorized.
6. Cancel only according to approved payroll policy.

## Salary Run Workflow

1. Open `/Payroll/SalaryRuns`.
2. Create salary run for employee/contract/month/year.
3. Calculate salary run.
4. Review result.
5. Approve when correct.
6. Commit period after all runs are approved.
7. Undo salary run or period only with documented approval.

## Common Mistakes

| Mistake | Avoidance |
| --- | --- |
| Running payroll before employee data is approved. | Confirm HR data sign-off first. |
| Missing contract assignment. | Review employee contract list before payroll. |
| Approving payroll before leave/attendance validation. | Require attendance and leave cut-off confirmation. |
| Undoing committed payroll without approval. | Use payroll change approval. |
| Incorrect WPS/bank data. | Validate IBAN and bank fields before payroll. |

## Troubleshooting

| Symptom | Check |
| --- | --- |
| Employee missing from payroll. | Employee status, contract assignment, salary structure, branch access. |
| Payslip amount is wrong. | Components, payroll inputs, loans, work entries, leave impact. |
| Cannot approve payroll. | `Payroll.SalaryRun.Approve` or `Payroll.Payslip.Approve` permission. |
| WPS data missing. | Saudi payroll info and bank details. |

## Escalation

Escalate employee master data issues to HR. Escalate attendance work-entry issues to Attendance Officer. Escalate accounting posting issues to Finance Reviewer. Escalate access issues to IT.
