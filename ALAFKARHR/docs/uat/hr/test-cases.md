# HR UAT executable test catalog

Generated from `build-hr-uat.mjs`. All 76 records are initially **Not run** in the workbook. Do not infer a passing result from these expected outcomes.

## Personnel and Employee Administration

Business owner: HR operations manager.

### HR-01-P — Maintain HR reference records — positive

- Parent: HR-01; priority: P1; execution phase: 1.
- Entry routes: /Employee/Position/List; /Employee/Specialization/List; /Employee/AcademicInistitution/List; /Employee/Teams. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: ORG; USERS; PEOPLE. Preparation references must be ready before execution.

**Steps:** Create one position, specialization, institution and team with UAT-prefixed bilingual names. Edit each. Refresh. Select the records in the relevant employee/team workflow and reopen the saved record.

**Acceptance criteria:** All four records retain saved values. Selectors and team membership show the intended records in the correct company.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-01-C — Maintain HR reference records — control

- Parent: HR-01; priority: P1; execution phase: 1.
- Entry routes: /Employee/Position/List; /Employee/Specialization/List; /Employee/AcademicInistitution/List; /Employee/Teams. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: ORG; USERS; PEOPLE. Preparation references must be ready before execution.

**Steps:** With a read-only user attempt create/edit/delete for each type. With a specialist submit a foreign-company or nonexistent reference through the corresponding request. Reopen the original record.

**Acceptance criteria:** Restricted writes and invalid references are rejected. Existing records and memberships remain unchanged.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-02-P — Create, edit, search and view employee — positive

- Parent: HR-02; priority: P1; execution phase: 1.
- Entry routes: /Employee/Employee/List; /Employee/Employee/Form/{Id:guid?}; /Employee/Employee/view/{Id:guid}; /Employee/Employee/360/{Id:guid}. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: ORG; USERS; PEOPLE. Preparation references must be ready before execution.

**Steps:** Create EMP-NEW using the prepared required fields and selected branch/department/position. Edit contact information. Search by the unique test name; apply company, branch and department filters. Refresh and compare list, profile and 360.

**Acceptance criteria:** Exactly the intended employee is returned. Saved details and assignments agree across views and survive refresh.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-02-C — Create, edit, search and view employee — control

- Parent: HR-02; priority: P1; execution phase: 1.
- Entry routes: /Employee/Employee/List; /Employee/Employee/Form/{Id:guid?}; /Employee/Employee/view/{Id:guid}; /Employee/Employee/360/{Id:guid}. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: ORG; USERS; PEOPLE. Preparation references must be ready before execution.

**Steps:** Submit with each required field missing. Try a nonexistent and foreign-company department/position. Search for an absent name and move between pages of results.

**Acceptance criteria:** Invalid records are not saved; validation identifies the issue. Empty search has no stale rows. Pagination does not mix or omit matching records.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-03-P — Maintain contacts, documents, skills and certificates — positive

- Parent: HR-03; priority: P1; execution phase: 1.
- Entry routes: /HR/EmployeeEmergencyContacts; /HR/EmployeeDocuments; /HR/EmployeeSkills. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PEOPLE; FILES; USERS. Preparation references must be ready before execution.

**Steps:** Select EMP-REG. Add and edit a contact, document link, skill and certificate. Refresh each page. Open the document/certificate reference. Switch to EMP-NIGHT and back.

**Acceptance criteria:** Values persist for EMP-REG; references open for the authorized tester. EMP-NIGHT has no copied records from EMP-REG.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-03-C — Maintain contacts, documents, skills and certificates — control

- Parent: HR-03; priority: P1; execution phase: 1.
- Entry routes: /HR/EmployeeEmergencyContacts; /HR/EmployeeDocuments; /HR/EmployeeSkills. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PEOPLE; FILES; USERS. Preparation references must be ready before execution.

**Steps:** Attempt the same writes as a read-only user. Change the employee or child-record ID in a captured request to another company. Try an invalid document reference.

**Acceptance criteria:** Unauthorized changes and foreign references fail without modifying either employee. Invalid references cannot be presented as successfully usable documents.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-04-P — Transfer, change position and process lifecycle — positive

- Parent: HR-04; priority: P0; execution phase: 1.
- Entry routes: /Employee/Employee/List; /HR/EmployeeLifecycle. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PEOPLE; ORG; POLICY; USERS. Preparation references must be ready before execution.

**Steps:** Record EMP-MOVE current department, branch and position. Process separate transfer and position-change records. For each applicable lifecycle type create, approve and complete at the approved effective date. Refresh employee and event history.

**Acceptance criteria:** Completed events apply the approved changes once and retain event history, actor and dates where required by POLICY. Employee assignments agree with the completed event.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-04-C — Transfer, change position and process lifecycle — control

- Parent: HR-04; priority: P0; execution phase: 1.
- Entry routes: /Employee/Employee/List; /HR/EmployeeLifecycle. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PEOPLE; ORG; POLICY; USERS. Preparation references must be ready before execution.

**Steps:** Create separate events and cancel them before completion. Attempt completion without approval, repeat completion and attempt a transfer to a prohibited branch.

**Acceptance criteria:** Cancelled events do not change assignments. Invalid/repeated transitions and prohibited transfers are rejected; neither branch gains unauthorized access.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-05-P — Terminate employee and reconcile downstream behavior — positive

