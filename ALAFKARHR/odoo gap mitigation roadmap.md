# Odoo Gap Mitigation Roadmap: Catalog, Purchase, Providers, Sales, Customers

This roadmap tracks the Odoo parity work as GitHub-style feature issues. Use the status line and checkboxes in each issue to pick the next implementation slice and update progress.

## Progress Summary

- [x] Phase 0 - Backend and data-contract foundation
- [x] Phase 1 - Partner and catalog UI enablement
- [x] Phase 2 - Purchase workflow controls
- [x] Phase 3 - Sales quotation and invoicing polish
- [x] Phase 4 - Templates, blanket orders, and tender foundation
- [x] Phase 5 - Smart buttons and cross-navigation
- [x] Phase 6 - Automation, expiry jobs, and document outputs
- [x] Phase 7 - Reports and dashboards
- [x] Phase 8 - Warehouse configuration parity
- [x] Phase 9 - Operation types, locations, and route rules
- [x] Phase 10 - Multi-step receipts and deliveries
- [x] Phase 11 - Replenishment completion
- [x] Phase 12 - Scrap and exception flows
- [x] Phase 13 - Warehouse execution polish

## Issue 1: Phase 0 - Backend And Data-Contract Foundation

**Status:** Implemented  
**Branch:** `codex-odoo-gap-mitigation`

### Goal

Add the persistent model and shared contract foundation required for Odoo-style partner, catalog, procurement, and sales enhancements.

### Delivered

- [x] Added partner address/contact type enums and smart-link summary DTOs.
- [x] Added product commercial metadata, costing policy, image/gallery, and SKU expiration fields.
- [x] Added customer/supplier fiscal, account, currency, and payment default fields.
- [x] Added procurement bill control, 3-way match, RFQ send trace, template, blanket, and tender references.
- [x] Added sales quotation/order template, signature, payment, pro-forma, down-payment, optional line, and delivery/invoice address fields.
- [x] Generated EF migrations for Catalog, Customers, Suppliers, Procurement, and SalesOrder.

### Acceptance Criteria

- [x] API build succeeds.
- [x] Blazor web build succeeds.
- [x] All new fields are persisted through EF migrations.
- [x] Existing create/update flows remain backward compatible.

## Issue 2: Phase 1 - Partner And Catalog UI Enablement

**Status:** Implemented
**Branch:** `codex-odoo-phase-1-ui-enablement`

### Goal

Expose the Phase 0 fields in the ERP UI so users can maintain richer Odoo-style master data.

### Implementation Changes

- [x] Update Customer create/edit/detail screens with typed addresses, typed contacts, fiscal position, account placeholders, currency placeholder, payment reference, and smart-link count display.
- [x] Update Supplier create/edit/detail screens with typed addresses, typed contacts, fiscal position, payable/expense account placeholders, currency placeholder, payment reference, and smart-link count display.
- [x] Update Product and SKU screens with sales/purchase descriptions, tax rates, income/expense account hooks, costing policy, product image URL, SKU expiration settings, and gallery URLs.
- [x] Use existing routes, services, permissions, `AdminLayout`, `PageHeader`, `AppCard`, `FilterBar`, bilingual labels, and design-system classes.

### Acceptance Criteria

- [x] User can create and edit typed customer/supplier addresses and contacts.
- [x] User can create and edit product commercial metadata.
- [x] User can create and edit SKU expiration/gallery metadata.
- [x] Detail/list pages show meaningful new metadata without visual clutter.
- [x] `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` succeeds.
- [x] `dotnet build src/Bootstraper/Api/Api.csproj` succeeds.

## Issue 3: Phase 2 - Purchase Workflow Controls

**Status:** Implemented
**Branch:** `codex-odoo-phase-2-purchase-controls`

### Goal

Make procurement behave closer to Odoo RFQ and Purchase flows.

### Implementation Changes

- [x] Add RFQ send/print/email action placeholders using existing procurement workflow service patterns.
- [x] Display and update RFQ sent state: `SentAt`, `SentBy`, current document status, and conversion trace.
- [x] Add purchase template picker placeholder to procurement form.
- [x] Add bill control policy UI at document and line level.
- [x] Add 3-way match, received quantity, billed quantity, and bill eligibility display on purchase order/goods receipt/supplier invoice screens.
- [x] Add list/detail chips for billable, partially received, partially billed, and match exception states.
- [x] Keep inventory/accounting posting as integration placeholders unless existing endpoints already support the action safely.

### Acceptance Criteria

- [x] RFQs can be marked and surfaced as sent from the UI.
- [x] PO and supplier invoice screens show bill-control and 3-way match status.
- [x] Users can distinguish billable vs not-yet-billable purchase documents.
- [x] No direct cross-module DbContext coupling is introduced.
- [x] Procurement and web builds succeed.

