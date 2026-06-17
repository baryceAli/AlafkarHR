# ERP User Role UAT Test Plan

## Purpose

This manual UAT plan validates the ERP from the user's point of view. It is organized by realistic user roles and journeys rather than by every individual permission. Each case includes prerequisites, required permissions, expected success behavior, and at least one failure or control expectation for the same feature family.

Source alignment:

- Permissions are based on `src/Shared/SharedWithUI/SharedWithUI/Permissions/PermissionList.cs`.
- User-visible routes are based on `UI/AlAfkarERP/AlAfkarERP.Shared/Layout/MuenuItem.cs` and Blazor `@page` routes.
- Workflow actions are based on feature pages and backend authorization patterns in module endpoints.

## Global Prerequisites

| Area | Requirement |
|---|---|
| Environment | Web UI and API are running against a test database. Testers know the environment URL and API base URL. |
| Administration | A system admin account can create roles, assign permissions, create users, and reset access if a test role becomes blocked. |
| Organization | At least one company, branch, administration, and two departments exist. One department should be unrelated to the test department for scope-negative tests. |
| People | Positions and employees exist for a regular employee, department head, HR admin, payroll user, sales user, sales head, procurement user, inventory/catalog user, and system admin. |
| Attendance and leave | Shifts, shift assignments, attendance configuration, holidays/weekends, leave balances, and at least one pending leave/permission request can be created. |
| Payroll | Salary contracts, payroll components, employee contract assignment, loan data, and at least one salary period are available. |
| Sales and customers | Customer groups, customers, product SKUs, pricing, stock, and a test sales order/POS data set are available. |
| Procurement and suppliers | Supplier groups, suppliers, products, warehouses, purchase request data, RFQ data, quotation data, purchase order data, goods receipt data, returns, and supplier invoices are available. |
| Inventory and catalog | Categories, brands, units, variants, products, SKUs, packages, price lists, warehouses, batches, and available stock exist. |
| Evidence | Tester can capture screenshots, downloaded reports, or record IDs for each executed case. |

## Test Users and Minimum Permissions

