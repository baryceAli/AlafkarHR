---
name: alafkar-erp-development-guide
description: Main project guide for Codex development work on the Alafkar ERP modular .NET/Blazor system. Use before creating or editing modules, backend endpoints, EF Core data access, Blazor pages, layout/sidebar/topbar, permissions, reports, integrations, localization, theme/UI design, or existing ERP functionality.
---

# Alafkar ERP Development Guide

## 1. Purpose

Use this as the main project guide for future Codex work on this ERP. Read it before creating a new module, editing an existing module, creating or editing Blazor pages, changing layout/sidebar/topbar, adding permissions, adding reports, integrating modules, or redesigning UI pages.

Preserve current business logic, routes, DTOs, permissions, services, APIs, and behavior unless the user explicitly asks to change them.

## 2. Credit-Saving Workflow

Read this skill first, then inspect only files directly related to the request. Avoid whole-repo scans unless required to find an unknown pattern. Reuse existing project patterns, keep changes small, avoid unrelated refactors, avoid duplicated CSS/components, and prefer extending local conventions over adding packages.

Before coding, list the expected files/modules to inspect or change. After coding, summarize changed files and manual test steps. Ask before large architectural changes.

For every new or changed business feature, make sure the user-facing UI is updated so the feature can actually be used from the application, unless the user explicitly requests backend-only work. Backend endpoints, DTOs, permissions, services, and migrations are not complete by themselves when the feature requires user interaction. Add or update the relevant Blazor page/component, feature service/interface, menu entry, permission-gated actions, forms/tables/modals, loading/error/toast states, and localization/RTL behavior needed for the user workflow.

Default repository behavior: never run the application, never open the browser, never perform visual validation, and never scan the whole repository. Inspect only files related to the task.

After code changes, verify only with `dotnet build <affected project>`. When a feature includes or expects a UI surface, also run `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj`. Run the application, open a browser, or perform visual validation only when the prompt explicitly says: "Run and verify visually".

## 3. Project Architecture

The solution is a modular monolith. Backend modules live under `src/Modules/<ModuleName>/`; shared backend infrastructure lives under `src/Shared/Shared`; shared contracts and UI DTOs live under `src/Shared/Shared.Contracts` and `src/Shared/SharedWithUI/SharedWithUI`; the Blazor UI lives under `UI/AlAfkarERP`.

Existing modules include Auth, Attendance, Catalog, Customers, Employee, GeneralSettings, Inventory, Leave, Organization, PayrollModule, PerformanceManagement, Pricing, SalesOrder, SuppliersModule, and TaskManagement. Many modules have both implementation and `.Contracts` projects. Put cross-module DTOs/contracts in the relevant contracts/shared project instead of referencing another module internals.

Modules register themselves with `Add<Module>Module(configuration)` and `Use<Module>Module(env)`. `src/Bootstraper/Api/Program.cs` gathers module assemblies for Carter and MediatR and chains module registrations.

## 4. Backend Patterns

Use the existing feature-folder style: `Features/<EntityPlural>/<ActionName>/` with endpoint and handler files, e.g. `CreateCompanyEndpoint.cs` and `CreateCompanyHandler.cs`. Endpoints are Carter `ICarterModule` classes, map routes using module `Utils` constants, call `ISender.Send(...)`, return typed response records, declare `Produces...`, and use `.RequireAuthorization(PermissionList.<Entity>Permissions.<Action>)`.

Commands/queries use shared CQRS contracts such as `ICommand<T>`, `IQuery<T>`, and handlers. Validators use FluentValidation near the handler. Mapping uses Mapster `Adapt<T>()`. Result/request/response shapes are small records.

Entities commonly inherit `Aggregate<Guid>` or shared DDD base types. Use private setters, static `Create(...)`, instance `Update(...)`/`Remove(...)`, audit fields, and soft delete (`IsDeleted`, `DeletedAt`, `DeletedBy`). DbContexts live in each module `Data` folder, set a schema with `HasDefaultSchema("<Module>")`, apply configurations from assembly, and add soft-delete filters where the module already does so.

EF uses SQL Server and per-module migrations under `Data/Migrations`. In development, modules call `UseMigration<TContext>("<Schema>")` and seed through `IDataSeeder<TContext>` where present. Shared MediatR registration adds validation and logging pipeline behaviors; do not bypass them.

Never create, edit, or repair EF Core migration files manually. Always use `dotnet ef migrations add ...` to generate migrations, inspect the generated files, and only make code/model changes that cause EF to generate the correct migration. If a migration is wrong, remove/regenerate it through EF tooling instead of hand-writing migration or designer files.

