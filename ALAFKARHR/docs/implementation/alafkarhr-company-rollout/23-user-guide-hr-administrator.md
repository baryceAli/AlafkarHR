# User Guide: HR Administrator

## Role Purpose

The HR Administrator owns employee master data, HR structure, employee lifecycle records, documents, skills, leave setup, leave balances, and HR reporting.

## Main Pages

- Employee dashboard: `/Employee/Dashboard`
- Employee list/form/view: `/Employee/Employee/List`, `/Employee/Employee/Form/{Id?}`, `/Employee/Employee/view/{Id}`, `/Employee/Employee/360/{Id}`
- HR command center: `/HR/CommandCenter`
- Lifecycle: `/HR/EmployeeLifecycle`
- Documents: `/HR/EmployeeDocuments`
- Skills: `/HR/EmployeeSkills`
- Emergency contacts: `/HR/EmployeeEmergencyContacts`
- Positions: `/Employee/Position/List`
- Teams: `/Employee/Teams`
- Academic institutions: `/Employee/AcademicInistitution/List`
- Specializations: `/Employee/Specialization/List`
- Leave setup and operations: `/HR/LeavePolicies`, `/HR/LeaveApplications`, `/HR/LeaveLedger`, `/LeavesManagement/Balances`, `/LeavesManagement/Reports`

## Required Permissions

- `Employees.Employee.*`
- `Employees.Team.*`
- `Employees.Position.*`
- `Employees.AcademicInistitution.*`
- `Employees.Specialization.*`
- `Employees.Lifecycle.*`
- `Employees.Document.*`
- `Employees.Skill.*`
- `Leave.Policy.*`
- `Leave.Application.*`
- `Leave.Ledger.*`
- `Leave.Leave.*` as approved

## Employee Record Workflow

1. Confirm branch, administration, department, and position exist.
2. Open `/Employee/Employee/List`.
3. Create or edit employee.
4. Fill personal, identity, contact, and employment assignment fields.
5. Save and verify the employee in list and view pages.
6. Add emergency contacts, documents, skills, certifications, and lifecycle events if required.

## Employee Change Workflow

| Change | Where to perform | Validation |
| --- | --- | --- |
| Position change | Employee form or employee action where available. | New position is active and approved. |
| Department transfer | Employee action/service-backed workflow. | New department belongs to correct company/branch. |
| Termination | Employee termination workflow. | Effective date, reason, payroll impact approved. |
| Document renewal | `/HR/EmployeeDocuments` | Expiry and attachment/link updated. |
| Skill verification | `/HR/EmployeeSkills` | Verification authority approved. |

## Leave Administration

1. Maintain leave types, periods, and policies in `/HR/LeavePolicies`.
2. Assign leave policies to employees or groups as approved.
3. Generate or enter allocations.
4. Maintain balances in `/LeavesManagement/Balances`.
5. Review applications in `/HR/LeaveApplications`.
6. Use `/HR/LeaveLedger` for adjustments and encashment when approved.
7. Use `/LeavesManagement/Reports` for monitoring.

## Data Entry Rules

- Use approved employee numbers and identity values.
- Do not create duplicate employees.
- Keep company, branch, administration, department, and position aligned.
- Attachments or document links must follow company naming rules.
- Do not adjust leave balances without approval and notes.

## Common Mistakes

| Mistake | Avoidance |
| --- | --- |
| Creating employee before department or position exists. | Complete organization setup first. |
| Updating payroll-sensitive fields without payroll notice. | Notify payroll before changes affecting salary or employment status. |
| Leaving documents without expiry dates. | Capture issue/expiry dates where applicable. |
| Manual leave balance changes without approval. | Use sign-off and keep notes. |

## Troubleshooting

| Symptom | Check |
| --- | --- |
| Employee missing from list. | Company/branch filters, status, search text, permission. |
| Cannot edit employee. | `Employees.Employee.Edit` permission. |
| Cannot see leave setup. | Leave policy/application/ledger permissions. |
| Manager cannot see employee. | Manager role, branch access, team/department scope. |

## Escalation

Escalate access issues to IT. Escalate policy conflicts to HR Sponsor. Escalate payroll-sensitive corrections to Payroll Lead.
