import fs from 'node:fs/promises';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import assert from 'node:assert/strict';
import { Workbook, SpreadsheetFile } from '@oai/artifact-tool';

// Static procedures, never production data. Regeneration refuses to overwrite results.
const here = path.dirname(fileURLToPath(import.meta.url));
const root = path.resolve(here, '../../..');
const out = path.join(root, 'outputs/01a1201d-ec45-7462-a3b0-78cf827573f8');
const target = path.join(out, 'HR-UAT-Tracker.xlsx');
try { await fs.access(target); throw new Error('Tracker already exists. Preserve executed results; use a fresh output directory for another cycle.'); }
catch (e) { if (e.code !== 'ENOENT') throw e; }

const groups = {
 HR: ['Personnel and Employee Administration', 'HR operations manager', 1],
 REC: ['Recruitment and Hiring', 'Recruitment lead', 2],
 ATT: ['Attendance and Workforce Scheduling', 'Attendance administrator', 2],
 LEV: ['Leave Management', 'Leave administrator', 2],
 PAY: ['Payroll and Compensation', 'Payroll manager / Finance', 3],
 PER: ['Performance Management', 'Performance manager', 4],
 TRN: ['Training and Development', 'Training coordinator', 4],
 CTL: ['Shared Reporting and Controls', 'HR manager / system administrator', 1],
 E2E: ['End-to-End Journeys', 'UAT coordinator / workstream owners', 5],
};

