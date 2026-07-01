# AlafkarERP Full-System Company Rollout Documentation Package

This package prepares a company implementation of the full Alafkar ERP platform based on the repository capabilities inspected on 2026-06-30.

Confirmed system areas include organization setup, security roles and users, HR and employees, attendance, roster control, leave, payroll, recruitment, performance, training, accounting/finance, sales, purchasing/procurement, customers, suppliers, catalog, pricing, warehouse/inventory, POS/StoreFront, catering, real estate, contracts, document management, media center, projects, tasks, fleet, maintenance, payments, reports, and general settings. When a workflow is not fully confirmable from the inspected code, the documents mark it as requiring business validation.

## Document Index

| File | Purpose |
| --- | --- |
| `01-implementation-plan.md` | End-to-end rollout approach, phases, timeline, risks, governance, and success criteria. |
| `02-project-charter.md` | Project authority, scope, goals, stakeholders, assumptions, and approval model. |
| `03-scope-of-work.md` | Confirmed functional scope, excluded items, dependencies, and validation notes. |
| `04-stakeholder-register.md` | Stakeholder groups, responsibilities, and engagement needs. |
| `05-raci-matrix.md` | RACI ownership across implementation activities. |
| `06-data-migration-plan.md` | HR data cleansing, migration, validation, and cutover approach. |
| `07-master-data-collection-template.md` | Templates for companies, branches, departments, positions, shifts, leave, payroll, and HR masters. |
| `08-employee-data-collection-template.md` | Employee profile, employment, attendance, leave, payroll, and document data templates. |
| `09-configuration-workbook.md` | Configuration checklist for organization, security, HR, attendance, leave, payroll, and settings. |
| `10-roles-permissions-matrix.md` | Recommended role model mapped to confirmed permission groups. |
| `11-workflow-mapping.md` | Business process maps for daily HR operations and approvals. |
| `12-test-plan.md` | SIT, UAT, regression, data, security, and go-live test coverage. |
| `13-uat-test-scripts.md` | Role-based UAT scenarios and acceptance criteria. |
| `14-training-plan.md` | Training audiences, agenda, materials, and attendance tracking. |
| `15-communication-change-plan.md` | Communication, change management, readiness, and adoption plan. |
| `16-go-live-checklist.md` | Go-live readiness checklist. |
| `17-cutover-plan.md` | Cutover sequence, freeze windows, validation, and rollback decision points. |
| `18-support-hypercare-plan.md` | Hypercare support process, triage, SLA, and handover. |
| `19-risk-issue-register.md` | Starting risk and issue register. |
| `20-sign-off-templates.md` | Sign-off templates for phase, data, UAT, training, and go-live approvals. |
| `21-post-go-live-review.md` | Post-go-live review template and improvement backlog. |
| `22-user-guide-system-administrator.md` | User guide for platform, company, security, role, and settings administrators. |
| `23-user-guide-hr-administrator.md` | User guide for HR administration, employee records, and HR master data. |
| `24-user-guide-employee.md` | User guide for employees using attendance, leave, and self-service pages. |
| `25-user-guide-manager.md` | User guide for managers and supervisors handling approvals and team oversight. |
| `26-user-guide-payroll-officer.md` | User guide for payroll setup, runs, payslips, loans, WPS, and Saudi payroll data. |
| `27-user-guide-specialist-roles.md` | User guide for attendance officers, recruiters, performance, training, finance reviewers, and executives. |
| `28-full-system-module-catalog.md` | Full ERP module catalog with confirmed workspaces, capabilities, routes, and validation notes. |
| `29-business-line-implementation-guide.md` | Implementation guide for Catering, Real Estate, and StoreFront/POS business lines. |
| `30-cross-module-configuration-guide.md` | Cross-module master data and configuration workbook for full ERP rollout. |
| `31-full-system-roles-and-permissions.md` | Full-system role and permission guide by workspace and module. |
| `32-user-guide-erp-functional-roles.md` | User guide for finance, sales, procurement, warehouse, POS, business-line, project, fleet, maintenance, document, media, and executive users. |

## Confirmed Application Entry Points

