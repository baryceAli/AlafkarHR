# AGENTS.md

## Project Overview

ALAFKARHR is a .NET ERP/HR system. The backend is a modular ASP.NET Core API, and the UI is a Blazor application shared between a server-hosted web app and a .NET MAUI Blazor app.

Observed business domains:

- Auth: users, roles, permissions, JWT login, refresh tokens, OTP/password flows.
- Organization: companies, branches, administrations, departments.
- Employee/HR: employees, positions, academic institutions, specializations, employee transfers/termination.
- Catalog: products, SKUs, brands, categories, units, variants, packages.
- Pricing: price lists, price list items, resolved pricing.
- Inventory: warehouses, batches, stock movements, reservations/releases/adjustments/transfers.
- SalesOrder: order lifecycle, lines, pricing integration.
- Customers and Suppliers: customer/supplier master data, groups, contacts, addresses, pricing profiles.
- Payroll, payroll engine, attendance, leave, and performance management exist as modules, but some are less wired into the API than the core modules above.

High-level architecture:

- `src/Bootstraper/Api` is the API composition root. It references modules, registers Carter endpoint assemblies, registers MediatR handlers/validators, adds module services, and maps module middleware.
- `src/Modules/*` contains backend modules. Most active modules own their EF Core `DbContext`, domain models, feature handlers, Carter endpoints, migrations, and seeders.
- `src/Shared/Shared.Contracts` contains CQRS abstractions based on MediatR.
- `src/Shared/Shared` contains shared backend infrastructure: Carter/MediatR registration helpers, EF migration/seeding helper, DDD base types, pipeline behaviors, exceptions, pagination, and image saving.
- `src/Shared/SharedWithUI` contains DTOs/enums shared by backend and UI.
- `UI/AlAfkarERP/AlAfkarERP.Shared` contains reusable Razor UI, layouts, API clients, auth utilities, and UI DTO wrappers.
- `UI/AlAfkarERP/AlAfkarERP.Web` hosts the shared UI as an interactive server Blazor app.
- `UI/AlAfkarERP/AlAfkarERP` hosts the shared UI in .NET MAUI Blazor.

## Technology Stack

- Language/runtime: C# on `net10.0`.
- Backend: ASP.NET Core minimal APIs with Carter.
- Messaging/CQRS: MediatR via custom `ICommand`, `IQuery`, `ICommandHandler`, and `IQueryHandler` interfaces.
- Validation: FluentValidation, registered from module assemblies and run through a MediatR validation behavior for commands.
- Data: Entity Framework Core with module-specific `DbContext` classes and migrations.
- Database provider in active module registration: SQL Server via `UseSqlServer`. Npgsql/PostgreSQL packages and Docker Compose PostgreSQL configuration also exist, but SQL Server is the active provider in module code.
- Auth: ASP.NET Core Identity Core with roles, JWT bearer authentication, custom permission policies.
- Mapping: Mapster, commonly via `Adapt<T>()` in endpoints/handlers.
- Frontend: Blazor components/Razor class library, Blazor Server host, .NET MAUI Blazor host.
- UI HTTP: typed `HttpClient` services plus `AuthMessageHandler` for bearer tokens and refresh.
- Containers: Docker Compose defines a PostgreSQL `alafkar_hr_db` service; API Docker service is present only as commented configuration.
- Tools: local `dotnet-ef` tool manifests exist under API/UI projects.

## Solution Structure

