# AlafkarERP Full-System Implementation Plan

## Purpose

This plan guides a company rollout of the full Alafkar ERP platform from preparation through post-go-live support. It is written for company leadership, finance, sales, procurement, warehouse, HR, payroll, operations, business-line owners, IT, implementation consultants and key users.

The implementation scope is not HR-only. It covers HR plus the confirmed ERP modules, workspaces and business lines found in the repository.

## Confirmed Full-System Capability Map

| Domain | Confirmed capabilities |
| --- | --- |
| Organization and business lines | Company, parent/child company context, branches, administrations, departments, business lines, license categories, branch access and business-line activation. |
| Security | Users, roles, role assignment, permission groups, company-scoped roles, branch access permissions, scoped StoreFront branch roles and security dashboard. |
| Accounting and finance | Accounting setup, templates, chart of accounts, fiscal periods, tax codes, posting profiles, bank/cash accounts, journals, documents, sales invoices, purchase invoices, receipts/payments, credit notes, debit notes, bank reconciliation, finance reports, ZATCA submissions and ZATCA settings. |
| Sales and orders | Sales dashboard, quotations, order intake, sales orders, delivery notes, returns and sales workflow validation. |
| Customers and pricing | Customer dashboard, customer records, customer groups, customer pricing profiles, catalog pricing and commercial master data. |
| Procurement and purchasing | Procurement dashboard, procurement documents, forms, detail views, tracker, vendor pricelists, supplier items, supplier scorecard, reordering rules and procurement enhancements. |
| Suppliers | Supplier list, supplier form, supplier view and supplier groups. |
| Catalog, warehouse and inventory | Product dashboard, product records, SKUs, categories, brands, units, variants, packages, price lists, warehouses, inventory records, batches, asset instances, movements, transfers, controls, stock-in, stock-out, stock adjustment, stock reservation and stock release. |
| StoreFront and POS | POS workspace, stores, departments, StoreFront organization and store management for the `store-front` business line. |
| Catering | Catering dashboard, contracts, meals, locations, schedules, deliveries, assignments and reports for the `catering` business line. |
| Real Estate | Real estate dashboard, properties, units, owner leases, tenant leases, collections, utilities, expenses and reports for the `real-estate` business line. |
| Contracts | Contract dashboard, contract list, forms, detail views, renewals, templates and party-specific contract views. |
| Document Management and Media Center | Document list, create, view, my documents, shared documents, source documents, upload policy, media activities, media activity detail and activity types. |
| Projects and tasks | Project dashboard, projects, distribution places, distribution schedules, project reports, task dashboard, my tasks, notifications, task list, kanban, task reports, task create/edit/view. |
| Fleet | Fleet dashboard, vehicles, assignments, expenses, documents, service rules and reports. |
| Maintenance | Maintenance dashboard, assets, work orders, my maintenance requests and reports. |
| Payments | Payment-related setup and transaction support that must be validated with finance and operations owners. |
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

- Establish accurate organization, branch, business-line and security foundations.
- Configure accounting, tax, ZATCA, fiscal, bank/cash and posting setup before transaction testing.
- Prepare commercial master data: customers, suppliers, products, SKUs, pricing, warehouses and opening inventory.
- Enable daily ERP operations across sales, procurement, warehouse, POS, contracts, projects, tasks, fleet, maintenance and payments.
- Implement confirmed business lines: `catering`, `real-estate` and `store-front`.
- Migrate and validate employee, HR, attendance, leave and payroll data.
- Configure permission-safe access by company, branch, workspace and business-line role.
- Train each role with the role-based guides in this package.
- Complete controlled go-live with sign-offs, support ownership and post-go-live review.

## Implementation Assumptions

| Assumption | Validation owner |
| --- | --- |
| The company has an approved organization, branch and department structure before configuration starts. | Project Sponsor |
| Business-line activation for `catering`, `real-estate` and `store-front` is approved before UAT. | System Owner |
| Accounting templates, chart of accounts, fiscal periods, tax/ZATCA rules, posting profiles and bank/cash accounts are approved before transactional testing. | Finance Lead |
| Customer, supplier, product, SKU, pricing, warehouse and opening-stock source data can be provided in a clean spreadsheet format. | Commercial and Warehouse Data Owners |
| Sales, procurement, POS, contract, project, fleet and maintenance workflows are approved by functional owners before UAT. | Operations Lead |
| Employee source data can be exported to spreadsheet format. | HR Data Owner |
| Payroll rules, components, periods, WPS requirements, and Saudi payroll fields are approved before payroll setup. | Payroll Lead |
| Attendance devices, geolocation rules, shifts, and holiday calendars are agreed before attendance UAT. | Attendance Lead |
| User roles, branch access and business-line visibility rules are approved before UAT. | IT Security Lead |
| Arabic and English terminology is reviewed by business owners before training. | Functional Leads |

