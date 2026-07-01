# Cutover Plan

## Cutover Objective

Move the company from legacy HR operations to AlafkarHR with controlled data freeze, final validation, user enablement, and rollback decision points.

## Cutover Roles

| Role | Responsibility |
| --- | --- |
| Cutover Manager | Owns cutover plan, timing, decisions, and status. |
| HR Data Owner | Approves final employee and leave data. |
| Payroll Owner | Approves payroll data and readiness. |
| IT Lead | Enables users, validates roles, resolves access issues. |
| Implementation Lead | Executes configuration and validation support. |
| Business Testers | Validate final production records. |

## Sequence

| Step | Activity | Owner | Status |
| --- | --- | --- | --- |
| 1 | Announce freeze window. | HR Sponsor | Not started |
| 2 | Export final source data. | Data Owners | Not started |
| 3 | Apply final data cleansing and delta log. | Data Owners | Not started |
| 4 | Load or enter final organization and employee data. | Implementation/HR | Not started |
| 5 | Validate employee counts and key attributes. | HR | Not started |
| 6 | Validate leave balances and open requests. | HR | Not started |
| 7 | Validate attendance shifts and assignments. | Attendance Lead | Not started |
| 8 | Validate payroll contracts, structures, loans, periods. | Payroll Lead | Not started |
| 9 | Validate roles, users, branch access. | IT Lead | Not started |
| 10 | Send go-live communication. | HR/IT | Not started |
| 11 | Enable production usage. | IT Lead | Not started |
| 12 | Start hypercare. | Support Lead | Not started |

## Rollback Decision Points

Rollback or go-live delay must be considered if:

- Active employee data has material unreconciled variance.
- Payroll go-live population is incomplete.
- Critical permissions expose confidential employee or payroll data.
- Attendance or leave workflows cannot be completed by target users.
- Production access is unavailable for critical roles.

## Post-Go-Live Day 1 Checks

- Users can log in.
- Menus reflect assigned roles.
- HR can find employee records.
- Employees can access assigned self-service pages.
- Managers can access approval pages.
- Payroll can view required payroll setup.
- Support channel is receiving and triaging issues.