- Parent: HR-05; priority: P0; execution phase: 1.
- Entry routes: /Employee/Employee/List; /HR/EmployeeLifecycle; /HR/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PEOPLE; POLICY; PAYREF; USERS. Preparation references must be ready before execution.

**Steps:** Use EMP-EXIT with history and approved termination date/reason. Terminate through the available employee workflow. Refresh the profile and historical reports. Check future roster, attendance, leave and payroll eligibility against POLICY.

**Acceptance criteria:** Termination details persist and historical records remain readable to authorized HR. Each downstream inclusion/exclusion matches the approved termination matrix; discrepancies are gaps.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### HR-05-C — Terminate employee and reconcile downstream behavior — control

- Parent: HR-05; priority: P0; execution phase: 1.
- Entry routes: /Employee/Employee/List; /HR/EmployeeLifecycle; /HR/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PEOPLE; POLICY; PAYREF; USERS. Preparation references must be ready before execution.

**Steps:** Try termination as an unauthorized user; repeat a completed termination. Test dates before/on/after the effective date in downstream eligibility. Check that employee access follows POLICY.

**Acceptance criteria:** Unauthorized/repeated actions cannot corrupt history. Boundary-date eligibility and account access match POLICY; absence of a required downstream control is a failure/gap.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

## Recruitment and Hiring

Business owner: Recruitment lead.

### REC-01-P — Maintain staffing plans and requisitions — positive

- Parent: REC-01; priority: P1; execution phase: 2.
- Entry routes: /HR/Recruitment. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: ORG; USERS; RECRUIT. Preparation references must be ready before execution.

**Steps:** Create a staffing plan for the test year and position. Create linked requisitions with planned headcount. Edit a draft, open it, then close it; use a separate requisition for cancellation. Refresh and filter.

**Acceptance criteria:** Year, position, headcount, links and status persist. Open, closed and cancelled records appear under the matching filters.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### REC-01-C — Maintain staffing plans and requisitions — control

- Parent: REC-01; priority: P1; execution phase: 2.
- Entry routes: /HR/Recruitment. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: ORG; USERS; RECRUIT. Preparation references must be ready before execution.

**Steps:** Attempt transitions from invalid states and repeat close/cancel. Submit missing required information and foreign-company position references. Repeat approval as a user without approval permission.

**Acceptance criteria:** Invalid references, states and unauthorized approvals are rejected without changing the requisition.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### REC-02-P — Register and progress applicants — positive

- Parent: REC-02; priority: P1; execution phase: 2.
- Entry routes: /HR/Recruitment. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: RECRUIT; USERS. Preparation references must be ready before execution.

**Steps:** Register two distinguishable applicants against separate requisitions. Move one through the available approved stages. Refresh; filter by requisition and status.

**Acceptance criteria:** Details persist and each applicant stays linked to the correct requisition. Status filters return only matching applicants.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### REC-02-C — Register and progress applicants — control

- Parent: REC-02; priority: P1; execution phase: 2.
- Entry routes: /HR/Recruitment. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: RECRUIT; USERS. Preparation references must be ready before execution.

**Steps:** Attempt an invalid stage transition and a foreign-company requisition reference. Repeat status change as a read-only user and search for an absent candidate.

**Acceptance criteria:** Invalid/unauthorized changes fail; applicant association remains correct and empty filters show no stale candidate.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### REC-03-P — Record interview feedback — positive

- Parent: REC-03; priority: P1; execution phase: 2.
- Entry routes: /HR/Recruitment. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: RECRUIT; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Open the intended applicant. Record interview feedback with the approved score scale and comments. Edit it; refresh and compare both applicants.

**Acceptance criteria:** Scores/comments persist only on the intended applicant and match the approved score scale.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### REC-03-C — Record interview feedback — control

- Parent: REC-03; priority: P1; execution phase: 2.
- Entry routes: /HR/Recruitment. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: RECRUIT; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Submit out-of-range scores or missing required feedback. Attempt edit/delete with a read-only user and change the applicant ID to another company.

**Acceptance criteria:** Invalid scores and unauthorized changes fail; neither applicant receives another applicant’s feedback.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### REC-04-P — Offer decision and employee handoff — positive

- Parent: REC-04; priority: P0; execution phase: 2.
- Entry routes: /HR/Recruitment; /Employee/Employee/Form/{Id:guid?}. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: RECRUIT; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Create an offer for the successful applicant. Accept it. Create/identify EMP-NEW through the employee workflow and link that employee to the accepted offer. Refresh offer and employee. Reject a separate offer.

**Acceptance criteria:** Accepted/rejected statuses are correct. The hiring handoff references exactly EMP-NEW. Record whether employee creation is manual; offer acceptance alone is not proof of creation.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### REC-04-C — Offer decision and employee handoff — control

- Parent: REC-04; priority: P0; execution phase: 2.
- Entry routes: /HR/Recruitment; /Employee/Employee/Form/{Id:guid?}. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: RECRUIT; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Repeat acceptance and employee linking. Link a nonexistent or foreign-company employee; try linking a rejected offer. Repeat hire action without hire permission.

