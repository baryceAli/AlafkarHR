# Project Charter

## Project Name

AlafkarHR Company Implementation.

## Project Objective

Implement AlafkarHR as the operational HR system for organization structure, employee records, attendance, leave, payroll, talent processes, role-based access, and HR reporting.

## Business Case

The company requires a centralized HR platform that supports bilingual HR operations, company and branch scoping, role-based permissions, structured employee data, attendance operations, leave control, payroll processing, and management visibility.

## In Scope

- Company, branch, administration, department, and business line setup.
- User and role setup using AlafkarHR permission groups.
- Employee records, employment attributes, positions, teams, specializations, academic institutions.
- Employee lifecycle, emergency contacts, document links, skills, and certifications.
- Attendance shifts, sessions, holidays, requests, approvals, roster controls, reports, and work entries.
- Leave types, periods, policies, applications, balances, ledger, emergency leave, and reports.
- Payroll components, contracts, salary structures, periods, entries, payslips, loans, WPS, Saudi payroll information, and payroll accounting posting where approved.
- Recruitment, performance, and training configuration after business validation.
- Data migration, testing, training, cutover, and hypercare.

## Out of Scope Unless Approved

- Application code changes.
- Custom integrations not already confirmed.
- Custom payroll rule development outside confirmed configuration.
- Historical document digitization beyond agreed upload or link scope.
- ERPNext or Odoo comparison.

## Key Stakeholders

| Role | Responsibility |
| --- | --- |
| Executive Sponsor | Approves budget, scope, go-live decision. |
| HR Sponsor | Owns HR process decisions and HR readiness. |
| Payroll Lead | Owns payroll setup, payroll testing, and payroll sign-off. |
| IT Lead | Owns users, access, environment readiness, and support transition. |
| Implementation Lead | Coordinates implementation execution and documentation. |
| Department Managers | Validate manager workflows, approvals, and team data. |
| Key Users | Execute UAT and support training. |

## Governance

- Weekly project status meeting.
- Twice-weekly workstream checkpoints during configuration and testing.
- Daily hypercare triage after go-live.
- Decision log for policy, data, access, and cutover decisions.
- Change request review for any scope not covered by this charter.

## Milestones

| Milestone | Approval owner |
| --- | --- |
| Project charter signed | Executive Sponsor |
| Scope and workflow sign-off | HR Sponsor |
| Data migration sign-off | HR Data Owner |
| Security and role sign-off | IT Lead |
| Payroll parallel run sign-off | Payroll Lead |
| UAT sign-off | Business Leads |
| Go-live approval | Steering Committee |
| Hypercare closure | Executive Sponsor |

## Acceptance Criteria

- Required documents in this package are reviewed and approved.
- Configuration matches signed business scope.
- Migrated data reconciles against approved source files.
- UAT pass criteria are met.
- Role guides are distributed to target audiences.
- Support ownership is agreed before go-live.