// ID, use case, UI entry routes, fixtures, positive steps, expected outcomes,
// control steps, expected control outcomes. Each listed subscenario needs evidence.
const definitions = [
 ['HR-01','Maintain HR reference records','/Employee/Position/List; /Employee/Specialization/List; /Employee/AcademicInistitution/List; /Employee/Teams','ORG; USERS; PEOPLE',
 'Create one position, specialization, institution and team with UAT-prefixed bilingual names. Edit each. Refresh. Select the records in the relevant employee/team workflow and reopen the saved record.',
 'All four records retain saved values. Selectors and team membership show the intended records in the correct company.',
 'With a read-only user attempt create/edit/delete for each type. With a specialist submit a foreign-company or nonexistent reference through the corresponding request. Reopen the original record.',
 'Restricted writes and invalid references are rejected. Existing records and memberships remain unchanged.'],
 ['HR-02','Create, edit, search and view employee','/Employee/Employee/List; /Employee/Employee/Form/{Id:guid?}; /Employee/Employee/view/{Id:guid}; /Employee/Employee/360/{Id:guid}','ORG; USERS; PEOPLE',
 'Create EMP-NEW using the prepared required fields and selected branch/department/position. Edit contact information. Search by the unique test name; apply company, branch and department filters. Refresh and compare list, profile and 360.',
 'Exactly the intended employee is returned. Saved details and assignments agree across views and survive refresh.',
 'Submit with each required field missing. Try a nonexistent and foreign-company department/position. Search for an absent name and move between pages of results.',
 'Invalid records are not saved; validation identifies the issue. Empty search has no stale rows. Pagination does not mix or omit matching records.'],
 ['HR-03','Maintain contacts, documents, skills and certificates','/HR/EmployeeEmergencyContacts; /HR/EmployeeDocuments; /HR/EmployeeSkills','PEOPLE; FILES; USERS',
 'Select EMP-REG. Add and edit a contact, document link, skill and certificate. Refresh each page. Open the document/certificate reference. Switch to EMP-NIGHT and back.',
 'Values persist for EMP-REG; references open for the authorized tester. EMP-NIGHT has no copied records from EMP-REG.',
 'Attempt the same writes as a read-only user. Change the employee or child-record ID in a captured request to another company. Try an invalid document reference.',
 'Unauthorized changes and foreign references fail without modifying either employee. Invalid references cannot be presented as successfully usable documents.'],
 ['HR-04','Transfer, change position and process lifecycle','/Employee/Employee/List; /HR/EmployeeLifecycle','PEOPLE; ORG; POLICY; USERS',
 'Record EMP-MOVE current department, branch and position. Process separate transfer and position-change records. For each applicable lifecycle type create, approve and complete at the approved effective date. Refresh employee and event history.',
 'Completed events apply the approved changes once and retain event history, actor and dates where required by POLICY. Employee assignments agree with the completed event.',
 'Create separate events and cancel them before completion. Attempt completion without approval, repeat completion and attempt a transfer to a prohibited branch.',
 'Cancelled events do not change assignments. Invalid/repeated transitions and prohibited transfers are rejected; neither branch gains unauthorized access.'],
 ['HR-05','Terminate employee and reconcile downstream behavior','/Employee/Employee/List; /HR/EmployeeLifecycle; /HR/Reports','PEOPLE; POLICY; PAYREF; USERS',
 'Use EMP-EXIT with history and approved termination date/reason. Terminate through the available employee workflow. Refresh the profile and historical reports. Check future roster, attendance, leave and payroll eligibility against POLICY.',
 'Termination details persist and historical records remain readable to authorized HR. Each downstream inclusion/exclusion matches the approved termination matrix; discrepancies are gaps.',
 'Try termination as an unauthorized user; repeat a completed termination. Test dates before/on/after the effective date in downstream eligibility. Check that employee access follows POLICY.',
 'Unauthorized/repeated actions cannot corrupt history. Boundary-date eligibility and account access match POLICY; absence of a required downstream control is a failure/gap.'],
 ['REC-01','Maintain staffing plans and requisitions','/HR/Recruitment','ORG; USERS; RECRUIT',
 'Create a staffing plan for the test year and position. Create linked requisitions with planned headcount. Edit a draft, open it, then close it; use a separate requisition for cancellation. Refresh and filter.',
 'Year, position, headcount, links and status persist. Open, closed and cancelled records appear under the matching filters.',
 'Attempt transitions from invalid states and repeat close/cancel. Submit missing required information and foreign-company position references. Repeat approval as a user without approval permission.',
 'Invalid references, states and unauthorized approvals are rejected without changing the requisition.'],
 ['REC-02','Register and progress applicants','/HR/Recruitment','RECRUIT; USERS',
 'Register two distinguishable applicants against separate requisitions. Move one through the available approved stages. Refresh; filter by requisition and status.',
 'Details persist and each applicant stays linked to the correct requisition. Status filters return only matching applicants.',
 'Attempt an invalid stage transition and a foreign-company requisition reference. Repeat status change as a read-only user and search for an absent candidate.',
 'Invalid/unauthorized changes fail; applicant association remains correct and empty filters show no stale candidate.'],
 ['REC-03','Record interview feedback','/HR/Recruitment','RECRUIT; USERS; POLICY',
 'Open the intended applicant. Record interview feedback with the approved score scale and comments. Edit it; refresh and compare both applicants.',
 'Scores/comments persist only on the intended applicant and match the approved score scale.',
 'Submit out-of-range scores or missing required feedback. Attempt edit/delete with a read-only user and change the applicant ID to another company.',
 'Invalid scores and unauthorized changes fail; neither applicant receives another applicant’s feedback.'],
 ['REC-04','Offer decision and employee handoff','/HR/Recruitment; /Employee/Employee/Form/{Id:guid?}','RECRUIT; PEOPLE; USERS',
 'Create an offer for the successful applicant. Accept it. Create/identify EMP-NEW through the employee workflow and link that employee to the accepted offer. Refresh offer and employee. Reject a separate offer.',
 'Accepted/rejected statuses are correct. The hiring handoff references exactly EMP-NEW. Record whether employee creation is manual; offer acceptance alone is not proof of creation.',
 'Repeat acceptance and employee linking. Link a nonexistent or foreign-company employee; try linking a rejected offer. Repeat hire action without hire permission.',
 'Invalid links/transitions fail; no duplicate employee or hire association appears; restricted users cannot complete hiring.'],
 ['ATT-01','Configure shifts, calendar and assignments','/Attendance/Shifts; /Attendance/ShiftAssignments; /Attendance/Holidays','CALENDAR; PEOPLE; USERS',
 'Create day and overnight shifts. Configure working days and a holiday through the available configuration surface. Assign shifts with effective dates. Check employee preview/report before, on and after the assignment boundary.',
 'The expected shift, holiday and work-day classification match CALENDAR at each tested date; saved configuration persists.',
 'Try invalid shift times/assignments and unavailable employee references. Change configuration as a read-only user. Test holiday and weekend boundaries.',
 'Invalid/unauthorized configuration does not save. Boundary classifications match the approved calendar; missing configuration UI is a gap.'],
 ['ATT-02','Roster publication, swaps and substitutions','/HR/AttendanceRoster','CALENDAR; PEOPLE; POLICY; USERS',
 'Create a schedule and assign EMP-REG and EMP-NIGHT. Publish and verify each employee/date. Approve one swap and apply an approved substitution. Lock the schedule and refresh.',
 'Published and changed assignments match the approved roster and are visible to the correct employees. Lock status persists.',
 'Cancel a separate swap and schedule. Try overlapping/invalid assignments, a prohibited-branch employee, and editing a locked schedule without authority.',
 'Cancelled swaps do not apply. Invalid or prohibited assignments and unauthorized locked edits fail without changing the roster.'],
 ['ATT-03','Sessions, breaks, overnight and missing checkout','/Attendance/MyAttendance; /Attendance/Sessions','TIMELINE; PEOPLE; USERS',
 'As EMP-REG check in, start/end a break and check out at recorded times. Repeat for EMP-NIGHT across midnight. Inspect sessions/report. Exercise missing-checkout resolution on a separate fixture.',
 'Session date/time, break minutes and net work duration reconcile to TIMELINE. Overnight date attribution and missing-checkout handling match POLICY.',
 'Attempt two active sessions, ending before starting, duplicate checkout, and invalid break order. Attempt another employee’s session action.',
 'Invalid sequences and foreign session actions fail; no duplicate sessions or negative durations are created.'],
 ['ATT-04','Location rules and late/midday requests','/Attendance/MyAttendance; /Attendance/LateRequests; /Attendance/PermissionRequests; /Attendance/ApprovePermissionRequests','GEO; TIMELINE; USERS; POLICY',
 'Check in inside the approved geofence with accepted accuracy. Submit late and midday permission requests. Approve one of each as the intended reviewer; reject separate requests. Refresh status and attendance report.',
 'Geofence result matches GEO. Each decision/comments persist; attendance treatment matches POLICY for approved versus rejected requests.',
 'Test outside boundary, denied/unavailable location, poor accuracy and mocked-location input. Try review without permission and repeat review of decided requests.',
 'Each location condition has the approved explicit outcome. Unauthorized or repeated review cannot change attendance or silently approve the request.'],
 ['ATT-05','Attendance correction and biometric import','/HR/AttendanceRoster; /Attendance/Sessions','TIMELINE; BIO; USERS',
 'Create and approve a correction for the known session, then apply it. Create a biometric batch with valid rows; review and post. Compare sessions and report to source timestamps. Locate the action UI or record a UI gap.',
 'Only the intended session is corrected. Valid import rows yield the expected sessions; timestamps reconcile to BIO and retain source traceability.',
 'Reject a separate correction and try applying it. Import invalid employee/time rows, duplicate rows and repost the same batch. Attempt correction as an unauthorized user.',
 'Rejected/unauthorized corrections do not apply. Invalid rows are identifiable; duplicate/reposted input cannot duplicate attendance. Backend-only capability does not pass manual UAT.'],
 ['ATT-06','Generate, approve and lock work entries','/HR/WorkEntries','TIMELINE; CALENDAR; LEAVEREF; USERS',
 'Generate entries for the prepared employee/date range. Compare work, breaks and approved leave to reference totals. Approve and lock the entries. Refresh.',
 'Entry population and quantities reconcile exactly to the approved attendance/leave reference; approval and lock states persist.',
 'Regenerate the same range and attempt editing locked entries. Try approval without permission and include another company’s employee.',
 'No duplicate or double-counted quantities result; locked/unauthorized changes and foreign-company entries are prevented.'],
 ['LEV-01','Policies, periods and allocations','/HR/LeavePolicies; /LeavesManagement/Balances','LEAVEREF; PEOPLE; USERS; POLICY',
 'Create leave types, period, policy and employee policy assignments. Generate allocations for the prepared population. Compare balance and ledger to LEAVEREF; refresh.',
 'Only assigned eligible employees receive the expected entitlement for the correct period and leave type.',
 'Generate the same allocation again; try invalid periods, foreign-company assignments and unauthorized allocation. Inspect ledger and balances.',
 'No duplicate entitlement is granted. Invalid/unauthorized allocations are rejected without changing balances.'],
 ['LEV-02','Employee leave application','/LeavesManagement/MyLeaveApplications','LEAVEREF; CALENDAR; FILES; USERS',
 'As EMP-REG create a leave draft across the prepared workdays/holiday. Attach the synthetic document, submit and refresh. Compare charged days with LEAVEREF and open the attachment.',
 'Dates/type/attachment persist; charged days match the approved calendar. Request belongs to the logged-in employee and reaches the intended submitted state.',
 'Test reversed dates, overlapping requests, insufficient balance, required attachment missing and a forged employee ID. Check existing requests and balance afterward.',
 'Each condition follows POLICY with clear validation. No unintended request or balance transaction is created, and another employee cannot be impersonated.'],
 ['LEV-03','Approve, reject and cancel leave','/HR/LeaveApplications; /HR/LeaveLedger; /LeavesManagement/Reports','LEAVEREF; USERS; POLICY',
 'Record starting balance. Approve a submitted application, reject a separate one and cancel the approved application using the permitted process. Inspect ledger and reports after each action.',
 'Approval consumes exactly the expected days once; rejection consumes none; cancellation restores the correct amount. Decisions/comments and terminal requests appear in reports.',
 'Repeat approve/cancel; try review without authority and cancellation from a prohibited state. Compare balance/ledger before and after.',
 'Repeated or unauthorized actions do not create extra debits/credits. Invalid transitions are rejected and ledger remains reconciled.'],
 ['LEV-04','Emergency leave','/LeavesManagement/EmergencyLeaves; /LeavesManagement/ApproveEmergencyLeaves','LEAVEREF; FILES; USERS; POLICY',
 'Submit an emergency request with required reason/attachment. Approve it as the assigned reviewer and reject a separate request. Refresh and compare attendance and balance impact.',
 'Requests, files, comments and decisions persist. Approved and rejected treatment match POLICY and LEAVEREF.',
 'Test missing information, overlap, repeated review and review without permission. Attempt another employee/company request through a modified request.',
 'Validation and access rules are enforced; rejected/duplicate requests do not silently consume leave or alter attendance.'],
 ['LEV-05','Ledger adjustments and encashment','/HR/LeaveLedger','LEAVEREF; PAYREF; USERS; POLICY',
 'Record opening balance. Apply an authorized credit/debit adjustment with reason, then encash the approved days. Compare the ledger and any required payroll handoff with LEAVEREF/PAYREF.',
 'Opening plus credits minus debits equals closing balance. Encashment days and amount match approved references; transactions remain traceable.',
 'Repeat the same submission, request excess encashment, use invalid employee/type references and attempt an adjustment without permission.',
 'No duplicate or unauthorized ledger/payroll transactions result. Excess/invalid requests follow POLICY; a missing required payroll handoff is recorded as a gap.'],
 ['PAY-01','Components, contracts and structures','/Payroll/Components; /Payroll/Contracts; /Payroll/AssignContract; /HR/PayrollStructures','PAYREF; PEOPLE; USERS; POLICY',
 'Create earnings/deduction components, a contract and a salary structure. Assign appropriate fixtures with effective dates. Reopen and inspect before/on/after each effective boundary.',
 'Amounts/formulas and assignment dates persist. The applicable salary basis is unambiguous and matches PAYREF for each boundary.',
 'Attempt invalid amounts/formulas, conflicting effective assignments, foreign-company employee references and edits without permission.',
 'Invalid/unauthorized assignments cannot create an ambiguous or unintended salary basis; errors identify rejected data.'],
 ['PAY-02','Employee loans and installment recovery','/Payroll/Loans; /HR/Payslips; /Payroll/SalaryRuns','PAYREF; PEOPLE; USERS; POLICY',
 'Create and approve EMP-LOAN loan with the reference installment schedule. Process one eligible payroll installment. Inspect deduction and outstanding balance. Cancel a separate unused loan.',
 'Only the approved eligible loan is recovered; installment and outstanding balance equal PAYREF; cancelled/unapproved loans are excluded.',
 'Recalculate/repeat payroll, reverse the installment via the permitted payroll process, and reprocess. Try unauthorized approval and cancellation after recovery.',
 'Recovery occurs once; reversal restores the intended balance once. Subsequent cancellation follows POLICY without losing recovered history.'],
 ['PAY-03','Salary-run workflow','/Payroll/SalaryRuns','PAYREF; PEOPLE; USERS; POLICY',
 'Create and calculate salary runs for the designated legacy-workflow fixtures. Compare gross/deductions/net independently. Approve and commit the period. Undo a separate disposable run/period using permitted controls.',
 'Amounts match PAYREF to the approved rounding tolerance. Approval/commit/undo statuses and monetary side effects reconcile with the reference.',
 'Create the same employee-period twice; repeat approve/commit/undo; attempt undo without override authority; use an employee-period already processed through payroll entries.',
 'No duplicate payout/accounting/recovery results across either workflow. Invalid/unauthorized actions fail; missing cross-workflow duplication control is a critical gap.'],
 ['PAY-04','Payroll periods, entries, inputs and payslips','/HR/Payslips; /HR/WorkEntries; /HR/SaudiPayroll','PAYREF; TIMELINE; LEAVEREF; USERS; POLICY',
 'Open the reference period; create employee payroll inputs and import the approved attendance work entries. Create/generate an entry, inspect and recalculate payslips, approve, mark paid and close. Use separate fixtures to reopen/cancel.',
 'Population, gross, deductions, net and loan balances match PAYREF. Attendance is counted once. Every allowed transition and resulting period/entry/payslip status persists.',
 'Repeat generation, work-entry import and paid action; attempt edits in a closed period, approval without authority and payroll for an ineligible employee. Compare totals before/after.',
 'No duplicate slips, inputs or deductions. Closed/invalid states and unauthorized actions are blocked. Eligibility matches POLICY.'],
 ['PAY-05','Payroll accounting posting','/HR/Payslips','PAYREF; ACCOUNTING; USERS',
 'Post the approved test payroll entry. Inspect resulting journals in accounting using Finance access. Compare company, branch, accounts, debit/credit and totals to ACCOUNTING.',
 'Debits equal credits; journals match the payroll reference and intended company/branch. Posting links remain traceable.',
 'Post again. On a separate fixture deliberately use incomplete accounting setup, inspect failure state, correct the setup and retry. Try posting without permission.',
 'Repeated posting cannot duplicate journals. Failed posting leaves no partial/double posting; corrected retry succeeds once; unauthorized posting fails.'],
 ['PAY-06','Saudi payroll, WPS and provision snapshots','/HR/SaudiPayroll','PAYREF; WPS; EOS; USERS; POLICY',
 'Enter approved employee payroll information/rates. Generate a WPS batch from eligible approved payslips; obtain the actual file and validate schema, employee population and totals against WPS. Create an EOS snapshot for the reference date and compare to EOS.',
 'Inputs, calculations, WPS contents and EOS totals match the independently approved references. An actual consumable validated file is required to pass export; status alone is insufficient.',
 'Use missing/invalid bank information, excluded employees and duplicate batch requests. Attempt exported-batch changes and snapshot creation without authority. Compare output again.',
 'Invalid WPS records are exposed and cannot appear as successful output. No duplicate payment batch or unauthorized change. If export only changes status, log a blocking export gap.'],
 ['PER-01','Cycles, goals and competencies','/HR/Performance','PERF; PEOPLE; USERS',
 'Create an appraisal cycle, goal definitions and competencies. Assign the prepared employee goals with reference weights. Start the cycle; refresh and inspect associations.',
 'Definitions, dates, weights and associations persist for the correct cycle and employees; available actions reflect the cycle state.',
 'Try invalid dates/weights, foreign-company employees, actions outside the active cycle and configuration without permission.',
 'Invalid or unauthorized configuration/actions are rejected without corrupting the cycle or employee goals.'],
 ['PER-02','Scores, achievement and calculation','/HR/Performance','PERF; USERS',
 'Enter the reference achievement and competency scores. Create/recalculate the appraisal and compare components, weights, total and rating to the independent PERF calculation.',
 'All score components and final rating match the approved reference/rounding. Recalculation yields the same result for unchanged inputs.',
 'Submit out-of-range scores, missing required scores and invalid weights. Change one valid score, recalculate, then restore it and recalculate again.',
 'Invalid inputs fail or visibly block a final result. Valid edits change the result as expected and restoration reproduces the reference total.'],
 ['PER-03','Appraisal submission, review and approval','/HR/Performance','PERF; USERS; POLICY',
 'Submit employee feedback, record manager review, approve and close the cycle. Use a separate appraisal for cancellation. Refresh evaluation/history.',
 'Employee/manager feedback, decisions and final results persist in the correct sequence; cancelled records remain distinct from approved results.',
 'Attempt approval before review, employee self-approval, unauthorized access to another employee, and edits after finalization. Repeat final approval.',
 'Role/state restrictions prevent unauthorized or repeated finalization and silent changes to finalized results.'],
 ['TRN-01','Training programs and events','/HR/Training','TRAIN; USERS',
 'Create a program and dated event. Edit the draft; open, start and complete it. Create a separate event and cancel it. Refresh and filter program/status.',
 'Program/event details and states persist; filters return the correct events and transitions follow the approved sequence.',
 'Try reversed dates, invalid transition order, editing a completed event without authority and create/cancel as a read-only user.',
 'Invalid/unauthorized actions fail without changing completed event data.'],
 ['TRN-02','Enrollment, attendance and results','/HR/Training','TRAIN; PEOPLE; USERS',
 'Enroll EMP-REG and EMP-NIGHT in the event. Mark attended/absent respectively and record their distinct results. Refresh and compare attendee lists.',
 'Correct employees are enrolled once; attendance/results remain associated with the intended attendee and event.',
 'Enroll the same employee twice; submit a foreign-company employee and invalid result; record attendance/results without permission.',
 'Duplicate/invalid enrollment and unauthorized writes fail; no result leaks to another attendee.'],
 ['TRN-03','Certificates and training history','/HR/Training; /HR/EmployeeSkills; /HR/Reports','TRAIN; FILES; PEOPLE; POLICY; USERS',
 'Link the synthetic certificate to the completed attendee. Open it and inspect event/employee training history. Compare any required employee-certification synchronization against POLICY.',
 'Certificate opens for the authorized user and belongs to the correct attendee; history reconciles with completed events. Required synchronization is demonstrated or logged as a gap.',
 'Try an invalid certificate link, another attendee/company ID and unauthorized access. Inspect history for an absent or cancelled attendee.',
 'Invalid/unauthorized certificate access and association fail. History does not falsely show completion or certification for cancelled/absent attendees.'],
 ['CTL-01','Dashboards, reports and exports','/Employee/Dashboard; /HR/Reports; /Attendance/Reports; /LeavesManagement/Reports','REPORTREF; USERS',
 'For each HR area, record filter values and run the matching report/dashboard. Compare record IDs, counts and monetary/time totals with REPORTREF. Export where offered and compare file contents.',
 'Displayed and exported populations/totals agree with the same reference and filters. Required but unavailable reporting/export is recorded as a gap.',
 'Test no-match filters, date cutoffs, branch changes and pagination. Export after changing filters; inspect for stale or out-of-scope records.',
 'Empty views show no stale totals. Boundary, paginated and exported data respect filters and access scope.'],
 ['CTL-02','Role, employee, company and branch access','All HR routes and their corresponding API requests','USERS; ORG; PEOPLE; POLICY',
 'For every workstream use its specialist, approver and employee test accounts. Exercise the permitted reads/writes. Repeat branch reads as the restricted manager and separately as the explicitly authorized view-all user.',
 'Allowed operations work only within assigned role, employee and company/branch scope. View-all scope does not grant missing action permission.',
 'For every workstream try read/create/edit/delete/approve as applicable with the permission removed. Replay corresponding requests using another employee/company/branch ID. Include no-HR-access and signed-out sessions; test transfer source and destination.',
 'Server rejects prohibited operations and discloses no protected data. Hidden buttons alone are insufficient. Record each endpoint/action/identity combination and expected denial in evidence.'],
 ['CTL-03','Arabic and English usability','All HR list/form/approval/report routes','USERS; PEOPLE',
 'In each workstream open a list and form; enter unsaved synthetic values. Toggle English/Arabic in both directions, then save. Inspect labels, direction, selectors, validation and tables.',
 'Language and direction update without losing values; controls, names, dates and numbers remain readable and usable; saved business values remain the same.',
 'Repeat toggle during validation, with long Arabic/English names, in modals and at a narrow viewport. Inspect approval and report controls for clipping or inaccessible actions.',
 'Errors/actions remain understandable and reachable. Toggle does not reset selected employee, duplicate save or reverse business values.'],
 ['CTL-04','Failures, validation and repeat actions','All HR create/edit/approval routes','USERS; POLICY',
 'For each workstream save a valid record and refresh. In the isolated UAT session simulate a failed request, observe feedback, restore connectivity and retry using the same fixture.',
 'Committed values survive refresh. Failed requests do not show success; retry produces the intended single committed change.',
 'Double-click save/approve, repeat the same request, submit invalid fields and refresh during a failed save. Compare record and transaction counts before/after.',
 'No duplicate business transaction or partial update is hidden. Clear errors preserve recoverable input; repeated actions cannot falsely succeed.'],
];