**Acceptance criteria:** Invalid links/transitions fail; no duplicate employee or hire association appears; restricted users cannot complete hiring.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

## Attendance and Workforce Scheduling

Business owner: Attendance administrator.

### ATT-01-P — Configure shifts, calendar and assignments — positive

- Parent: ATT-01; priority: P1; execution phase: 2.
- Entry routes: /Attendance/Shifts; /Attendance/ShiftAssignments; /Attendance/Holidays. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: CALENDAR; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Create day and overnight shifts. Configure working days and a holiday through the available configuration surface. Assign shifts with effective dates. Check employee preview/report before, on and after the assignment boundary.

**Acceptance criteria:** The expected shift, holiday and work-day classification match CALENDAR at each tested date; saved configuration persists.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-01-C — Configure shifts, calendar and assignments — control

- Parent: ATT-01; priority: P1; execution phase: 2.
- Entry routes: /Attendance/Shifts; /Attendance/ShiftAssignments; /Attendance/Holidays. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: CALENDAR; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Try invalid shift times/assignments and unavailable employee references. Change configuration as a read-only user. Test holiday and weekend boundaries.

**Acceptance criteria:** Invalid/unauthorized configuration does not save. Boundary classifications match the approved calendar; missing configuration UI is a gap.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-02-P — Roster publication, swaps and substitutions — positive

- Parent: ATT-02; priority: P1; execution phase: 2.
- Entry routes: /HR/AttendanceRoster. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: CALENDAR; PEOPLE; POLICY; USERS. Preparation references must be ready before execution.

**Steps:** Create a schedule and assign EMP-REG and EMP-NIGHT. Publish and verify each employee/date. Approve one swap and apply an approved substitution. Lock the schedule and refresh.

**Acceptance criteria:** Published and changed assignments match the approved roster and are visible to the correct employees. Lock status persists.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-02-C — Roster publication, swaps and substitutions — control

- Parent: ATT-02; priority: P1; execution phase: 2.
- Entry routes: /HR/AttendanceRoster. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: CALENDAR; PEOPLE; POLICY; USERS. Preparation references must be ready before execution.

**Steps:** Cancel a separate swap and schedule. Try overlapping/invalid assignments, a prohibited-branch employee, and editing a locked schedule without authority.

**Acceptance criteria:** Cancelled swaps do not apply. Invalid or prohibited assignments and unauthorized locked edits fail without changing the roster.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-03-P — Sessions, breaks, overnight and missing checkout — positive

- Parent: ATT-03; priority: P0; execution phase: 2.
- Entry routes: /Attendance/MyAttendance; /Attendance/Sessions. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TIMELINE; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** As EMP-REG check in, start/end a break and check out at recorded times. Repeat for EMP-NIGHT across midnight. Inspect sessions/report. Exercise missing-checkout resolution on a separate fixture.

**Acceptance criteria:** Session date/time, break minutes and net work duration reconcile to TIMELINE. Overnight date attribution and missing-checkout handling match POLICY.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-03-C — Sessions, breaks, overnight and missing checkout — control

- Parent: ATT-03; priority: P0; execution phase: 2.
- Entry routes: /Attendance/MyAttendance; /Attendance/Sessions. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TIMELINE; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Attempt two active sessions, ending before starting, duplicate checkout, and invalid break order. Attempt another employee’s session action.

**Acceptance criteria:** Invalid sequences and foreign session actions fail; no duplicate sessions or negative durations are created.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-04-P — Location rules and late/midday requests — positive

- Parent: ATT-04; priority: P1; execution phase: 2.
- Entry routes: /Attendance/MyAttendance; /Attendance/LateRequests; /Attendance/PermissionRequests; /Attendance/ApprovePermissionRequests. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: GEO; TIMELINE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Check in inside the approved geofence with accepted accuracy. Submit late and midday permission requests. Approve one of each as the intended reviewer; reject separate requests. Refresh status and attendance report.

**Acceptance criteria:** Geofence result matches GEO. Each decision/comments persist; attendance treatment matches POLICY for approved versus rejected requests.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-04-C — Location rules and late/midday requests — control

- Parent: ATT-04; priority: P1; execution phase: 2.
- Entry routes: /Attendance/MyAttendance; /Attendance/LateRequests; /Attendance/PermissionRequests; /Attendance/ApprovePermissionRequests. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: GEO; TIMELINE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Test outside boundary, denied/unavailable location, poor accuracy and mocked-location input. Try review without permission and repeat review of decided requests.

**Acceptance criteria:** Each location condition has the approved explicit outcome. Unauthorized or repeated review cannot change attendance or silently approve the request.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-05-P — Attendance correction and biometric import — positive

- Parent: ATT-05; priority: P0; execution phase: 2.
- Entry routes: /HR/AttendanceRoster; /Attendance/Sessions. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TIMELINE; BIO; USERS. Preparation references must be ready before execution.

**Steps:** Create and approve a correction for the known session, then apply it. Create a biometric batch with valid rows; review and post. Compare sessions and report to source timestamps. Locate the action UI or record a UI gap.

**Acceptance criteria:** Only the intended session is corrected. Valid import rows yield the expected sessions; timestamps reconcile to BIO and retain source traceability.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-05-C — Attendance correction and biometric import — control