- `ALAFKARHR.slnx`: solution file. Use this for solution-level restore/build.
- `src/Bootstraper/Api`: ASP.NET Core API composition root and static files under `wwwroot`.
- `src/Modules/Auth/Auth`: Identity, JWT, permissions, auth endpoints, `AuthDbContext`.
- `src/Modules/Auth/Auth.Contracts`: auth query/command contracts used by other modules.
- `src/Modules/Catalog/Catalog`: catalog models, features, endpoints, `CatalogDbContext`, migrations, seeding.
- `src/Modules/Catalog/Catalog.Contracts`: catalog cross-module query contracts.
- `src/Modules/Customers/CustomersModule`: customers, customer groups, contacts/addresses, pricing profiles, `CustomerDbContext`.
- `src/Modules/Customers/Customers.Contracts`: customer cross-module query contracts.
- `src/Modules/Employee/EmployeeModule`: employee HR features and `EmployeeDbContext`; references `Auth.Contracts`.
- `src/Modules/GeneralSettings/GeneralSettings`: currencies/settings and `GeneralSettingsDbContext`.
- `src/Modules/Inventory/Inventory`: warehouse, batch, stock, inventory operations, `InventoryDbContext`; references catalog contracts.
- `src/Modules/Organization/Organization`: company hierarchy and `OrganizationDbContext`; references auth contracts.
- `src/Modules/Pricing/Pricing`: pricing features, `PricingDbContext`, `IPriceResolver`; references catalog and customer contracts.
- `src/Modules/Pricing/Pricing.Contracts`: `ResolvePriceQuery` and related pricing contracts used by SalesOrder.
- `src/Modules/SalesOrder/SalesOrder`: sales order lifecycle, `SalesOrderDbContext`; references pricing contracts and sends pricing queries.
- `src/Modules/SuppliersModule/SuppliersModule`: supplier master data and `SupplierDbContext`.
- `src/Modules/Attendance`, `Leave`, `PayrollEngine`, `PayrollModule`, `PerformanceManagement`: domain/module projects present in the solution. They have models/data folders, but are not all registered in `src/Bootstraper/Api/Program.cs`.
- `src/Shared/Shared.Contracts`: shared CQRS interfaces.
- `src/Shared/Shared`: shared backend infrastructure.
- `src/Shared/SharedWithUI`: shared DTOs/enums grouped by domain (`Catalog`, `Inventory`, `SalesOrder`, `Employees`, `Organization`, etc.).
- `UI/AlAfkarERP/AlAfkarERP.Shared`: Razor components, feature pages, layouts, UI services, auth state/token utilities, common components.
- `UI/AlAfkarERP/AlAfkarERP.Web`: Blazor Server host for the shared UI.
- `UI/AlAfkarERP/AlAfkarERP`: .NET MAUI Blazor host.

## Architecture Rules

- Keep module data isolated. Each backend module owns its `DbContext`, EF configurations, migrations, seed data, models, features, and endpoints.
- Add active API modules through `src/Bootstraper/Api/Program.cs`: add the module assembly to Carter and MediatR registration, call `AddXModule(configuration)`, then call `UseXModule(environment)`.
- Use Carter endpoint classes implementing `ICarterModule`; endpoints live beside their feature handler folders.
- Use MediatR `ISender` from endpoints. Endpoints should adapt request DTOs to commands/queries and send them rather than using `DbContext` directly.
- Use `ICommand<T>` for writes and `IQuery<T>` for reads. Handlers implement `ICommandHandler<,>` or `IQueryHandler<,>`.
- Put command validators near the command handler. Validation runs only for commands because `ValidationBehavior<TRequest,TResponse>` is constrained to `ICommand<TResponse>`.
- Keep cross-module calls contract-based. Examples: SalesOrder references `Pricing.Contracts` and sends `ResolvePriceQuery`; Inventory references `Catalog.Contracts`; Pricing references `Catalog.Contracts` and `Customers.Contracts`.
- Do not reference another module's concrete project just to read its database or models. Add or use a contracts project when cross-module communication is needed.
- Use shared DTOs/enums from `SharedWithUI` for payloads consumed by both backend and UI.
- Use backend shared infrastructure from `Shared`, not from UI projects.
- Domain models generally derive from `Shared.DDD.Entity<T>` or `Aggregate<T>` and include audit/soft-delete properties.
- Most mutations read the authenticated user id from `ClaimTypes.NameIdentifier` via `IHttpContextAccessor`; preserve this audit pattern.
- EF migrations and seeders are module-specific. Development startup calls `UseMigration<TContext>(moduleName)` for registered modules.
- `UseMigration<TContext>` currently seeds every time and has migration code guarded by an internal `isFirstModule` flag that starts as `true` per call. Be careful changing this because it affects startup behavior for all modules.