## Phase Plan

| Phase | Duration | Main activities | Key deliverables |
| --- | --- | --- | --- |
| 1. Mobilization | Week 1 | Kickoff, governance, stakeholder confirmation, project workspace, document sign-off process. | Project charter, RACI, timeline, issue log. |
| 2. Full-System Discovery and Fit | Weeks 1-2 | Confirm organization, security, finance, sales, procurement, inventory, POS, HR, payroll, business-line and operational workflows. | Scope of work, workflow mapping, module catalog, validation register. |
| 3. Data Preparation | Weeks 2-4 | Collect organization, finance, customer, supplier, catalog, inventory, HR, payroll, POS, catering, real estate, contract, project, fleet and maintenance data. | Master data templates, migration plan, cleansing tracker. |
| 4. Foundation Configuration | Weeks 3-5 | Configure organization, business lines, roles, users, settings, currencies, chart of accounts, fiscal periods, tax, ZATCA, branches and accounting templates. | Configured foundation, access matrix, finance setup sign-off. |
| 5. Operational Configuration | Weeks 4-7 | Configure commercial, procurement, warehouse, catalog, pricing, POS, contracts, documents, projects, tasks, fleet, maintenance, HR, attendance, leave and payroll masters. | Configuration workbook, module readiness evidence. |
| 6. Migration and Reconciliation | Weeks 5-8 | Load or enter data, reconcile counts, validate opening balances, validate employee records, prepare attachments and identifiers. | Migration reconciliation, exception log, data sign-off. |
| 7. Testing | Weeks 6-9 | SIT, UAT, role-based workflows, finance posting checks, inventory movement checks, POS scenarios, payroll parallel run if payroll is in scope and security testing. | Test plan results, UAT sign-off. |
| 8. Training and Change | Weeks 8-10 | Train administrators, finance, sales, procurement, warehouse, POS, business-line users, HR, payroll, managers, employees and specialist roles. | Training attendance, quick references, readiness report. |
| 9. Cutover and Go-Live | Week 11 | Freeze source changes, final migration, final validation, enable users, go-live communications. | Cutover checklist, go-live sign-off. |
| 10. Hypercare | Weeks 11-13 | Daily triage, issue resolution, adoption support, report reconciliation and handover to support. | Hypercare log, support handover, post-go-live review. |

## Workstreams

| Workstream | Scope |
| --- | --- |
| Governance | Kickoff, steering committee, status reporting, decisions, risks, issues, change requests. |
| Organization, Business Lines and Security | Company setup, branches, departments, administrations, business lines, roles, users, permission assignment, branch access and StoreFront branch roles. |
| Accounting and Finance | Accounts, fiscal periods, taxes, posting profiles, bank/cash accounts, journals, invoices, payments, reconciliation, ZATCA and finance reporting. |
| Sales and Customers | Customers, groups, pricing profiles, quotations, order intake, sales orders, delivery notes and returns. |
| Procurement and Suppliers | Suppliers, supplier groups, procurement documents, vendor prices, supplier items, scorecards and reordering rules. |
| Catalog, Pricing, Warehouse and Inventory | Products, SKUs, categories, brands, units, variants, packages, price lists, warehouses, batches, assets, movements, transfers and stock operations. |
| POS and StoreFront | StoreFront organization, stores, departments, POS operation, cashier readiness and daily reconciliation. |
| Catering | Contracts, meals, locations, schedules, deliveries, assignments and catering reports. |
| Real Estate | Properties, units, owner leases, tenant leases, collections, utilities, expenses and real estate reports. |
| Contracts, Documents and Media | Contract templates and renewals, document upload and visibility, source documents, media activities and activity types. |
| Projects, Tasks, Fleet and Maintenance | Project setup, distribution schedules, task assignment, kanban, fleet vehicles/assignments/expenses/documents/service rules, maintenance assets and work orders. |
| HR Core | Employees, positions, teams, academic institutions, specializations, lifecycle, documents, skills, certifications. |
| Attendance | Shifts, sessions, check-in rules, holidays, late requests, mid-day permissions, roster, corrections, biometric import. |
| Leave | Leave types, periods, policies, balances, applications, emergency leave, ledger, reports. |
| Payroll | Components, contracts, salary structures, periods, payroll entries, payslips, loans, WPS, Saudi payroll, accounting posting. |
| Talent | Recruitment, performance, and training workflows. |
| Data | Templates, cleansing, migration, reconciliation, attachment tracking. |
| Testing and Training | SIT, UAT, training, role guides, readiness. |
| Go-Live and Support | Cutover, communications, hypercare, support transition. |

