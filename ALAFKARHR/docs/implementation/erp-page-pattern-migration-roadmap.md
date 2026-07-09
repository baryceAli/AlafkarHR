# ERP Page Pattern Migration Roadmap

## Purpose

Track the phase-by-phase migration of AdminLayout business pages to the shared ERP UI pattern:
`erp-page`, `PageHeader`, `FilterBar`, `AppCard`, `EmptyState`, `StatsCard`, `StatusChip`,
`erp-table-wrap`, and `erp-table`.

This roadmap is for UI-only modernization. It must not change routes, DTOs, permissions, service
calls, localization, event handlers, modals, toasts, paging, sorting, search behavior, backend APIs,
database schema, or business rules.

## Progress Checklist

| Phase | Scope | Status | Notes |
|---|---|---|---|
| Phase 0 | Baseline scan and migration rules | Complete | Baseline recorded below. |
| Phase 1 | Customers + Organization | Complete | Customer and Organization admin pages migrated to shared ERP surfaces. |
| Phase 2 | Attendance + Fleet + Maintenance | Planned | Replace repeated raw-card surfaces. |
| Phase 3 | ProjectManagement + RealEstate + Catering | Planned | Reports, dashboards, and domain panels. |
| Phase 4 | Inventory + Employees + TaskManagement | Planned | Mixed pages with partial modern patterns. |
| Phase 5 | DocumentManagement + Auth management + remaining partial-debt pages | Planned | Admin surfaces and smaller cleanup targets. |
| Phase 6 | Final sweep, verification, and cleanup | Planned | Re-scan, build, and update deferred exceptions. |

## Baseline Snapshot

Captured from the current worktree before this roadmap was added.

| Metric | Count |
|---|---:|
| AdminLayout routed feature pages | 226 |
| Pages missing `erp-page` | 48 |
| Pages missing `AppCard` | 102 |
| Pages with raw Bootstrap `card` usage | 105 |
| Pages with `table-responsive` usage | 42 |

### Baseline By Module

| Module | Pages | Missing `erp-page` | Missing `AppCard` | Raw card | `table-responsive` |
|---|---:|---:|---:|---:|---:|
| Accounting | 14 | 0 | 0 | 0 | 0 |
| Attendance | 10 | 0 | 9 | 9 | 0 |
| Auth | 5 | 3 | 3 | 0 | 0 |
| Catalog | 9 | 0 | 0 | 0 | 0 |
| Catering | 11 | 0 | 11 | 11 | 0 |
| Contracts | 7 | 0 | 0 | 0 | 0 |
| Customers | 4 | 4 | 4 | 4 | 4 |
| DocumentManagement | 7 | 4 | 4 | 0 | 0 |
| Employees | 27 | 11 | 6 | 8 | 10 |
| Fleet | 9 | 0 | 9 | 9 | 0 |
| GeneralSettings | 4 | 0 | 0 | 2 | 2 |
| Inventories | 20 | 5 | 12 | 13 | 0 |
| LeavesManagement | 5 | 0 | 0 | 0 | 0 |
| Maintenance | 8 | 0 | 8 | 8 | 0 |
| MediaCenter | 3 | 0 | 0 | 0 | 0 |
| Organization | 10 | 2 | 9 | 9 | 7 |
| Payroll | 5 | 0 | 0 | 0 | 0 |
| Procurement | 13 | 0 | 0 | 0 | 0 |
| ProjectManagement | 10 | 0 | 10 | 9 | 7 |
| RealEstate | 13 | 0 | 13 | 13 | 9 |
| SalesOrder | 14 | 12 | 0 | 0 | 0 |
| StoreFront | 5 | 2 | 0 | 0 | 2 |
| Suppliers | 4 | 0 | 0 | 0 | 0 |
| TaskManagement | 9 | 4 | 4 | 4 | 0 |

## Migration Rules