const journeys = [
 ['E2E-01','Recruit to salary assignment','/HR/Recruitment; /Employee/Employee/List; /HR/PayrollStructures','RECRUIT; PEOPLE; PAYREF; USERS',
 'Create/open requisition; register applicant; record interview; accept offer; create/identify employee and link offer; assign salary basis. Refresh each stage and capture IDs.',
 'One applicant, accepted offer and employee are linked correctly; the employee has the approved effective salary assignment. Manual handoffs are documented; no automatic creation is assumed.'],
 ['E2E-02','Attendance to accounting','/Attendance/MyAttendance; /HR/AttendanceRoster; /HR/WorkEntries; /HR/SaudiPayroll; /HR/Payslips','TIMELINE; LEAVEREF; PAYREF; ACCOUNTING; USERS',
 'Record session; approve/apply correction; generate, approve and lock work entry; import once to payroll; generate and approve payslip; post accounting. Compare quantities and amounts at every handoff.',
 'Session, work entry, payroll input, payslip and journal IDs are traceable. Hours and money reconcile without duplicate consumption or posting. Missing action UI is a gap.'],
 ['E2E-03','Leave allocation to cancellation','/HR/LeavePolicies; /LeavesManagement/MyLeaveApplications; /HR/LeaveApplications; /HR/LeaveLedger; /Attendance/Reports','LEAVEREF; CALENDAR; USERS',
 'Allocate entitlement; employee submits leave; manager approves; inspect attendance and payroll-facing work entries; reconcile balance; cancel through permitted process and reconcile again.',
 'Approval affects intended days once; cancellation restores correct leave/attendance treatment. Opening, debit, reversal and closing amounts reconcile across screens.'],
 ['E2E-04','Transfer and termination downstream','/Employee/Employee/List; /HR/EmployeeLifecycle; /HR/AttendanceRoster; /HR/Payslips; /HR/Reports','PEOPLE; POLICY; PAYREF; USERS',
 'Transfer EMP-MOVE and check source/destination access, roster, leave, payroll and reporting at effective dates. Separately terminate EMP-EXIT and repeat downstream eligibility/access checks.',
 'Every downstream behavior agrees with the approved transfer/termination matrix; history remains available to authorized HR. Each missing control is linked to an issue and owner.'],
];

