---
name: erp-ui-designer
description: Use when creating or editing Alafkar ERP Blazor UI pages, layout, shared components, dashboards, tables, forms, modals, filters, empty/loading states, or page-level styling so the interface stays themeable, RTL/LTR-aware, and credit-efficient.
---

# Alafkar ERP UI Designer

## Purpose

Use this skill whenever Codex creates or edits ERP UI in `UI/AlAfkarERP`. Keep the UI modern, professional, clean, accessible, themeable, responsive, and suitable for Arabic/English business workflows.

Always preserve existing routes, DTOs, services, APIs, permissions, localization behavior, and business logic unless the user explicitly asks to change them.

## Credit-Saving Workflow

1. Read `.codex/skills/alafkar-erp-development-guide/SKILL.md` first.
2. Inspect only the files needed for the current UI task: target page, related layout/shared component, relevant CSS, and one nearby pattern if needed.
3. Before editing, summarize the current UI structure briefly.
4. Prefer small, incremental edits over broad rewrites.
5. Reuse existing components and CSS variables before adding new markup or page-specific CSS.
6. Stop after the requested scope. Do not migrate unrelated pages opportunistically.

## Theme Rules

Use `UI/AlAfkarERP/AlAfkarERP.Shared/wwwroot/theme.css` as the central theme file.

Use CSS variables for:

- colors: `--erp-primary`, `--erp-bg`, `--erp-surface`, `--erp-text`, `--erp-text-muted`, `--erp-border`
- semantic states: `--erp-success`, `--erp-warning`, `--erp-danger`, `--erp-info`
- spacing: `--erp-space-*`
- radius: `--erp-radius-*`
- shadows: `--erp-shadow-*`
- layout: `--erp-sidebar-width`, `--erp-topbar-height`
- focus: `--erp-focus-ring`

Do not hard-code new colors, shadows, radii, or spacing unless a value is temporary and unavoidable. Add a token when the value is reused.

Theme selection is handled by `ThemeSelector.razor` and `wwwroot/theme.js`, persisted in `localStorage`, and applied through `data-color-scheme` and `data-theme-mode` on the document element.

## Reusable Components

Prefer the shared components in `Pages/Reuable2`:

- `PageHeader` for title, subtitle, overline, and actions.
- `AppCard` for grouped content.
- `StatsCard` for metrics.
- `FilterBar` for search/filter/action rows.
- `EmptyState` for no-data states.
- `SectionTitle` for sections inside pages.
- `ThemeSelector` only in layout/topbar/settings areas.

Create new shared UI components only when they remove real duplication or encode a repeated ERP pattern.

## Page Structure

A standard list page should use:

```razor
<div class="erp-page">
    <PageHeader Title="..." Subtitle="..." Overline="...">
        <Actions>
            <button class="btn btn-primary">...</button>
        </Actions>
    </PageHeader>

    <AppCard>
        <FilterBar>...</FilterBar>
        <div class="erp-table-wrap">
            <table class="table erp-table">...</table>
        </div>
    </AppCard>
</div>
```

A standard form page should use:

```razor
<div class="erp-page">
    <PageHeader Title="..." Subtitle="..." />
    <AppCard Title="...">
        <EditForm Model="model" OnValidSubmit="SaveAsync">
            <DataAnnotationsValidator />
            <div class="row g-3">...</div>
            <div class="d-flex justify-content-end gap-2 mt-4">...</div>
        </EditForm>
    </AppCard>
</div>
```

## RTL/LTR Rules

Use `SharedDataService.SelectViewLang(en, ar)` for visible bilingual text.

Respect `SharedDataService.PageDirection` from the layout. Prefer logical CSS properties:

- `margin-inline-start`, `margin-inline-end`
- `padding-inline-start`, `padding-inline-end`
- `border-inline-start`, `border-inline-end`
- `inset-inline-start`, `inset-inline-end`

Avoid new left/right-specific rules. If existing rules use left/right, add RTL overrides only when needed.

## Tables

Tables should have:

- a `FilterBar` or clear search/filter area
- `.erp-table-wrap` around responsive tables
- compact, readable headers
- action buttons grouped at the row end
- loading, empty, and error states
- permission checks preserved exactly
- pagination preserved if already present

Do not replace existing data loading, sorting, paging, or service calls for UI-only work.

## Forms

Forms should use:

- `AppCard` grouping
- Bootstrap grid with `row g-3`
- `EditForm`, `DataAnnotationsValidator`, and `ValidationMessage`
- themed `.form-control`, `.form-select`, `.form-check`
- save/cancel actions at the logical end
- toast/modal behavior already used by the page

Do not change DTO fields, validation contracts, or submit endpoints during visual work.

## Modals

Use the existing `ModalService` and `AppModal`. Keep modal headers, body spacing, footer actions, and error details clear. Do not introduce a second modal system.

## Dashboards

Dashboards should use:

- `PageHeader`
- `StatsCard` metrics
- section titles for grouped areas
- responsive Bootstrap grids
- `EmptyState` for no data
- existing dashboard APIs/services

Avoid fake metrics or new backend calls unless requested.

## CSS Hygiene

Prefer global reusable classes in `theme.css`. Keep page-local CSS small and limited to layout details specific to that page. Avoid inline styles. Avoid duplicated page-specific colors and shadows.

Cards should be calm and work-focused: clear hierarchy, soft shadow, 8-12px radius, enough spacing, no marketing hero treatments for ERP screens.

## Final Response For UI Tasks

Summarize:

- files changed
- reusable components used or created
- how to adjust theme variables later
- manual checks performed
- migration plan or next pages if the work is a sample migration