- Parent: ATT-05; priority: P0; execution phase: 2.
- Entry routes: /HR/AttendanceRoster; /Attendance/Sessions. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TIMELINE; BIO; USERS. Preparation references must be ready before execution.

**Steps:** Reject a separate correction and try applying it. Import invalid employee/time rows, duplicate rows and repost the same batch. Attempt correction as an unauthorized user.

**Acceptance criteria:** Rejected/unauthorized corrections do not apply. Invalid rows are identifiable; duplicate/reposted input cannot duplicate attendance. Backend-only capability does not pass manual UAT.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-06-P — Generate, approve and lock work entries — positive

- Parent: ATT-06; priority: P0; execution phase: 2.
- Entry routes: /HR/WorkEntries. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TIMELINE; CALENDAR; LEAVEREF; USERS. Preparation references must be ready before execution.

**Steps:** Generate entries for the prepared employee/date range. Compare work, breaks and approved leave to reference totals. Approve and lock the entries. Refresh.

**Acceptance criteria:** Entry population and quantities reconcile exactly to the approved attendance/leave reference; approval and lock states persist.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### ATT-06-C — Generate, approve and lock work entries — control

- Parent: ATT-06; priority: P0; execution phase: 2.
- Entry routes: /HR/WorkEntries. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TIMELINE; CALENDAR; LEAVEREF; USERS. Preparation references must be ready before execution.

**Steps:** Regenerate the same range and attempt editing locked entries. Try approval without permission and include another company’s employee.

**Acceptance criteria:** No duplicate or double-counted quantities result; locked/unauthorized changes and foreign-company entries are prevented.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

## Leave Management

Business owner: Leave administrator.

### LEV-01-P — Policies, periods and allocations — positive

- Parent: LEV-01; priority: P0; execution phase: 2.
- Entry routes: /HR/LeavePolicies; /LeavesManagement/Balances. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; PEOPLE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Create leave types, period, policy and employee policy assignments. Generate allocations for the prepared population. Compare balance and ledger to LEAVEREF; refresh.

**Acceptance criteria:** Only assigned eligible employees receive the expected entitlement for the correct period and leave type.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-01-C — Policies, periods and allocations — control

- Parent: LEV-01; priority: P0; execution phase: 2.
- Entry routes: /HR/LeavePolicies; /LeavesManagement/Balances. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; PEOPLE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Generate the same allocation again; try invalid periods, foreign-company assignments and unauthorized allocation. Inspect ledger and balances.

**Acceptance criteria:** No duplicate entitlement is granted. Invalid/unauthorized allocations are rejected without changing balances.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-02-P — Employee leave application — positive

- Parent: LEV-02; priority: P0; execution phase: 2.
- Entry routes: /LeavesManagement/MyLeaveApplications. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; CALENDAR; FILES; USERS. Preparation references must be ready before execution.

**Steps:** As EMP-REG create a leave draft across the prepared workdays/holiday. Attach the synthetic document, submit and refresh. Compare charged days with LEAVEREF and open the attachment.

**Acceptance criteria:** Dates/type/attachment persist; charged days match the approved calendar. Request belongs to the logged-in employee and reaches the intended submitted state.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-02-C — Employee leave application — control

- Parent: LEV-02; priority: P0; execution phase: 2.
- Entry routes: /LeavesManagement/MyLeaveApplications. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; CALENDAR; FILES; USERS. Preparation references must be ready before execution.

**Steps:** Test reversed dates, overlapping requests, insufficient balance, required attachment missing and a forged employee ID. Check existing requests and balance afterward.

**Acceptance criteria:** Each condition follows POLICY with clear validation. No unintended request or balance transaction is created, and another employee cannot be impersonated.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-03-P — Approve, reject and cancel leave — positive

- Parent: LEV-03; priority: P0; execution phase: 2.
- Entry routes: /HR/LeaveApplications; /HR/LeaveLedger; /LeavesManagement/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Record starting balance. Approve a submitted application, reject a separate one and cancel the approved application using the permitted process. Inspect ledger and reports after each action.

**Acceptance criteria:** Approval consumes exactly the expected days once; rejection consumes none; cancellation restores the correct amount. Decisions/comments and terminal requests appear in reports.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-03-C — Approve, reject and cancel leave — control

- Parent: LEV-03; priority: P0; execution phase: 2.
- Entry routes: /HR/LeaveApplications; /HR/LeaveLedger; /LeavesManagement/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Repeat approve/cancel; try review without authority and cancellation from a prohibited state. Compare balance/ledger before and after.

**Acceptance criteria:** Repeated or unauthorized actions do not create extra debits/credits. Invalid transitions are rejected and ledger remains reconciled.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-04-P — Emergency leave — positive

- Parent: LEV-04; priority: P1; execution phase: 2.
- Entry routes: /LeavesManagement/EmergencyLeaves; /LeavesManagement/ApproveEmergencyLeaves. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; FILES; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Submit an emergency request with required reason/attachment. Approve it as the assigned reviewer and reject a separate request. Refresh and compare attendance and balance impact.

**Acceptance criteria:** Requests, files, comments and decisions persist. Approved and rejected treatment match POLICY and LEAVEREF.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-04-C — Emergency leave — control

