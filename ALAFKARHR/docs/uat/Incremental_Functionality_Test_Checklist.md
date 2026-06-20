# AlAfkar ERP Incremental Functionality Testing Checklist

Use this checklist in order. It is intentionally grouped by dependency: platform and tenant setup first, then company administration/security, HR foundation, and then modules that depend on those records.

Status suggestion: leave the checkbox empty until the function is tested, and record proof in `Incremental_Functionality_Test_Checklist.csv` using the `Evidence` and `Notes` columns.

## 1. Platform / Tenant Management

- [ ] Login, logout, forgot password, protected route behavior
- [ ] Platform dashboard / control panel
- [ ] Tenant companies / parent companies: list, create, edit, delete/suspend
- [ ] Parent company license management
- [ ] Reset parent company admin password
- [ ] Child companies: list, create, edit, disable
- [ ] Reset child company admin password
- [ ] Company switcher / active company context

## 2. Parent Company Admin Setup

- [ ] Organization dashboard
- [ ] Company profile list and edit
- [ ] Branches: list, create, edit, delete/search/paging
- [ ] Administrations: list, create, edit, delete/search/paging
- [ ] Departments: list, create, edit, delete/search/paging
- [ ] Security dashboard
- [ ] Roles management: list, create, edit, permissions assignment, delete
- [ ] Users: create/edit user pages exist, assign user roles
- [ ] System settings
- [ ] Currency management: list, available currencies, create, edit, delete
- [ ] Permission-based menu visibility and authorization errors

## 3. HR Master Data

- [ ] HR dashboard
- [ ] Positions: list, create, edit, delete/search/paging
- [ ] Academic institutions: list, create, edit, delete/search/paging
- [ ] Specializations: list, create, edit, delete/search/paging
- [ ] Employees: list, create, edit, delete/search/paging
- [ ] Employee profile view
- [ ] Employee protected/self view
- [ ] Employee 360 view
- [ ] Employee QR page

## 4. Attendance and Leave

- [ ] Attendance dashboard
- [ ] My attendance: check-in/check-out/break flows where available
- [ ] Attendance sessions: list/filter/view
- [ ] Shifts: create/edit/list
- [ ] Shift assignments: assign shifts to employees
- [ ] Late requests: list/review
- [ ] Attendance configuration: view/manage settings
- [ ] Holidays: create/edit/delete/list
- [ ] Mid-day permission requests: request and approve
- [ ] Emergency leave requests: request and approve
- [ ] Leave balances: view/manage balances
- [ ] Attendance reports: overview, daily attendance, employee summary, late arrival, early leave, break, permission requests, absence, holiday/weekend
- [ ] Leave reports

## 5. Payroll

- [ ] Payroll components: list/create/edit/delete
- [ ] Salary contracts: list/create/edit/delete
- [ ] Assign contract to employee
- [ ] Loans and deductions: create/edit/approve/cancel
- [ ] Salary runs: generate, view, edit, approve, admin override

## 6. Products, Pricing, and Inventory Foundation

- [ ] Product dashboard
- [ ] Categories: list/create/edit/delete
- [ ] Brands: list/create/edit/delete
- [ ] Units: list/create/edit/delete
- [ ] Product options / variants: list/create/edit/delete
- [ ] Products: list/create/edit/delete
- [ ] Product SKUs: list/add/edit/delete
- [ ] Packages: list/create/edit/delete
- [ ] Pricing list / price list items
- [ ] Inventory dashboard
- [ ] Warehouses: list/create/edit/delete
- [ ] Current stock / inventory list
- [ ] Batches: list/create/edit/delete
- [ ] Stock in
- [ ] Stock out
- [ ] Stock adjustment
- [ ] Stock reservation
- [ ] Stock release
- [ ] Warehouse transfer form route exists, but appears not currently in the active menu

## 7. Suppliers and Procurement

