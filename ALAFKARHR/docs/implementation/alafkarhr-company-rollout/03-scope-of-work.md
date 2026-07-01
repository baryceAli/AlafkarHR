# Scope of Work

## Implementation Scope

Alafkar will be implemented for the company as a full ERP implementation platform covering HR and the confirmed operational, commercial, finance, inventory, and business-line modules found in the repository. The scope is based on confirmed repository modules, Blazor pages, service contracts, permissions, menu metadata, and business-line keys.

## Confirmed Functional Scope

| Area | Included functionality |
| --- | --- |
| Security | Login, roles, permissions, user list, user forms, role assignment, security dashboard. |
| Organization and business lines | Company list, parent/child company management, branch list/form, administrations, departments, business lines, license categories, business-line activation. |
| Employees | Employee dashboard, employee list/form/view, 360 view, public view, QR view, team setup, positions, academic institutions, specializations. |
| HR enhancements | Command center, lifecycle events, employee documents, emergency contacts, skills, certifications, HR reports. |
| Attendance | Dashboard, sessions, check-in/check-out, breaks, location ping, shifts, assignments, holidays, late check-in requests, mid-day permissions, approvals, reports. |
| Roster | Roster controls, substitute configurations, schedules, assignments, publish/lock/cancel, shift swaps, corrections. |
| Biometric and work entries | Biometric import batches and rows, payroll work-entry generation, edit, approve, lock. |
| Leave | Emergency leave, attachments, review, balances, leave types, periods, policies, assignments, allocations, applications, ledger, adjustments, encashment, reports. |
| Payroll | Components, contracts, employee contracts, salary runs, loan management, structures, periods, entries, payslips, payroll inputs, Saudi payroll, WPS, EOS provision, accounting posting, work-entry import. |
| Accounting and finance | Accounting setup, templates, chart of accounts, fiscal periods, tax codes, posting profiles, bank/cash accounts, journals, accounting documents, invoices, receipts/payments, credit/debit notes, bank reconciliation, reports, ZATCA submissions and settings. |
| Sales and order management | Sales dashboard, quotations, order intake, sales orders, delivery notes, returns, sales reporting and downstream integration validation. |
| Customers and pricing | Customer dashboard, customer list/form, customer groups, customer pricing profiles, pricing lists and commercial master data. |
| Purchasing and procurement | Procurement dashboard, procurement documents by kind, forms, details, tracker, vendor price lists, supplier items, supplier scorecard, reordering rules, procurement enhancements. |
| Suppliers | Supplier list/form/view and supplier groups. |
| Warehouse, inventory, catalog and pricing | Product dashboard, products, SKUs, categories, brands, units, variants, packages, warehouses, inventories, batches, asset instances, movements, transfers, controls, stock-in, stock-out, adjustment, reservation and release. |
| POS and StoreFront | POS workspace, stores, departments, organization setup and store management pages for the `store-front` business line. |
| Catering business line | Catering dashboard, contracts, meals, locations, schedules, deliveries, assignments and reports for the `catering` business line. |
| Real Estate business line | Real estate dashboard, properties, units, owner leases, tenant leases, collections, utilities, expenses and reports for the `real-estate` business line. |
| Contracts | Contract dashboard, list, form, details, renewals, templates and party-specific contract views. |
| Document Management and Media Center | Document list/create/view, my documents, shared documents, source documents, upload policy, media activities, activity details and activity types. |
| Projects and tasks | Project dashboard, project list/details, distribution places and schedules, project reports, task dashboard, my tasks, notifications, list, kanban, reports, create/edit/view. |
| Fleet | Fleet dashboard, vehicles, assignments, expenses, documents, service rules and reports. |
| Maintenance | Maintenance dashboard, assets, work orders, my requests and reports. |
| Payments | Payment module support for payment-related setup and transaction flows that must be validated with accounting and operations owners. |
| Recruitment | Staffing plans, requisitions, applicants, interview feedback, offers, offer acceptance/rejection, employee creation marking. |
| Performance | Cycles, goals, competencies, employee goals, competency scores, evaluations, submit/review/approve/cancel. |
| Training | Programs, events, attendees, attendance marking, results, certificate links. |
| Settings | System settings, home page templates, currencies. |