## Issue 4: Phase 3 - Sales Quotation And Invoicing Polish

**Status:** Implemented
**Branch:** `codex-odoo-phase-3-sales-quotation-polish`

### Goal

Improve sales quotation/order handling toward Odoo quotation workflows.

### Implementation Changes

- [x] Add quotation template picker placeholder to quotation create/edit.
- [x] Add optional product line support in quotation UI.
- [x] Add quote deadline, signature required, online payment required, pro-forma, down-payment amount/percent, invoice address, and delivery address fields.
- [x] Show quotation status chips for draft/sent/accepted/expired/converted with existing workflow permissions.
- [x] Add quote-to-order conversion summary fields on order detail.
- [x] Add ordered-vs-delivered invoice policy display placeholders where current data supports it.

### Acceptance Criteria

- [x] User can maintain quotation commercial controls from the quotation form.
- [x] Optional lines are visually distinct from mandatory quote lines.
- [x] Sales order detail reflects quotation-origin metadata.
- [x] Existing quotation send/convert permissions remain intact.
- [x] SalesOrder and web builds succeed.

## Issue 5: Phase 4 - Templates, Blanket Orders, And Tender Foundation

**Status:** Implemented
**Branch:** `codex-odoo-phase-4-templates-tenders`

### Goal

Add first-class workflow entities or lightweight managed records for reusable purchase/sales templates and strategic procurement flows.

### Implementation Changes

- [x] Add purchase template storage and UI if placeholders from Phase 2 are not enough.
- [x] Add quotation template storage and UI if placeholders from Phase 3 are not enough.
- [x] Add blanket order and call-for-tender lifecycle models in Procurement.
- [x] Link RFQs/POs to blanket orders and tenders.
- [x] Add backend handlers/endpoints with normal procurement permissions and company/branch validation.
- [x] Generate EF migrations through `dotnet ef`, never manually.

### Acceptance Criteria

- [x] Users can create and select purchase templates.
- [x] Users can create and select sales quotation templates.
- [x] Procurement can group RFQs/POs under tender or blanket references.
- [x] Ownership and branch/company validation exists on mutations.
- [x] API and web builds succeed.

## Issue 6: Phase 5 - Smart Buttons And Cross-Navigation

**Status:** Implemented
**Branch:** `codex-odoo-phase-5-smart-buttons`

### Goal

Add Odoo-style smart buttons and drill-down navigation across partners, products, purchase, sales, deliveries, invoices, and receipts.

### Implementation Changes

- [x] Add backend summary queries for customer and supplier smart-link counts where static DTO fields are not enough.
- [x] Add smart buttons on customer detail for quotations, sales orders, deliveries, invoices, POS/orders, and ledger placeholder.
- [x] Add smart buttons on supplier detail for RFQs, purchase orders, receipts, supplier bills, scorecards, and supplier items.
- [x] Add product detail links to SKUs, vendor pricelists, reordering rules, sales lines, purchase lines, and inventory placeholders where available.
- [x] Preserve permission-gated visibility and use existing route patterns.

### Acceptance Criteria

- [x] Smart buttons show accurate counts or gracefully hide unavailable counts.
- [x] Clicking a smart button navigates to the filtered target page.
- [x] Users without permissions do not see restricted actions.
- [x] No backend authorization is weakened.
- [x] API and web builds succeed.

## Issue 7: Phase 6 - Automation, Expiry Jobs, And Document Outputs

**Status:** Implemented
**Branch:** `codex-odoo-phase-6-automation-documents`

### Goal

Add operational automation and document output placeholders that align with Odoo workflows.

### Implementation Changes

- [x] Add quote expiry job/handler to mark overdue quotations expired.
- [x] Add RFQ/quote PDF export placeholder or integrate with existing export/document patterns.
- [x] Add email/send action placeholders for RFQ and quotation workflows.
- [x] Add receipt-driven vendor bill eligibility computation.
- [x] Add scheduled or command-triggered recomputation for bill-control and match statuses.
- [x] Keep email/PDF/accounting posting behind existing infrastructure; do not invent a new document engine if one already exists.

### Acceptance Criteria

- [x] Expired quotations transition predictably.
- [x] RFQ and quotation screens expose print/send affordances.
- [x] Purchase documents recompute bill eligibility consistently.
- [x] Automation can be tested without running the full app visually.
- [x] Relevant backend and web builds succeed.

## Issue 8: Phase 7 - Reports And Dashboards

**Status:** Implemented
**Branch:** `codex-odoo-phase-7-reports-dashboards`

### Goal

Surface the new Odoo-style controls in dashboards and operational reports.

### Implementation Changes

