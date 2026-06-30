# AlafkarHR Implementation Plan

## Purpose

This plan guides a company rollout of AlafkarHR from preparation through post-go-live support. It is written for company leadership, HR, payroll, IT, implementation consultants, and key users.

## Confirmed System Capability Map

| Domain | Confirmed capabilities |
| --- | --- |
| Organization | Company, parent/child company context, branches, administrations, departments, business lines, license categories, branch access. |
| Security | Users, roles, role assignment, permission groups, company-scoped roles, branch access permissions, security dashboard. |
| Employees | Employee records, company/branch/administration/department/position filters, employee view, public view, QR view, 360 view, teams, positions, academic institutions, specializations. |
| Employee enhancements | Lifecycle events, emergency contacts, document links, skills, certifications, command center, HR reports. |
| Attendance | Dashboard, my attendance, attendance sessions, check-in preview, shifts, shift assignment, holidays, late check-in requests, mid-day permission requests, approvals, reports. |
| Roster and work entries | Roster controls, substitute configuration, shift schedules, shift schedule assignments, shift swaps, corrections, biometric import batches, payroll work entries. |
| Leave | Emergency leave, leave balances, leave reports, leave types, periods, policies, assignments, allocations, leave applications, leave ledger, adjustments, encashment. |
| Payroll | Components, contracts, employee contract assignment, salary runs, period commit/undo, employee loans, salary structures, payroll periods, payroll entries, payslips, payroll inputs, Saudi payroll info, WPS batches, EOS provision snapshots, payroll accounting posting, work-entry import. |
| Recruitment | Staffing plans, job requisitions, applicants, interview feedback, offers, offer-to-employee marking. Requires business validation for final approval rules. |
| Performance | Appraisal cycles, goals, competencies, employee goals, competency scores, evaluations, submit/review/approve/cancel flow. Requires business validation for scoring policy. |
| Training | Training programs, events, attendees, attendance, results, certificate links. Requires business validation for training governance. |
| General settings | System settings, home page templates, currencies. |

## Implementation Goals

- Establish accurate organization and HR master data.
- Configure permission-safe access by role and branch.
- Migrate employee records with validated employment, attendance, leave, and payroll attributes.
- Enable daily HR operations: employee maintenance, attendance, leave, payroll, recruitment, performance, and training where in scope.
- Train each role with the role-based guides in this package.
- Complete controlled go-live with sign-offs, support ownership, and post-go-live review.

## Implementation Assumptions

| Assumption | Validation owner |
| --- | --- |
| The company has an approved organization structure before configuration starts. | HR Sponsor |
| Employee source data can be exported to spreadsheet format. | HR Data Owner |
| Payroll rules, components, periods, WPS requirements, and Saudi payroll fields are approved before payroll setup. | Payroll Lead |
| Attendance devices, geolocation rules, shifts, and holiday calendars are agreed before attendance UAT. | Attendance Lead |
| User roles and branch access rules are approved before UAT. | IT Security Lead |
| Arabic and English terminology is reviewed by HR before training. | HR Lead |

## Phase Plan

| Phase | Duration | Main activities | Key deliverables |
| --- | --- | --- | --- |
| 1. Mobilization | Week 1 | Kickoff, governance, stakeholder confirmation, project workspace, document sign-off process. | Project charter, RACI, timeline, issue log. |
| 2. Discovery and Fit | Weeks 1-2 | Confirm HR policies, organization structure, employee lifecycle, attendance, leave, payroll, recruitment, performance, training. | Scope of work, workflow mapping, configuration workbook draft. |
| 3. Data Preparation | Weeks 2-4 | Collect masters, cleanse employee data, map departments/positions, validate payroll and leave balances. | Master data template, employee data template, migration plan. |
| 4. Configuration | Weeks 3-6 | Configure organization, roles, branches, security, HR masters, shifts, leave policies, payroll components and periods. | Configured environment, roles-permissions matrix, configuration sign-off. |
| 5. Migration and Integration Readiness | Weeks 5-7 | Load or enter data, reconcile counts, validate migrated records, prepare attachments and identifiers. | Migration reconciliation, exception log, data sign-off. |
| 6. Testing | Weeks 6-8 | SIT, UAT, role-based workflows, payroll parallel run if payroll is in scope, security testing. | Test plan results, UAT sign-off. |
| 7. Training and Change | Weeks 7-9 | Train administrators, HR, payroll, managers, employees, and specialist roles. | Training attendance, quick references, readiness report. |
| 8. Cutover and Go-Live | Week 10 | Freeze source changes, final migration, final validation, enable users, go-live communications. | Cutover checklist, go-live sign-off. |
| 9. Hypercare | Weeks 10-12 | Daily triage, issue resolution, adoption support, handover to support. | Hypercare log, support handover, post-go-live review. |

## Workstreams

| Workstream | Scope |
| --- | --- |
| Governance | Kickoff, steering committee, status reporting, decisions, risks, issues, change requests. |
| Organization and Security | Company setup, branches, departments, administrations, roles, users, permission assignment, branch access. |
| HR Core | Employees, positions, teams, academic institutions, specializations, lifecycle, documents, skills, certifications. |
| Attendance | Shifts, sessions, check-in rules, holidays, late requests, mid-day permissions, roster, corrections, biometric import. |
| Leave | Leave types, periods, policies, balances, applications, emergency leave, ledger, reports. |
| Payroll | Components, contracts, salary structures, periods, payroll entries, payslips, loans, WPS, Saudi payroll, accounting posting. |
| Talent | Recruitment, performance, and training workflows. |
| Data | Templates, cleansing, migration, reconciliation, attachment tracking. |
| Testing and Training | SIT, UAT, training, role guides, readiness. |
| Go-Live and Support | Cutover, communications, hypercare, support transition. |

## Success Criteria

- 100% of approved active employees are available in AlafkarHR.
- Organization, branch, department, and position structures match approved HR records.
- Role-based access is approved and tested for each role.
- Attendance, leave, and payroll pilot scenarios are completed successfully.
- Critical UAT defects are closed or formally accepted before go-live.
- End users complete training or receive approved reference material.
- Hypercare triage process is active from the first business day after go-live.

## Business Validation Required

- Final payroll calculations, allowances, deductions, EOS, WPS, and accounting posting policy.
- Attendance geolocation, biometric import format, late check-in, mid-day permission, correction, and roster approval rules.
- Leave accrual, carry-forward, encashment, emergency leave, attachment, and approval rules.
- Recruitment approval thresholds, offer approval, and employee creation policy.
- Performance rating scales, appraisal cycles, manager review, and approval authority.
- Training attendance, result recording, certification, and completion rules.