- Parent: LEV-04; priority: P1; execution phase: 2.
- Entry routes: /LeavesManagement/EmergencyLeaves; /LeavesManagement/ApproveEmergencyLeaves. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; FILES; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Test missing information, overlap, repeated review and review without permission. Attempt another employee/company request through a modified request.

**Acceptance criteria:** Validation and access rules are enforced; rejected/duplicate requests do not silently consume leave or alter attendance.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-05-P — Ledger adjustments and encashment — positive

- Parent: LEV-05; priority: P0; execution phase: 2.
- Entry routes: /HR/LeaveLedger. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; PAYREF; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Record opening balance. Apply an authorized credit/debit adjustment with reason, then encash the approved days. Compare the ledger and any required payroll handoff with LEAVEREF/PAYREF.

**Acceptance criteria:** Opening plus credits minus debits equals closing balance. Encashment days and amount match approved references; transactions remain traceable.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### LEV-05-C — Ledger adjustments and encashment — control

- Parent: LEV-05; priority: P0; execution phase: 2.
- Entry routes: /HR/LeaveLedger. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; PAYREF; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Repeat the same submission, request excess encashment, use invalid employee/type references and attempt an adjustment without permission.

**Acceptance criteria:** No duplicate or unauthorized ledger/payroll transactions result. Excess/invalid requests follow POLICY; a missing required payroll handoff is recorded as a gap.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

## Payroll and Compensation

Business owner: Payroll manager / Finance.

### PAY-01-P — Components, contracts and structures — positive

- Parent: PAY-01; priority: P0; execution phase: 3.
- Entry routes: /Payroll/Components; /Payroll/Contracts; /Payroll/AssignContract; /HR/PayrollStructures. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; PEOPLE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Create earnings/deduction components, a contract and a salary structure. Assign appropriate fixtures with effective dates. Reopen and inspect before/on/after each effective boundary.

**Acceptance criteria:** Amounts/formulas and assignment dates persist. The applicable salary basis is unambiguous and matches PAYREF for each boundary.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-01-C — Components, contracts and structures — control

- Parent: PAY-01; priority: P0; execution phase: 3.
- Entry routes: /Payroll/Components; /Payroll/Contracts; /Payroll/AssignContract; /HR/PayrollStructures. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; PEOPLE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Attempt invalid amounts/formulas, conflicting effective assignments, foreign-company employee references and edits without permission.

**Acceptance criteria:** Invalid/unauthorized assignments cannot create an ambiguous or unintended salary basis; errors identify rejected data.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-02-P — Employee loans and installment recovery — positive

- Parent: PAY-02; priority: P0; execution phase: 3.
- Entry routes: /Payroll/Loans; /HR/Payslips; /Payroll/SalaryRuns. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; PEOPLE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Create and approve EMP-LOAN loan with the reference installment schedule. Process one eligible payroll installment. Inspect deduction and outstanding balance. Cancel a separate unused loan.

**Acceptance criteria:** Only the approved eligible loan is recovered; installment and outstanding balance equal PAYREF; cancelled/unapproved loans are excluded.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-02-C — Employee loans and installment recovery — control

- Parent: PAY-02; priority: P0; execution phase: 3.
- Entry routes: /Payroll/Loans; /HR/Payslips; /Payroll/SalaryRuns. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; PEOPLE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Recalculate/repeat payroll, reverse the installment via the permitted payroll process, and reprocess. Try unauthorized approval and cancellation after recovery.

**Acceptance criteria:** Recovery occurs once; reversal restores the intended balance once. Subsequent cancellation follows POLICY without losing recovered history.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-03-P — Salary-run workflow — positive

- Parent: PAY-03; priority: P0; execution phase: 3.
- Entry routes: /Payroll/SalaryRuns. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; PEOPLE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Create and calculate salary runs for the designated legacy-workflow fixtures. Compare gross/deductions/net independently. Approve and commit the period. Undo a separate disposable run/period using permitted controls.

**Acceptance criteria:** Amounts match PAYREF to the approved rounding tolerance. Approval/commit/undo statuses and monetary side effects reconcile with the reference.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-03-C — Salary-run workflow — control

- Parent: PAY-03; priority: P0; execution phase: 3.
- Entry routes: /Payroll/SalaryRuns. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; PEOPLE; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Create the same employee-period twice; repeat approve/commit/undo; attempt undo without override authority; use an employee-period already processed through payroll entries.

**Acceptance criteria:** No duplicate payout/accounting/recovery results across either workflow. Invalid/unauthorized actions fail; missing cross-workflow duplication control is a critical gap.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-04-P — Payroll periods, entries, inputs and payslips — positive

- Parent: PAY-04; priority: P0; execution phase: 3.
- Entry routes: /HR/Payslips; /HR/WorkEntries; /HR/SaudiPayroll. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; TIMELINE; LEAVEREF; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Open the reference period; create employee payroll inputs and import the approved attendance work entries. Create/generate an entry, inspect and recalculate payslips, approve, mark paid and close. Use separate fixtures to reopen/cancel.

**Acceptance criteria:** Population, gross, deductions, net and loan balances match PAYREF. Attendance is counted once. Every allowed transition and resulting period/entry/payslip status persists.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-04-C — Payroll periods, entries, inputs and payslips — control

