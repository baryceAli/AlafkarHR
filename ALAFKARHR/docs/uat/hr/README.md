# HR user acceptance test pack

Prepared 2026-10-09 for Alafkar ERP. This pack implements the agreed manual HR UAT plan: **seven functional workstreams, shared controls, 36 use cases and four end-to-end journeys**. The workstreams are testing responsibilities, not the company's organizational departments.

## Start here

1. Open [HR-UAT-Tracker.xlsx](../../../outputs/01a1201d-ec45-7462-a3b0-78cf827573f8/HR-UAT-Tracker.xlsx). Save a separately named working copy for each release/cycle. Assign a UAT coordinator and named business owners.
2. Complete the **Preparation** sheet. Fill actual fixture IDs, approved reference/evidence and approver/date before marking an item Ready. Record environment URL, API URL, release commit/version, cycle and browser/device. Use an isolated database and synthetic employees.
3. Start Foundation tests, then daily operations, period processing, performance/training, end-to-end journeys, and retests. Run shared access and failure controls in every workstream. Filter **Results** by workstream and consult **Test Cases** for steps/routes. The readable [test catalog](test-cases.md) contains the same procedures.
4. For every record enter assigned tester, actual role/scope, actual result, evidence, executed date, build/cycle and reviewer. Log failed/blocked cases in **Issues** and reference their issue IDs. Capture each subscenario separately in evidence; a grouped record passes only when all its subscenarios pass.
5. Retest fixes on the specified build and repeat affected end-to-end handoffs. Preserve original failure evidence; add dated retest result/evidence and build before changing status. Obtain each **Sign-off** after evidence and outstanding issues are reviewed.

Nothing in this pack represents executed business testing. All 76 test records start Not run. Application code, database schemas and permissions are unchanged.

## Workstreams and owners

| Workstream | Parent use cases | Test records | Business owner |
|---|---|---:|---|
| Personnel and Employee Administration | HR-01–HR-05 | 10 | HR operations manager |
| Recruitment and Hiring | REC-01–REC-04 | 8 | Recruitment lead |
| Attendance and Workforce Scheduling | ATT-01–ATT-06 | 12 | Attendance administrator / workforce supervisor |
| Leave Management | LEV-01–LEV-05 | 10 | Leave administrator; managers approve |
| Payroll and Compensation | PAY-01–PAY-06 | 12 | Payroll manager; Finance validates |
| Performance Management | PER-01–PER-03 | 6 | Performance manager |
| Training and Development | TRN-01–TRN-03 | 6 | Training coordinator |
| Shared Reporting and Controls | CTL-01–CTL-04 | 8 | HR manager / system administrator |
| End-to-end journeys | E2E-01–E2E-04 | 4 | UAT coordinator and workstream owners |

Each parent use case has a positive (`-P`) and control (`-C`) record. P0 cases are critical; P1 cases are required. These are test priorities, separate from defect severity. P0 records must Pass; they cannot be waived through Not applicable.

## Preparation and independent expected values

Prepare two companies, two branches in the primary company, departments, positions and specialist/approver/employee/read-only/no-access accounts. Include two restricted line managers and a separate explicitly authorized view-all account. Record the actual action permissions and company/branch scope, including employee/user links. Never store credentials, session cookies or tokens in evidence.

The employee fixture set covers new joiner, regular employee, overnight worker, loan recipient, transfer and termination, plus another-company employee. Use disposable copies for cancellation, termination, reversal and failure tests so earlier cases retain their evidence.

Before execution, owners approve a full payroll month's calendar, timestamp timeline, leave ledger, payroll reference calculations, accounting expectations, WPS output specification, provision calculations, appraisal scores and training rules. Attach a versioned reference; do not use application output as the independent expected value. Amounts must match the owner-approved currency precision/rounding tolerance; counts and identifiers must match exactly. Record timezone and UTC equivalents for attendance boundaries; display timezone is Asia/Riyadh.

No payroll rates or legal compliance conclusions are inferred from existing code defaults. Missing HR policy or an expected-value baseline blocks the affected case and receives a Policy / baseline issue. The pack provides fields and requirements for these inputs, not invented approved values.

## Evidence, issues and status rules

| Status | Meaning |
|---|---|
| Not run | No execution performed. |
| Pass | Every acceptance criterion/subscenario met on the recorded build with actual results, evidence and reviewer. |
| Fail | Observed behavior or required capability did not meet acceptance; link an issue. |
| Blocked | Environment, dependency or unapproved baseline prevents evaluation; link an issue. |
| Not applicable | P1 only; owner explains why the case is unnecessary for this release and reviewer approves the disposition. |

