---
name: erp-ui-designer
description: Use when creating or editing Alafkar ERP Blazor UI pages, layouts, authentication screens, shared components, dashboards, tables, forms, modals, filters, reports, empty/loading states, responsive styling, RTL/LTR behavior, or theme styling so the interface stays modern, premium, themeable, accessible, reusable, and credit-efficient without unwanted design regressions.
---

# Alafkar ERP UI Designer

## Purpose

Use this skill whenever Codex creates or edits UI in `UI/AlAfkarERP`. Produce a modern ERP experience that is premium, calm, professional, trustworthy, efficient, readable, responsive, accessible, RTL/LTR-aware, and consistent with the local design system.

Always preserve existing routes, DTOs, services, APIs, permissions, localization behavior, authentication flow, validation contracts, and business logic unless the user explicitly asks to change them.

Think like a senior product designer before implementing like a frontend engineer. Treat strong SaaS products such as Linear, Stripe, Notion, Vercel, Figma, Slack, or Microsoft Fluent 2 as quality inspiration only; the source of truth is the local ERP design system, Razor/component patterns, Bootstrap icons, and theme variables. Clarity first, simplicity first, readability first, productivity first. Never sacrifice usability for decoration.

## Workflow

1. Read `.codex/skills/alafkar-erp-development-guide/SKILL.md` first.
2. Inspect only files required for the task: target page/component, related layout if relevant, scoped CSS/theme files, and one nearby pattern if needed.
3. Before editing, briefly summarize the current UI structure, what should change, and what must stay untouched.
4. Translate user-provided references into existing Blazor, Bootstrap icon, component, and CSS-variable patterns. Do not copy external framework code directly.
5. Prefer small incremental edits over broad rewrites. Stop after the requested scope.
6. After editing, build the affected project with `dotnet build <affected project>`. Run the app or browser visual checks only when the prompt explicitly requests visual verification.

## Design Guardrails

Avoid these failure modes:

- Generic Bootstrap/AdminLTE/legacy ERP appearance.
- External styling dependencies such as Tailwind, CDN fonts, Material Symbols, or new UI libraries unless the project already uses them or the user explicitly asks.
- Hard-coded colors, shadows, radii, and spacing when theme tokens exist.
- Decorative clutter, heavy gradients, excessive glow effects, oversized logos, giant typography, or too many icons.
- Multiple competing focal points.
- Mismatched input widths, heights, radii, padding, or icon placement.
- Layouts that overflow, clip, overlap, or shift between mobile and desktop.
- Designs that look good in one direction but break in RTL or LTR.
- Marketing/branding panels that overpower the actual workflow.

Use this self-check before finishing, without asking the user unless information is genuinely missing:

- Is the page understandable within 3 seconds?
- Is there one clear focal point?
- Is the primary action visually dominant?
- Is visual hierarchy obvious without relying on oversized typography?
- Are form controls consistent and keyboard accessible?
- Is the tab order logical, and are focus states visible through `--erp-focus-ring` or the existing focus pattern?
- Do icon-only buttons, ambiguous icons, and grouped actions have accessible names or visible labels?
- Are loading and disabled states clear without removing keyboard or screen-reader context?
- Are validation messages adjacent to their fields and still visible after submit failures?
- Do tables remain usable in their responsive wrapper without clipped actions or unreadable columns?
- Do text and control colors keep accessible contrast in light, dark, and selected color schemes?
- Is whitespace doing the hierarchy work before typography size increases?
- Does the design remain usable on mobile and desktop?
- Do Arabic and English text, icons, and spacing work with `SharedDataService.PageDirection`?

## Theme Rules

Use `UI/AlAfkarERP/AlAfkarERP.Shared/wwwroot/theme.css` as the central theme file.

Prefer existing variables:

- Colors: `--erp-primary`, `--erp-primary-hover`, `--erp-primary-soft`, `--erp-bg`, `--erp-bg-subtle`, `--erp-surface`, `--erp-surface-muted`, `--erp-text`, `--erp-text-muted`, `--erp-text-soft`, `--erp-border`, `--erp-border-strong`.
- Semantic states: `--erp-success`, `--erp-warning`, `--erp-danger`, `--erp-info`.
- Spacing: `--erp-space-*`.
- Radius: `--erp-radius-*`.
- Shadows: `--erp-shadow-*`, `--erp-auth-shadow`.
- Layout/focus: `--erp-sidebar-width`, `--erp-topbar-height`, `--erp-focus-ring`.

