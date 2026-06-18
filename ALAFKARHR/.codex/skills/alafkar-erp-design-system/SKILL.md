---
name: alafkar-erp-design-system
description: Apply the Alafkar ERP design system to Blazor UI work. Use when Codex is asked to redesign, migrate, or review Alafkar ERP pages, layouts, sidebars, topbars, auth/public/POS/employee views, theme tokens, reusable UI components, RTL/LTR styling, status chips, data grids, forms, dashboards, or hardcoded visual-style cleanup in UI/AlAfkarERP.
---

# Alafkar ERP Design System

Use this skill for UI-only design-system work in the Alafkar ERP Blazor app.

## Required Workflow

1. Read `.codex/skills/alafkar-erp-development-guide/SKILL.md` first and follow its repo rules.
2. Treat `D:/Development/AlAfkar/UI Design/stitch_alafkar_erp_ui_redesign/DESIGN.md` as the design contract when it is available.
3. Treat `UI/AlAfkarERP/AlAfkarERP.Shared/wwwroot/theme.css` and the shared Blazor components as the current implementation pattern.
4. Preserve routes, layouts, permissions, services, DTOs, localization, auth/session behavior, validation, event handlers, paging, sorting, search, export, modals, and toasts unless the user explicitly asks to change them.
5. Put reusable styling in `theme.css` or existing component CSS. Avoid page-local duplication, hardcoded colors, hardcoded spacing, hardcoded radii, hardcoded shadows, and broad rewrites.
6. Use logical CSS properties for RTL/LTR support. Avoid `left`, `right`, `margin-left`, `padding-right`, and similar physical-direction styling unless a browser/API constraint requires it.

## What To Load

- Read `references/design-contract.md` before adding or changing tokens, colors, typography, spacing, radii, surfaces, buttons, inputs, grids, sidebar behavior, company switcher behavior, status chips, or RTL/LTR rules.
- Read `references/blazor-usage.md` before migrating Blazor pages, layouts, shared components, auth/public/POS/employee surfaces, or cleaning hardcoded UI debt.

## Implementation Defaults

- Centralize design work in `theme.css`; keep compatibility aliases such as `--erp-*`, Bootstrap button variables, and existing class names working.
- Prefer existing components: `PageHeader`, `AppCard`, `FilterBar`, `StatsCard`, `EmptyState`, `TableExportActions`, `StatusChip`, `CompanySwitcher`, loaders, modals, autocomplete, and paged tables.
- Use Bootstrap Icons for icons and the app's existing Bootstrap patterns.
- Keep dynamic inline styles only for true data-driven values, preferably as CSS custom properties such as progress width variables.
- When editing localized UI that uses `SharedDataService`, preserve or add the `OnChange1` subscribe/dispose pattern from the development guide.

## Verification

For UI implementation work, run focused static checks for hardcoded visual debt, then run `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj`. Do not run the app or perform browser visual QA unless the user explicitly says `Run and verify visually`.