- Preserve existing behavior: routes, layouts, permissions, services, DTO bindings, validation,
  event handlers, modals, toasts, search, paging, sorting, export, localization, and RTL/LTR behavior.
- Prefer shared components and classes already present in the app:
  `PageHeader`, `AppCard`, `FilterBar`, `StatsCard`, `EmptyState`, `TableExportActions`,
  `StatusChip`, `erp-loading`, `erp-table-toolbar`, `erp-table-wrap`, and `erp-table`.
- Replace old page-specific structural wrappers such as `branches-page`, `branches-header`,
  `sessions-header`, `filter-panel`, `sessions-panel`, `requests-panel`, `shift-panel`,
  raw `card-body table-responsive`, and `panel-title` where a shared ERP surface fits.
- Use `StatusChip` for workflow/status badges where practical. Keep lightweight non-status chips
  for compact metadata such as dates, counts, configured days, or short labels.
- Add `CssClass="allow-overflow"` to `AppCard` when it contains autocomplete, dropdown, popover,
  menu, or similar overlay content.
- Keep page-local CSS only for truly page-specific layout. Put reusable styling in
  `UI/AlAfkarERP/AlAfkarERP.Shared/wwwroot/theme.css`.
- Use logical CSS properties for RTL/LTR support. Avoid new physical `left`, `right`,
  `margin-left`, `padding-right`, and similar directional styling unless required by an API or browser constraint.

## Exceptions

Do not force these surfaces into the AdminLayout business-page pattern unless explicitly requested:

- Login, register, forgot-password, and other auth-public pages.
- POS checkout and cashier-first flows.
- Print pages.
- Public employee, protected employee, QR, and other special-purpose public views.
- Highly custom operational surfaces where the current layout is intentional and the shared pattern would reduce usability.

Document any deferred exception in the phase notes before marking a phase complete.

## Phase 1: Customers + Organization

Goal: migrate the highest-signal legacy admin pages first.

- Customers: replace older `branches-*`, raw-card, and `table-responsive` structures with
  `erp-page`, `PageHeader`, `StatsCard`, `AppCard`, `FilterBar`, `EmptyState`, `erp-table-wrap`, and `erp-table`.
- Organization: replace raw cards and table wrappers while preserving company/branch scope behavior,
  hierarchy rules, selectors, inline editors, modals, permissions, and toasts.
- Preserve existing `SharedDataService.OnChange1` language-change handling, or add it when a touched page depends on localized text or direction.

Completion criteria:

- Customers and Organization target pages re-scan with no avoidable missing `erp-page`, missing `AppCard`,
  raw-card, or `table-responsive` hits.
- UI build succeeds.
- Any intentional exceptions are listed in this roadmap.

## Phase 2: Attendance + Fleet + Maintenance

Goal: convert repeated raw-card surfaces in workflow-heavy modules.

- Attendance: migrate request, approval, shift, holiday, session, report, and personal attendance pages while preserving workflow actions.
- Fleet: migrate dashboard, list, detail, form, assignments, documents, expenses, service rules, and report pages.
- Maintenance: migrate asset, work-order, dashboard, form, view, and report pages.
- Keep domain-specific workflow panels if they are genuinely custom, but wrap reusable content in shared ERP surfaces.

Completion criteria:

- Phase modules have no avoidable raw Bootstrap card wrappers.
- Existing workflow actions and permission-gated buttons remain unchanged.
- UI build succeeds.

## Phase 3: ProjectManagement + RealEstate + Catering

Goal: migrate report-heavy and domain-panel pages without changing report behavior.

- ProjectManagement: convert dashboards, project lists/details, distribution places/schedules, and report tables.
- RealEstate: convert dashboard, properties, units, leases, collections, utilities, expenses, details, forms, and reports.
- Catering: convert dashboard, contracts, meals, locations, schedules, deliveries, assignments, plans, packaging, projects, and reports.
- Preserve report filters, loaded data, calculated values, export behavior, and custom domain layout where needed.

Completion criteria:

- Report and dashboard panels use shared cards/tables where applicable.
- No new API or service methods are introduced for visual-only migration.
- UI build succeeds.

## Phase 4: Inventory + Employees + TaskManagement

Goal: finish mixed modules carefully because several pages already have partial modern patterns.

- Inventory: migrate asset, stock movement, transfer, operation, picking, scrap, and stock operation pages while preserving barcode and stock workflows.
- Employees: migrate lookup/list/detail/enhancement pages while preserving employee selectors, public/protected route behavior, and HR workflow state.
- TaskManagement: migrate notifications, reports, task list, dashboard, and kanban surfaces without changing task interactions.
- Add `allow-overflow` to cards containing autocomplete/dropdown overlays.

Completion criteria:

- Partial modern pages are normalized without regressing existing behavior.
- Barcode, stock operation, employee lookup, and task workflow flows are preserved.
- UI build succeeds.

## Phase 5: Remaining Admin Surfaces

Goal: sweep smaller and partially modern admin pages.

- DocumentManagement: migrate list, library, source, shared, and personal document surfaces where appropriate.
- Auth management: migrate admin roles, users, user assignment, and role forms; leave auth-public pages as exceptions.
- GeneralSettings, Payroll, Catalog, Procurement, SalesOrder, StoreFront: remove remaining avoidable raw-card,
  missing-wrapper, and table-wrapper debt while respecting special surfaces.

Completion criteria:

- Remaining AdminLayout business pages are either migrated or explicitly documented as exceptions.
- Special POS, print, auth-public, and public employee surfaces are not accidentally flattened into the admin pattern.
- UI build succeeds.

## Phase 6: Final Verification

Goal: confirm the roadmap is complete and useful for future maintenance.

- Re-run the baseline scan and add a final snapshot below this section.
- Run design-system static checks:

```powershell
git diff --check
rg -n '#[0-9a-fA-F]{3,6}|rgba\(|box-shadow:|border-radius:|border-left:|border-right:|left:|right:' UI/AlAfkarERP/AlAfkarERP.Shared -g '*.razor' -g '*.css' -g '!**/Layout/Old/**'
rg -n 'style=' UI/AlAfkarERP/AlAfkarERP.Shared -g '*.razor' -g '!**/Layout/Old/**'
dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj
```

- Treat static-scan hits as review targets, not automatic failures. Token definitions in `theme.css`
  and data-driven inline CSS custom properties are allowed.
- Update the progress checklist, final snapshot, and deferred exception list.

### Final Snapshot

To be completed during Phase 6.

| Metric | Count |
|---|---:|
| AdminLayout routed feature pages | TBD |
| Pages missing `erp-page` | TBD |
| Pages missing `AppCard` | TBD |
| Pages with raw Bootstrap `card` usage | TBD |
| Pages with `table-responsive` usage | TBD |

## Tracking Notes

- After each implementation phase, update the status table and append a short dated note here.
- Include files/modules migrated, intentional exceptions, verification command results, and any follow-up risks.
- Keep this roadmap focused on UI pattern migration. Create separate documents for unrelated functional work.

### Notes

- 2026-07-09: Roadmap created and Phase 0 baseline recorded.
- 2026-07-09: Phase 1 completed for Customers and Organization. Migrated customer list, group, pricing profile, and dashboard pages plus organization dashboard, branch, administration, department, and company management pages to `erp-page`, `AppCard`, shared header/table wrappers, and overflow-safe cards where overlays are present. `BranchForm` was already migrated and left unchanged. Verification: focused Phase 1 scan passed with no avoidable missing `erp-page`, missing `AppCard`, raw-card, or `table-responsive` hits; `git diff --check` passed for touched pages; focused design scan found only existing tokenized radius/shadow page-local CSS; `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj -p:BaseOutputPath=artifacts/build-phase1/` passed with existing warnings. The normal build output path was locked by running process `AlAfkarERP.Web (21304)`, so alternate output was used for compile verification.
