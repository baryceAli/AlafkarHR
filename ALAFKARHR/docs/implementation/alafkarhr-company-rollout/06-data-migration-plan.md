# Data Migration Plan

## Objective

Move approved company HR data into AlafkarHR with traceable cleansing, loading, validation, and sign-off.

## Data Objects

| Data object | Required before | Key fields |
| --- | --- | --- |
| Company | Configuration | Company names, license/status where applicable, business lines. |
| Branches | Employee migration | Branch name/code, main branch, address, active status. |
| Administrations | Employee migration | Administration name, company, branch if applicable. |
| Departments | Employee migration | Department name, administration, branch, attendance location if used. |
| Positions | Employee migration | Position name, grade/category if used, company. |
| Teams | Optional before go-live | Team name, category, members. |
| Employees | UAT | Personal data, identity, contacts, employment, company, branch, administration, department, position, status. |
| Employee documents | UAT or post-go-live wave | Document type, number, issue/expiry dates, attachment/link. |
| Skills and certifications | Optional wave | Skill/certification name, level, issuer, dates. |
| Attendance | Configuration/UAT | Shifts, assignments, holidays, calendars, roster schedules. |
| Leave | Configuration/UAT | Leave types, periods, policies, assignments, balances, open applications. |
| Payroll | Payroll UAT | Components, contracts, structures, assignments, periods, loans, Saudi payroll info, bank/WPS data. |

## Migration Stages

| Stage | Activities | Exit criteria |
| --- | --- | --- |
| Extract | Export source data from legacy HR, spreadsheets, payroll, attendance devices, or manual files. | Source files versioned and owned. |
| Cleanse | Remove duplicates, normalize names, validate IDs, map old departments/positions to target structure. | Exception list reviewed. |
| Map | Map each source field to AlafkarHR target template. | Mapping approved by data owners. |
| Trial load | Load sample or full data into test environment/manual configuration set. | Load errors logged and resolved. |
| Reconcile | Compare counts and key values against source. | Reconciliation signed off. |
| Final load | Apply data freeze and load final approved files. | Final counts match approved totals. |
| Post-load validation | Business users validate records and workflows. | Data sign-off complete. |

## Cleansing Rules

- Every active employee must have a valid company, branch, department, and position.
- Employee status must be standardized to active, inactive, terminated, or other approved status supported by the system.
- Identity numbers, employee numbers, and email addresses must be unique where company policy requires uniqueness.
- Arabic and English names should be provided where the business requires bilingual reporting.
- Payroll data must be separated from general HR data and approved by payroll owner.
- Attachments should be linked using approved filenames and document types.

## Reconciliation

| Check | Tolerance |
| --- | --- |
| Employee count by company | 0 variance unless approved. |
| Employee count by branch | 0 variance unless approved. |
| Employee count by department | 0 variance unless approved. |
| Active/terminated counts | 0 variance unless approved. |
| Leave balance totals | Payroll/HR-approved variance only. |
| Payroll contract assignments | 0 variance for payroll go-live population. |
| Open loans | 0 variance for payroll go-live population. |

## Cutover Data Freeze

- Freeze organization, employee, leave, attendance, and payroll changes in legacy sources before final load.
- Record emergency changes in a cutover delta log.
- Apply deltas after final load and before go-live validation.

## Sign-Off

Data migration is complete only when HR, payroll, attendance, and IT owners sign the migration reconciliation and open exception log.
