# User Guide: Specialist Roles

## Attendance Officer

### Purpose

Manage shifts, attendance sessions, exceptions, holidays, roster controls, biometric imports, work entries, and attendance reports.

### Main Pages

- `/Attendance/Dashboard`
- `/Attendance/Sessions`
- `/Attendance/Shifts`
- `/Attendance/ShiftAssignments`
- `/Attendance/Holidays`
- `/Attendance/PermissionRequests`
- `/Attendance/ApprovePermissionRequests`
- `/Attendance/LateRequests`
- `/Attendance/Reports`
- `/HR/AttendanceRoster`
- `/HR/WorkEntries`

### Required Permissions

- `Attendance.Attendance.*`
- `Attendance.Roster.*`
- `Attendance.WorkEntry.*`

### Daily Tasks

1. Review attendance dashboard.
2. Check exceptions and pending requests.
3. Maintain shifts and shift assignments.
4. Review sessions and missing check-ins.
5. Generate or approve work entries for payroll.
6. Review attendance reports.

## Recruiter

### Purpose

Manage staffing plans, job requisitions, applicants, interviews, offers, and hire handoff.

### Main Page

- `/HR/Recruitment`

### Required Permissions

- `HR.Recruitment.Select`
- `HR.Recruitment.View`
- `HR.Recruitment.Create`
- `HR.Recruitment.Edit`
- `HR.Recruitment.Approve`
- `HR.Recruitment.Hire`

### Workflow

1. Create staffing plan.
2. Create and open job requisition.
3. Add applicants.
4. Record interview feedback.
5. Create job offer.
6. Accept/reject offer.
7. Mark employee created after HR creates the employee record.

Requires validation: approval authority, offer templates, and employee creation handoff.

## Performance Reviewer

### Purpose

Manage appraisal cycles, goals, competencies, evaluations, reviews, and approvals.

### Main Page

- `/HR/Performance`

### Required Permissions

- `HR.Performance.Select`
- `HR.Performance.View`
- `HR.Performance.Create`
- `HR.Performance.Edit`
- `HR.Performance.Review`
- `HR.Performance.Approve`

### Workflow

1. Create appraisal cycle.
2. Define goals and competencies.
3. Start cycle.
4. Create employee evaluation.
5. Capture employee and manager feedback where enabled.
6. Recalculate, review, approve, or cancel evaluation.
7. Close cycle.

Requires validation: scoring, rating scales, weighting, calibration, and approval hierarchy.

## Training Coordinator

### Purpose

Manage training programs, events, attendees, attendance, results, certificates, and completion.

### Main Page

- `/HR/Training`

### Required Permissions

- `HR.Training.Select`
- `HR.Training.View`
- `HR.Training.Create`
- `HR.Training.Edit`
- `HR.Training.Complete`

### Workflow

1. Create training program.
2. Create training event.
3. Open and start event.
4. Add attendees.
5. Mark attendance.
6. Record results.
7. Link certificates.
8. Complete or cancel event.

Requires validation: nomination rules, certificate policy, and training budget controls.

## Finance Reviewer

### Purpose

Review payroll outputs and accounting impact where payroll accounting posting is in scope.

### Main Pages

- `/HR/Payslips`
- Payroll pages available to finance by policy
- Accounting pages if finance module access is granted

### Typical Permissions

- Payroll view/report permissions as approved.
- Accounting document or journal permissions only where finance review is part of the implementation.

### Workflow

1. Review payroll summary with Payroll Officer.
2. Check approved payslips and totals.
3. Review payroll accounting posting if enabled.
4. Approve finance sign-off or raise exception.

## Executive or Management Viewer

### Purpose

Use dashboards and reports for oversight without maintaining operational records.

### Main Pages

- `/Employee/Dashboard`
- `/Attendance/Dashboard`
- `/Attendance/Reports`
- `/LeavesManagement/Reports`
- `/HR/Reports`
- Payroll reports/pages as approved

### Guidelines

- Use view/report access only.
- Do not approve operational requests unless explicitly assigned.
- Escalate data discrepancies to the business owner.
