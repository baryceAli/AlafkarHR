# User Guide: ERP Functional Roles

## Purpose

This user guide helps functional users operate the full Alafkar ERP implementation. Each role should use this guide together with the detailed HR, payroll, manager and employee guides already included in the documentation package.

## Finance and Accounting Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Review finance dashboard | `/Accounting/Dashboard` | Open the dashboard, review period status, exceptions and report links. | Confirm numbers against approved finance reports. |
| Maintain finance setup | `/Accounting/Setup`, `/Accounting/Templates` | Review setup records and templates before transaction testing. | Requires finance owner approval. |
| Maintain chart of accounts | `/Accounting/Accounts` | Create or update accounts based on approved chart. | Do not change production account structure without sign-off. |
| Maintain fiscal and tax setup | `/Accounting/FiscalPeriods`, `/Accounting/TaxCodes`, `/Accounting/Zatca/Settings` | Validate periods, tax codes and ZATCA settings. | Statutory treatment requires business/legal validation. |
| Process journals and documents | `/Accounting/Journals`, `/Accounting/Documents` | Create, review and post accounting documents based on policy. | Segregation of duties may apply. |
| Review invoices and payments | `/Accounting/SalesInvoices`, `/Accounting/PurchaseInvoices`, `/Accounting/ReceiptsPayments` | Review invoice/payment records and reconcile with source transactions. | Confirm posting rules during UAT. |
| Reconcile bank/cash | `/Accounting/BankReconciliation`, `/Accounting/BankCashAccounts` | Review bank/cash accounts and reconciliation status. | Daily/period close process requires finance approval. |

## Sales and Customer Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Manage customers | `/Customers/Customer/List` | Search, create, review or update customer records. | Validate customer group and pricing profile. |
| Review customer dashboard | `/Customers/Customer/Dashboard` | Monitor customer activity and follow-up items. | Use approved reporting definitions. |
| Create quotations | `/Sales/Quotations` | Create quotation, select customer/items/prices and submit according to policy. | Discounts and validity require validation. |
| Process sales orders | `/Sales/Orders`, `/Orders/Intakes` | Convert or create order, confirm details and proceed with delivery flow. | Check stock and customer terms. |
| Manage delivery and returns | `/Sales/DeliveryNotes`, `/Sales/Returns` | Record delivery or return activity and review exceptions. | Return policy must be approved. |

## Procurement and Supplier Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Manage suppliers | `/Suppliers/Supplier/List`, `/Suppliers/Supplier/Form` | Create or update supplier profile and group. | Supplier onboarding documents require validation. |
| Review procurement dashboard | `/Procurement/Dashboard` | Check pending documents, tracker status and alerts. | Confirm owner for each document type. |
| Process procurement documents | `/Procurement/{KindRoute}` | Open the relevant procurement document list, create or update records, submit for approval. | Document kinds and approval rules require validation. |
| Maintain vendor prices | `/Procurement/VendorPricelists` | Maintain approved vendor price lists. | Validate effective dates and currency. |
| Review supplier scorecard | `/Procurement/SupplierScorecard` | Review supplier performance indicators. | Scoring model requires procurement approval. |
| Maintain reordering rules | `/Procurement/ReorderingRules` | Configure reorder criteria after inventory validation. | Reorder thresholds must be approved by operations. |

## Warehouse, Inventory, Catalog and Pricing Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Maintain product catalog | `/Catalog/Product/List` | Create and update products, SKUs and product attributes. | Product codes and naming rules require governance. |
| Maintain classifications | `/Warehouse/Product/Category/List`, `/Warehouse/Product/Brand/List`, `/Warehouse/Product/Unit/List`, `/Catalog/Variant/List` | Maintain categories, brands, units and variants. | Keep data clean before importing stock. |
| Maintain pricing | `/Catalog/Pricing/List` | Review or update price lists according to approved policy. | Price changes require commercial approval. |
| Manage warehouses | `/Inventory/Warehouse/List` | Maintain warehouse records and branch alignment. | Validate access by branch/store. |
| Process stock operations | `/Inventory/Operations/StockIn`, `/Inventory/Operations/StockOut`, `/Inventory/Operations/StockAdjustment` | Record stock activity and validate quantities. | Adjustment reasons require approval. |
| Process transfers | `/Inventory/Transfers` | Create and track transfers between locations. | Confirm in-transit responsibility. |
| Reserve or release stock | `/Inventory/Operations/StockReservation`, `/Inventory/Operations/StockRelease` | Reserve or release stock for business process needs. | Confirm reservation policy with sales and operations. |