## Backend Feature Pattern

Typical feature folder shape:

- `CreateXHandler.cs`: command/result records, optional FluentValidation validator, handler implementation.
- `CreateXEndpoint.cs` or `CreateXEndPoint.cs`: Carter route, request/response records, `ISender.Send`.
- Query features mirror this with `GetXQuery`, `GetXResult`, and `GetXEndpoint`.

Endpoint conventions observed:

- API routes usually start with `/api/v1/...`; the UI uses `api/{ApiConfig.Version}/...`.
- Endpoints commonly call `.WithName`, `.Produces`, `.ProducesProblem`, `.WithSummary`, `.WithDescription`.
- Protected endpoints call `.RequireAuthorization(...)` with permission strings from `PermissionList`.
- Request records wrap shared DTOs, e.g. `CreateProductRequest(CreateProductDto Product)`.
- Responses often return `CreateResponseDto`, `UpdateDeleteResponseDto`, or feature-specific result records.

Handler conventions observed:

- Handlers use primary-constructor dependency injection.
- Queries use `AsNoTracking()` where appropriate in many places; follow local style in the module.
- Pagination uses `PaginationRequest` and `PaginatedResult<T>`.
- Not-found cases throw `NotFoundException`; bad input can throw `BadRequestException`; FluentValidation errors are converted to problem details by `CustomExceptionHandler`.
- Write handlers call aggregate/model methods such as `Create`, `Update`, `Delete`, `AddLine`, `ApplyResolvedPriceList` instead of setting all properties from endpoints.

## Frontend/Blazor Rules

- Put reusable UI and feature pages in `UI/AlAfkarERP/AlAfkarERP.Shared`.
- The web host registers all UI services and typed HTTP clients in `UI/AlAfkarERP/AlAfkarERP.Web/Program.cs`.
- UI API service classes usually inherit `BaseApiService`, build `HttpRequestMessage`s, and return `ApiResult<T>`.
- Add a matching interface and service implementation under the feature service folder when adding UI API calls.
- Use `AuthMessageHandler` on authenticated HTTP clients so bearer tokens and refresh-token retry behavior are preserved.
- `CustomAuthStateProvider` parses JWT claims and adds role claims for Blazor authorization.
- Shared routes are defined in `Routes.razor`; layouts live under `Layout`.
- Reusable UI services/components exist under `Pages/Reuable2` and older components under `Pages/Reuable` / `Layout/Old`. Prefer current patterns near the feature being changed.

## Coding Standards

- Nullable and implicit usings are enabled in most projects.
- Prefer file/folder names already used by the feature. Note that both `Endpoint` and `EndPoint` spellings exist; match the nearby module convention.
- Keep command/query/result/request/response record names explicit: `CreateProductCommand`, `CreateProductResult`, `CreateProductRequest`, `CreateProductResponse`.
- Validators are named `CreateXCommandValidator`, `UpdateXCommandValidator`, or similar and inherit `AbstractValidator<TCommand>`.
- Use DTO suffixes for shared payload objects: `ProductDto`, `WarehouseDto`, `SalesOrderDto`.
- Use `Name` and `NameEng` pairs where the domain already supports Arabic/English naming.
- Put EF configuration classes under `Data/Configurations`.
- Put migrations under the owning module's `Data/Migrations`.
- Put seed data under `Data/Seed` and implement `IDataSeeder<TContext>`.
- Do not add broad shared abstractions unless several existing modules already need them.

## Dependency Boundaries

- `Api` may reference concrete active modules because it is the composition root.
- Modules may reference `Shared`, `Shared.Contracts`, `SharedWithUI`, and specific `*.Contracts` projects.
- Contracts projects should stay lightweight and should not depend on concrete module implementations.
- UI projects should reference `SharedWithUI` and UI shared code, not backend module implementations.
- Cross-module reads/writes should go through MediatR contracts, not direct `DbContext` access across module boundaries.

## Build and Run Commands

Run from the repository root unless noted.