When running any `dotnet ef` command, always pass the API project as the startup project, e.g. `--startup-project src/Bootstraper/Api/Api.csproj`, so the Docker Compose project is not selected and does not block EF tooling.

## 5. Frontend / Blazor Patterns

Shared Blazor code is in `UI/AlAfkarERP/AlAfkarERP.Shared`. Pages are feature-scoped under `Pages/Features/<Module>/Pages`, services under `Pages/Features/<Module>/Services`, reusable components under `Pages/Reuable` and `Pages/Reuable2`, and layout under `Layout`.

Use `@layout AdminLayout` for admin ERP pages unless the existing feature uses another layout. Inject feature services, `NavigationManager`, `AuthenticationStateProvider`, `SharedDataService`, `ToastService`, `LoadingService`, `ModalService`, or `SearchModalService` only as needed. Use existing `EditForm`, `DataAnnotationsValidator`, `ValidationMessage`, loading booleans, empty/error states, and toast messages.

Use existing reusable helpers: `AppModal`, `ModalService`, `AppToast`, `ToastService`, `AppLoader`, `LoadingService`, `SearchModal`, `SearchModalService`, `AutoCompleteComponent`, and existing paged table/card components where they fit. Do not replace working service calls during UI-only work.

## 6. ERP UI Design System Rules

Use a modern professional SaaS dashboard style: clean spacing, clear hierarchy, soft shadows, rounded cards, responsive layout, accessible contrast, bilingual Arabic/English support, and RTL/LTR-friendly layout.

Prefer CSS variables for theme values. Avoid hard-coded colors, inline styles, duplicated page-specific CSS, and duplicate components. Use Bootstrap utility patterns and Bootstrap icons already used by the app. Preserve existing routes, DTOs, services, APIs, permission constants, localization, and business behavior during UI work.

## 7. Theme System Rules

If a theme system exists, use it. If creating or improving one, put variables in the existing central CSS location or `wwwroot/css/theme.css`. Use CSS variables for primary/secondary colors, background, surface/card color, text, muted text, border, focus ring, danger/success/warning/info, spacing, radius, shadows, typography, sidebar width, and topbar height.

Support blue, emerald, purple, orange, and slate color schemes where practical, plus light/dark mode where practical. Make future color changes possible by editing variables only.

## 8. User-Selectable Theme Rules

If implementing theme selection, add a simple `ThemeSelector` component in the topbar, settings area, or another suitable existing location. Persist the selected theme in `localStorage`, apply it globally with attributes such as `data-theme` or `data-color-scheme`, update without full refresh where practical, and avoid heavy theme libraries unless already present.

## 9. Reusable UI Components

Prefer existing components first. If missing and the task benefits from reuse, create simple components such as `PageHeader`, `AppCard`, `StatsCard`, `FilterBar`, `ActionButtonGroup`, `EmptyState`, `SectionTitle`, or `ThemeSelector`. Components must support Arabic/English, work in RTL/LTR, use theme variables, avoid business logic, and avoid forcing broad page rewrites.

## 10. Layout Rules

Layout/sidebar/topbar files live under `UI/AlAfkarERP/AlAfkarERP.Shared/Layout`. Preserve authentication, authorization, existing navigation routes, menu permission checks, sidebar responsiveness, and RTL/LTR behavior. `Sidebar` and `SidebarItem` filter menu items by `PermissionPolicy` claims; do not weaken this.

## 11. Table/List Page Rules

List pages should use a clear page header, filter/search area, responsive table container, action buttons, status badges, empty state, loading state, pagination where the existing pattern supports it, and permission-based action visibility. Do not change data loading logic unless the requested work requires it.

## 12. Form/Create/Edit Page Rules

Form pages should use a page header, card/section grouping, `EditForm`, validation messages, save/cancel actions, loading state, toast messages, existing localization helpers, and existing service methods. Do not change DTOs or service calls unless required by the task.

## 13. Modal Rules

Use the existing `ModalService`/`AppModal` pattern. Keep header/body/footer spacing consistent, actions clear, RTL/LTR support intact, and modal CSS reusable.

## 14. Dashboard Rules

Dashboard pages should use stats cards, section titles, clear responsive grids, meaningful empty/loading states, and theme variables. Do not invent new dashboard APIs when existing services already expose the data.

## 15. Permissions and Security