## Business Lines and Workspace Rollout

| Business line or workspace | Rollout focus | Success condition |
| --- | --- | --- |
| `catering` | Catering contracts, meals, service locations, schedules, deliveries, assignments and reports. | Catering users can execute and report daily service operations. |
| `real-estate` | Properties, units, owner leases, tenant leases, collections, utilities, expenses and reports. | Property users can manage leases, collections and operating expenses. |
| `store-front` | StoreFront organization, stores, departments, POS, cashier access and store management. | Store users can operate POS and reconcile daily sales with finance. |
| Accounting/Finance | Finance setup, journals, invoices, payments, bank/cash, ZATCA and reports. | Finance owner signs off posting, tax, reconciliation and reporting scenarios. |
| Sales | Customer setup, quotations, orders, delivery notes and returns. | Sales owner signs off quote-to-order-to-delivery scenarios. |
| Purchasing | Supplier setup, procurement documents, vendor prices and reordering. | Procurement owner signs off purchase lifecycle and supplier controls. |
| Warehouse | Catalog, pricing, inventory, transfers, stock operations and controls. | Warehouse owner signs off stock accuracy, movement and adjustment scenarios. |
| HR | Employees, attendance, leave, payroll, recruitment, performance and training. | HR/payroll owners sign off employee lifecycle and payroll scenarios. |
| Admin/Security | Users, roles, permissions, branches, business-line visibility and settings. | Security owner signs off access matrix and branch/business-line visibility. |

## Success Criteria

- Full ERP scope is confirmed and signed off, including HR and all operational, finance and business-line modules.
- Organization, branch, department and business-line structures match approved company records.
- Accounting, tax, fiscal, posting, bank/cash and ZATCA setup is approved by finance.
- Customers, suppliers, products, SKUs, pricing, warehouses and opening inventory are reconciled.
- Sales, procurement, inventory, POS, catering, real estate, contracts, documents, projects, tasks, fleet and maintenance UAT scenarios are completed successfully.
- 100% of approved active employees are available in AlafkarERP.
- Role-based access is approved and tested for each workspace and business line.
- Critical UAT defects are closed or formally accepted before go-live.
- End users complete training or receive approved reference material.
- Hypercare triage process is active from the first business day after go-live.

## Business Validation Required

- Accounting policies, chart of accounts, posting profiles, fiscal periods, tax treatment, ZATCA setup, invoice rules, receipts/payments and bank reconciliation.
- Sales quotation, discount, customer pricing, delivery, return and credit policies.
- Procurement approval thresholds, supplier onboarding, vendor price rules, scorecards and reordering policy.
- Product coding, SKU, unit, warehouse, opening stock, costing, reservation, transfer and adjustment rules.
- POS cashier control, payment methods, refund policy, store reconciliation and branch-role scope.
- Catering contract terms, meal plans, delivery proof, schedule exceptions and billing triggers.
- Real estate lease templates, rent schedules, utility recovery, owner settlement, expense allocation and legal terms.
- Contract approval chains, renewal rules, templates and document retention.
- Project/task ownership, reporting cadence, fleet assignment, service thresholds, maintenance work-order escalation and asset policies.
- Final payroll calculations, allowances, deductions, EOS, WPS, and accounting posting policy.
- Attendance geolocation, biometric import format, late check-in, mid-day permission, correction, and roster approval rules.
- Leave accrual, carry-forward, encashment, emergency leave, attachment, and approval rules.
- Recruitment approval thresholds, offer approval, and employee creation policy.
- Performance rating scales, appraisal cycles, manager review, and approval authority.
- Training attendance, result recording, certification, and completion rules.
