# Codex Project Instructions

Before any development task in this repository, read and follow:

`.codex/skills/alafkar-erp-development-guide/SKILL.md`

This applies to creating or editing modules, backend endpoints, EF Core data access, Blazor pages, layout/sidebar/topbar, permissions, reports, integrations, localization, theme/UI design, and existing ERP functionality.

Before UI, theme, layout, or visual design work, also read and follow:

`.codex/skills/alafkar-erp-design-system/SKILL.md`

This applies to Blazor pages, layouts, sidebar/topbar, auth/public/POS/employee views, reusable components, theme tokens, RTL/LTR behavior, status chips, tables, forms, dashboards, and visual-style cleanup.

Before workspace navigation, sidebar, topbar navigation, menu metadata, workspace trail, functional group, or journey group work, also read and follow:

`.codex/skills/alafkar-erp-workspace-navigation/SKILL.md`

Keep work focused: inspect only task-relevant files, reuse existing project patterns, avoid unrelated refactoring, and preserve current business logic, routes, DTOs, permissions, services, APIs, and localization unless the user explicitly asks to change them.

## Alafkar Public Website

Use **Alafkar Public Website** as the permanent internal development name for the public website, with **PublicWebsite** as its technical label. Gold and black describes its current theme, not its identity; the label remains valid if the theme changes.

Requests using this label, such as "Change the Alafkar Public Website header", are scoped to:

- Website components: `UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/`
- Website styles and assets: `UI/AlAfkarERP/AlAfkarERP.Web/wwwroot/website/`
- Website routes: those identified by `WebsiteRoutes.IsPublic`.

Keep these requests within the public website scope. Preserve ERP pages, layouts, themes, login, and the separate `/store` storefront unless the user explicitly includes them. Any necessary shared-file change must preserve ERP behavior and styling; do not apply public website styling globally to ERP surfaces.

This label is for development instructions only. Establishing it does not change visible branding, routes, or the current theme.

When asked to compare any project functionality, workflow, module, or design against ERPNext, use the official ERPNext documentation at `https://docs.frappe.io/erpnext/` and relevant child links as the source of truth for ERPNext behavior, terminology, and feature details.

When asked to compare any project functionality, workflow, module, or design against Odoo, use the official Odoo 19.0 documentation at `https://www.odoo.com/documentation/19.0/` and relevant child links as the source of truth for Odoo behavior, terminology, and feature details.