Do not invent duplicate token names such as `--erp-surface-elevated` when the current theme uses `--surface-elevated`. Add a new token only when a reused value deserves central control.

Theme selection is handled by `ThemeSelector.razor` and `wwwroot/theme.js`, persisted in `localStorage`, and applied through `data-color-scheme` and `data-theme-mode`.

## Visual Standards

Prioritize visual hierarchy in this order:

1. Primary action.
2. Main content.
3. Supporting content.
4. Decorative content.

Preferred typography ranges:

- Hero title: `32px-40px`.
- Page title: `24px-32px`.
- Section title: `18px-24px`.
- Body text: `14px-16px`.
- Helper text: `12px-14px`.

Avoid excessive font weights, unnecessary uppercase text, and oversized headings used to compensate for weak layout.

Spacing creates hierarchy. Prefer whitespace before increasing font size, weight, borders, or decoration.

Preferred spacing increments: `8px`, `12px`, `16px`, `24px`, `32px`, `48px`, represented with `--erp-space-*` when available. Avoid arbitrary one-off spacing values unless matching an existing local pattern.

Cards should be calm and work-focused: clear hierarchy, moderate border, soft shadow, 8-12px radius, and enough spacing. Avoid nested cards unless the existing pattern already does so.

Buttons should be visually clear without harsh effects. Primary buttons should be dominant, have clear hover/focus/disabled/loading states, and use `48px-56px` height for forms and authentication pages. Avoid overly saturated colors, harsh shadows, and excessive animations.

## Reference Designs

When the user provides screenshots, HTML, Tailwind, Figma-like snippets, or other design references:

- Extract intent: layout, spacing, hierarchy, proportions, colors, interaction states, and responsive behavior.
- Rebuild with existing project tools: Razor, existing shared components, Bootstrap utilities/icons, scoped CSS, and theme variables.
- Do not import referenced external scripts, fonts, icon sets, Tailwind classes, CDN resources, or unrelated markup.
- Preserve the current component logic and data bindings unless explicitly requested.
- Match important details that affect usability: equal control widths, visible labels, focus states, touch targets, loading/disabled states, and RTL/LTR icon placement.

## Reusable Components

Prefer shared components in `Pages/Reuable2`:

- `PageHeader` for title, subtitle, overline, and actions.
- `AppCard` for grouped content.
- `StatsCard` for metrics.
- `FilterBar` for search/filter/action rows.
- `EmptyState` for no-data states.
- `SectionTitle` for sections inside pages.
- `ThemeSelector` only in layout/topbar/settings areas.

Create new shared components only when they remove real duplication or encode a repeated ERP pattern.

Use the Catalog pages as the canonical admin page header pattern. For routed `AdminLayout` workflow pages (lists, dashboards, forms, reports, and operations), use `PageHeader` with `Overline`, `Icon`, `Title`, optional `Subtitle`, and optional `<Actions>`. The overline is the module or section name above the title; the icon sits beside the title using the existing `erp-page-title-icon` spacing. Move existing top-level actions into `<Actions>` while preserving their handlers, permissions, disabled states, labels, and icons. Do not force this header onto public auth pages, layout components, modal-only content, nested panels, or small helper components.

## Authentication Pages

Authentication screens include login, register, forgot password, reset password, and OTP verification.

Rules:

- The form is the primary focus. Branding and marketing content are secondary.
- Keep form width around `420px-480px`.
- Use generous whitespace, concise headings, one primary CTA, and clear loading/disabled states.
- Prevent large headings from wrapping awkwardly or pushing the form below the first viewport.
- On desktop, prefer balanced splits such as `50/50` or `55/45`; use branding larger than the form only when the form remains visually dominant.
- On mobile, show the form first and hide or simplify branding if needed.
- Preserve authentication flow, redirects, validation messages, password toggles, and service calls.

## Forms

Use:

- `EditForm`, `DataAnnotationsValidator`, `ValidationMessage`.
- Themed `.form-control`, `.form-select`, `.form-check`, or scoped equivalents.
- Visible labels that remain stable during validation.
- Consistent control height, radius, padding, icon position, and full-width behavior within each form group.
- Clear focus, hover, disabled, loading, error, and empty states.
- Save/cancel actions at the logical end.