- [ ] Supplier groups: list/create/edit/delete
- [ ] Suppliers: list/create/edit/delete/detail view
- [ ] Procurement dashboard
- [ ] Purchase requests: list/create/edit/detail/delete/cancel/submit or workflow actions
- [ ] Requests for quotation: list/create/edit/detail/delete/cancel
- [ ] Supplier quotations: list/create/edit/detail/approve/cancel/close
- [ ] Purchase orders: list/create/edit/detail/approve/cancel/close
- [ ] Goods receipts: list/create/edit/detail/receive/cancel
- [ ] Purchase returns: list/create/edit/detail/receive/cancel
- [ ] Supplier invoices: list/create/edit/detail/approve/cancel/close

## 8. Customers, Sales, and POS

- [ ] Customer dashboard
- [ ] Customer groups: list/create/edit/delete
- [ ] Customers: list/create/edit/delete
- [ ] Customer pricing profiles / special customer pricing
- [ ] POS: create cart, add/update/remove/clear lines, checkout
- [ ] Order intakes: list/detail/workflow
- [ ] Sales dashboard
- [ ] Sales orders: list/detail
- [ ] Sales reports/dashboard metrics

## 9. Contracts

- [ ] Contracts dashboard
- [ ] Contracts list
- [ ] Create/edit contract
- [ ] Contract detail
- [ ] Party contracts by party type/id
- [ ] Contract renewals
- [ ] Contract templates

## 10. Document Management

- [ ] Document library
- [ ] My documents
- [ ] Shared with me
- [ ] Create/new document
- [ ] Document detail/view
- [ ] Document attachments panel
- [ ] Source documents
- [ ] Sharing/version/attachment behavior where exposed by the UI

## 11. Task Management

- [ ] Task dashboard
- [ ] My tasks
- [ ] Notifications
- [ ] Task list
- [ ] Create task
- [ ] Edit task
- [ ] View task
- [ ] Kanban board
- [ ] Comments
- [ ] Progress updates
- [ ] Attachments
- [ ] Assignment/reassignment
- [ ] Close task
- [ ] Task reports

## 12. Platform Operations

- [ ] Fleet dashboard
- [ ] Vehicles: list/create/edit/delete/detail
- [ ] Vehicle odometer update
- [ ] Vehicle assignments: create/list/return/cancel
- [ ] Fleet expenses: create/edit/submit/approve/delete
- [ ] Fleet documents: create/edit/renew/delete
- [ ] Fleet service rules: create/edit/complete/delete
- [ ] Create maintenance work order from fleet service/emergency maintenance
- [ ] Fleet reports
- [ ] Real estate dashboard
- [ ] Properties: list/create/edit/delete/detail
- [ ] Units: list/create/edit/delete/detail
- [ ] Owner leases and tenant leases: list/create/edit/detail
- [ ] Lease actions: generate installments, activate, suspend, terminate
- [ ] Rent collections: view installments and record payments
- [ ] Utilities: list/create/edit/delete
- [ ] Expenses: list/create/edit/delete
- [ ] Real estate reports
- [ ] Maintenance dashboard
- [ ] Maintenance assets: list/create/edit/delete/detail
- [ ] Work orders: list/create/edit/delete/detail
- [ ] My maintenance requests
- [ ] Work order actions: assign, change status, approve cost, comment, upload attachment
- [ ] Maintenance reports

## 13. Cross-Cutting Checks

- [ ] Arabic/English language toggle on all major pages
- [ ] RTL/LTR layout behavior
- [ ] Search, filters, paging, empty states, loading states
- [ ] Toast/error messages for successful and failed operations
- [ ] Create/edit/delete permission checks per role
- [ ] Company scoping: records should appear only under the correct company/branch scope
- [ ] Dashboard counts update after transactions
- [ ] Detail pages open correctly from list rows/actions
- [ ] Form validation for required fields and invalid data

## Assumptions

- This checklist is based on the current Blazor menu, routed pages, permission groups, and backend endpoint surface.
- Menu-visible features are prioritized over backend-only endpoints.
- Commented-out menu items such as inventory transfers/movements/expiry tracking/stock count/reports are treated as lower priority unless they are enabled in navigation.