Permissions are centralized in `src/Shared/SharedWithUI/SharedWithUI/Permissions/PermissionList.cs`. The naming convention is `Group.Entity.Action`, with common actions `Select`, `View`, `Create`, `Edit`, and `Delete`; some modules add specialized actions such as review or workflow permissions.

Blazor menu visibility uses `MenuItem.PermissionPolicy` and user claims. Backend endpoints use `.RequireAuthorization(PermissionList.<Entity>Permissions.<Action>)`. Role/user pages use grouped permissions from `PermissionList.GetGroupedPermissions(...)`. Preserve all permission checks during redesigns and add new permissions in the same nested static-class style.

## 16. Localization and RTL/LTR

Use `SharedDataService.SelectViewLang(en, ar)` for bilingual text and `SharedDataService.PageDirection` for direction. Data models often use Arabic and English name fields such as `Name` and `NameEng`; preserve those names.

Any Blazor page or component that renders language-dependent text or direction from `SharedDataService` must respond to the language button toggle. Add `@implements IDisposable`, subscribe in `OnInitialized`, rerender in `HandleChangeAsync`, and unsubscribe in `Dispose`:

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

Avoid left/right-specific CSS where possible. Prefer logical CSS properties such as `margin-inline-start`, `margin-inline-end`, `padding-inline-start`, `padding-inline-end`, `inset-inline-start`, and `border-inline-start`. Respect `dir`/`lang` behavior and verify icons, spacing, and action groups work in Arabic and English.

## 17. API and Service Patterns

Backend API routes generally follow `api/{version}/<module>/<entities>` through module route constants. Frontend services implement module service interfaces, inherit `BaseApiService`, inject `HttpClient` and `ApiConfig`, build `_path = $"api/{_apiConfig.Version}/..."`, send `HttpRequestMessage`, use `JsonContent.Create(...)`, and return `ApiResult<T>` from `SendAsync<T>(request, responseNode)`.

The Blazor server app registers services and typed `HttpClient`s in `UI/AlAfkarERP/AlAfkarERP.Web/Program.cs`, using `AuthMessageHandler` for authenticated API calls. Do not change API contracts during UI-only tasks.

## 18. Database Rules

IDs are usually `Guid`. Shared entity base classes provide audit fields and soft-delete fields. Many entities are scoped by company, branch, administration, department, user, or related module IDs; preserve this scoping. Use module DbContexts, schema names, EF configurations, migrations, and seeders already present in the target module.

## 19. Menu and Navigation

Menu structure is in `UI/AlAfkarERP/AlAfkarERP.Shared/Layout/MuenuItem.cs`. Each item has `TextEn`, `TextAr`, `Icon` using Bootstrap icon class names, `Url`, `PermissionPolicy`, optional badges, and children. URLs use module-style paths such as `/Organization/Company/List`. Add navigation only when requested or clearly required.

## 20. Reporting Pattern

Report pages live beside feature pages when present, e.g. TaskManagement reports. Use filter controls, date ranges where relevant, existing services/DTOs, responsive summary/table layouts, export patterns only if already present in the feature, and report-specific permissions. Do not add reporting endpoints without matching backend authorization.

## 21. Module Integration Rules

Avoid tight coupling between modules. Use shared contracts for data crossing module boundaries, integration/domain events where existing module style supports them, and application services or APIs when a feature already exposes them. Attendance, Leave, Payroll, Employee, Organization, Auth, and Reporting work should preserve identity, company/branch/organization scoping, permission checks, and existing workflow state transitions.

## 22. Required Output Format for Future Codex Tasks

For future development tasks, respond with: brief understanding; files/modules to inspect; minimal implementation plan; files changed; UI exposure; manual test checklist; assumptions or risks.

UI exposure: describe where the user can access the feature, including route/menu/action/button/form changes, or state explicitly that the user requested backend-only work.

## 23. UI Migration Strategy

When asked to redesign UI, do not redesign the whole ERP at once. Start with the main layout, sidebar/topbar, one representative list page, one representative form page, and one dashboard page if available. Stop after the sample implementation, provide a migration plan for the rest, and continue page-by-page only when requested.

## 24. Examples

Example page structure: `PageHeader`, `FilterBar`, `AppCard`, responsive table, empty/loading states, and permission-based action buttons.

Example form structure: `PageHeader`, `AppCard`, `EditForm`, grouped fields, validation messages, save/cancel actions, and toast feedback.

Example theme usage: `var(--erp-primary)`, `var(--erp-surface)`, `var(--erp-border)`, `var(--erp-radius-lg)`, and `var(--erp-shadow-sm)`.