## POS and StoreFront Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Open POS | `/StoreFront/POS` | Sign in, confirm authorized store and start POS activity. | Cashier access should be limited and reviewed. |
| Maintain stores | `/StoreFront/Stores` | Create or review store records. | Link stores to branch and reporting ownership. |
| Maintain departments | `/StoreFront/Departments` | Create or review store departments. | Department structure affects reporting. |
| Manage store | `/StoreFront/Stores/{StoreId}/Manage` | Review store-specific settings and readiness. | Store manager should sign off before go-live. |
| Reconcile daily activity | POS and accounting reports | Compare expected sales, payment totals and exceptions. | Daily reconciliation policy requires finance approval. |

## Catering Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Review dashboard | `/Catering/Dashboard` | Review operational status and exceptions. | Confirm report definitions. |
| Manage contracts | `/Catering/Contracts` | Create or review catering contract records. | Billing terms require finance validation. |
| Maintain meals and locations | `/Catering/Meals`, `/Catering/Locations` | Maintain service catalog and service locations. | Meal and location naming rules should be approved. |
| Manage schedules and deliveries | `/Catering/Schedules`, `/Catering/Deliveries` | Create schedules, track deliveries and handle exceptions. | Delivery proof and exception rules require validation. |
| Review assignments and reports | `/Catering/Assignments`, `/Catering/Reports` | Confirm users/teams and review operational reporting. | Assignments should match real operating teams. |

## Real Estate Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Maintain properties and units | `/RealEstate/Properties`, `/RealEstate/Units` | Create or update property hierarchy and unit details. | Legal identifiers and availability status require validation. |
| Manage leases | `/RealEstate/OwnerLeases`, `/RealEstate/TenantLeases` | Create and review owner or tenant lease records. | Templates and terms require legal/finance approval. |
| Record collections | `/RealEstate/Collections` | Record or review collection activity. | Reconcile with accounting and bank/cash records. |
| Track utilities and expenses | `/RealEstate/Utilities`, `/RealEstate/Expenses` | Record utility and expense items. | Recovery and allocation policy requires validation. |
| Review reports | `/RealEstate/Reports` | Review occupancy, lease, collection and expense reports. | Report definitions require business owner approval. |

## Contracts, Documents and Media Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Manage contracts | `/Contracts/List`, `/Contracts/Form`, `/Contracts/Detail/{Id}` | Create, review and maintain contract records. | Approval and renewal policy require validation. |
| Manage renewals and templates | `/Contracts/Renewals`, `/Contracts/Templates` | Track renewals and maintain approved templates. | Legal owner must approve templates. |
| Create or view documents | `/DocumentManagement/Create`, `/DocumentManagement/View/{Id}` | Upload or review documents according to source process. | Confidentiality and retention rules apply. |
| Review my/shared documents | `/DocumentManagement/MyDocuments`, `/DocumentManagement/SharedWithMe` | Locate documents assigned or shared with the user. | Report incorrect access to security. |
| Maintain media activities | `/MediaCenter/Activities`, `/MediaCenter/ActivityTypes` | Create or maintain media activities and activity types. | Publishing workflow requires approval. |

## Projects, Tasks, Fleet and Maintenance Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Manage projects | `/ProjectManagement/Projects`, `/ProjectManagement/Projects/{Id}` | Create or update project records and review details. | Project code and owner should be approved. |
| Manage distribution schedules | `/ProjectManagement/DistributionPlaces`, `/ProjectManagement/DistributionSchedule` | Maintain places and schedules for distribution operations. | Validate operational cadence. |
| Manage tasks | `/TaskManagement/MyTasks`, `/TaskManagement/List`, `/TaskManagement/Kanban` | Review, update, assign and track tasks. | Users should keep task status current. |
| Manage fleet | `/Fleet/Vehicles`, `/Fleet/Assignments`, `/Fleet/Expenses`, `/Fleet/Documents` | Maintain vehicle data, assignments, costs and documents. | Document expiry and allocation rules require validation. |
| Manage maintenance | `/Maintenance/Assets`, `/Maintenance/WorkOrders`, `/Maintenance/WorkOrders/MyRequests` | Maintain assets, submit requests and track work orders. | Preventive maintenance rules require validation. |

## Executive and Management Users

| Task | Where to work | Steps | Notes |
| --- | --- | --- | --- |
| Review dashboards | Module dashboards | Review HR, finance, sales, procurement, inventory, project, fleet, maintenance and business-line dashboards. | Executive users should normally be read-only. |
| Review reports | Module report pages | Open approved reports and compare with expected management pack. | Report definitions require sign-off. |
| Monitor readiness | Implementation documents and UAT evidence | Review risks, issues, sign-offs and open validation items. | Go-live decision should use signed evidence, not informal readiness. |

## Support Guidance

- Use the module owner first for business-process questions.
- Use the security administrator for access issues.
- Use the system owner for configuration issues.
- Use finance, HR, legal or operations owner for policy interpretation.
- Log unresolved issues in the support register during hypercare.
