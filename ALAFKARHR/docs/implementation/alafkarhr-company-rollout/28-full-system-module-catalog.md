# Full System Module Catalog

## Purpose

This catalog gives the implementation team one confirmed view of the Alafkar ERP modules that must be included in the company rollout. Items marked confirmed from codebase are based on repository modules, routes, menu metadata, and permission groups. Items marked requires business validation must be approved by process owners before go-live.

## Confirmed ERP Module Catalog

| Module or workspace | Confirmed capabilities | Representative routes | Implementation notes |
| --- | --- | --- | --- |
| Security | Roles, users, permissions, role assignment, dashboard. | `/Auth/Dashboard`, `/Auth/Role/List`, `/Auth/User/List`, `/Auth/User/AssignRole` | Confirm permission assignments with data owner and audit owner. |
| Organization | Companies, branches, administrations, departments, business lines, license categories. | `/Organization/Dashboard`, `/Organization/Company/List`, `/Organization/Branch/List`, `/Organization/BusinessLines` | Business-line activation must be validated before module testing. |
| HR and employees | Employee master data, employee view, 360 view, teams, positions, HR command center, lifecycle, documents, skills and reports. | `/Employee/Dashboard`, `/Employee/Employee/List`, `/HR/CommandCenter`, `/HR/EmployeeLifecycle` | Requires employee data cleansing and branch visibility validation. |
| Attendance and roster | Attendance sessions, shifts, assignments, holidays, permissions, late requests, approvals, reports and roster controls. | `/Attendance/Dashboard`, `/Attendance/Sessions`, `/Attendance/Shifts`, `/Attendance/Reports` | Device import formats and schedule policies require business validation. |
| Leave | Leave applications, balances, emergency leave, policies, ledger, adjustments, encashment and reports. | `/LeavesManagement/MyLeaveApplications`, `/LeavesManagement/Balances`, `/HR/LeavePolicies`, `/HR/LeaveLedger` | Leave rules must be signed off by HR and payroll owners. |
| Payroll | Components, contracts, salary runs, loans, structures, work entries, payslips, Saudi payroll, WPS and accounting posting. | `/Payroll/Components`, `/Payroll/SalaryRuns`, `/HR/Payslips`, `/HR/SaudiPayroll` | Calculation rules, WPS and statutory treatment require formal approval. |
| Accounting and finance | Setup, templates, accounts, fiscal periods, tax codes, posting profiles, bank/cash accounts, journals, invoices, payments, reconciliation, reports and ZATCA. | `/Accounting/Dashboard`, `/Accounting/Setup`, `/Accounting/Accounts`, `/Accounting/Zatca/Submissions` | Chart of accounts, tax, posting and ZATCA settings require finance validation. |
| Sales and order management | Quotations, order intake, sales orders, delivery notes and returns. | `/Sales/Dashboard`, `/Sales/Quotations`, `/Orders/Intakes`, `/Sales/Orders` | Confirm quotation-to-order, delivery and return approval policies. |
| Customers and pricing | Customer dashboard, customer records, groups and pricing profiles. | `/Customers/Customer/Dashboard`, `/Customers/Customer/List`, `/Customers/CustomerPricingProfile/List` | Customer credit, pricing, discount and tax rules require validation. |
| Procurement and purchasing | Procurement dashboard, document forms/details, tracker, vendor price lists, supplier items, scorecard and reordering rules. | `/Procurement/Dashboard`, `/Procurement/Tracker`, `/Procurement/VendorPricelists` | Approvals, purchase document lifecycle and reorder thresholds require validation. |
| Suppliers | Supplier records, supplier forms, supplier groups and supplier view. | `/Suppliers/Supplier/List`, `/Suppliers/Supplier/Form`, `/Suppliers/SupplierGroup/List` | Supplier onboarding documents and approval rules require validation. |
| Catalog, pricing, warehouse and inventory | Product masters, SKUs, categories, brands, units, variants, packages, pricing, warehouses, inventories, batches, assets, movements, transfers, controls and stock operations. | `/Catalog/Product/List`, `/Inventory/Dashboard`, `/Inventory/Transfers`, `/Inventory/Operations/StockAdjustment` | Opening stock, costing, inventory controls and count policy require validation. |
| StoreFront and POS | POS page, stores, departments, organization setup and store management. | `/StoreFront/POS`, `/StoreFront/Stores`, `/StoreFront/Departments`, `/StoreFront/Stores/{StoreId}/Manage` | Cashier roles, sessions, payment methods and reconciliation require validation. |
| Catering | Dashboard, contracts, meals, locations, schedules, deliveries, assignments and reports. | `/Catering/Dashboard`, `/Catering/Contracts`, `/Catering/Schedules`, `/Catering/Deliveries` | Contract rules, meal plans and delivery proof require business validation. |
| Real Estate | Properties, units, owner leases, tenant leases, collections, utilities, expenses and reports. | `/RealEstate/Dashboard`, `/RealEstate/Properties`, `/RealEstate/TenantLeases`, `/RealEstate/Collections` | Rent schedules, utilities, owner settlement and legal templates require validation. |
| Contracts | Contract list, forms, details, renewals, templates and party-specific views. | `/Contracts/Dashboard`, `/Contracts/List`, `/Contracts/Renewals`, `/Contracts/Templates` | Contract templates, approval chains and renewal reminders require validation. |
| Document Management | Document list, create, view, my documents, shared documents, source documents and upload policy. | `/DocumentManagement/List`, `/DocumentManagement/Create`, `/DocumentManagement/MyDocuments`, `/DocumentManagement/UploadPolicy` | Retention, confidentiality and naming rules require validation. |
| Media Center | Activities, activity details and activity types. | `/MediaCenter/Activities`, `/MediaCenter/Activities/{Id}`, `/MediaCenter/ActivityTypes` | Publishing workflow and media taxonomy require validation. |
| Projects and tasks | Project dashboard, project records, distribution places and schedules, reports, task dashboard, my tasks, kanban and task forms. | `/ProjectManagement/Dashboard`, `/ProjectManagement/Projects`, `/TaskManagement/Kanban`, `/TaskManagement/MyTasks` | Project coding, task ownership and reporting cadence require validation. |
| Fleet | Vehicles, assignments, expenses, documents, service rules and reports. | `/Fleet/Dashboard`, `/Fleet/Vehicles`, `/Fleet/Assignments`, `/Fleet/Reports` | Vehicle documents, allocation rules and service thresholds require validation. |
| Maintenance | Assets, work orders, my requests and reports. | `/Maintenance/Dashboard`, `/Maintenance/Assets`, `/Maintenance/WorkOrders` | Preventive maintenance plans and escalation rules require validation. |
| Payments | Payment-related setup and transaction support. | Confirmed module area. | Payment methods, approval, reconciliation and integration behavior require finance validation. |
| General settings | System settings, home page templates and currencies. | `/GeneralSettings/SystemSettings`, `/GeneralSettings/HomePageTemplates`, `/GeneralSettings/Currencies` | Baseline settings must be frozen before UAT. |