- [x] Add procurement dashboard widgets for RFQs sent, POs awaiting receipt, billable receipts, 3-way match exceptions, and tender/blanket summaries.
- [x] Add sales dashboard widgets for quotations expiring soon, optional product adoption, down payments, pro-forma quotes, and conversion status.
- [x] Add customer/supplier summary panels for commercial/accounting defaults and recent document activity.
- [x] Use existing dashboard/report page patterns and avoid new APIs when existing service data is sufficient.

### Acceptance Criteria

- [x] Dashboards show actionable counts, not just decorative cards.
- [x] Reports respect company/branch scope and permissions.
- [x] Empty/loading/error states are present.
- [x] API and web builds succeed.

## Issue 9: Phase 8 - Warehouse Configuration Parity

**Status:** Implemented
**Branch:** `codex-odoo-phase-7-reports-dashboards` (branch creation for `codex-odoo-phase-8-warehouse-config` was blocked by local `.git` permissions)

### Goal

Bring warehouse setup closer to Odoo-style warehouse configuration while preserving the current company, branch, and warehouse behavior.

### Implementation Changes

- [x] Add warehouse short code support for document-friendly warehouse references.
- [x] Add inbound and outbound flow settings for one-step, two-step, and three-step warehouse operations.
- [x] Add resupply-from warehouse links for inter-warehouse replenishment planning.
- [x] Add default source, destination, quality, packing, output, and transit operation locations where the flow requires them.
- [x] Keep existing `CompanyId`, `BranchId`, `WarehouseType`, branch access checks, and warehouse list/form routes intact.

### Acceptance Criteria

- [x] Users can maintain warehouse short codes and inbound/outbound flow settings.
- [x] Warehouse configuration validates company and branch ownership for all selected warehouses and locations.
- [x] Existing warehouse create/edit/list behavior remains backward compatible.
- [x] Warehouse UI displays the new settings without hiding current branch scope information.
- [x] Inventory and web builds succeed.

## Issue 10: Phase 9 - Operation Types, Locations, And Route Rules

**Status:** Implemented
**Branch:** `codex-odoo-phase-7-reports-dashboards` (branch creation for `codex-odoo-phase-9-inventory-routes` was blocked by local `.git` permissions)

### Goal

Add the operational foundation needed for Odoo-style inventory routes, push/pull rules, virtual locations, and warehouse task types.

### Implementation Changes

- [x] Add inventory operation types for receipt, internal transfer, delivery, return, scrap, pick, pack, and quality control.
- [x] Extend warehouse locations to distinguish internal, vendor, customer, transit, production placeholder, and virtual scrap locations.
- [x] Add route and rule models for simple push/pull automation between warehouse locations.
- [x] Allow rules to target products, SKUs, product categories, warehouses, and operation types where existing catalog contracts support it.
- [x] Expose route/rule management through inventory controls using existing permissions, localization, and branch-aware warehouse selectors.

### Acceptance Criteria

- [x] Users can define operation types and route rules from the inventory controls area.
- [x] Push rules can propose internal moves after stock arrives at a configured source location.
- [x] Pull rules can propose demand-driven moves for delivery and replenishment scenarios.
- [x] Virtual scrap and transit locations are excluded from normal physical stock counts unless explicitly requested.
- [x] API and web builds succeed.

## Issue 11: Phase 10 - Multi-Step Receipts And Deliveries

**Status:** Implemented
**Branch:** `codex-odoo-phase-7-reports-dashboards` (branch creation for `codex/odoo-phase-10-multistep-operations` was blocked by local `.git` permissions)

### Goal

Use warehouse flow settings to turn purchase receipts and sales deliveries into one-step, two-step, or three-step inventory operation chains.

### Implementation Changes

- [x] Keep Procurement `GoodsReceipt` and Sales `SalesDeliveryNote` as the source commercial documents.
- [x] Generate receipt, quality, putaway, pick, pack, and ship operations according to warehouse inbound/outbound flow settings.
- [x] Post stock only at the correct stock-affecting step while preserving source document and source line traceability on movements.
- [x] Support partial processing and backorder placeholders when received or delivered quantities are lower than the requested quantity.
- [x] Surface operation status, next action, and source document links on inventory, procurement, and sales screens.

### Acceptance Criteria

- [x] One-step receipt and delivery preserve current behavior.
- [x] Two-step and three-step flows create the expected operation chain and movement history.
- [x] Partial receipt or delivery records remaining quantities without losing source document traceability.
- [x] Branch access is enforced for every operation read, filter, and mutation path.
- [x] Inventory, Procurement, SalesOrder, API, and web builds succeed.

## Issue 12: Phase 11 - Replenishment Completion

**Status:** Implemented
**Branch:** `codex-odoo-phase-7-reports-dashboards` (branch creation for `codex/odoo-phase-11-replenishment` was blocked by local `.git` permissions)

### Goal

Complete Odoo-style replenishment behavior by connecting forecasted stock, reordering rules, manual review, and automatic procurement creation.

### Implementation Changes