Preferred control height: `48px-56px`. Prefer `--erp-radius-md` or `--erp-radius-lg` unless matching an established local pattern.

Do not change DTO fields, validation contracts, submit endpoints, or service calls during visual-only work.

## Lists And Tables

List pages should use:

```razor
<div class="erp-page">
    <PageHeader
        Title='@SharedDataService.SelectViewLang("Items", "العناصر")'
        Subtitle='@SharedDataService.SelectViewLang("Manage active records.", "إدارة السجلات النشطة.")'
        Overline='@SharedDataService.SelectViewLang("Catalog", "الكتالوج")'
        Icon="bi-box">
        <Actions>
            <button class="btn btn-primary">
                @SharedDataService.SelectViewLang("Create", "إضافة")
            </button>
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

Tables should have a clear search/filter area, responsive wrapper, compact readable headers, grouped row actions, loading state, empty state, error state, existing pagination, and permission checks preserved exactly.

Do not replace existing loading, sorting, paging, service calls, or API shapes for UI-only work.

## Standard Form Pages

Form pages should use:

```razor
<div class="erp-page">
    <PageHeader
        Title='@SharedDataService.SelectViewLang("Edit Item", "تعديل العنصر")'
        Subtitle='@SharedDataService.SelectViewLang("Update the required details.", "تحديث البيانات المطلوبة.")'
        Overline='@SharedDataService.SelectViewLang("Catalog", "الكتالوج")'
        Icon="bi-pencil-square" />
    <AppCard Title='@SharedDataService.SelectViewLang("Details", "البيانات")'>
        <EditForm Model="model" OnValidSubmit="SaveAsync">
            <DataAnnotationsValidator />
            <div class="row g-3">...</div>
            <div class="d-flex justify-content-end gap-2 mt-4">...</div>
        </EditForm>
    </AppCard>
</div>
```

## Dashboards

Use `PageHeader` with overline and icon, `StatsCard`, responsive grids, `SectionTitle`, loading/empty states, and existing dashboard APIs/services. Avoid fake metrics, decorative widgets, and unnecessary animations.

## Modals

Use the existing `ModalService` and `AppModal`. Keep modal headers, body spacing, footer actions, error details, and focus behavior clear. Do not introduce a second modal framework.

## RTL/LTR Rules

Use `SharedDataService.SelectViewLang(en, ar)` for visible bilingual text.

Respect `SharedDataService.PageDirection`. Prefer logical CSS properties:

- `margin-inline-start`, `margin-inline-end`.
- `padding-inline-start`, `padding-inline-end`.
- `border-inline-start`, `border-inline-end`.
- `inset-inline-start`, `inset-inline-end`.

Avoid new left/right-specific rules. If existing code uses left/right, add narrow RTL/LTR overrides only where required.

Any Blazor page or component that renders language-dependent text or direction from `SharedDataService` must respond to the language button toggle. Add this pattern when missing:

```razor
@implements IDisposable

@code {
    protected override void OnInitialized()
    {
        SharedDataService.OnChange1 += HandleChangeAsync;
    }

    private async Task HandleChangeAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        SharedDataService.OnChange1 -= HandleChangeAsync;
    }
}
```

When editing existing localized pages, check whether this pattern already exists before finishing. If the page already implements `IDisposable`, merge the unsubscribe into the existing `Dispose` method instead of creating a duplicate.

## CSS Hygiene

Prefer global reusable classes in `theme.css` for repeated patterns and scoped CSS for page-specific layout. Avoid inline styles, duplicate colors, duplicate shadows, duplicate spacing systems, and broad selectors that leak into unrelated pages.

Use `box-sizing: border-box`, `min-width: 0`, responsive grid/flex constraints, and logical padding/insets to avoid clipping and unequal widths.

## Final Response For UI Tasks

Extend the main project guide summary with:

- Files changed.
- Components reused or created.
- Design changes and UX/accessibility improvements.
- Theme variables reused or added.
- Validation performed, including the `dotnet build <affected project>` result.
- Manual responsive, RTL, browser, or visual checks only when the prompt explicitly requested them and they were actually performed; otherwise state that they were not run.
- Related next UI improvements only when they follow directly from the task.