- Parent: PAY-04; priority: P0; execution phase: 3.
- Entry routes: /HR/Payslips; /HR/WorkEntries; /HR/SaudiPayroll. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; TIMELINE; LEAVEREF; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Repeat generation, work-entry import and paid action; attempt edits in a closed period, approval without authority and payroll for an ineligible employee. Compare totals before/after.

**Acceptance criteria:** No duplicate slips, inputs or deductions. Closed/invalid states and unauthorized actions are blocked. Eligibility matches POLICY.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-05-P — Payroll accounting posting — positive

- Parent: PAY-05; priority: P0; execution phase: 3.
- Entry routes: /HR/Payslips. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; ACCOUNTING; USERS. Preparation references must be ready before execution.

**Steps:** Post the approved test payroll entry. Inspect resulting journals in accounting using Finance access. Compare company, branch, accounts, debit/credit and totals to ACCOUNTING.

**Acceptance criteria:** Debits equal credits; journals match the payroll reference and intended company/branch. Posting links remain traceable.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-05-C — Payroll accounting posting — control

- Parent: PAY-05; priority: P0; execution phase: 3.
- Entry routes: /HR/Payslips. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; ACCOUNTING; USERS. Preparation references must be ready before execution.

**Steps:** Post again. On a separate fixture deliberately use incomplete accounting setup, inspect failure state, correct the setup and retry. Try posting without permission.

**Acceptance criteria:** Repeated posting cannot duplicate journals. Failed posting leaves no partial/double posting; corrected retry succeeds once; unauthorized posting fails.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-06-P — Saudi payroll, WPS and provision snapshots — positive

- Parent: PAY-06; priority: P0; execution phase: 3.
- Entry routes: /HR/SaudiPayroll. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; WPS; EOS; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Enter approved employee payroll information/rates. Generate a WPS batch from eligible approved payslips; obtain the actual file and validate schema, employee population and totals against WPS. Create an EOS snapshot for the reference date and compare to EOS.

**Acceptance criteria:** Inputs, calculations, WPS contents and EOS totals match the independently approved references. An actual consumable validated file is required to pass export; status alone is insufficient.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PAY-06-C — Saudi payroll, WPS and provision snapshots — control

- Parent: PAY-06; priority: P0; execution phase: 3.
- Entry routes: /HR/SaudiPayroll. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PAYREF; WPS; EOS; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Use missing/invalid bank information, excluded employees and duplicate batch requests. Attempt exported-batch changes and snapshot creation without authority. Compare output again.

**Acceptance criteria:** Invalid WPS records are exposed and cannot appear as successful output. No duplicate payment batch or unauthorized change. If export only changes status, log a blocking export gap.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

## Performance Management

Business owner: Performance manager.

### PER-01-P — Cycles, goals and competencies — positive

- Parent: PER-01; priority: P1; execution phase: 4.
- Entry routes: /HR/Performance. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PERF; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Create an appraisal cycle, goal definitions and competencies. Assign the prepared employee goals with reference weights. Start the cycle; refresh and inspect associations.

**Acceptance criteria:** Definitions, dates, weights and associations persist for the correct cycle and employees; available actions reflect the cycle state.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PER-01-C — Cycles, goals and competencies — control

- Parent: PER-01; priority: P1; execution phase: 4.
- Entry routes: /HR/Performance. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PERF; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Try invalid dates/weights, foreign-company employees, actions outside the active cycle and configuration without permission.

**Acceptance criteria:** Invalid or unauthorized configuration/actions are rejected without corrupting the cycle or employee goals.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PER-02-P — Scores, achievement and calculation — positive

- Parent: PER-02; priority: P1; execution phase: 4.
- Entry routes: /HR/Performance. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PERF; USERS. Preparation references must be ready before execution.

**Steps:** Enter the reference achievement and competency scores. Create/recalculate the appraisal and compare components, weights, total and rating to the independent PERF calculation.

**Acceptance criteria:** All score components and final rating match the approved reference/rounding. Recalculation yields the same result for unchanged inputs.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PER-02-C — Scores, achievement and calculation — control

- Parent: PER-02; priority: P1; execution phase: 4.
- Entry routes: /HR/Performance. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PERF; USERS. Preparation references must be ready before execution.

**Steps:** Submit out-of-range scores, missing required scores and invalid weights. Change one valid score, recalculate, then restore it and recalculate again.

**Acceptance criteria:** Invalid inputs fail or visibly block a final result. Valid edits change the result as expected and restoration reproduces the reference total.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PER-03-P — Appraisal submission, review and approval — positive

- Parent: PER-03; priority: P1; execution phase: 4.
- Entry routes: /HR/Performance. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PERF; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Submit employee feedback, record manager review, approve and close the cycle. Use a separate appraisal for cancellation. Refresh evaluation/history.

**Acceptance criteria:** Employee/manager feedback, decisions and final results persist in the correct sequence; cancelled records remain distinct from approved results.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### PER-03-C — Appraisal submission, review and approval — control

- Parent: PER-03; priority: P1; execution phase: 4.
- Entry routes: /HR/Performance. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PERF; USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Attempt approval before review, employee self-approval, unauthorized access to another employee, and edits after finalization. Repeat final approval.

**Acceptance criteria:** Role/state restrictions prevent unauthorized or repeated finalization and silent changes to finalized results.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

