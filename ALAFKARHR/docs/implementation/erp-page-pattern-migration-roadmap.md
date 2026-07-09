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
| Phase 2 | Attendance + Fleet + Maintenance | Complete | Replaced repeated raw-card surfaces with shared ERP wrappers. |
| Phase 3 | ProjectManagement + RealEstate + Catering | Complete | Reports, dashboards, and domain panels migrated to shared ERP surfaces. |
| Phase 4 | Inventory + Employees + TaskManagement | Complete | Inventory, Employee admin cleanup, and TaskManagement migrated; special Employee public/protected surfaces documented as exceptions. |
| Phase 5 | DocumentManagement + Auth management + remaining partial-debt pages | Complete | Admin surfaces migrated; wrappers/special surfaces documented as exceptions. |
| Phase 6 | Final sweep, verification, and cleanup | Complete | Final scan, build, and deferred exceptions updated. |
| Phase 7 | Residual exception hardening and scan governance | Complete | Avoidable final table-wrapper debt removed; remaining scan hits are allowlisted below. |

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
| AdminLayout routed feature pages | 233 |
| Pages missing `erp-page` | 7 |
| Pages missing `AppCard` | 16 |
| Pages with raw Bootstrap `card` usage | 0 |
| Pages with `table-responsive` usage | 1 |

Remaining scan hits are intentional exceptions or class-name false positives:

- DocumentManagement route wrapper pages delegate to `DocumentLibraryView`, which owns the shared ERP shell.
- `EmployeeViewProtected` and other protected/public employee surfaces keep their special employee-view route behavior.
- SalesOrder POS and StoreFront POS keep cashier/POS-specific layouts.
- Guided employee workspace, TaskManagement kanban, inventory operational workbenches, and stats-only dashboards keep custom operational layouts where forcing an `AppCard` would reduce usability.

## Phase 7: Residual Exception Hardening and Scan Governance

Goal: keep the completed migration maintainable by separating approved custom surfaces from new page-pattern debt.

- Remove the final avoidable `table-responsive` wrapper where a shared ERP table wrapper fits without changing the POS/cashier shell.
- Keep delegated wrappers, POS shells, protected/public employee views, and custom operational workbenches as documented exceptions.
- Treat future page-pattern scan output as actionable only when a hit is not covered by the exception table below.

### Residual Exception Table

| Category | Files / surfaces | Reason | Expected scan hits |
|---|---|---|---|
| Delegated document shell | DocumentManagement route wrapper pages such as `DocumentList`, `MyDocuments`, `SharedWithMe`, and `SourceDocuments` | The route pages delegate to `DocumentLibraryView`, which owns the shared `erp-page` shell and document list surface. | Missing `erp-page`, missing `AppCard` on wrappers only |
| POS/cashier shell | SalesOrder POS and StoreFront POS | Cashier-first flows intentionally use `pos-page` to preserve dense transactional layout and permissions. | Missing `erp-page` |
| Protected/public employee view | `EmployeeViewProtected` and related public employee surfaces | Special employee route behavior is separate from normal AdminLayout business pages. | Missing `erp-page`, missing `AppCard` |
| Custom operational layout | Guided employee workspace, TaskManagement kanban, inventory workbenches, stock movement/transfer operations, project stats dashboards, attendance request-card flows | These pages use specialized boards, workbenches, or stats layouts where forcing an `AppCard` wrapper would reduce usability. | Missing `AppCard` only |
| Class-name false positives and embedded item cards | Existing metric, status, setup, request, catalog, and workbench item surfaces whose page shell already uses the shared ERP pattern or is covered by a custom-shell exception | Broad regex scans can match names such as `stat-card`, `metric-card`, or cashier item cards; these are not raw Bootstrap page-shell debt. | Raw-card regex review hits |

### Phase 7 Snapshot

| Metric | Count |
|---|---:|
| AdminLayout routed feature pages | 233 |
| Pages missing `erp-page` | 7 |
| Pages missing `AppCard` | 16 |
| Pages with raw Bootstrap `card` usage | 0 |
| Pages with `table-responsive` usage | 0 |
| Broad raw-card/class-name review hits | 30 |
| Unapproved residual page-pattern hits | 0 |

## Tracking Notes

- After each implementation phase, update the status table and append a short dated note here.
- Include files/modules migrated, intentional exceptions, verification command results, and any follow-up risks.
- Keep this roadmap focused on UI pattern migration. Create separate documents for unrelated functional work.

### Notes