const cases = definitions.flatMap(d => ['P','C'].map(variant => {
 const [id,title,route,data,steps,expected,controls,controlExpected] = d;
 const [area,owner,phase] = groups[id.split('-')[0]];
 const critical = ['HR-04','HR-05','REC-04','ATT-03','ATT-05','ATT-06','LEV-01','LEV-02','LEV-03','LEV-05','CTL-02','CTL-04'].includes(id) || id.startsWith('PAY');
 return {id:`${id}-${variant}`,parent:id,area,owner,phase,priority:critical?'P0':'P1',title:`${title} — ${variant==='P'?'positive':'control'}`,route,data,steps:variant==='P'?steps:controls,expected:variant==='P'?expected:controlExpected};
}));
for (const d of journeys) cases.push({id:d[0],parent:d[0],area:groups.E2E[0],owner:groups.E2E[1],phase:5,priority:'P0',title:d[1],route:d[2],data:d[3],steps:d[4],expected:d[5]});
assert.equal(definitions.length,36);
assert.equal(cases.length,76);
assert.equal(new Set(cases.map(c=>c.id)).size,76);

const prep = [
 ['ENV','Environment','UAT coordinator','Record UI/API URL, isolated database, release/commit, deployment time, browser/device and UAT cycle. Confirm backup/reset procedure.'],
 ['ORG','Organizations','System administrator','Create UAT-CO-A and UAT-CO-B; A-MAIN and A-SECOND branches; two departments and positions. Record real generated IDs. No production records.'],
 ['USERS','Accounts and access','System administrator','Create HR admin, specialists per area, approver, two branch-restricted managers, two employee accounts, read-only and no-HR-access users. Record action permissions and explicit scope; never passwords/tokens.'],
 ['PEOPLE','Employee fixtures','HR operations manager','Create EMP-NEW, EMP-REG, EMP-NIGHT, EMP-LOAN, EMP-MOVE and EMP-EXIT in A; another employee in B. Map each alias to actual ID, branch, department, position, dates and user. Separate disposable variants for destructive tests.'],
 ['POLICY','Approved business rules','HR / Payroll owners','Attach signed/versioned rules for approval authority, leave days/overlap/negative balance, location handling, effective dates, termination and transfer downstream treatment, training synchronization and payroll rounding. Identify production payroll workflow.'],
 ['CALENDAR','Working calendar','Attendance / Leave owners','Choose full payroll month; list workdays, weekends, holidays, day and overnight shift dates. Approve expected leave-day and overnight-date attribution. Use Asia/Riyadh display and record UTC equivalents.'],
 ['TIMELINE','Attendance references','Attendance administrator','Independent timestamp reference: normal day, break, overnight, late arrival, missing checkout and correction. Approve expected gross/break/net minutes per date plus approved leave inputs.'],
 ['GEO','Location cases','Attendance administrator','Record test geofence, approved in/out/boundary coordinates and accuracy; expected outcomes for denied/unavailable, low accuracy and mocked location. Use test device.'],
 ['BIO','Biometric import rows','Attendance administrator','Synthetic valid, duplicate, invalid-employee and invalid-time rows with expected sessions and counts. Record source timezone, device identifier and batch IDs.'],
 ['FILES','Synthetic attachments','HR operations manager','Prepare accessible synthetic document and certificate files plus missing/invalid references. Record permitted test attachment types and access expectations.'],
 ['RECRUIT','Hiring fixtures','Recruitment lead','One staffing plan, two requisitions, two applicants, scored interview, accepted and rejected offers. Approve score scale, transitions and manual employee-creation handoff.'],
 ['LEAVEREF','Leave reference ledger','Leave administrator','Independently approve per-employee/type/period opening, allocation, adjustment, approval debit, rejection zero, cancellation reversal and encashment debit. Record expected closing after each step.'],
 ['PAYREF','Payroll reference sheet','Payroll manager','Approve one full-period employee population; salary components, effective assignments, attendance quantities, prorating, loan installments, rates, gross/deductions/net and rounding tolerance. Separate legacy/entry fixtures plus a cross-workflow duplicate fixture. Do not copy application output as expected values.'],
 ['ACCOUNTING','Accounting reference','Finance','Approve per-company/branch journals/accounts and expected debits/credits and payroll reconciliation. Prepare a disposable missing-configuration fixture for failed-posting recovery.'],
 ['WPS','WPS output specification','Payroll / Finance','Supply approved recipient file format, sample/schema, eligible population, bank-data validation and expected payment totals. Export status alone is not evidence of a file.'],
 ['EOS','End-of-service reference','Payroll / Finance','Supply approved calculation method/rates, employee inputs, service dates, reference date and expected provision amounts. No legal-rate assumptions from code defaults.'],
 ['PERF','Appraisal reference','Performance manager','Approve cycle, two employees, goals/weights, competencies, score ranges, independently calculated components/total/rating, and allowed transition roles.'],
 ['TRAIN','Training fixtures','Training coordinator','Create program/event; two attendees with distinct attendance/results; approved completion/certificate rules and employee-history expectations.'],
 ['REPORTREF','Report controls','Workstream owners','Approve record-ID populations and counts/totals by employee, date, company and branch for every HR report; include no-match, boundary and pagination examples.'],
];

