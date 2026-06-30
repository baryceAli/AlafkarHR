# UAT Test Scripts

## Script Format

| Field | Description |
| --- | --- |
| Scenario ID | Unique scenario reference. |
| Role | User role executing the scenario. |
| Preconditions | Required data, role, and configuration. |
| Steps | Actions the tester performs. |
| Expected Result | System result required for pass. |
| Status | Pass, fail, blocked, not run. |

## Core Scripts

| ID | Role | Scenario | Steps | Expected result |
| --- | --- | --- | --- | --- |
| UAT-001 | System Administrator | Create role | Open roles, create role, assign permissions, save. | Role is created and visible for assignment. |
| UAT-002 | System Administrator | Assign user role | Open user role assignment, choose user, assign role, save. | User receives expected menu/page access. |
| UAT-003 | HR Administrator | Create employee | Open employee list, create employee, assign branch/department/position, save. | Employee appears in list and view page. |
| UAT-004 | HR Administrator | Update employee assignment | Change department or position. | Employee record reflects new assignment. |
| UAT-005 | HR Administrator | Add employee document | Open HR employee documents, add document data/link. | Document appears for employee. |
| UAT-006 | HR Administrator | Add lifecycle event | Add lifecycle event and transition it. | Lifecycle status changes as expected. |
| UAT-007 | Employee | Check attendance | Open My Attendance, start session, start/end break, end session. | Attendance session is recorded. |
| UAT-008 | Attendance Officer | Review attendance session | Open sessions and filter employee/date. | Session appears with correct status. |
| UAT-009 | Employee | Submit mid-day permission | Create permission request. | Request is pending review. |
| UAT-010 | Manager | Approve mid-day permission | Open approval page, approve request. | Request status changes to approved. |
| UAT-011 | Attendance Officer | Configure shift | Create shift and assign employee. | Shift and assignment are available. |
| UAT-012 | Employee | Submit leave application | Open my leave applications, create and submit. | Application is submitted/pending. |
| UAT-013 | Manager | Approve leave application | Review submitted leave. | Application is approved and visible in reports. |
| UAT-014 | HR Administrator | Maintain leave balance | Open leave balances, adjust balance. | Balance is saved and reflected in reports. |
| UAT-015 | Payroll Officer | Create payroll component | Create earning/deduction component. | Component is available for structures/contracts. |
| UAT-016 | Payroll Officer | Assign contract | Assign contract to employee. | Employee contract appears in payroll list. |
| UAT-017 | Payroll Officer | Run payroll | Create/generate payroll entry or salary run. | Payroll output generated for selected period. |
| UAT-018 | Payroll Officer | Approve payslip | Review and approve payslip. | Payslip status changes to approved. |
| UAT-019 | Payroll Officer | Employee loan | Create, approve, and cancel loan test cases. | Loan status follows expected actions. |
| UAT-020 | Recruiter | Recruitment flow | Create requisition, add applicant, record interview, create offer. | Applicant and offer statuses update. |
| UAT-021 | Performance Reviewer | Appraisal flow | Create cycle, goals, evaluation, submit/review/approve. | Evaluation reaches approved status. |
| UAT-022 | Training Coordinator | Training event | Create program/event, add attendee, mark attendance/result, complete. | Event and attendee statuses update. |
| UAT-023 | Executive | Dashboard/report access | Open HR, attendance, leave, and payroll reports allowed for role. | Reports open without edit access. |
| UAT-024 | Unauthorized user | Negative security test | Attempt restricted payroll/security page. | Access is denied or menu is hidden. |

## Sign-Off

UAT is accepted when all critical and high scenarios pass or have signed business acceptance with workaround.
