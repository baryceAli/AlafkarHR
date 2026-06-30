# Full System Roles and Permissions

## Purpose

This guide expands the HR-only role model into a full ERP role and permission guide. Permission groups must be confirmed against `PermissionList.cs`, menu policies and actual production responsibilities before go-live.

## Role Matrix

| Role | Workspace | Core responsibilities | Access principle |
| --- | --- | --- | --- |
| System Administrator | Admin, Security, General Settings | Maintain system settings, currencies, home page templates and administrative setup. | Broad platform administration with audit review. |
| Security Administrator | Security | Maintain users, roles, permissions and access reviews. | Full security administration, limited business transaction entry. |
| Organization Administrator | Organization | Maintain companies, branches, departments, administrations, license categories and business lines. | Organization master access only. |
| HR Administrator | HR, Employees | Maintain employee records, positions, teams, HR lifecycle, documents and HR reports. | HR master data and employee administration. |
| Attendance Officer | Attendance | Maintain sessions, shifts, assignments, holidays, permissions and attendance reports. | Attendance operations without payroll final approval. |
| Leave Administrator | Leave, HR | Maintain leave policies, balances, applications, adjustments, encashment and reports. | Leave administration with HR approval controls. |
| Payroll Officer | Payroll, HR | Maintain payroll components, contracts, salary runs, loans, work entries, payslips, WPS and Saudi payroll. | Payroll processing with finance review where required. |
| Finance/Accounting Officer | Accounting/Finance | Maintain accounts, fiscal periods, tax codes, posting profiles, journals, invoices, payments, reconciliation, ZATCA and reports. | Finance transaction and reporting access based on segregation of duties. |
| Sales Officer | Sales | Create and process quotations, orders, delivery notes and returns. | Sales transaction access for assigned branch or business unit. |
| Customer Officer | Customers | Maintain customer records, groups and pricing profiles. | Customer master data access with commercial approval. |
| Procurement Officer | Procurement/Purchasing | Maintain procurement documents, vendor prices, supplier items, tracker and reordering rules. | Purchasing access with approval limits validated by policy. |
| Supplier Officer | Suppliers | Maintain supplier records and supplier groups. | Supplier master access with onboarding review. |
| Warehouse Officer | Warehouse/Inventory | Maintain warehouses, inventory, batches, movements, transfers, controls and stock operations. | Inventory operation access with stock control approvals. |
| Catalog/Pricing Officer | Catalog/Pricing | Maintain product, SKU, categories, brands, units, variants, packages and price lists. | Product and price master access with change approval. |
| StoreFront Manager | StoreFront/POS | Manage stores, departments, store organization, cashier readiness and store review. | Store administration and review access. |
| Cashier | POS | Operate POS for authorized store and shift/session. | Narrow POS access only. |
| Catering Manager | Catering | Maintain catering contracts, meals, locations, schedules, deliveries, assignments and reports. | Catering business-line administration. |
| Real Estate Manager | Real Estate | Maintain properties, units, leases, collections, utilities, expenses and reports. | Real estate business-line administration. |
| Contract Manager | Contracts | Maintain contracts, templates, renewals and party-specific contract views. | Contract lifecycle administration with legal/finance review. |
| Document Controller | Document Management | Maintain documents, source documents, upload policy, shared documents and document access. | Document administration by source and confidentiality. |
| Media Coordinator | Media Center | Maintain media activities and activity types. | Media content administration. |
| Project Manager | Project Management | Maintain projects, distribution places, schedules and project reports. | Project administration and reporting access. |
| Task Manager | Task Management | Create, assign, review and report tasks. | Team task management access. |
| Task User | Task Management | View and update assigned tasks and notifications. | Own-task access. |
| Fleet Manager | Fleet | Maintain vehicles, assignments, expenses, documents, service rules and reports. | Fleet administration. |
| Maintenance Manager | Maintenance | Maintain assets, work orders, requests and reports. | Maintenance administration and escalation access. |
| Executive Viewer | Executive/Reports | Review dashboards and approved reports across workspaces. | Read-only management reporting access. |

## Permission Group Examples

| Permission area | Expected use |
| --- | --- |
| Auth/Security permissions | Users, roles and permission assignment. |
| Organization permissions | Company, branch, administration, department, business line and license category administration. |
| Employee and HR permissions | Employee records, teams, positions, lifecycle, HR documents, skills, recruitment, performance and training. |
| Attendance permissions | Attendance sessions, shifts, assignments, permissions, late requests and reports. |
| Leave permissions | Leave types, policies, balances, applications, ledger, adjustments, encashment and reports. |
| Payroll permissions | Components, contracts, salary runs, loans, work entries, payslips, Saudi payroll and WPS. |
| Accounting permissions | Accounts, fiscal periods, tax codes, posting profiles, journals, invoices, payments, reconciliation, reports and ZATCA. |
| Sales permissions | Quotations, orders, delivery notes and returns. |
| Procurement permissions | Procurement documents, tracker, vendor price lists, supplier items, scorecard and reordering rules. |
| Inventory and catalog permissions | Products, SKUs, categories, brands, units, variants, warehouses, inventory operations and pricing. |
| StoreFront/POS permissions | Store setup, POS operation, departments and store management. |
| Business-line permissions | Catering, Real Estate and StoreFront role-specific visibility and transactions. |
| Document/media permissions | Document management, upload policy, media activities and activity types. |
| Project/task permissions | Project management, reports, task dashboards, kanban, assignment and task forms. |
| Fleet/maintenance permissions | Vehicles, assignments, expenses, service rules, assets, work orders and maintenance reports. |

## Access Review Checklist

| Check | Owner | Evidence |
| --- | --- | --- |
| Every active user has one approved role profile. | Security administrator | User-role export or screenshot. |
| No cashier has unrelated finance, HR or admin privileges. | Security administrator | POS role review. |
| Finance duties are segregated where business policy requires it. | Finance manager | Approved finance access matrix. |
| Master data owners can maintain their own module but not unrelated modules. | Functional owners | Role test evidence. |
| Executive viewers are read-only unless explicitly approved. | Sponsor | Executive access sign-off. |
| Business-line users only see relevant business-line workspaces. | System owner | Business-line visibility test. |
| Leavers and transferred employees are reviewed before go-live. | HR and security | Updated user list. |

## Go-Live Rule

Production access should not be granted from informal requests. Every role must have an owner, approved responsibility description, permission group mapping, branch/business-line scope and UAT evidence.