| Test role | Minimum permissions to assign |
|---|---|
| Regular employee | `Attendance.Attendance.Create`, `Attendance.Attendance.RequestEmergencyLeave`, `Attendance.Attendance.RequestMidDayPermission`, `Attendance.Attendance.ViewLeaveBalances`, `TaskManagement.Task.View`, `TaskManagement.Task.Comment` |
| Department head | Regular employee permissions plus `TaskManagement.Task.Create`, `TaskManagement.Task.Assign`, `TaskManagement.Task.Edit`, `TaskManagement.Task.Close`, `TaskManagement.Task.ViewReports`, `Attendance.Attendance.ApproveEmergencyLeave`, `Attendance.Attendance.ApproveMidDayPermission`, `Attendance.Attendance.ViewScopedReports` |
| HR/attendance admin | `Employees.Employee.*`, `Employees.Position.*`, `Attendance.Attendance.Select`, `Attendance.Attendance.View`, `Attendance.Attendance.Edit`, `Attendance.Attendance.ReviewRequests`, `Attendance.Attendance.ViewConfiguration`, `Attendance.Attendance.ManageConfiguration`, `Attendance.Attendance.ManageHolidays`, `Attendance.Attendance.ViewLeaveBalances`, `Attendance.Attendance.ManageLeaveBalances`, `Attendance.Attendance.ViewReports`, `Attendance.Attendance.ViewAllReports` |
| Payroll user/head | `Payroll.Contract.Select`, `Payroll.Contract.View`, `Payroll.Contract.Create`, `Payroll.Contract.Edit`, `Payroll.Loan.Select`, `Payroll.Loan.View`, `Payroll.Loan.Create`, `Payroll.Loan.Edit`, `Payroll.Loan.Approve`, `Payroll.Loan.Cancel`, `Payroll.SalaryRun.Select`, `Payroll.SalaryRun.View`, `Payroll.SalaryRun.Create`, `Payroll.SalaryRun.Edit`, `Payroll.SalaryRun.Approve`, `Payroll.SalaryRun.AdminOverride` |
| Sales personnel | `Customers.Customer.Select`, `Customers.Customer.View`, `Customers.Customer.Create`, `Customers.Customer.Edit`, `Catalog.Product.View`, `Catalog.Variant.View`, `Inventory.Inventory.View`, `SalesOrders.Order.Select`, `SalesOrders.Order.View`, `SalesOrders.Order.Create`, `Cart.Cart.*` where POS/cart actions are enabled |
| Sales head/approver | Sales personnel permissions plus `SalesOrders.Order.Edit`, `SalesOrders.Order.Confirm`, `SalesOrders.Order.Deliver`, `SalesOrders.Order.Invoice`, `SalesOrders.Order.Complete`, `SalesOrders.Order.Cancel`, `SalesOrders.Order.Return`, `SalesOrders.Order.ViewReports`, `Payments.Payment.Approve`, `Payments.Payment.Reject`, `Payments.Payment.Refund` |
| Procurement user/head | `Procurement.PurchaseRequest.*`, `Procurement.RequestForQuotation.*`, `Procurement.SupplierQuotation.*`, `Procurement.PurchaseOrder.*`, `Procurement.GoodsReceipt.*`, `Procurement.PurchaseReturn.*`, `Procurement.SupplierInvoice.*`, `Suppliers.Supplier.View`, `Catalog.Product.View`, `Inventory.Warehouse.View` |
| Inventory/catalog user | `Catalog.Category.*`, `Catalog.Brand.*`, `Catalog.Unit.*`, `Catalog.Variant.*`, `Catalog.Product.*`, `Catalog.ProductPackage.*`, `Pricing.PriceList.*`, `Inventory.Warehouse.*`, `Inventory.Batch.*`, `Inventory.Inventory.*`, `Inventory.InventoryItem.*` |
| System admin | `Authentication.Users.*`, `Authentication.Roles.*`, `Organization.Company.*`, `Organization.Branch.*`, `Organization.Administration.*`, `Organization.Department.*`, `GeneralSettings.SystemSettings.*` |

`*` means all actions listed for that permission group in `PermissionList.cs`.

## Role Journey Test Cases