const wb = Workbook.create();
const sheets = Object.fromEntries(['Overview','Results','Test Cases','Preparation','Issues','Sign-off'].map(n=>[n,wb.worksheets.add(n)]));
const letters = n => {let s=''; for(n++;n;n=Math.floor((n-1)/26))s=String.fromCharCode(65+(n-1)%26)+s;return s;};
function table(sheet, headers, rows, widths, name) {
 const end=letters(headers.length-1), last=rows.length+5;
 sheet.showGridLines=false;
 sheet.getRange(`A2:${end}2`).format.rowHeight=23;
 sheet.getRange('A2').values=[[name.replaceAll('_',' ')]];
 sheet.getRange('A2').format.font={name:'Arial',size:15,bold:true,color:'#243746'};
 sheet.getRangeByIndexes(4,0,rows.length+1,headers.length).values=[headers,...rows];
 const used=sheet.getRange(`A5:${end}${last}`);
 used.format={font:{name:'Arial',size:11,color:'#243746'},wrapText:true,verticalAlignment:'top',rowHeight:48};
 sheet.getRange(`A5:${end}5`).format={fill:'#243746',font:{name:'Arial',size:11,color:'#FFFFFF',bold:true},rowHeight:34,verticalAlignment:'center',horizontalAlignment:'center'};
 widths.forEach((w,i)=>sheet.getRange(`${letters(i)}5:${letters(i)}${last}`).format.columnWidth=w);
 sheet.tables.add(`A5:${end}${last}`,true,name.replaceAll('-','_'));
 sheet.freezePanes.freezeRows(5); sheet.freezePanes.freezeColumns(1);
 return last;
}
function dropdown(sheet,range,values){sheet.getRange(range).dataValidation={rule:{type:'list',values}};}
function input(sheet,range){sheet.getRange(range).format.fill='#FFF2CC';}
function colorStatus(sheet,range){
 for(const [text,fill] of [['Fail','#FCE4D6'],['Blocked','#FFF2CC'],['Not run','#E7E6E6'],['Pass','#E2EFDA']])sheet.getRange(range).conditionalFormats.add('containsText',{text,format:{fill}});
}

