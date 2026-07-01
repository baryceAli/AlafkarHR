# Workflow Mapping

## Organization Setup

1. Create or confirm company.
2. Ensure main branch exists.
3. Create branches.
4. Create administrations and departments.
5. Create positions and optional teams.
6. Validate branch and department visibility with security roles.

## User and Role Setup

1. Define roles using the roles-permissions matrix.
2. Create roles in `/Auth/Role/List`.
3. Create or confirm users in `/Auth/User/List`.
4. Assign roles in `/Auth/User/AssignRole`.
5. Assign branch access where the user should be limited to selected branches.
6. Test menu visibility and page access.

## Employee Creation

1. Confirm required organization masters exist.
2. Open `/Employee/Employee/List`.
3. Create employee from `/Employee/Employee/Form/{Id?}`.
4. Assign company, branch, administration, department, and position.
5. Add emergency contacts, documents, skills, certifications, and lifecycle events where required.
6. Review employee view or 360 view.

## Employee Lifecycle

1. Open `/HR/EmployeeLifecycle`.
2. Select employee.
3. Add lifecycle event.
4. Submit or transition event based on approved workflow.
5. Manager or HR approver reviews where approval is required.
6. Complete the lifecycle event and verify employee record impact.

## Attendance Daily Work

1. Employee opens `/Attendance/MyAttendance`.
2. Employee checks preview if location rules are used.
3. Employee starts session, starts/ends break if applicable, and ends session.
4. Attendance officer reviews sessions in `/Attendance/Sessions`.
5. Exceptions are handled through late check-in requests, mid-day permission requests, or attendance corrections.
6. Attendance reports are reviewed in `/Attendance/Reports`.

## Attendance Setup and Roster

1. Configure calendar settings and holidays.
2. Configure shifts in `/Attendance/Shifts`.
3. Assign shifts in `/Attendance/ShiftAssignments`.
4. Configure roster schedules in `/HR/AttendanceRoster` where roster is in scope.
5. Publish or lock schedules after validation.
6. Review shift swaps and corrections where applicable.

## Leave Application

1. HR configures leave types, periods, policies, assignments, and balances.
2. Employee opens `/LeavesManagement/MyLeaveApplications`.
3. Employee creates leave application and uploads attachment if required.
4. Employee submits request.
5. Manager/HR reviews the application.
6. Approved application updates leave workflow and reports.
7. HR monitors ledger and balances through `/HR/LeaveLedger` and `/LeavesManagement/Balances`.

## Emergency Leave

1. Employee or HR creates emergency leave request.
2. Attachment is uploaded where policy requires it.
3. Approver opens `/LeavesManagement/ApproveEmergencyLeaves`.
4. Approver approves or rejects request with comments.
5. HR reviews report impact.

## Payroll Run

1. Configure components and contracts.
2. Assign contracts and salary structures to employees.
3. Configure payroll period.
4. Import or generate attendance work entries if attendance is used.
5. Create payroll entry or salary run.
6. Generate/recalculate payslips.
7. Review payroll inputs, loans, Saudi payroll data, and WPS batches.
8. Approve payslips or salary runs.
9. Mark payslips paid where applicable.
10. Post payroll accounting if finance integration is in scope.

## Recruitment

1. Create staffing plan.
2. Create job requisition.
3. Open requisition.
4. Add applicants.
5. Record interview feedback.
6. Create job offer.
7. Accept/reject offer.
8. Mark employee created when candidate is hired.

Requires validation: approval thresholds, offer approvals, and employee creation ownership.

## Performance

1. Create appraisal cycle.
2. Create goals and competencies.
3. Start cycle.
4. Create employee evaluations.
5. Employee submits feedback if enabled by policy.
6. Manager reviews.
7. HR or approver approves.
8. Close cycle.

Requires validation: rating scale, weighting, calibration, and approval authority.

## Training

1. Create training program.
2. Create training event.
3. Open/start event.
4. Add attendees.
5. Mark attendance.
6. Record results.
7. Link certificates.
8. Complete event.

Requires validation: nomination rules, training budget, certificate policy, and completion criteria.