## Training and Development

Business owner: Training coordinator.

### TRN-01-P — Training programs and events — positive

- Parent: TRN-01; priority: P1; execution phase: 4.
- Entry routes: /HR/Training. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TRAIN; USERS. Preparation references must be ready before execution.

**Steps:** Create a program and dated event. Edit the draft; open, start and complete it. Create a separate event and cancel it. Refresh and filter program/status.

**Acceptance criteria:** Program/event details and states persist; filters return the correct events and transitions follow the approved sequence.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### TRN-01-C — Training programs and events — control

- Parent: TRN-01; priority: P1; execution phase: 4.
- Entry routes: /HR/Training. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TRAIN; USERS. Preparation references must be ready before execution.

**Steps:** Try reversed dates, invalid transition order, editing a completed event without authority and create/cancel as a read-only user.

**Acceptance criteria:** Invalid/unauthorized actions fail without changing completed event data.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### TRN-02-P — Enrollment, attendance and results — positive

- Parent: TRN-02; priority: P1; execution phase: 4.
- Entry routes: /HR/Training. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TRAIN; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Enroll EMP-REG and EMP-NIGHT in the event. Mark attended/absent respectively and record their distinct results. Refresh and compare attendee lists.

**Acceptance criteria:** Correct employees are enrolled once; attendance/results remain associated with the intended attendee and event.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### TRN-02-C — Enrollment, attendance and results — control

- Parent: TRN-02; priority: P1; execution phase: 4.
- Entry routes: /HR/Training. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TRAIN; PEOPLE; USERS. Preparation references must be ready before execution.

**Steps:** Enroll the same employee twice; submit a foreign-company employee and invalid result; record attendance/results without permission.

**Acceptance criteria:** Duplicate/invalid enrollment and unauthorized writes fail; no result leaks to another attendee.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### TRN-03-P — Certificates and training history — positive

- Parent: TRN-03; priority: P1; execution phase: 4.
- Entry routes: /HR/Training; /HR/EmployeeSkills; /HR/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TRAIN; FILES; PEOPLE; POLICY; USERS. Preparation references must be ready before execution.

**Steps:** Link the synthetic certificate to the completed attendee. Open it and inspect event/employee training history. Compare any required employee-certification synchronization against POLICY.

**Acceptance criteria:** Certificate opens for the authorized user and belongs to the correct attendee; history reconciles with completed events. Required synchronization is demonstrated or logged as a gap.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### TRN-03-C — Certificates and training history — control

- Parent: TRN-03; priority: P1; execution phase: 4.
- Entry routes: /HR/Training; /HR/EmployeeSkills; /HR/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TRAIN; FILES; PEOPLE; POLICY; USERS. Preparation references must be ready before execution.

**Steps:** Try an invalid certificate link, another attendee/company ID and unauthorized access. Inspect history for an absent or cancelled attendee.

**Acceptance criteria:** Invalid/unauthorized certificate access and association fail. History does not falsely show completion or certification for cancelled/absent attendees.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

## Shared Reporting and Controls

Business owner: HR manager / system administrator.

### CTL-01-P — Dashboards, reports and exports — positive

- Parent: CTL-01; priority: P1; execution phase: 1.
- Entry routes: /Employee/Dashboard; /HR/Reports; /Attendance/Reports; /LeavesManagement/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: REPORTREF; USERS. Preparation references must be ready before execution.

**Steps:** For each HR area, record filter values and run the matching report/dashboard. Compare record IDs, counts and monetary/time totals with REPORTREF. Export where offered and compare file contents.

**Acceptance criteria:** Displayed and exported populations/totals agree with the same reference and filters. Required but unavailable reporting/export is recorded as a gap.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### CTL-01-C — Dashboards, reports and exports — control

- Parent: CTL-01; priority: P1; execution phase: 1.
- Entry routes: /Employee/Dashboard; /HR/Reports; /Attendance/Reports; /LeavesManagement/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: REPORTREF; USERS. Preparation references must be ready before execution.

**Steps:** Test no-match filters, date cutoffs, branch changes and pagination. Export after changing filters; inspect for stale or out-of-scope records.

**Acceptance criteria:** Empty views show no stale totals. Boundary, paginated and exported data respect filters and access scope.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### CTL-02-P — Role, employee, company and branch access — positive

- Parent: CTL-02; priority: P0; execution phase: 1.
- Entry routes: All HR routes and their corresponding API requests. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: USERS; ORG; PEOPLE; POLICY. Preparation references must be ready before execution.

**Steps:** For every workstream use its specialist, approver and employee test accounts. Exercise the permitted reads/writes. Repeat branch reads as the restricted manager and separately as the explicitly authorized view-all user.

**Acceptance criteria:** Allowed operations work only within assigned role, employee and company/branch scope. View-all scope does not grant missing action permission.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### CTL-02-C — Role, employee, company and branch access — control

- Parent: CTL-02; priority: P0; execution phase: 1.
- Entry routes: All HR routes and their corresponding API requests. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: USERS; ORG; PEOPLE; POLICY. Preparation references must be ready before execution.

**Steps:** For every workstream try read/create/edit/delete/approve as applicable with the permission removed. Replay corresponding requests using another employee/company/branch ID. Include no-HR-access and signed-out sessions; test transfer source and destination.