const last=table(sheets.Results,['Test ID','Workstream','Priority','Assigned tester','Test role / scope','Status','Actual result','Evidence location / IDs','Issue / gap IDs','Retest result / evidence','Executed date','Build / cycle','N/A owner / reason','Reviewer','Evidence check'],cases.map(c=>[c.id,c.area,c.priority,'','','Not run','','','','','','','','','']),[17,30,10,22,26,19,55,42,22,45,17,25,40,22,31],'HR_Results');
input(sheets.Results,`D6:N${last}`);
dropdown(sheets.Results,`F6:F${last}`,['Not run','Pass','Fail','Blocked','Not applicable']);
colorStatus(sheets.Results,`F6:F${last}`);
sheets.Results.getRange(`K6:K${last}`).setNumberFormat('yyyy-mm-dd');
const evidenceFormula=r=>`=IF(F${r}="Not run","Not run",IF(F${r}="Not applicable",IF(AND(M${r}<>"",N${r}<>""),"Review disposition","Missing N/A approval"),IF(AND(D${r}<>"",E${r}<>"",G${r}<>"",H${r}<>"",K${r}<>"",L${r}<>"",N${r}<>"",OR(F${r}="Pass",I${r}<>"")),"Recorded","Missing evidence / issue")))`;
sheets.Results.getRange(`O6:O${last}`).formulas=cases.map((_,i)=>[evidenceFormula(i+6)]);
sheets.Results.getRange(`O6:O${last}`).conditionalFormats.add('containsText',{text:'Missing',format:{fill:'#FCE4D6',font:{color:'#9C0006'}}});

table(sheets['Test Cases'],['Test ID','Parent use case','Workstream / owner','Phase','Priority','Scenario','Entry routes','Required fixture IDs','Steps — all listed variants','Acceptance criteria','Evidence required'],cases.map(c=>[c.id,c.parent,`${c.area}\n${c.owner}`,c.phase,c.priority,c.title,c.route,c.data,c.steps,c.expected,'Before/after IDs, screenshots or request/response outcome, calculations/files where applicable. Identify every subscenario and role.']),[17,18,38,9,10,43,48,30,90,85,55],'HR_Test_Cases');
sheets['Test Cases'].getRange(`A6:K${last}`).format.rowHeight=190;