| Area | Confirmed pages/routes |
| --- | --- |
| Security | `/Auth/Dashboard`, `/Auth/Role/List`, `/Auth/Role/Form`, `/Auth/User/List`, `/Auth/User/Form`, `/Auth/User/AssignRole` |
| Organization | `/Organization/Dashboard`, `/Organization/Company/List`, `/Organization/Branch/List`, `/Organization/Administration/List`, `/Organization/Department/List`, `/Organization/BusinessLines`, `/Organization/LicenseCategories` |
| Employees | `/Employee/Dashboard`, `/Employee/Employee/List`, `/Employee/Employee/Form/{Id?}`, `/Employee/Employee/view/{Id}`, `/Employee/Employee/360/{Id}`, `/Employee/Teams`, `/Employee/Position/List`, `/Employee/AcademicInistitution/List`, `/Employee/Specialization/List` |
| HR workspaces | `/HR/CommandCenter`, `/HR/EmployeeLifecycle`, `/HR/EmployeeDocuments`, `/HR/EmployeeSkills`, `/HR/EmployeeEmergencyContacts`, `/HR/Recruitment`, `/HR/Performance`, `/HR/Training`, `/HR/Reports` |
| Attendance | `/Attendance/Dashboard`, `/Attendance/MyAttendance`, `/Attendance/Sessions`, `/Attendance/Shifts`, `/Attendance/ShiftAssignments`, `/Attendance/Holidays`, `/Attendance/PermissionRequests`, `/Attendance/ApprovePermissionRequests`, `/Attendance/LateRequests`, `/Attendance/Reports` |
| Leave | `/LeavesManagement/MyLeaveApplications`, `/LeavesManagement/EmergencyLeaves`, `/LeavesManagement/ApproveEmergencyLeaves`, `/LeavesManagement/Balances`, `/LeavesManagement/Reports`, `/HR/LeavePolicies`, `/HR/LeaveApplications`, `/HR/LeaveLedger` |
| Payroll | `/Payroll/Components`, `/Payroll/Contracts`, `/Payroll/AssignContract`, `/Payroll/SalaryRuns`, `/Payroll/Loans`, `/HR/PayrollStructures`, `/HR/WorkEntries`, `/HR/Payslips`, `/HR/SaudiPayroll` |
| Accounting/Finance | `/Accounting/Dashboard`, `/Accounting/Setup`, `/Accounting/Templates`, `/Accounting/Accounts`, `/Accounting/FiscalPeriods`, `/Accounting/TaxCodes`, `/Accounting/PostingProfiles`, `/Accounting/BankCashAccounts`, `/Accounting/Journals`, `/Accounting/Documents`, `/Accounting/SalesInvoices`, `/Accounting/PurchaseInvoices`, `/Accounting/ReceiptsPayments`, `/Accounting/CreditNotes`, `/Accounting/DebitNotes`, `/Accounting/BankReconciliation`, `/Accounting/Reports`, `/Accounting/Zatca/Submissions`, `/Accounting/Zatca/Settings` |
| Sales and Orders | `/Sales/Dashboard`, `/Sales/Quotations`, `/Orders/Intakes`, `/Sales/Orders`, `/Sales/DeliveryNotes`, `/Sales/Returns` |
| Customers | `/Customers/Customer/Dashboard`, `/Customers/Customer/List`, `/Customers/CustomerGroup/List`, `/Customers/CustomerPricingProfile/List` |
| Procurement/Purchasing | `/Procurement/Dashboard`, `/Procurement/{KindRoute}`, `/Procurement/{KindRoute}/Form`, `/Procurement/{KindRoute}/Detail/{Id}`, `/Procurement/Tracker`, `/Procurement/VendorPricelists`, `/Procurement/SupplierItems`, `/Procurement/SupplierScorecard`, `/Procurement/ReorderingRules`, `/Procurement/Enhancements` |
| Suppliers | `/Suppliers/Supplier/List`, `/Suppliers/Supplier/Form`, `/Suppliers/Supplier/View/{Id}`, `/Suppliers/SupplierGroup/List` |
| Warehouse, Catalog, Inventory and Pricing | `/Warehouse/Product/Dashboard`, `/Catalog/Product/List`, `/Catalog/Product/{ProductId}/ProductSku`, `/Catalog/Product/ProductSku/List`, `/Warehouse/Product/Category/List`, `/Warehouse/Product/Brand/List`, `/Warehouse/Product/Unit/List`, `/Catalog/Variant/List`, `/Warehouse/Product/Packages/List`, `/Catalog/Pricing/List`, `/Inventory/Dashboard`, `/Inventory/Warehouse/List`, `/Inventories/List`, `/Inventory/Batch/List`, `/Inventory/AssetInstances`, `/Inventory/Movements`, `/Inventory/Transfers`, `/Inventory/Controls`, `/Inventory/Operations/StockIn`, `/Inventory/Operations/StockOut`, `/Inventory/Operations/StockAdjustment`, `/Inventory/Operations/StockReservation`, `/Inventory/Operations/StockRelease` |
| StoreFront/POS | `/StoreFront/POS`, `/StoreFront/Stores`, `/StoreFront/Departments`, `/StoreFront/Organization`, `/StoreFront/Stores/{StoreId}/Manage` |
| Catering | `/Catering/Dashboard`, `/Catering/Contracts`, `/Catering/Meals`, `/Catering/Locations`, `/Catering/Schedules`, `/Catering/Deliveries`, `/Catering/Assignments`, `/Catering/Reports` |
| Real Estate | `/RealEstate/Dashboard`, `/RealEstate/Properties`, `/RealEstate/Units`, `/RealEstate/OwnerLeases`, `/RealEstate/TenantLeases`, `/RealEstate/Collections`, `/RealEstate/Utilities`, `/RealEstate/Expenses`, `/RealEstate/Reports` |
| Contracts | `/Contracts/Dashboard`, `/Contracts/List`, `/Contracts/Form`, `/Contracts/Detail/{Id}`, `/Contracts/Renewals`, `/Contracts/Templates`, `/Contracts/Party/{PartyType}/{PartyId}` |
| Document Management and Media Center | `/DocumentManagement/List`, `/DocumentManagement/Create`, `/DocumentManagement/View/{Id}`, `/DocumentManagement/MyDocuments`, `/DocumentManagement/SharedWithMe`, `/DocumentManagement/SourceDocuments`, `/DocumentManagement/UploadPolicy`, `/MediaCenter/Activities`, `/MediaCenter/Activities/{Id}`, `/MediaCenter/ActivityTypes` |
| Projects and Tasks | `/ProjectManagement/Dashboard`, `/ProjectManagement/Projects`, `/ProjectManagement/Projects/{Id}`, `/ProjectManagement/DistributionPlaces`, `/ProjectManagement/DistributionSchedule`, `/ProjectManagement/Reports/DailyDistribution`, `/ProjectManagement/Reports/PlaceDistribution`, `/ProjectManagement/Reports/CustomerDistribution`, `/ProjectManagement/Reports/PlannedProductDemand`, `/ProjectManagement/Reports/Costs`, `/TaskManagement/Dashboard`, `/TaskManagement/MyTasks`, `/TaskManagement/Notifications`, `/TaskManagement/List`, `/TaskManagement/Kanban`, `/TaskManagement/Reports`, `/TaskManagement/Create`, `/TaskManagement/Edit/{Id}`, `/TaskManagement/View/{Id}` |
| Fleet and Maintenance | `/Fleet/Dashboard`, `/Fleet/Vehicles`, `/Fleet/Assignments`, `/Fleet/Expenses`, `/Fleet/Documents`, `/Fleet/ServiceRules`, `/Fleet/Reports`, `/Maintenance/Dashboard`, `/Maintenance/Assets`, `/Maintenance/WorkOrders`, `/Maintenance/WorkOrders/MyRequests`, `/Maintenance/Reports` |
| General Settings | `/GeneralSettings/SystemSettings`, `/GeneralSettings/HomePageTemplates`, `/GeneralSettings/Currencies` |

## Validation Notes

- This documentation does not modify application behavior.
- Permission names come from `PermissionList.cs`.
- Page routes come from Blazor `@page` directives and menu metadata.
- Recruitment, performance, and training service contracts are present; detailed customer-specific process rules still require business validation before go-live.
