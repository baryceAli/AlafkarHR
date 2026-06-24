# Alafkar ERP Blazor Usage Guide

Use this reference when applying the design system to Blazor components, pages, and layouts.

## Core Files And Patterns

- Central stylesheet: `UI/AlAfkarERP/AlAfkarERP.Shared/wwwroot/theme.css`.
- Layout shell: `AdminLayout`, `DesktopWorkspaceNav`, topbar components, `AuthNewLayout`, `LandingPageLayout`, and `ViewEmpLayout`.
- Reusable UI lives under `UI/AlAfkarERP/AlAfkarERP.Shared/Pages/Reuable` and `Pages/Reuable2`.
- Current design helpers include `erp-page`, `PageHeader`, `AppCard`, `FilterBar`, stats cards, `EmptyState`, `erp-loading`, `erp-table-toolbar`, `erp-table-wrap`, `erp-table`, modals, autocomplete, export actions, paged tables, `StatusChip`, and `CompanySwitcher`.
- Keep page routes, `@layout`, injected services, DTO bindings, `EditForm` validation, event handlers, permissions, modals, toasts, search, paging, sorting, and export behavior intact.

## Migration Workflow

1. Inspect the target page/component and the nearest existing redesigned pattern.
2. Move repeated visual values into `theme.css`; use page-local CSS only for truly page-specific layout.
3. Replace old page-specific structural wrappers such as `sessions-header`, `filter-panel`, `sessions-panel`, `requests-panel`, `shift-panel`, raw `card-body table-responsive`, and `panel-title` with shared components and tokenized classes when the shared pattern fits.
4. Use `PageHeader` for page titles/actions, `FilterBar` for filters/actions, `AppCard` for panels, `EmptyState` for no-data states, `erp-loading` for loading states, and `erp-table-toolbar` plus `erp-table-wrap` plus `erp-table` for table surfaces.
5. Replace hardcoded colors, `rgba(...)`, shadows, radii, spacing, and physical left/right CSS with theme tokens, shared classes, and logical RTL/LTR properties.
6. Replace workflow/status badge markup with `StatusChip`. Choose tones through explicit semantic helpers such as `StatusTone(...)`, `RequestTone(...)`, `SessionTone(...)`, or a similarly named page-local helper, using tones `primary`, `success`, `warning`, `danger`, `info`, or `muted`.
7. Keep lightweight non-status chips only for compact value metadata such as configured days, salary-like values, dates, counts, or short labels that are not workflow states.
8. Keep dynamic inline styles only when values come from data, such as progress width or uploaded image visibility. Prefer CSS variables like `--erp-progress-value`.
9. Check localized pages for `SharedDataService.OnChange1` subscription/disposal when rendered text or direction depends on language.

## Page Patterns

- Dashboard pages: use `erp-page`, `PageHeader`, stats cards, responsive grids, `StatusChip`, tokenized cards, and meaningful loading/empty states. Do not add new dashboard APIs when existing data exists.
- List/grid pages: use `erp-page`, `PageHeader`, `FilterBar` or `erp-table-toolbar`, action/export group, `.erp-table-wrap`, dense `.erp-table`, `StatusChip`, empty/loading/error states, and existing pagination.
- Form/workflow pages: use `erp-page`, `PageHeader`, tokenized `AppCard` sections, persistent labels, existing `EditForm`/validators, save/cancel action area, loading state, and toast feedback.
- Auth/public pages: use `erp-auth-layout`, auth surfaces, tokenized inputs/actions, and existing auth flow logic.
- POS pages: use POS shell/surface helpers and preserve cart, product, payment, and order event behavior.
- Employee public/QR views: use employee public page/card helpers, preserve public route and protected route behavior, and keep QR/image visibility data-driven.
- Kanban/task pages: use tokenized kanban page, panel, task-card, progress, and status chip helpers while preserving drag/drop and workflow logic.

## CSS And Component Rules

- Prefer existing classes such as `erp-page`, `erp-card`, `erp-card-header`, `erp-card-body`, `erp-loading`, `erp-table-toolbar`, `erp-table-wrap`, `erp-table`, `erp-status-chip`, `erp-progress`, `erp-search-control`, `erp-hidden-input`, `erp-loader-overlay`, `erp-modal-layer`, and feature helpers already in `theme.css`.
- Use Bootstrap button variants after they have been tokenized by `theme.css`: `btn-primary`, `btn-outline-secondary`, `btn-secondary`, `btn-sm`, and icon button helpers.
- Use Bootstrap Icons with `bi bi-*`; do not add a new icon library.
- Avoid cards inside cards. Use cards for repeated items, modals, framed tools, and content panels; use unframed or full-width layouts for sections.
- Keep page-local CSS only for truly page-specific layouts such as normalization rows, schedule grids, request/action panels, and similarly unique layout mechanics. Use theme tokens, shared classes, and logical properties for reusable spacing, borders, alignment, and positioning.
- Do not copy the HTML references blindly. Translate their structure into reusable Blazor markup and shared classes.

## Static Checks

For design-system implementation work, run:

```powershell
git diff --check
rg -n '#[0-9a-fA-F]{3,6}|rgba\(|box-shadow:|border-radius:|border-left:|border-right:|left:|right:' UI/AlAfkarERP/AlAfkarERP.Shared -g '*.razor' -g '*.css' -g '!**/Layout/Old/**'
rg -n 'style=' UI/AlAfkarERP/AlAfkarERP.Shared -g '*.razor' -g '!**/Layout/Old/**'
dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj
```

Treat static scan hits as debt to inspect, not automatic failures. Allow token definitions in `theme.css` and data-driven inline CSS custom properties. Do not open the browser unless the user explicitly requests visual verification.