## Business Lines and Workspaces

### Confirmed Business-Line Keys

| Business-line key | Implementation meaning | Confirmed from codebase |
| --- | --- | --- |
| `catering` | Catering contracts, meals, locations, schedules, deliveries, assignments and reports. | Business-line key and Catering module/pages. |
| `real-estate` | Real estate properties, units, owner leases, tenant leases, collections, utilities, expenses and reports. | Business-line key and Real Estate module/pages. |
| `store-front` | Storefront stores, departments, organization setup, POS operation and store management. | Business-line key and StoreFront/POS module/pages. |

### Operational Workspaces

| Workspace | Primary users | Implementation focus |
| --- | --- | --- |
| HR | HR administrators, attendance officers, payroll officers, managers, employees. | Employee lifecycle, attendance, leave, payroll, recruitment, performance and training. |
| Sales | Sales managers, sales officers, customer service users. | Quotations, orders, delivery notes, returns, customer setup and pricing validation. |
| Purchasing | Procurement managers, buyers, supplier coordinators. | Procurement documents, supplier catalog, vendor prices, scorecards and reordering. |
| Warehouse | Warehouse managers, stock controllers, catalog users. | Product masters, SKUs, warehouses, inventories, batches, movements, transfers and controls. |
| Accounting/Finance | Finance manager, accountants, cash/bank users, tax users. | Chart of accounts, fiscal periods, tax, posting profiles, journals, invoices, payments, reconciliation and reports. |
| StoreFront/POS | Store managers, cashiers, POS supervisors. | Store setup, departments, POS readiness, cashier operation and store reporting. |
| Catering | Catering operations, dispatch, contract coordinators. | Contracts, meals, schedules, locations, deliveries and assignment tracking. |
| Real Estate | Property managers, leasing users, collection users. | Property/unit masters, leases, rent collection, utilities, expenses and reports. |
| Admin | System owner, implementation lead, master data owners. | Organization, settings, currencies, home page templates and cross-module readiness. |
| IT/Security | System administrators, security administrators, auditors. | Users, roles, permission assignment, access reviews and production support. |

## Technical Scope

- Use existing UI pages and API services.
- Configure normal permission claims through company roles.
- Respect branch access where pages or backend flows use company and branch context.
- Use existing bilingual Arabic/English UI behavior.
- Use existing data structures and DTO fields.

## Exclusions

| Exclusion | Handling |
| --- | --- |
| Custom code development | Requires separate approved development request. |
| Manual EF migration edits | Not applicable; no application code changes are planned. |
| Unconfirmed external biometric formats | Validate device export format during discovery. |
| Payroll statutory interpretation | Payroll rules must be approved by company payroll/legal owners. |
| Tax, ZATCA and accounting policy interpretation | Finance, tax and legal owners must approve statutory treatment, posting rules and invoice/reporting policies. |
| External integrations not visible in the repository | Integration specifications, credentials, data contracts and operational ownership require separate validation. |
| Historical archive digitization | Can be planned as separate workstream. |

## Dependencies

- Approved organization hierarchy.
- Approved business-line activation model for `catering`, `real-estate`, and `store-front`.
- Clean employee source data.
- Approved payroll and leave rules.
- Approved shift, attendance, and holiday policies.
- Approved roles and permission assignment.
- Approved accounting templates, chart of accounts, fiscal calendar, tax/ZATCA rules, posting profiles and bank/cash setup.
- Approved customer, supplier, product, SKU, warehouse, pricing and inventory opening-balance data.
- Approved sales, procurement, POS, catering, real estate, contract, document, project, fleet and maintenance operating procedures.
- Availability of key users for UAT and training.

## Validation Notes

- Recruitment, performance, and training capabilities are present in service contracts and HR routes; final process design must be validated with HR.
- Payroll has extensive confirmed functionality, but calculation rules and statutory interpretations remain business-owned.
- Branch visibility and mutation rules must be validated in UAT for each role.
- Accounting, inventory, sales, procurement, POS, catering, real estate, contracts, projects, fleet and maintenance pages are confirmed from the codebase, while detailed policy, approval, statutory, posting and integration behavior requires business validation.
- Module readiness must be signed off by both the functional owner and the system owner before go-live.