const prepLast=table(sheets.Preparation,['Fixture ID','Requirement','Owner','Required content','Status','Actual IDs / approved reference / evidence','Approved by / date','Ready with evidence (1/0)'],prep.map(p=>[...p,'Not ready','','','']),[18,29,29,95,20,60,30,20],'HR_Preparation');
input(sheets.Preparation,`E6:G${prepLast}`);
dropdown(sheets.Preparation,`E6:E${prepLast}`,['Not ready','In preparation','Ready']);
sheets.Preparation.getRange(`A6:G${prepLast}`).format.rowHeight=100;
sheets.Preparation.getRange(`H6:H${prepLast}`).formulas=prep.map((_,i)=>{const r=i+6;return [`=IF(AND(E${r}="Ready",F${r}<>"",G${r}<>""),1,0)`];});
sheets.Preparation.getRange('A3').values=[['Ready requires actual IDs or an approved reference AND approver/date; placeholders are not approved baselines.']];

const issueLast=table(sheets.Issues,['Issue ID','Test IDs','Type','Severity','Summary / reproduction','Expected / actual','Evidence','Owner','Status','Workaround / acceptance','Target date','Retest evidence / build','Minor still open (1/0)'],Array.from({length:40},()=>Array(13).fill('')),[20,26,18,18,70,65,40,25,24,60,18,55,20],'HR_Issues');
input(sheets.Issues,`A6:L${issueLast}`);
dropdown(sheets.Issues,`C6:C${issueLast}`,['Defect','Capability gap','Environment','Policy / baseline']);
dropdown(sheets.Issues,`D6:D${issueLast}`,['Critical','Major','Minor']);
dropdown(sheets.Issues,`I6:I${issueLast}`,['Open','In progress','Ready for retest','Closed','Accepted minor']);
sheets.Issues.getRange(`K6:K${issueLast}`).setNumberFormat('yyyy-mm-dd');
sheets.Issues.getRange(`M6:M${issueLast}`).formulas=Array.from({length:40},(_,i)=>{const r=i+6;return [`=IF(D${r}<>"Minor",0,IF(I${r}="Closed",0,IF(AND(I${r}="Accepted minor",H${r}<>"",J${r}<>"",ISNUMBER(K${r})),0,1)))`];});

const signRows=[
 ['HR operations manager','Personnel / lifecycle','All HR cases, E2E-01 and E2E-04; historical and downstream behavior'],
 ['Recruitment lead','Recruitment','All REC cases and E2E-01; hiring handoff'],
 ['Attendance administrator','Attendance','All ATT cases and E2E-02; minutes and work-entry reconciliation'],
 ['Leave administrator','Leave','All LEV cases and E2E-03; ledger reconciliation'],
 ['Payroll manager','Payroll','All PAY cases, E2E-02 / E2E-04; approved reference amounts and production workflow'],
 ['Finance','Payroll accounting / output','PAY-05 / PAY-06; journals, WPS usable file and provision references'],
 ['Performance manager','Performance','All PER cases; independent score calculation'],
 ['Training coordinator','Training','All TRN cases; employee history and certificates'],
 ['System administrator','Access controls','CTL-02 for every HR workstream and transfer source/destination'],
 ['HR manager','Operational release acceptance','All critical cases passed; all other cases passed or owner-dispositioned; no unresolved critical/major issues; accepted minor issues have workaround, owner and date'],
];
table(sheets['Sign-off'],['Owner role','Area','Review requirement','Decision','Named approver','Date','Evidence / accepted minor issue IDs','Comments','Approved with evidence (1/0)'],signRows.map(r=>[...r,'Pending','','','','','']),[29,32,85,22,26,18,55,50,20],'HR_Signoff');
input(sheets['Sign-off'],'D6:H15');
dropdown(sheets['Sign-off'],'D6:D15',['Pending','Approved','Rejected']);
sheets['Sign-off'].getRange('F6:F15').setNumberFormat('yyyy-mm-dd');
sheets['Sign-off'].getRange('A6:H15').format.rowHeight=85;
sheets['Sign-off'].getRange('I6:I15').formulas=signRows.map((_,i)=>{const r=i+6;return [`=IF(AND(D${r}="Approved",E${r}<>"",ISNUMBER(F${r}),G${r}<>""),1,0)`];});