| Case ID | Module | Role | Feature | Scenario | Required permissions | Route | Priority | Type | Expected result |
|---|---|---|---|---|---|---|---|---|---|
| UAT-001 | Auth | All roles | Login | Log in with valid assigned user and open the default dashboard/menu. | Valid user credentials | `/login` | P0 | Positive | Login succeeds; menu shows only items allowed by the user's permission claims. |
| UAT-002 | Auth | All roles | Login | Try invalid password and expired/disabled user if available. | None | `/login` | P0 | Negative | Login is rejected; protected routes remain inaccessible. |
| UAT-003 | Security | All roles | Authorization | Paste a route that the role does not have permission to view. | Missing target permission | Any protected route | P0 | Negative | Page blocks access or actions are hidden/disabled; backend rejects direct action attempt. |
| UAT-004 | UX | All roles | Localization | Toggle English/Arabic and navigate a list and form page. | Any allowed page permission | Any allowed page | P2 | Positive | Text, direction, layout spacing, and action alignment remain usable in both languages. |
| UAT-005 | Attendance | Regular employee | My attendance | Open My Attendance and perform the normal daily attendance action available in the page. | `Attendance.Attendance.Create` | `/Attendance/MyAttendance` | P0 | Positive | Attendance action succeeds; latest session/status updates for the logged-in employee only. |
| UAT-006 | Attendance | Regular employee | My attendance validation | Repeat an attendance action that is no longer valid for the current state. | `Attendance.Attendance.Create` | `/Attendance/MyAttendance` | P0 | Negative | Duplicate or invalid state transition is rejected with a visible validation/error message. |
| UAT-007 | Leave | Regular employee | Emergency leave request | Create an emergency leave request for an available date and reason. | `Attendance.Attendance.RequestEmergencyLeave` | `/LeavesManagement/EmergencyLeaves` | P0 | Positive | Request is saved as pending and appears in the employee's request list. |
| UAT-008 | Leave | Regular employee | Leave balance | Open leave balances and verify own available/taken/remaining values. | `Attendance.Attendance.ViewLeaveBalances` | `/LeavesManagement/Balances` | P0 | Positive | Employee can view allowed leave balance data without seeing unrelated employees unless permitted. |
| UAT-009 | Leave | Regular employee | Leave validation | Submit leave with invalid date range, missing reason, or insufficient balance. | `Attendance.Attendance.RequestEmergencyLeave` | `/LeavesManagement/EmergencyLeaves` | P0 | Negative | Request is rejected; balance is not changed and clear validation is shown. |
| UAT-010 | Attendance | Regular employee | Mid-day permission | Submit a same-day permission request for a valid time window. | `Attendance.Attendance.RequestMidDayPermission` | `/Attendance/PermissionRequests` | P0 | Positive | Request is submitted and appears with pending/review status. |
| UAT-011 | Task management | Regular employee | My tasks | Open My Tasks, view an assigned task, add a comment/progress update. | `TaskManagement.Task.View`, `TaskManagement.Task.Comment` | `/TaskManagement/MyTasks` | P0 | Positive | Only assigned/relevant tasks appear; comment/progress is saved in task history. |
| UAT-012 | Task management | Regular employee | Task authorization | Try to edit, close, assign, or delete a task without those permissions. | Missing `Edit`, `Close`, `Assign`, or `Delete` | `/TaskManagement/View/{id}` | P0 | Negative | Restricted actions are hidden/disabled or rejected by backend. |
| UAT-013 | Task management | Department head | Create department task | Create a task for an employee in the same department and assign it. | `TaskManagement.Task.Create`, `TaskManagement.Task.Assign` | `/TaskManagement/Create` | P0 | Positive | Task is created, assigned employee can see it, and notification appears where supported. |
| UAT-014 | Task management | Department head | Department scope | Try assigning or viewing tasks for employees outside the department without manage-all permission. | Department head permissions without `ManageAllTasks` | `/TaskManagement/List` | P0 | Negative | Out-of-scope employee/task is unavailable or action is rejected. |
| UAT-015 | Task management | Department head | Kanban/status | Move a department task through allowed statuses and close it. | `TaskManagement.Task.View`, `TaskManagement.Task.Edit`, `TaskManagement.Task.Close` | `/TaskManagement/Kanban` | P0 | Positive | Status updates persist; closed state is visible in list/detail/report views. |
| UAT-016 | Leave | Department head | Approve emergency leave | Approve or reject a pending emergency leave for a subordinate. | `Attendance.Attendance.ApproveEmergencyLeave` | `/LeavesManagement/ApproveEmergencyLeaves` | P0 | Positive | Request status changes; employee sees the decision; balance impact follows business rules. |
| UAT-017 | Attendance | Department head | Approve permission | Approve or reject a pending mid-day permission request. | `Attendance.Attendance.ApproveMidDayPermission` | `/Attendance/ApprovePermissionRequests` | P0 | Positive | Request is reviewed and no longer appears as pending. |
| UAT-018 | Reports | Department head | Scoped task/attendance reports | Open scoped reports and filter by date/department. | `TaskManagement.Task.ViewReports`, `Attendance.Attendance.ViewScopedReports` | `/TaskManagement/Reports`, `/Attendance/Reports` | P1 | Positive | Reports show only allowed department scope and respect filters. |
| UAT-019 | Employee | HR admin | Employee CRUD | Create employee, edit core details, view employee profile/360, and validate list search. | `Employees.Employee.Create`, `View`, `Edit` | `/Employee/Employee/List` | P0 | Positive | Employee appears in list/profile and can be used by related modules. |
| UAT-020 | Employee | HR admin | Employee validation | Create employee with missing required fields or duplicate identity/code where applicable. | `Employees.Employee.Create` | `/Employee/Employee/Form/{id?}` | P1 | Negative | Save is blocked; no partial employee is created. |
| UAT-021 | Attendance | HR admin | Shifts and assignments | Create/update shift and assign to employee/department. | `Attendance.Attendance.Edit` | `/Attendance/Shifts`, `/Attendance/ShiftAssignments` | P0 | Positive | Shift and assignment save; attendance pages use updated schedule. |
| UAT-022 | Attendance | HR admin | Attendance sessions review | Review late check-in/session normalization request. | `Attendance.Attendance.ReviewRequests` | `/Attendance/Sessions`, `/Attendance/LateRequests` | P0 | Positive | Request/session review decision is saved and reflected in reports. |
| UAT-023 | Attendance | HR admin | Configuration and holidays | Update break policy/configuration and add holiday/weekend entry. | `ViewConfiguration`, `ManageConfiguration`, `ManageHolidays` | `/Attendance/Configuration`, `/Attendance/Holidays` | P1 | Positive | Configuration persists and affects relevant validation/reporting behavior. |
| UAT-024 | Leave | HR admin | Leave balance management | Adjust or configure employee leave balance. | `Attendance.Attendance.ManageLeaveBalances` | `/LeavesManagement/Balances` | P0 | Positive | Balance changes persist with correct available/taken/remaining calculation. |
| UAT-025 | Payroll | Payroll user/head | Salary contracts | Create/edit salary contract and assign it to an employee. | `Payroll.Contract.Create`, `Edit`, `View` | `/Payroll/Contracts`, `/Payroll/AssignContract` | P1 | Positive | Contract is saved and employee assignment is visible for payroll runs. |
| UAT-026 | Payroll | Payroll user/head | Contract validation | Save contract with missing employee/component/date or invalid amount. | `Payroll.Contract.Create` | `/Payroll/Contracts` | P1 | Negative | Validation prevents save; no incorrect payroll contract is used. |
| UAT-027 | Payroll | Payroll user/head | Employee loans | Create, approve, and cancel a loan according to status rules. | `Payroll.Loan.Create`, `Edit`, `Approve`, `Cancel` | `/Payroll/Loans` | P1 | Positive | Loan status changes correctly and cannot be edited in invalid states. |
| UAT-028 | Payroll | Payroll user/head | Salary run | Generate salary run for a valid period and approve it. | `Payroll.SalaryRun.Create`, `View`, `Approve` | `/Payroll/SalaryRuns` | P0 | Positive | Salary run is created with employees, totals, and approved status. |
| UAT-029 | Payroll | Payroll user/head | Salary run control | Attempt duplicate generation or undo without admin override. | Missing `AdminOverride` or duplicate period | `/Payroll/SalaryRuns` | P0 | Negative | Duplicate/unauthorized operation is rejected; existing run remains intact. |
| UAT-030 | Customers | Sales personnel | Customer maintenance | Create/edit a customer and assign group/pricing profile where available. | `Customers.Customer.Create`, `Edit`, `View` | `/Customers/Customer/List` | P1 | Positive | Customer is saved and selectable in sales/POS flows. |
| UAT-031 | Sales | Sales personnel | POS/sales order create | Create a POS/order using valid customer, product SKU, pricing, and stock. | `SalesOrders.Order.Create`, product/variant/inventory view permissions | `/SalesOrder/POS` | P0 | Positive | Order/cart is saved; totals calculate; inventory/pricing data is used. |
| UAT-032 | Sales | Sales personnel | Sales validation | Attempt order with unavailable stock, missing customer/product, or invalid quantity. | `SalesOrders.Order.Create` | `/SalesOrder/POS` | P0 | Negative | Order is blocked or line is rejected; stock and totals remain unchanged. |
| UAT-033 | Sales | Sales head | Order lifecycle | Confirm, deliver, invoice, complete an eligible order. | `SalesOrders.Order.Confirm`, `Deliver`, `Invoice`, `Complete` | Sales order/POS/order views | P0 | Positive | Order moves through expected lifecycle and final status is visible. |
| UAT-034 | Sales | Sales head | Sales lifecycle control | Try invoice before delivery, complete cancelled order, or return without allowed state. | Sales lifecycle permissions | Sales order/POS/order views | P0 | Negative | Invalid state transition is rejected; audit/status remains correct. |
| UAT-035 | Payments | Sales head | Payment approval/refund | Approve/reject payment and process refund where available. | `Payments.Payment.Approve`, `Reject`, `Refund` | Payment-related sales route/API | P1 | Positive | Payment status updates and order financial state reflects the decision. |
| UAT-036 | Procurement | Procurement user/head | Purchase request | Create purchase request, submit it, approve/reject it. | `Procurement.PurchaseRequest.Create`, `Submit`, `Approve`, `Reject` | `/Procurement/purchase-requests` | P0 | Positive | Request status changes through draft/submitted/approved or rejected. |
| UAT-037 | Procurement | Procurement user/head | RFQ | Create RFQ from valid data and send/close/cancel it. | `Procurement.RequestForQuotation.Create`, `Submit`, `Close`, `Cancel` | `/Procurement/requests-for-quotation` | P0 | Positive | RFQ is saved and workflow action updates status. |
| UAT-038 | Procurement | Procurement user/head | Supplier quotation | Create supplier quotation and accept/reject it. | `Procurement.SupplierQuotation.Create`, `Approve`, `Reject` | `/Procurement/supplier-quotations` | P0 | Positive | Accepted quotation can feed the next procurement step where supported. |
| UAT-039 | Procurement | Procurement user/head | Purchase order | Create/send/approve/cancel/close purchase order. | `Procurement.PurchaseOrder.Create`, `Submit`, `Approve`, `Cancel`, `Close` | `/Procurement/purchase-orders` | P0 | Positive | Purchase order status and downstream availability update correctly. |
| UAT-040 | Procurement | Procurement user/head | Receiving and invoice | Post goods receipt, create supplier invoice, match/post/cancel invoice. | `GoodsReceipt.Receive`, `SupplierInvoice.Approve`, `SupplierInvoice.Close`, `SupplierInvoice.Cancel` | `/Procurement/goods-receipts`, `/Procurement/supplier-invoices` | P0 | Positive | Inventory/receiving and invoice statuses update consistently. |
| UAT-041 | Procurement | Procurement user/head | Procurement validation | Try approving draft, posting cancelled document, or editing closed document. | Relevant procurement permissions | `/Procurement/{kind}` | P0 | Negative | Invalid workflow state is rejected; document remains unchanged. |
| UAT-042 | Catalog | Inventory/catalog user | Product master data | Create category, brand, unit, variant, product, SKU, and package. | Relevant `Catalog.*` create/edit/view permissions | Catalog/product routes | P1 | Positive | Master data is saved and selectable by inventory, sales, and procurement flows. |
| UAT-043 | Catalog | Inventory/catalog user | Catalog validation | Create duplicate or incomplete master data record. | Relevant `Catalog.*.Create` | Catalog/product routes | P1 | Negative | Save is blocked or duplicate is rejected according to validation rules. |
| UAT-044 | Pricing | Inventory/catalog user | Price list | Create/edit a price list for products/SKUs. | `Pricing.PriceList.Create`, `Edit`, `View` | `/Catalog/Pricing/List` | P1 | Positive | Price list is visible and sales/POS pricing reflects expected value. |
| UAT-045 | Inventory | Inventory/catalog user | Warehouse and batches | Create warehouse and batch, then verify current stock page. | `Inventory.Warehouse.*`, `Inventory.Batch.*`, `Inventory.Inventory.View` | `/Inventory/Warehouse/List`, `/Inventory/Batch/List`, `/Inventories/List` | P1 | Positive | Warehouse/batch records save and appear in inventory selection/list views. |
| UAT-046 | Inventory | Inventory/catalog user | Stock operations | Perform stock in, stock out, adjustment, reserve, and release. | `Inventory.Inventory.Create/Edit`, `Inventory.InventoryItem.Create/Edit` | `/Inventory/Operations/StockIn`, `/StockOut`, `/StockAdjustment`, `/StockReservation`, `/StockRelease` | P0 | Positive | Stock quantities, reservations, and availability update correctly. |
| UAT-047 | Inventory | Inventory/catalog user | Stock validation | Try stock out more than available or release more than reserved. | Inventory operation permissions | Inventory operation routes | P0 | Negative | Operation is rejected; stock quantities remain unchanged. |
| UAT-048 | Security | System admin | Role management | Create role, select permissions, edit role, and verify grouped permission list. | `Authentication.Roles.Create`, `View`, `Edit` | `/Auth/Role/List`, `/Auth/Role/Form` | P0 | Positive | Role is saved with selected permission claims and can be assigned. |
| UAT-049 | Security | System admin | User role assignment | Assign and unassign role for a test user, then log in as that user. | `Authentication.Users.View`, `Authentication.Roles.Create/Delete` | `/Auth/User/AssignRole` | P0 | Positive | User menu/actions change according to assigned role after login/session refresh. |
| UAT-050 | Organization | System admin | Organization setup | Create/edit branch, administration, and department. | `Organization.Branch.*`, `Organization.Administration.*`, `Organization.Department.*` | Organization routes | P1 | Positive | Structure saves and becomes available for employee assignment and scoped reports. |
| UAT-051 | Organization | System admin | Child company controls | View/create/edit/disable child company and reset child admin password where available. | `Organization.Company.ViewChild`, `CreateChild`, `EditChild`, `DisableChild`, `ResetChildAdminPassword` | `/Organization/ChildCompanies` | P1 | Positive | Child company operation completes and status/admin access changes as expected. |
| UAT-052 | General settings | System admin | System settings/currencies | View/edit system settings and currencies. | `GeneralSettings.SystemSettings.View`, `Edit` | `/GeneralSettings/SystemSettings`, `/GeneralSettings/Currencies` | P2 | Positive | Setting/currency changes persist and are reflected in dependent UI where applicable. |
| UAT-053 | Lists and search | All roles | Lists | Search, filter, paginate, open empty-state list, and verify loading/error handling on allowed pages. | Relevant `View` permission | Any list route | P2 | Positive | Lists remain usable; filters/paging work; empty/error states are understandable. |
| UAT-054 | Reports | Reporting roles | Reports | Run attendance, leave, task, sales, procurement, and inventory reports where available. | Relevant `ViewReports`, `ViewScopedReports`, or `ViewAllReports` | Report routes | P2 | Positive | Reports respect filters, user scope, and show/export expected data where supported. |
| UAT-055 | API/security | All roles | Direct action control | Use browser/dev tools or direct API client to call an action without the required permission. | Missing action permission | Relevant API endpoint | P0 | Negative | API returns unauthorized/forbidden; no data is changed. |

## Execution Guidance

1. Start with `UAT-001` to `UAT-004` for every role before feature testing.
2. Execute P0 cases first. Do not proceed to broad P1/P2 testing if login, authorization, or primary workflows fail.
3. For every create/approve lifecycle, record the created entity ID, status before action, status after action, and screenshot/evidence.
4. For negative tests, confirm both UI behavior and data integrity: no record created, no status changed, no balance/stock/payroll amount altered.
5. Retest affected cases after role/permission changes, because menu visibility and backend authorization are both part of acceptance.

## Acceptance Criteria

The system is acceptable for UAT completion when:

- Each role can access only the expected menu items and actions.
- Regular employee self-service works for attendance, leave/permission requests, tasks, and allowed balance/payroll views.
- Department heads can manage department work and approvals without seeing unrelated department data unless explicitly permitted.
- HR, payroll, sales, procurement, inventory/catalog, and system admin workflows complete their main lifecycle actions.
- Negative cases fail safely with no unintended data changes.
- Reports and lists respect permissions, scope, filters, language direction, and pagination.
