# User Guide: Manager and Supervisor

## Role Purpose

Managers and supervisors review team-related HR, attendance, and leave activity, approve requests where authorized, and support accurate team data.

## Main Pages

- Employee list/view if authorized: `/Employee/Employee/List`
- Attendance approvals: `/Attendance/ApprovePermissionRequests`
- Late requests: `/Attendance/LateRequests`
- Attendance reports: `/Attendance/Reports`
- Leave approvals: `/HR/LeaveApplications`, `/LeavesManagement/ApproveEmergencyLeaves`
- Leave reports: `/LeavesManagement/Reports`
- Performance review pages where enabled: `/HR/Performance`

## Required Permissions

- `Employees.Employee.View`
- `Attendance.Attendance.ReviewRequests`
- `Attendance.Attendance.ApproveMidDayPermission`
- `Attendance.Roster.ApproveSwap` if roster swaps are in scope
- `Leave.Leave.ApproveEmergencyLeave`
- `Leave.Application.Approve`
- `HR.Performance.Review` where performance reviews are in scope

## Attendance Approval Workflow

1. Open approval page.
2. Filter pending requests by date, employee, or status.
3. Review request reason and attendance context.
4. Approve or reject with clear comment.
5. Confirm status changes.

## Leave Approval Workflow

1. Open leave application or emergency leave approval page.
2. Review employee, dates, leave type, balance, attachment, and overlap risk.
3. Approve or reject according to company policy.
4. Add comment.
5. Confirm employee receives updated status.

## Team Review Workflow

1. Open employee or report page available to your role.
2. Filter by branch, department, or team where available.
3. Review missing data, attendance exceptions, or upcoming leave.
4. Escalate corrections to HR or attendance owner.

## Performance Workflow

1. Open `/HR/Performance` if activated.
2. Select appraisal cycle and employee.
3. Review goals and competency scores.
4. Add manager feedback.
5. Submit review or approve based on assigned authority.

Requires validation: exact manager authority and rating policy.

## Common Mistakes

| Mistake | Avoidance |
| --- | --- |
| Approving requests without checking dates or balance. | Review details before approval. |
| Leaving approvals pending. | Check approval pages daily during business days. |
| Trying to edit employee master data. | Send data corrections to HR unless your role permits edits. |
| Sharing payroll or employee data outside policy. | Follow confidentiality rules. |

## Troubleshooting

| Symptom | Check |
| --- | --- |
| Team member missing from report. | Branch/department/team assignment or manager scope. |
| Approval button missing. | Approval permission not assigned. |
| Cannot open performance page. | Performance module/role may not be enabled. |

## Escalation

Escalate employee data issues to HR Administrator, attendance issues to Attendance Officer, and access issues to IT.