## Full-System Rollout Sequence

| Sequence | Workstream | Key outcome |
| --- | --- | --- |
| 1 | Foundation | Organization, branches, business lines, currencies, system settings and security roles are ready. |
| 2 | Finance foundation | Accounting templates, accounts, fiscal periods, tax, ZATCA, bank/cash and posting profiles are approved. |
| 3 | Commercial foundation | Customers, suppliers, product catalog, pricing, warehouses and opening inventory are validated. |
| 4 | HR foundation | Employees, attendance, leave, payroll and manager hierarchy are validated. |
| 5 | Transaction workstreams | Sales, procurement, inventory operations, POS, contracts, projects, tasks, fleet and maintenance are tested. |
| 6 | Business-line workstreams | Catering, Real Estate and StoreFront/POS scenarios are tested end to end. |
| 7 | Reporting and controls | Operational, finance, HR and executive reports are reconciled and signed off. |
| 8 | Cutover and go-live | Data freeze, final migration, user access, operational readiness and support model are complete. |

## Validation Rules

- Confirmed from codebase: module, page, route, menu item, permission group or service surface exists locally.
- Requires business validation: policy, approval, calculation, statutory interpretation, posting design, external integration, data ownership or operating procedure.
- No module should go live until its master data, roles, key workflows, reports and support ownership are signed off.
