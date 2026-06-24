---
name: alafkar-erp-workspace-navigation
description: Apply Alafkar ERP workspace navigation rules. Use when Codex changes MenuItem metadata, workspace icons, desktop or enterprise sidebar navigation, workspace trails, functional groups, journey groups, menu ordering, navigation search/favorites/recent behavior, or AGENTS navigation guidance.
---

# Alafkar ERP Workspace Navigation

Use this after the main development guide, and after the design-system skill when UI markup or styling is affected.

## Navigation Contract

- Preserve existing routes, permissions, DTOs, services, localization, favorites, recent items, search, and active-route behavior unless the user explicitly requests a behavior change.
- Model navigation in three levels: workspace, functional group, then journey group.
- For new menu entries, set or inherit:
  - `WorkspaceKey`
  - `NavigationFunctionalGroupKey`
  - `NavigationGroupKey`
  - `NavigationOrder`
  - `PermissionPolicy`
  - `Icon`
  - English and Arabic labels
  - English and Arabic keywords
- Keep functional groups permission-safe: show a functional group only when at least one authorized navigable child is available.
- Keep journey groups ordered as: Start / Overview, Approvals, Daily Work, Master Data, Setup, Reports. Keep compatibility-only groups such as Adjustments and Administration after those unless the user requests a full taxonomy migration.

## Functional Groups

Assign every business navigation entry to one of the workspace functional groups below. Home and More are shell-level views and do not use functional groups.

### HR

- Employees: employee dashboard, command center, directory, teams, positions, academic institutions, specializations, lifecycle, documents, emergency contacts, skills, certifications, and HR reports.
- Attendance: attendance dashboard, attendance daily work, shifts, rosters, holidays, requests, approvals, work entries, and attendance reports.
- Leaves: emergency leaves, approvals, balances, policies, applications, ledger, and leave reports.
- Payroll: salary runs, contracts, assignments, components, salary structures, payslips, payroll work entries, Saudi payroll, loans, and deductions.
- Recruitment: recruitment workspace and related pages.
- Performance: performance workspace and related pages.
- Training: training workspace and related pages.

### POS

- Checkout: POS checkout, storefront POS, and cashier flows.

### Sales

- POS: POS checkout discoverability from Sales.
- Sales: sales dashboard, orders, intakes, quotations, and sales work.
- Customers: customer directory, customer groups, and customer pricing.
- Reports: sales reporting pages.

### Purchasing

- Suppliers: supplier directory and supplier setup.
- Procurement: purchase requests, RFQs, quotations, purchase orders, receipts, returns, invoices, procurement dashboard, and procurement reports.

### Warehouse

- Products: product catalog, SKU, variants, packages, brands, categories, units, and pricing.
- Inventory: inventory dashboard, warehouses, current stock, batches, transfers, movements, controls, and asset instances.
- Stock Operations: stock-in, stock-out, and stock operation pages.

### Accounting / Finance

- Setup: accounting dashboard, setup, templates, fiscal periods, posting profiles, and company defaults.
- Chart of Accounts: account tree and account setup.
- Banking & Cash: bank accounts, cash accounts, and bank reconciliation.
- Journals & Documents: journal entries and accounting documents.
- Invoices: sales invoices and purchase invoices.
- Receipts & Payments: receipt and payment workflows.
- Adjustments: credit notes, debit notes, and finance adjustments.
- Tax / ZATCA: tax codes, ZATCA submissions, and ZATCA settings.
- Reports: accounting and finance reports.

### Catering

- Contracts: catering dashboard and contracts.
- Meals: meal catalog and related setup.
- Locations: distribution locations.
- Schedules: daily schedules.
- Deliveries: delivery and refrigerated vehicle receiving.
- Assignments: assignment workflows.
- Reports: catering reports.

### Real Estate

- Properties: real estate dashboard, properties, and units.
- Leasing: owner leases and tenant leases.
- Collections: rent collections and installment work.
- Utilities & Expenses: utilities and expense tracking.
- Reports: real estate reports.

### Admin

- Organization: control panel, companies, branches, departments, business lines, license categories, and storefront administration.
- Contracts: contract dashboard, contract lists, renewals, templates, and contract setup.
- Documents: document management and media center.
- Projects: project dashboard, projects, distribution places, schedules, and project reports.
- Tasks: task dashboard, personal tasks, notifications, task list, kanban, and task reports.
- Fleet: fleet dashboard, vehicles, assignments, expenses, documents, service rules, and fleet reports.
- Maintenance: maintenance dashboard, assets, work orders, requests, and reports.
- General Settings: system settings, home page templates, currencies, and global configuration.

### IT / Security

- Security: security dashboard and security overview pages.
- Roles: role management.
- User Access: user role assignments and access administration.

## Implementation Rules

- Use `NavigationMenuResolver` helpers rather than duplicating filtering logic in components.
- Keep synthetic navigation nodes non-favoritable and non-navigable unless they represent a real page.
- Update both `DesktopWorkspaceNav` and `EnterpriseNavigationPanel` when changing workspace drilldown behavior.
- For direct navigation to a page, expand or select the matching functional group and journey group.
- When clicking a workspace icon, show the workspace's functional group level first when that workspace has functional groups.
- Keep POS compatible with both entry points: POS remains its own workspace icon and is also discoverable under the Sales POS functional group.
- Keep accounting sales/purchase aliases under Accounting / Finance unless the user explicitly asks to move accounting links into Sales or Purchasing.

## Verification

- Run `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj`.
- For UI changes, run the design-system static checks from `alafkar-erp-design-system`.
- Confirm routes and permission policies on leaf menu items were not changed unintentionally.