const overview=sheets.Overview;
overview.showGridLines=false;
overview.getRange('A1:I42').format.font={name:'Arial',size:11,color:'#243746'};
overview.getRange('A2').values=[['Alafkar ERP | HR UAT']];
overview.getRange('A2').format.font={name:'Arial',size:16,bold:true,color:'#243746'};
overview.getRange('A4').values=[['76 test records • 36 use cases • 4 journeys • Initial state: Not run']];
overview.getRange('A6:I6').values=[['Workstream','Total','Not run','Pass','Fail','Blocked','N/A','P0 not passed','Recorded / reviewed']];
overview.getRange('A6:I6').format={fill:'#243746',font:{color:'#FFFFFF',bold:true},wrapText:true,rowHeight:34};
overview.getRange('A7:A15').values=Object.values(groups).map(g=>[g[0]]);
const count=(col,value,extra='')=>`COUNTIFS('Results'!$B$6:$B$${last},$A{r},'Results'!$${col}$6:$${col}$${last},${value}${extra})`;
for(let r=7;r<=15;r++){
 const f=[`=COUNTIFS('Results'!$B$6:$B$${last},$A${r})`,...['Not run','Pass','Fail','Blocked','Not applicable'].map(s=>'='+count('F',`"${s}"`).replace('{r}',r)),`=${count('C','"P0"',`,'Results'!$F$6:$F$${last},"<>Pass"`).replace('{r}',r)}`,`=${count('O','"Recorded"').replace('{r}',r)}+${count('O','"Review disposition"').replace('{r}',r)}`];
 overview.getRange(`B${r}:I${r}`).formulas=[f];
}
overview.getRange('A16').values=[['Total']];
overview.getRange('B16:I16').formulas=[Array.from({length:8},(_,i)=>`=SUM(${letters(i+1)}7:${letters(i+1)}15)`)];
overview.getRange('A16:I16').format={font:{bold:true},fill:'#E7EEF4'};
overview.getRange('A19:A24').values=[['Preparation items still incomplete'],['Unresolved critical / major issues'],['Minor issues lacking closure or accepted workaround'],['Evidence / disposition still incomplete'],['Sign-offs still incomplete'],['Decision support']];
overview.getRange('B19').formulas=[[`=${prep.length}-SUM('Preparation'!$H$6:$H$${prepLast})`]];
overview.getRange('B20').formulas=[[`=COUNTIFS('Issues'!$D$6:$D$${issueLast},"Critical",'Issues'!$I$6:$I$${issueLast},"<>Closed")+COUNTIFS('Issues'!$D$6:$D$${issueLast},"Major",'Issues'!$I$6:$I$${issueLast},"<>Closed")`]];
overview.getRange('B21').formulas=[[`=SUM('Issues'!$M$6:$M$${issueLast})`]];
overview.getRange('B22').formulas=[['=B16-I16']];
overview.getRange('B23').formulas=[['=10-SUM(\'Sign-off\'!$I$6:$I$15)']];
overview.getRange('B24').values=[['Manual release review required']];
overview.getRange('A27').values=[['Start with Preparation, then filter Results by phase/workstream using Test Cases. Yellow cells are editable.']];
overview.getRange('A29').values=[['P0 = critical; P1 = required. All subscenarios must pass before a test record can Pass.']];
overview.getRange('A31').values=[['Do not mark missing functionality Pass. Log a capability gap. N/A needs owner approval and reviewer.']];
overview.getRange('A33').values=[['Counts support review; they do not verify evidence contents or authorize release. See docs/uat/hr/README.md.']];
overview.getRange('A35').values=[['Append issues inside the table; extend Overview formula ranges beyond the 40 reserved issue rows.']];
overview.getRange('A37').values=[['Retests: keep original actual/evidence, add dated retest evidence/build and update status only after review.']];
overview.getRange('A7:I16').format.rowHeight=42;
overview.getRange('A7:A16').format.wrapText=true;
overview.getRange('A6:A37').format.columnWidth=43;
overview.getRange('B6:I16').format.columnWidth=15;
overview.getRange('A19:A24').format.columnWidth=43;
overview.getRange('A19:B24').format.rowHeight=42;
overview.getRange('A19:A24').format.wrapText=true;
overview.getRange('B19:B23').conditionalFormats.add('cellIs',{operator:'greaterThan',formula:0,format:{fill:'#FFF2CC'}});

// Authoring checks change inputs temporarily and restore them; no business UAT is executed.
wb.recalculate();
assert.equal(overview.getRange('B16').values[0][0],76);
assert.equal(overview.getRange('C16').values[0][0],76);
assert.equal(overview.getRange('D16').values[0][0],0);
assert.equal(overview.getRange('B19').values[0][0],19);
assert.equal(overview.getRange('B22').values[0][0],76);
assert.equal(overview.getRange('B23').values[0][0],10);
sheets.Results.getRange('F6').values=[['Pass']]; wb.recalculate();
assert.equal(overview.getRange('D16').values[0][0],1);
assert.equal(sheets.Results.getRange('O6').values[0][0],'Missing evidence / issue');
sheets.Results.getRange('F6').values=[['Not run']];
sheets.Preparation.getRange('E6').values=[['Ready']]; wb.recalculate();
assert.equal(overview.getRange('B19').values[0][0],19);
sheets.Preparation.getRange('F6:G6').values=[['test reference','test approver / date']]; wb.recalculate();
assert.equal(overview.getRange('B19').values[0][0],18);
sheets.Preparation.getRange('E6:G6').values=[['Not ready','','']];
sheets.Issues.getRange('A6:D6').values=[['TEST-ONLY','HR-01-P','Defect','Critical']];
sheets.Issues.getRange('I6').values=[['Open']]; wb.recalculate();
assert.equal(overview.getRange('B20').values[0][0],1);
sheets.Issues.getRange('A6:L6').values=[Array(12).fill('')];
wb.recalculate();

await fs.mkdir(out,{recursive:true});
const scan=await wb.inspect({kind:'match',searchTerm:'#REF!|#DIV/0!|#VALUE!|#NAME\\?|#N/A|#NUM!|#NULL!|#SPILL!|#CALC!',options:{useRegex:true,maxResults:30},summary:'HR UAT formula error scan'});
console.log(scan.ndjson);
console.log((await wb.inspect({kind:'table',range:'Overview!A16:I24',include:'values,formulas',tableMaxRows:9,tableMaxCols:9,maxChars:2200})).ndjson);
const previewDir=path.join(root,'tmp/hr-uat-previews');
await fs.mkdir(previewDir,{recursive:true});
for(const [name,range] of [['Overview','A1:I24'],['Results','A5:H8'],['Test Cases','F5:J7'],['Preparation','A5:G8'],['Issues','A5:F8'],['Sign-off','A5:H8']]){
 const image=await wb.render({sheetName:name,range,scale:1,format:'png'});
 await fs.writeFile(path.join(previewDir,`${name.replaceAll(' ','-')}.png`),new Uint8Array(await image.arrayBuffer()));
}
await (await SpreadsheetFile.exportXlsx(wb)).save(target);

let md='# HR UAT executable test catalog\n\nGenerated from `build-hr-uat.mjs`. All 76 records are initially **Not run** in the workbook. Do not infer a passing result from these expected outcomes.\n\n';
for(const [key,[area,owner]] of Object.entries(groups)){
 md+=`## ${area}\n\nBusiness owner: ${owner}.\n\n`;
 for(const c of cases.filter(c=>c.id.startsWith(key+'-'))){
  md+=`### ${c.id} — ${c.title}\n\n- Parent: ${c.parent}; priority: ${c.priority}; execution phase: ${c.phase}.\n- Entry routes: ${c.route}. Replace route placeholders with fixture IDs; a route is an entry point, not proof that every action has UI.\n- Prerequisites / test data: ${c.data}. Preparation references must be ready before execution.\n\n**Steps:** ${c.steps}\n\n**Acceptance criteria:** ${c.expected}\n\n**Evidence:** Record actual role/scope, before/after record IDs, each subscenario, response/screenshot or exported file, and independent reconciliation where applicable. Missing UI or behavior is a capability gap; missing prerequisites are Blocked.\n\n`;
 }
}
await fs.writeFile(path.join(here,'test-cases.md'),md);
console.log(JSON.stringify({workbook:target,useCases:36,tests:cases.length,preparationItems:prep.length,initialStatus:'Not run',checks:'coverage, formula calculation, status change, missing evidence, preparation gating, issue counting'}));