```powershell
dotnet restore ALAFKARHR.slnx
dotnet build ALAFKARHR.slnx
dotnet run --project src/Bootstraper/Api/Api.csproj
dotnet run --project UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj
docker compose up -d
```

Observed validation result: `dotnet build ALAFKARHR.slnx` succeeds, but currently emits many existing warnings, especially nullable and unused-field warnings.

## EF Core Migrations

The repo uses a local `dotnet-ef` tool manifest under `src/Bootstraper/Api`.

Restore tools:

```powershell
dotnet tool restore --tool-manifest src/Bootstraper/Api/dotnet-tools.json
```

Use the API project as startup and target the owning module project/context. Examples:

```powershell
dotnet ef migrations add CatalogMigrationName --project src/Modules/Catalog/Catalog/Catalog.csproj --startup-project src/Bootstraper/Api/Api.csproj --context CatalogDbContext --output-dir Data/Migrations
dotnet ef migrations add PricingMigrationName --project src/Modules/Pricing/Pricing/Pricing.csproj --startup-project src/Bootstraper/Api/Api.csproj --context PricingDbContext --output-dir Data/Migrations
dotnet ef database update --project src/Modules/Catalog/Catalog/Catalog.csproj --startup-project src/Bootstraper/Api/Api.csproj --context CatalogDbContext
```

Existing migration folders are present for Auth, Catalog, Customers, Employee, GeneralSettings, Inventory, Organization, Pricing, SalesOrder, and Suppliers.

## Testing

- No test projects were found in the current solution/tree.
- No CI workflow files were found.
- Before committing backend or UI changes, at minimum run `dotnet build ALAFKARHR.slnx`.
- If adding tests later, place them in clearly named test projects and add them to `ALAFKARHR.slnx`; then update this file with `dotnet test` commands.

## Configuration and Secrets

- `src/Bootstraper/Api/appsettings*.json` contains database, JWT, OTP, and SMTP configuration.
- Some committed config values appear secret-like. Do not copy these values into docs, logs, or new files. Prefer user secrets, environment variables, or deployment secrets for new sensitive settings.
- `docker-compose.yml` and `docker-compose.override.yml` define a local PostgreSQL container named `hrdb`, but active module code currently uses SQL Server unless changed.
- API static image assets live under `src/Bootstraper/Api/wwwroot/Images/...`; avoid deleting existing uploaded/sample assets without explicit direction.

## Important Constraints

- Do not edit generated `bin/` or `obj/` output.
- Do not hand-edit EF migration designer/model snapshot files unless intentionally repairing a migration; prefer creating a new migration.
- Do not bypass Carter/MediatR by adding controller actions for module features unless the architecture is intentionally being changed.
- Do not introduce direct references from one concrete module to another concrete module for business queries; use contracts.
- Do not duplicate DTOs between backend and UI when an existing `SharedWithUI` DTO fits.
- Do not add new secrets to `appsettings*.json`.
- Be careful with `PermissionList`: protected endpoints and UI behavior depend on these strings.
- Be careful with SalesOrder pricing behavior: order creation and line additions resolve prices by sending `ResolvePriceQuery` to the Pricing contract.
- Be careful with Inventory stock operations: reservation, release, in/out, and adjustment features modify quantities and movement history.
- Some folders contain legacy/old code (`zOld`, `Layout/Old`, typoed `Reuable`). Treat them as legacy unless the current feature already uses them.

## Development Workflow

- Keep changes inside the owning module and shared projects needed for the feature.
- Update API registration in `Program.cs` only when adding a new active module assembly.
- Add/update DTOs in `SharedWithUI` when both API and UI need the contract.
- Add/update UI services in `AlAfkarERP.Shared` and register typed clients in `AlAfkarERP.Web/Program.cs` when adding UI API calls.
- Run `dotnet build ALAFKARHR.slnx` before handing off changes.
- There is no repository-level formatter, lint configuration, or CI definition found. Follow existing C# style and local formatting.
- For pull requests, include the affected module(s), build result, migration notes if any, and any configuration changes required.