Missing required functionality is a **Capability gap**, not Pass or automatically Not applicable. An implemented backend action with no usable UI cannot pass manual business UAT. Routes in the catalog are verified page entry points; testers must establish the actual action surface. If an action is inaccessible, record it instead of silently substituting an API-only success.

Use stable issue IDs such as HRUAT-001. Record related test IDs, reproduction steps, expected/actual, evidence, owner, severity, status, workaround/acceptance, target date and retest evidence/build. Severity definitions:

- **Critical:** access leakage, data loss, duplicate processing/payment, incorrect payroll or accounting. Cannot be accepted unresolved.
- **Major:** essential daily HR process blocked or materially wrong. Cannot be accepted unresolved.
- **Minor:** nonblocking issue with an explicitly accepted workaround, owner and target date. Acceptance must identify the approving owner.

Store evidence by cycle/test/attempt, for example `cycle-01/LEV-03-P/attempt-01/`. Include before/after record IDs, exact filters, role/scope, build, screenshots, exported files and calculation comparisons. Record expected and actual HTTP outcome for direct-request access tests, removing secrets. Authorization checks cover read/create/edit/delete/approve where available, each workstream, foreign company/branch/employee IDs, and both sides of a transfer. UI visibility alone is insufficient evidence of backend enforcement.

The Results evidence-check formula checks populated fields, not evidence truth or correctness. Overview summaries update from results, preparation, issues and sign-offs. They support manual review; they never authorize release. Unlogged defects, invalid pasted statuses, false evidence, missing table rows or incorrect classifications still require coordinator review.

Forty blank issue rows are reserved. Add more rows inside the Issues table as needed and extend Overview issue formula ranges. Keep Test IDs attached to their entire row when sorting. Cases/Results have one row per fixed ID; when adding cases, add both rows and extend Overview ranges. Do not delete cases to improve completion counts.

## End-to-end acceptance and release

Run all four journeys after their component cases:

1. Requisition → applicant → interview → accepted offer → employee handoff → salary assignment.
2. Check-in → correction → approved/locked work entry → payroll import → payslip → accounting.
3. Leave allocation → employee request → approval → attendance impact → ledger reconciliation → cancellation/reversal.
4. Transfer and separate termination → source/destination access → roster → leave → payroll eligibility → reports, including effective-date boundaries.

Release requires all cases executed or explicitly dispositioned by their business owner; all P0 cases and financial reconciliations Pass; no unresolved Critical/Major issues; accepted Minor issues have workaround, named acceptance, owner and date. Reconcile original failures with retest evidence and issue closures. Obtain HR operational sign-off, Payroll and Finance monetary/accounting sign-off, and administrator access-control sign-off, plus specialist reviews recorded in the workbook. Pending or rejected sign-off prevents acceptance.

Pay particular attention to these repository observations; they are **inspection-based checks, not confirmed runtime defects**:

- Recruitment links accepted offers to an existing employee ID; verify and document the manual employee-creation handoff.
- Salary runs and payroll entries/payslips coexist. Choose the production workflow in POLICY and test duplicate employee-period processing across both.
- WPS export action calls a mark-exported service. Passing export requires an actual recipient-compatible file, not just a changed status.
- Attendance correction and biometric import interfaces exist; their complete user-facing action coverage must be demonstrated.
- Required termination propagation and training-to-certification synchronization must be measured against approved HR expectations.

## Source alignment and regeneration

The catalog is based on the agreed plan plus targeted inspection of `@page` routes and employee, attendance, leave, payroll, recruitment, performance and training service interfaces in `UI/AlAfkarERP/AlAfkarERP.Shared/Pages/Features/`. Relevant backend checks include RecruitmentFeatures, TrainingFeatures and centralized PermissionList. Sources establish test entry points and available operations, not acceptance results. Employee data is mixed company/branch scope; access acceptance also follows the repository development guide's branch and permission rules.

The standalone builder `build-hr-uat.mjs` writes the workbook and this catalog using the bundled `@oai/artifact-tool` runtime. It refuses to overwrite an existing tracker to protect execution evidence. For a new baseline, use a new output directory in the builder; do not rebuild an executed workbook. Obtain bundled Node/package paths through `load_workspace_dependencies`, resolve `@oai/artifact-tool` through a temporary Windows junction to those packages, then execute the builder from that runtime. Do not install dependencies into the ERP projects.

Workbook verification covers 36-parent/76-test coverage, initial statuses, formula calculations, status/evidence behavior, preparation gating and issue counting. Workbook previews are supporting QA artifacts only. No application build is needed because this pack changes no application source. Manual UAT remains to be executed in the prepared environment.
