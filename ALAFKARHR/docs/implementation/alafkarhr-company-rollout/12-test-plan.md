# Test Plan

## Test Objectives

- Prove that configured AlafkarHR workflows support the company operating model.
- Validate migrated data accuracy.
- Confirm permission and branch access security.
- Confirm payroll, attendance, and leave outputs before go-live.

## Test Types

| Test type | Purpose | Owner |
| --- | --- | --- |
| Configuration smoke test | Confirm configured pages open and save records. | Implementation Lead |
| System integration test | Test cross-module flows such as employee to attendance, leave, payroll. | Implementation Lead |
| Data migration test | Reconcile loaded data. | Data Owner |
| Security test | Confirm role and branch restrictions. | IT Lead |
| UAT | Business users validate real work scenarios. | Business Leads |
| Payroll parallel test | Compare AlafkarHR payroll outputs to approved payroll results. | Payroll Lead |
| Regression test | Retest critical workflows after fixes. | Implementation Lead |

## Entry Criteria

- Scope signed.
- Configuration workbook populated.
- Test users and roles created.
- Test data loaded.
- UAT scripts approved.

## Exit Criteria

- All critical defects closed.
- High defects closed or accepted with workaround.
- UAT sign-off completed.
- Payroll parallel run signed if payroll is in scope.
- Security test passed.

## Defect Severity

| Severity | Meaning | Go-live impact |
| --- | --- | --- |
| Critical | Blocks core HR/payroll/attendance/leave operation or exposes unauthorized data. | Must fix before go-live. |
| High | Major workflow failure with limited workaround. | Fix or formal approval required. |
| Medium | Usability or data issue with workaround. | Can defer with owner approval. |
| Low | Cosmetic or minor improvement. | Can defer. |

## Minimum Test Coverage

- Organization setup and branch visibility.
- Role creation and user role assignment.
- Employee create, edit, view, transfer, position change, termination.
- Employee documents, lifecycle, skills, certifications.
- Attendance shifts, sessions, late request, mid-day permission, holidays, reports.
- Leave policies, balances, applications, emergency leave, approvals, ledger.
- Payroll components, contracts, salary structures, periods, runs, payslips, loans, WPS, Saudi payroll.
- Recruitment, performance, and training where activated.
- Reports and dashboards available to intended roles only.