- 2026-07-09: Roadmap created and Phase 0 baseline recorded.
- 2026-07-09: Phase 1 completed for Customers and Organization. Migrated customer list, group, pricing profile, and dashboard pages plus organization dashboard, branch, administration, department, and company management pages to `erp-page`, `AppCard`, shared header/table wrappers, and overflow-safe cards where overlays are present. `BranchForm` was already migrated and left unchanged. Verification: focused Phase 1 scan passed with no avoidable missing `erp-page`, missing `AppCard`, raw-card, or `table-responsive` hits; `git diff --check` passed for touched pages; focused design scan found only existing tokenized radius/shadow page-local CSS; `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj -p:BaseOutputPath=artifacts/build-phase1/` passed with existing warnings. The normal build output path was locked by running process `AlAfkarERP.Web (21304)`, so alternate output was used for compile verification.
- 2026-07-09: Phase 2 completed for Attendance, Fleet, and Maintenance. Migrated Attendance request, session, shift, holiday, report, dashboard, and personal attendance surfaces; Fleet dashboard, list/detail/form, assignment, document, expense, service-rule, report, and reusable table surfaces; and Maintenance dashboard, asset, work-order, form/view, and report surfaces to `AppCard`, shared toolbar/table wrappers, `FilterBar`, `EmptyState`, and `StatusChip` where appropriate. `MyAttendance` now uses the `erp-page` shell while preserving its terminal workflow layout. Overlay cards containing autocomplete selectors were marked with `allow-overflow`. Verification: focused Phase 2 scan found no source-level raw `erp-card` wrappers or `table-responsive` usage in migrated markup; `git diff --check` passed; design static scans were reviewed and produced broad existing/global/generated CSS hits plus allowed data-driven progress `style=` usage; `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` passed with existing warnings.
- 2026-07-09: Phase 3 completed for ProjectManagement, RealEstate, and Catering. Migrated report filters, forms, domain panels, detail sections, dashboards, and tables to `AppCard`, `erp-table-toolbar`, `erp-table-wrap`, `erp-table`, and `StatusChip` where applicable while preserving routes, services, DTO bindings, event handlers, report filters, workflow actions, localization, and toasts. `ProjectDashboard` was left as a stats-only dashboard because it already uses `PageHeader` and `StatsCard` without raw card/table debt. Verification: focused Phase 3 scan found no avoidable `erp-card`, `table-responsive`, non-ERP table, or Bootstrap status-badge hits; design static scan found only existing tokenized `border-radius: var(--erp-radius-lg)` rules in phase-local CSS; `git diff --check` passed; `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` passed with existing warnings.
- 2026-07-09: Phase 4 partially completed for Inventory and TaskManagement. Migrated inventory stock operation shells to `erp-page` plus overflow-safe `AppCard`, converted inventory barcode/picking/warehouse status pills to `StatusChip`, and normalized TaskManagement list, reports, notifications, dashboard, kanban, KPI, task detail, and my-task status surfaces away from raw Bootstrap cards/badges where practical. Employee public/protected/QR views remain intentional exceptions, and the broader Employee enhancement cleanup is deferred for a dedicated encoding-safe pass to avoid damaging existing localized strings. Verification: `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` passed with existing warnings.
- 2026-07-09: Phase 5 completed for DocumentManagement, Auth management, and remaining admin surfaces. Migrated Auth role list/form/user placeholder surfaces to `erp-page`, `PageHeader`, `AppCard`, `erp-table-wrap`, and `erp-table`; normalized `UserAssignRoles` table and chip styling while preserving role, branch, and StoreFront branch-role workflows. Document route wrappers remain intentional false positives because they delegate to `DocumentLibraryView`, which renders the shared ERP shell; document detail/library/upload policy badges were tokenized. SalesOrder admin pages gained the `erp-page` shell and workflow statuses moved to `StatusChip`; POS/cashier and quotation print surfaces remain exceptions. GeneralSettings, Payroll, Catalog, and StoreFront admin cleanup removed avoidable Bootstrap badges, raw table wrappers, and a dead commented raw-card block. Focused scan now leaves only documented wrapper/special-surface exceptions and class-name false positives; `git diff --check` passed with line-ending warnings only; design static scans produced broad existing/token/generated CSS hits plus allowed data-driven inline style hits; `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` passed with existing warnings.
- 2026-07-09: Phase 6 completed. Closed the deferred Employee admin cleanup by moving academic institution, specialization, position, HR command center, payroll structure, payslip, Saudi payroll, performance, training, reports, work-entry, leave-policy, team, and guided workspace admin surfaces toward the shared `erp-page`, `AppCard`, `erp-table-wrap`, and `erp-table` patterns without changing routes, services, DTO bindings, permissions, localization, modals, toasts, paging, sorting, or workflow handlers. Low-risk Accounting list table wrappers were also normalized. Final scan: 233 AdminLayout routed feature pages, 7 missing `erp-page`, 16 missing `AppCard`, 0 raw Bootstrap card hits, and 1 `table-responsive` hit; remaining hits are documented exceptions or custom operational false positives. Verification: `git diff --check` passed with line-ending warnings only; design static scan produced broad existing/tokenized CSS and generated/global hits; inline-style scan found existing data-driven custom-property usage plus pre-existing narrow width/tree-depth styles; `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` passed with existing warnings.
- 2026-07-09: Phase 7 completed. Removed the final avoidable StoreFront POS `table-responsive` wrappers by converting cashier session and cash-account tables to `erp-table-wrap` and `erp-table` while preserving the `pos-page` cashier shell, permissions, modals, selectors, toasts, and checkout/session behavior. Added residual scan governance for delegated document wrappers, POS shells, protected/public employee views, custom operational layouts, and class-name false positives. Final Phase 7 scan: 233 AdminLayout routed feature pages by the established roadmap snapshot, 7 missing `erp-page`, 16 missing `AppCard`, 0 raw Bootstrap page-shell card hits, 0 `table-responsive` hits, 30 broad raw-card/class-name review hits, and 0 unapproved residual page-pattern hits. Verification: `git diff --check` passed with line-ending warnings only; page-pattern scan confirmed no `table-responsive` hits; design static scan produced broad existing/tokenized CSS and generated/global hits; inline-style scan found existing data-driven custom-property usage; `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` passed with 0 warnings and 0 errors.