**Acceptance criteria:** Server rejects prohibited operations and discloses no protected data. Hidden buttons alone are insufficient. Record each endpoint/action/identity combination and expected denial in evidence.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### CTL-03-P — Arabic and English usability — positive

- Parent: CTL-03; priority: P1; execution phase: 1.
- Entry routes: All HR list/form/approval/report routes. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: USERS; PEOPLE. Preparation references must be ready before execution.

**Steps:** In each workstream open a list and form; enter unsaved synthetic values. Toggle English/Arabic in both directions, then save. Inspect labels, direction, selectors, validation and tables.

**Acceptance criteria:** Language and direction update without losing values; controls, names, dates and numbers remain readable and usable; saved business values remain the same.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### CTL-03-C — Arabic and English usability — control

- Parent: CTL-03; priority: P1; execution phase: 1.
- Entry routes: All HR list/form/approval/report routes. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: USERS; PEOPLE. Preparation references must be ready before execution.

**Steps:** Repeat toggle during validation, with long Arabic/English names, in modals and at a narrow viewport. Inspect approval and report controls for clipping or inaccessible actions.

**Acceptance criteria:** Errors/actions remain understandable and reachable. Toggle does not reset selected employee, duplicate save or reverse business values.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### CTL-04-P — Failures, validation and repeat actions — positive

- Parent: CTL-04; priority: P0; execution phase: 1.
- Entry routes: All HR create/edit/approval routes. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: USERS; POLICY. Preparation references must be ready before execution.

**Steps:** For each workstream save a valid record and refresh. In the isolated UAT session simulate a failed request, observe feedback, restore connectivity and retry using the same fixture.

**Acceptance criteria:** Committed values survive refresh. Failed requests do not show success; retry produces the intended single committed change.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### CTL-04-C — Failures, validation and repeat actions — control

- Parent: CTL-04; priority: P0; execution phase: 1.
- Entry routes: All HR create/edit/approval routes. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: USERS; POLICY. Preparation references must be ready before execution.

**Steps:** Double-click save/approve, repeat the same request, submit invalid fields and refresh during a failed save. Compare record and transaction counts before/after.

**Acceptance criteria:** No duplicate business transaction or partial update is hidden. Clear errors preserve recoverable input; repeated actions cannot falsely succeed.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

## End-to-End Journeys

Business owner: UAT coordinator / workstream owners.

### E2E-01 — Recruit to salary assignment

- Parent: E2E-01; priority: P0; execution phase: 5.
- Entry routes: /HR/Recruitment; /Employee/Employee/List; /HR/PayrollStructures. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: RECRUIT; PEOPLE; PAYREF; USERS. Preparation references must be ready before execution.

**Steps:** Create/open requisition; register applicant; record interview; accept offer; create/identify employee and link offer; assign salary basis. Refresh each stage and capture IDs.

**Acceptance criteria:** One applicant, accepted offer and employee are linked correctly; the employee has the approved effective salary assignment. Manual handoffs are documented; no automatic creation is assumed.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### E2E-02 — Attendance to accounting

- Parent: E2E-02; priority: P0; execution phase: 5.
- Entry routes: /Attendance/MyAttendance; /HR/AttendanceRoster; /HR/WorkEntries; /HR/SaudiPayroll; /HR/Payslips. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: TIMELINE; LEAVEREF; PAYREF; ACCOUNTING; USERS. Preparation references must be ready before execution.

**Steps:** Record session; approve/apply correction; generate, approve and lock work entry; import once to payroll; generate and approve payslip; post accounting. Compare quantities and amounts at every handoff.

**Acceptance criteria:** Session, work entry, payroll input, payslip and journal IDs are traceable. Hours and money reconcile without duplicate consumption or posting. Missing action UI is a gap.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### E2E-03 — Leave allocation to cancellation

- Parent: E2E-03; priority: P0; execution phase: 5.
- Entry routes: /HR/LeavePolicies; /LeavesManagement/MyLeaveApplications; /HR/LeaveApplications; /HR/LeaveLedger; /Attendance/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: LEAVEREF; CALENDAR; USERS. Preparation references must be ready before execution.

**Steps:** Allocate entitlement; employee submits leave; manager approves; inspect attendance and payroll-facing work entries; reconcile balance; cancel through permitted process and reconcile again.

**Acceptance criteria:** Approval affects intended days once; cancellation restores correct leave/attendance treatment. Opening, debit, reversal and closing amounts reconcile across screens.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

### E2E-04 — Transfer and termination downstream

- Parent: E2E-04; priority: P0; execution phase: 5.
- Entry routes: /Employee/Employee/List; /HR/EmployeeLifecycle; /HR/AttendanceRoster; /HR/Payslips; /HR/Reports. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.
- Prerequisites / test data: PEOPLE; POLICY; PAYREF; USERS. Preparation references must be ready before execution.

**Steps:** Transfer EMP-MOVE and check source/destination access, roster, leave, payroll and reporting at effective dates. Separately terminate EMP-EXIT and repeat downstream eligibility/access checks.

**Acceptance criteria:** Every downstream behavior agrees with the approved transfer/termination matrix; history remains available to authorized HR. Each missing control is linked to an issue and owner.

**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.