- [x] Extend existing Procurement reordering rules with min quantity, max quantity, multiple rounding, trigger mode, preferred supplier, and lead-time metadata.
- [x] Use inventory projected stock and open operation quantities to calculate replenishment suggestions.
- [x] Add manual replenishment review actions that create purchase requests from selected suggestions.
- [x] Add a command-triggered automatic replenishment path for rules marked automatic.
- [x] Link replenishment-created procurement documents back to their rule and demand context for smart navigation.

### Acceptance Criteria

- [x] Manual rules appear in a replenishment report with current, forecasted, minimum, maximum, and suggested quantities.
- [x] Automatic rules can create purchase requests when forecasted stock falls below minimum.
- [x] Suggested quantities respect max quantity, minimum supplier order quantity, and multiple rounding.
- [x] Generated procurement documents preserve company, branch, warehouse, supplier, product, and SKU validation.
- [x] Procurement, Inventory, API, and web builds succeed.

## Issue 13: Phase 12 - Scrap And Exception Flows

**Status:** Implemented
**Branch:** `codex-odoo-phase-7-reports-dashboards` (branch creation for `codex/odoo-phase-12-scrap-orders` and `codex-odoo-phase-12-scrap-orders` was blocked by local `.git` permissions)

### Goal

Add first-class scrap and exception handling so inventory losses and operation exceptions are traceable instead of being hidden as generic adjustments.

### Implementation Changes

- [x] Add scrap order model, endpoints, and UI for scrapping from stock.
- [x] Allow scrap orders to reference an existing receipt, delivery, transfer, or operation.
- [x] Move scrapped quantities to the virtual scrap location and create stock movements with source document traceability.
- [x] Support batch and serial validation for scrapped stock.
- [x] Add optional replenish-quantity behavior that can feed existing replenishment suggestions.

### Acceptance Criteria

- [x] Users can create and validate scrap orders from stock.
- [x] Users can create scrap orders from an existing operation without bypassing operation permissions.
- [x] Scrap movements reduce available physical stock and increase virtual scrap balances.
- [x] Lot, serial, warehouse, location, company, and branch validations are enforced.
- [x] Inventory, API, and web builds succeed.

## Issue 14: Phase 13 - Warehouse Execution Polish

**Status:** Implemented
**Branch:** `codex-odoo-phase-7-reports-dashboards` (branch creation for `codex/odoo-phase-13-warehouse-execution` and `codex-odoo-phase-13-warehouse-execution` was blocked by local `.git` permissions)

### Goal

Polish warehouse execution workflows with barcode application, picking groups, operational dashboards, and practical Odoo-style navigation.

### Implementation Changes

- [x] Enhance Barcode Workbench so complete receipt, delivery, transfer, cycle count, and scrap sessions can apply directly to the matching operation.
- [x] Add lightweight batch or wave picking groups over open pick/delivery operations.
- [x] Add operation smart buttons and drill-down links between warehouses, products, batches, movements, source documents, and replenishment records.
- [x] Extend inventory dashboards with operations to process, replenishment exceptions, scrap totals, route bottlenecks, and backorders.
- [x] Keep carrier integrations, RFID/EPC, and advanced shipping labels as explicit future integrations, not part of this phase.

### Acceptance Criteria

- [x] Barcode sessions can apply validated scans to supported inventory operations.
- [x] Users can group open picking work and process grouped picks without changing sales document ownership.
- [x] Dashboards show actionable warehouse execution counts and exceptions.
- [x] Smart buttons respect existing permissions and branch scope.
- [x] Inventory and web builds succeed.

## Global Test Plan

- [ ] Run focused backend builds for touched modules after each issue.
- [ ] Always run `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` for UI phases.
- [ ] Run `dotnet build src/Bootstraper/Api/Api.csproj` before marking each phase complete.
- [ ] For EF changes, generate migrations using `dotnet ef migrations add ... --startup-project src/Bootstraper/Api/Api.csproj`.
- [ ] Use manual smoke tests for create/edit/detail/list flows in Arabic and English.
- [ ] Use `git diff --check` and inspect edited UI files for hardcoded visual debt.

## Assumptions

- Phase 0 remains the base branch work.
- Phase 8 should be implemented next because Phase 7 completes the partner, catalog, procurement, sales, and dashboard roadmap slices.
- Inventory and warehouse work targets pragmatic Odoo-style parity, not full Odoo cloning.
- Full accounting, marketplace connectors, eCommerce, CRM lead scoring, subscriptions, rentals, shipping carrier integrations, RFID/EPC, manufacturing/subcontracting, full accounting valuation redesign, and full Odoo accounting parity are out of scope unless explicitly requested later.
- Existing routes, permissions, localization, company/branch scope rules, DTOs, and business behavior stay intact unless a roadmap issue explicitly changes them.
