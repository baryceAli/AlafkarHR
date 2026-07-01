# Roles and Permissions Matrix

## Permission Model

AlafkarHR uses permission strings grouped by module and entity. Normal company-wide authorization is based on `Permission` claims. Branch access permissions such as `Organization.BranchAccess.ViewAll` and branch assignments are additional visibility controls and do not replace normal permissions.

## Recommended Roles

| Role | Purpose | Main permission groups |
| --- | --- | --- |
| System Administrator | Full setup, security, organization, and support administration. | Authentication.Users, Authentication.Roles, Organization.Company, Organization.Branch, Organization.Administration, Organization.Department, GeneralSettings.SystemSettings. |
| HR Administrator | Own employee master data and HR operations. | Employees.Employee, Employees.Team, Employees.Position, Employees.AcademicInistitution, Employees.Specialization, Employees.Lifecycle, Employees.Document, Employees.Skill, Leave.Policy, Leave.Application, Leave.Ledger. |
| Employee | Use self-service attendance and leave. | Attendance.Attendance request/view actions, Leave.Leave.RequestEmergencyLeave, Leave.Application request/cancel actions, limited employee view where allowed. |
| Manager/Supervisor | Review team attendance/leave and inspect team HR data. | Attendance.Attendance.ReviewRequests, Attendance.Attendance.ApproveMidDayPermission, Attendance.Roster.ApproveSwap, Leave.Leave.ApproveEmergencyLeave, Leave.Application.Approve, Employees.Employee.View. |
| Payroll Officer | Configure and run payroll. | Payroll.Contract, Payroll.Loan, Payroll.SalaryRun, Payroll.Structure, Payroll.Payslip, Payroll.WorkEntry. |
| Attendance Officer | Manage attendance configuration and exceptions. | Attendance.Attendance, Attendance.Roster, Attendance.WorkEntry. |
| Recruiter | Manage recruitment process. | HR.Recruitment. |
| Performance Reviewer | Manage performance cycles and reviews. | HR.Performance. |
| Training Coordinator | Manage training programs/events/attendees. | HR.Training. |
| Finance Reviewer | Review payroll output and accounting posting. | Payroll.Payslip view/approve as approved, Accounting.Document/JournalEntry where finance module is in scope. |
| Executive/Management | View dashboards and reports. | View/report permissions only for HR, attendance, leave, payroll, and organization as approved. |

## Permission Assignment Matrix

| Permission group | System Admin | HR Admin | Employee | Manager | Payroll | Attendance | Recruiter | Performance | Training | Executive |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Authentication.Users | CRUD | - | - | - | - | - | - | - | - | - |
| Authentication.Roles | CRUD | - | - | - | - | - | - | - | - | - |
| Organization.Company | View/Edit | View | - | View | View | View | - | - | - | View |
| Organization.Branch | CRUD + AssignUsers | View | - | View | View | View | - | - | - | View |
| Organization.Administration | CRUD | View | - | View | View | View | - | - | - | View |
| Organization.Department | CRUD | CRUD | - | View | View | View | - | - | - | View |
| Employees.Employee | CRUD | CRUD | Own view if configured | View team | View payroll population | View | Candidate conversion support | View | View | View/report |
| Employees.Lifecycle | CRUD/approve | CRUD/approve | - | View/approve if policy allows | View | - | - | - | - | Report |
| Employees.Document | CRUD/renew | CRUD/renew | Own documents if configured | View team if approved | View payroll documents if approved | - | - | - | - | Report |
| Employees.Skill | CRUD/verify | CRUD/verify | Own skills if configured | View team | - | - | - | Performance input | Training input | Report |
| Attendance.Attendance | Full | View/report | My attendance/request | Review/approve | Work-entry dependency | Full | - | - | - | Reports |
| Attendance.Roster | Full | View | View own | Approve swap if approved | Work-entry dependency | Full | - | - | - | Reports |
| Attendance.WorkEntry | Full | View | - | Review if approved | Generate/edit/approve | Generate/edit/approve | - | - | - | Reports |
| Leave.Leave | Full | Manage | Request | Approve | View payroll impact | - | - | - | - | Reports |
| Leave.Policy | Full | CRUD/assign | - | View | View | - | - | - | - | View |
| Leave.Application | Full | CRUD/review | Request/cancel | Approve | View payroll impact | - | - | - | - | Reports |
| Leave.Ledger | Full | View/adjust | Own balance if configured | View team | View/encash if approved | - | - | - | - | Reports |
| Payroll.Contract | Full | View | - | - | CRUD | - | - | - | - | Reports |
| Payroll.Loan | Full | View | - | - | CRUD/approve/cancel | - | - | - | - | Reports |
| Payroll.SalaryRun | Full | View | - | - | Create/edit/approve | Work-entry dependency | - | - | - | Reports |
| Payroll.Structure | Full | View | - | - | CRUD/activate/assign | - | - | - | - | Reports |
| Payroll.Payslip | Full | View | Own payslip if configured | - | Generate/approve/pay/cancel | - | - | - | - | Reports |
| HR.Recruitment | Full | View | - | Interview input if approved | - | - | CRUD/approve/hire | - | - | Reports |
| HR.Performance | Full | View | Own feedback if configured | Review | - | - | - | CRUD/review/approve | - | Reports |
| HR.Training | Full | View | Attend/view if configured | Nominate/review if approved | - | - | - | - | CRUD/complete | Reports |

CRUD means select, view, create, edit, and delete where the permission group supports those actions.

## Access Approval Checklist

- [ ] Role names approved by HR and IT.
- [ ] Permission groups mapped to each role.
- [ ] Branch access reviewed for each user.
- [ ] Default branch assignment confirmed where branch assignment is used.
- [ ] No role grants payroll edit rights to non-payroll users.
- [ ] No role grants security administration to business-only users.
- [ ] UAT includes negative tests for unauthorized pages/actions.
