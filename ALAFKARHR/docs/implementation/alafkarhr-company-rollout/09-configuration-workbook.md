# Configuration Workbook

## Configuration Principles

- Configure company and branch structure before users, employees, attendance, leave, or payroll.
- Configure permissions through roles, then assign users to roles.
- Keep branch access separate from normal permission claims.
- Validate every configuration item with a business owner.

## Organization Configuration

| Item | Required | Owner | Status | Notes |
| --- | --- | --- | --- | --- |
| Company record | Yes | HR/IT | Not started | |
| Main branch | Yes | HR/IT | Not started | Every company must have a main branch. |
| Branches | Yes | HR | Not started | |
| Administrations | Recommended | HR | Not started | |
| Departments | Yes | HR | Not started | |
| Business lines | If licensed/used | HR/IT | Not started | |
| License categories | If platform-managed | IT | Not started | |

## Security Configuration

| Item | Required | Owner | Status | Notes |
| --- | --- | --- | --- | --- |
| System Administrator role | Yes | IT | Not started | Full setup/security access. |
| HR Administrator role | Yes | HR/IT | Not started | HR master and employee maintenance. |
| Employee role | Yes | HR/IT | Not started | Self-service attendance/leave. |
| Manager role | Yes | HR/IT | Not started | Review/approve team requests. |
| Payroll Officer role | If payroll in scope | Payroll/IT | Not started | Payroll setup and processing. |
| Attendance Officer role | If attendance in scope | Attendance/IT | Not started | Shifts, sessions, reports, approvals. |
| Recruiter role | If recruitment in scope | HR/IT | Not started | Recruitment workflows. |
| Performance Reviewer role | If performance in scope | HR/IT | Not started | Appraisal workflows. |
| Training Coordinator role | If training in scope | HR/IT | Not started | Training workflows. |
| Branch access | If branches in scope | IT | Not started | Assign selected/default branches. |

## HR Core Configuration

| Item | Required | Owner | Status |
| --- | --- | --- | --- |
| Positions | Yes | HR | Not started |
| Teams | Optional | HR | Not started |
| Academic institutions | Optional | HR | Not started |
| Specializations | Optional | HR | Not started |
| Employee document categories | Validate | HR | Not started |
| Lifecycle event types/status rules | Validate | HR | Not started |

## Attendance Configuration

| Item | Required | Owner | Status |
| --- | --- | --- | --- |
| Calendar settings | Yes | Attendance Lead | Not started |
| Shifts | Yes | Attendance Lead | Not started |
| Shift assignments | Yes | Attendance Lead | Not started |
| Holidays | Yes | Attendance Lead | Not started |
| Check-in location rules | Validate | Attendance Lead | Not started |
| Late check-in rules | Validate | Attendance Lead | Not started |
| Mid-day permission rules | Validate | Attendance Lead | Not started |
| Roster schedules | If roster in scope | Attendance Lead | Not started |
| Biometric import format | If biometric in scope | IT/Attendance | Not started |

## Leave Configuration

| Item | Required | Owner | Status |
| --- | --- | --- | --- |
| Leave types | Yes | HR | Not started |
| Leave periods | Yes | HR | Not started |
| Leave policies | Yes | HR | Not started |
| Policy assignments | Yes | HR | Not started |
| Opening balances | Yes | HR | Not started |
| Emergency leave approval rules | Validate | HR | Not started |
| Encashment rules | If in scope | HR/Payroll | Not started |

## Payroll Configuration

| Item | Required | Owner | Status |
| --- | --- | --- | --- |
| Payroll components | Yes | Payroll | Not started |
| Contracts | Yes | Payroll | Not started |
| Employee contract assignments | Yes | Payroll | Not started |
| Salary structures | If structure-based payroll in scope | Payroll | Not started |
| Payroll periods | Yes | Payroll | Not started |
| Payroll entries | Yes | Payroll | Not started |
| Payslip approval and payment rules | Yes | Payroll | Not started |
| Employee loans | If in scope | Payroll | Not started |
| Saudi payroll info | If in scope | Payroll | Not started |
| WPS batches | If in scope | Payroll | Not started |
| Accounting posting | If in scope | Payroll/Finance | Not started |

## Talent Configuration

| Area | Required | Owner | Status |
| --- | --- | --- | --- |
| Recruitment staffing plans | If recruitment in scope | HR | Requires validation |
| Job requisition workflow | If recruitment in scope | HR | Requires validation |
| Appraisal cycles | If performance in scope | HR | Requires validation |
| Goals and competencies | If performance in scope | HR | Requires validation |
| Training programs | If training in scope | HR | Requires validation |
| Training event workflow | If training in scope | HR | Requires validation |
