# End-to-End UAT Time Estimate and Execution Plan

## Summary

This plan estimates the effort required to conduct full end-to-end UAT for the priority modules:

- HR Attendance
- HR Leaves
- Salary / Payroll
- HR Employees
- Task Management
- Organization

The estimate is based on the existing UAT pack in `docs/uat`, especially `UAT_Master_Matrix.csv`, which contains 135 relevant UAT rows for the selected modules.

Estimated calendar duration:

| Tester Model | Estimated Duration | Best Use |
| --- | ---: | --- |
| 1 tester | 22-26 working days | Small UAT team, slower execution, simpler coordination. |
| 2 testers | 12-15 working days | Recommended option for balanced speed and daily defect triage. |

If many blocking defects appear, add a 20-30% contingency for fixes, retesting, and sign-off delays.

## Estimation Basis

The estimate assumes full UAT coverage, not a quick smoke test. Each selected module should include:

- Positive happy-path validation.
- Invalid input or invalid status validation where relevant.
- Permission-denied validation.
- Arabic / English and RTL / LTR smoke checks.
- Evidence capture for each test case.
- Defect logging and retesting.
- Business owner sign-off.

Baseline UAT matrix counts:

| Module | UAT Rows | Relative Complexity |
| --- | ---: | --- |
| Organization | 31 | High: company, branch, department, administration, business line, licensing, access scope. |
| HR Employees | 25 | Medium: employee records, lifecycle, teams, positions, specializations, reporting. |
| Attendance + Leaves | 41 | High: shifts, attendance sessions, permissions, emergency leaves, balances, approvals, reports. |
| Salary / Payroll | 22 | High: contracts, salary components, salary generation, loans, approval/status behavior. |
| Task Management | 16 | Medium: tasks, assignment, workflow status, Kanban, reports, comments, attachments. |

## Time Estimate by Module

| Module | UAT Rows Found | 1 Tester Estimate | 2 Tester Calendar Estimate | Notes |
| --- | ---: | ---: | ---: | --- |
| Organization | 31 | 4-5 days | 2.5-3 days | Validate organization structure, branch/company scope, departments, administrations, business lines, licenses, and permissions. |
| HR Employees | 25 | 3-4 days | 2 days | Validate employee records, employee assignment, employee lifecycle, teams, positions, specializations, and reports. |
| Attendance + Leaves | 41 | 5-6 days | 3-4 days | Highest workflow risk. Validate shifts, sessions, check-in flows, permission requests, leave balances, emergency leave, approvals, reports, RTL, and permissions. |
| Salary / Payroll | 22 | 4-5 days | 2.5-3 days | Validate payroll components, contracts, assigned contracts, salary generation, loans, approval states, and Saudi payroll checks. |
| Task Management | 16 | 2-3 days | 1.5-2 days | Validate task create/edit/assign flows, Kanban, dashboard, reports, comments, attachments, notifications, and status changes. |
| Shared setup, permissions, evidence, retest, sign-off | - | 4-5 days | 3-4 days | Test data, roles, bilingual smoke, evidence review, defect retest, and final sign-off. |
| **Total** | **135** | **22-26 days** | **12-15 days** | Assumes approximately 6 productive UAT hours per tester per day. |

## Execution Plan

### Preparation: 1-2 Days

Prepare the environment before module execution starts.

Activities:

- Confirm that the UAT environment is stable and points to the correct database.
- Confirm that test users exist for Admin, HR Manager, Employee, Approver, and No-Permission roles.
- Prepare company, branch, administration, department, employee, payroll, attendance, leave, and task test data.
- Verify that the required menus and routes are accessible.
- Confirm where screenshots, notes, and defect evidence will be stored.
- Review `UI_Coverage_Gaps.csv`; any item marked as not represented in UI must be clarified before execution.

Exit criteria:

- Test users can sign in.
- Required master data exists.
- Priority modules are reachable from the UI.
- UAT tracker columns are ready: Result, Evidence, Notes, Defect ID, Retest Result, and Sign-off.

### Wave 1: Organization

Recommended order:

1. Company and parent/child company setup.
2. Branch creation and branch update.
3. Department and administration setup.
4. Business line and license setup.
5. Branch/company access and permissions.

Key validations:

- Organization hierarchy is visible and consistent.
- Branch and department data belongs to the correct company.
- Users only see and act on allowed company/branch data.
- Permission-denied users cannot access restricted routes or actions.
- Arabic and English labels remain usable.

Estimated duration:

- 1 tester: 4-5 days.
- 2 testers: 2.5-3 days.

### Wave 2: HR Employees

Recommended order:

1. Employee creation and edit.
2. Employee assignment to company, branch, administration, and department.
3. Employee teams, positions, and specializations.
4. Employee lifecycle and 360-degree reporting.
5. Employee permissions and restricted access checks.

Key validations:

- Employee data saves correctly and appears in lists and reports.
- Organization placement is enforced correctly.
- Employee updates do not break downstream Attendance, Leave, or Payroll setup.
- Unauthorized users cannot view or mutate employee data.

Estimated duration:

- 1 tester: 3-4 days.
- 2 testers: 2 days.

### Wave 3: Attendance + Leaves

Recommended order:

1. Attendance configuration, shifts, and shift assignment.
2. Attendance sessions, check-in, check-out, and break flows.
3. Late check-in and mid-day permission request workflows.
4. Holidays and attendance reports.
5. Leave policies, leave applications, balances, emergency leave, and approvals.
6. Employee self-service views and manager/approver views.

Key validations:

- Attendance works for the right employee, branch, shift, and date context.
- Invalid location, invalid status, and invalid workflow transitions are handled correctly.
- Leave balance and approval flows are reflected in the UI.
- Reports reflect the latest approved/recorded transactions.
- Permission-denied users cannot access approval or restricted reporting actions.

Estimated duration:

- 1 tester: 5-6 days.
- 2 testers: 3-4 days.

### Wave 4: Salary / Payroll

Recommended order:

1. Payroll components and salary contract setup.
2. Employee contract assignment.
3. Employee loans and deductions.
4. Salary generation.
5. Salary run review and approval/status checks.
6. Saudi payroll checks, including WPS/end-of-service readiness where available.

Key validations:

- Payroll components and contracts calculate as expected.
- Only eligible employees are included in salary generation.
- Loans and deductions appear correctly.
- Payroll statuses prevent invalid next actions.
- Payroll data is isolated by company and relevant employee scope.

Estimated duration:

- 1 tester: 4-5 days.
- 2 testers: 2.5-3 days.

### Wave 5: Task Management

Recommended order:

1. Task creation.
2. Task edit and assignment.
3. Task status changes and progress updates.
4. Comments and attachments.
5. My Tasks, Kanban, dashboard, reports, and notifications.
6. Permission-denied cases.

Key validations:

- Task lifecycle works from creation through completion or closure.
- Assigned users see the correct tasks.
- Dashboard and reports reflect task changes.
- Restricted users cannot access unauthorized task operations.

Estimated duration:

- 1 tester: 2-3 days.
- 2 testers: 1.5-2 days.

### Final Regression and Sign-off

Activities:

- Retest fixed defects.
- Re-run failed permission-denied cases.
- Re-run critical Attendance, Leave, Payroll, Employee, Organization, and Task workflows after fixes.
- Confirm Arabic/English smoke checks.
- Review evidence completeness.
- Prepare final UAT status summary for management.

Estimated duration:

- 1 tester: 2-3 days within the shared setup/sign-off allowance.
- 2 testers: 1.5-2 days within the shared setup/sign-off allowance.

## Suggested Calendar Schedule

### Option A: 1 Tester

| Phase | Duration | Calendar Position |
| --- | ---: | --- |
| Preparation | 1-2 days | Days 1-2 |
| Organization | 4-5 days | Days 3-7 |
| HR Employees | 3-4 days | Days 8-11 |
| Attendance + Leaves | 5-6 days | Days 12-17 |
| Salary / Payroll | 4-5 days | Days 18-22 |
| Task Management | 2-3 days | Days 23-25 |
| Final retest and sign-off | 1-2 days | Days 25-26 |

Recommended management estimate: **22-26 working days**.

### Option B: 2 Testers

| Phase | Tester 1 | Tester 2 | Calendar Position |
| --- | --- | --- | --- |
| Preparation | Test data, roles, tracker | Environment and access validation | Days 1-2 |
| Wave 1 | Organization | HR Employees | Days 3-5 |
| Wave 2 | Attendance setup and sessions | Leaves and employee self-service flows | Days 6-8 |
| Wave 3 | Salary / Payroll | Task Management and permission retests | Days 9-11 |
| Final | Defect retest and evidence review | Bilingual smoke and sign-off pack | Days 12-15 |

Recommended management estimate: **12-15 working days**.

## Daily Operating Rhythm

Use the same rhythm every UAT day:

1. Morning: confirm planned test cases, blockers, and test data.
2. Execution block: run assigned cases and capture evidence immediately.
3. Defect triage: classify blockers, high, medium, and low issues.
4. Retest block: retest fixes from previous days.
5. End-of-day summary: report executed, passed, failed, blocked, and pending counts.

## Defect Severity Guide

| Severity | Definition | UAT Impact |
| --- | --- | --- |
| Blocker | Prevents core workflow execution or prevents sign-in/access to the module. | Stops module sign-off. |
| High | Core workflow works incorrectly, wrong data is saved, or permission/security behavior is wrong. | Must be fixed or formally accepted before sign-off. |
| Medium | Workaround exists but behavior is confusing, incomplete, or inconsistent. | Can be deferred only with business approval. |
| Low | Cosmetic, wording, minor alignment, or non-blocking usability issue. | Can be logged for later unless it affects adoption. |

## Acceptance Criteria

Module sign-off can be requested when:

- All planned UAT cases for the module have Result, Evidence, and Notes.
- No blocker defects remain open.
- High defects are fixed or formally accepted by the business owner.
- Permission-denied cases pass for sensitive actions.
- Arabic and English smoke checks pass for primary screens.
- The module owner confirms the workflow is acceptable for business use.

## Assumptions

- The existing `docs/uat` pack remains the source of truth for test coverage.
- Each tester has about 6 productive UAT hours per working day.
- The estimate includes evidence capture, defect logging, and retesting.
- Attendance/Leaves and Payroll are treated as higher-risk modules because they depend heavily on setup data and business rules.
- This document is an estimate and execution plan; it does not change application code or UAT matrix data.
