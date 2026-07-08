# Odoo Gap Mitigation Roadmap: Catalog, Purchase, Providers, Sales, Customers

This roadmap tracks the Odoo parity work as GitHub-style feature issues. Use the status line and checkboxes in each issue to pick the next implementation slice and update progress.

## Progress Summary

- [x] Phase 0 - Backend and data-contract foundation
- [ ] Phase 1 - Partner and catalog UI enablement
- [ ] Phase 2 - Purchase workflow controls
- [ ] Phase 3 - Sales quotation and invoicing polish
- [ ] Phase 4 - Templates, blanket orders, and tender foundation
- [ ] Phase 5 - Smart buttons and cross-navigation
- [ ] Phase 6 - Automation, expiry jobs, and document outputs
- [ ] Phase 7 - Reports and dashboards

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

**Status:** Next  
**Suggested branch:** `codex/odoo-phase-1-ui-enablement`

### Goal

Expose the Phase 0 fields in the ERP UI so users can maintain richer Odoo-style master data.

### Implementation Changes

- [ ] Update Customer create/edit/detail screens with typed addresses, typed contacts, fiscal position, account placeholders, currency placeholder, payment reference, and smart-link count display.
- [ ] Update Supplier create/edit/detail screens with typed addresses, typed contacts, fiscal position, payable/expense account placeholders, currency placeholder, payment reference, and smart-link count display.
- [ ] Update Product and SKU screens with sales/purchase descriptions, tax rates, income/expense account hooks, costing policy, product image URL, SKU expiration settings, and gallery URLs.
- [ ] Use existing routes, services, permissions, `AdminLayout`, `PageHeader`, `AppCard`, `FilterBar`, bilingual labels, and design-system classes.

### Acceptance Criteria

- [ ] User can create and edit typed customer/supplier addresses and contacts.
- [ ] User can create and edit product commercial metadata.
- [ ] User can create and edit SKU expiration/gallery metadata.
- [ ] Detail/list pages show meaningful new metadata without visual clutter.
- [ ] `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` succeeds.
- [ ] `dotnet build src/Bootstraper/Api/Api.csproj` succeeds.

## Issue 3: Phase 2 - Purchase Workflow Controls

**Status:** Planned  
**Suggested branch:** `codex/odoo-phase-2-purchase-controls`

### Goal

Make procurement behave closer to Odoo RFQ and Purchase flows.

### Implementation Changes

- [ ] Add RFQ send/print/email action placeholders using existing procurement workflow service patterns.
- [ ] Display and update RFQ sent state: `SentAt`, `SentBy`, current document status, and conversion trace.
- [ ] Add purchase template picker placeholder to procurement form.
- [ ] Add bill control policy UI at document and line level.
- [ ] Add 3-way match, received quantity, billed quantity, and bill eligibility display on purchase order/goods receipt/supplier invoice screens.
- [ ] Add list/detail chips for billable, partially received, partially billed, and match exception states.
- [ ] Keep inventory/accounting posting as integration placeholders unless existing endpoints already support the action safely.

### Acceptance Criteria

- [ ] RFQs can be marked and surfaced as sent from the UI.
- [ ] PO and supplier invoice screens show bill-control and 3-way match status.
- [ ] Users can distinguish billable vs not-yet-billable purchase documents.
- [ ] No direct cross-module DbContext coupling is introduced.
- [ ] Procurement and web builds succeed.

## Issue 4: Phase 3 - Sales Quotation And Invoicing Polish

**Status:** Planned  
**Suggested branch:** `codex/odoo-phase-3-sales-quotation-polish`

### Goal

Improve sales quotation/order handling toward Odoo quotation workflows.

### Implementation Changes

- [ ] Add quotation template picker placeholder to quotation create/edit.
- [ ] Add optional product line support in quotation UI.
- [ ] Add quote deadline, signature required, online payment required, pro-forma, down-payment amount/percent, invoice address, and delivery address fields.
- [ ] Show quotation status chips for draft/sent/accepted/expired/converted with existing workflow permissions.
- [ ] Add quote-to-order conversion summary fields on order detail.
- [ ] Add ordered-vs-delivered invoice policy display placeholders where current data supports it.

### Acceptance Criteria

- [ ] User can maintain quotation commercial controls from the quotation form.
- [ ] Optional lines are visually distinct from mandatory quote lines.
- [ ] Sales order detail reflects quotation-origin metadata.
- [ ] Existing quotation send/convert permissions remain intact.
- [ ] SalesOrder and web builds succeed.

## Issue 5: Phase 4 - Templates, Blanket Orders, And Tender Foundation

**Status:** Planned  
**Suggested branch:** `codex/odoo-phase-4-templates-tenders`

### Goal

Add first-class workflow entities or lightweight managed records for reusable purchase/sales templates and strategic procurement flows.

### Implementation Changes

- [ ] Add purchase template storage and UI if placeholders from Phase 2 are not enough.
- [ ] Add quotation template storage and UI if placeholders from Phase 3 are not enough.
- [ ] Add blanket order and call-for-tender lifecycle models in Procurement.
- [ ] Link RFQs/POs to blanket orders and tenders.
- [ ] Add backend handlers/endpoints with normal procurement permissions and company/branch validation.
- [ ] Generate EF migrations through `dotnet ef`, never manually.

### Acceptance Criteria

- [ ] Users can create and select purchase templates.
- [ ] Users can create and select sales quotation templates.
- [ ] Procurement can group RFQs/POs under tender or blanket references.
- [ ] Ownership and branch/company validation exists on mutations.
- [ ] API and web builds succeed.

## Issue 6: Phase 5 - Smart Buttons And Cross-Navigation

**Status:** Planned  
**Suggested branch:** `codex/odoo-phase-5-smart-buttons`

### Goal

Add Odoo-style smart buttons and drill-down navigation across partners, products, purchase, sales, deliveries, invoices, and receipts.

### Implementation Changes

- [ ] Add backend summary queries for customer and supplier smart-link counts where static DTO fields are not enough.
- [ ] Add smart buttons on customer detail for quotations, sales orders, deliveries, invoices, POS/orders, and ledger placeholder.
- [ ] Add smart buttons on supplier detail for RFQs, purchase orders, receipts, supplier bills, scorecards, and supplier items.
- [ ] Add product detail links to SKUs, vendor pricelists, reordering rules, sales lines, purchase lines, and inventory placeholders where available.
- [ ] Preserve permission-gated visibility and use existing route patterns.

### Acceptance Criteria

- [ ] Smart buttons show accurate counts or gracefully hide unavailable counts.
- [ ] Clicking a smart button navigates to the filtered target page.
- [ ] Users without permissions do not see restricted actions.
- [ ] No backend authorization is weakened.
- [ ] API and web builds succeed.

## Issue 7: Phase 6 - Automation, Expiry Jobs, And Document Outputs

**Status:** Planned  
**Suggested branch:** `codex/odoo-phase-6-automation-documents`

### Goal

Add operational automation and document output placeholders that align with Odoo workflows.

### Implementation Changes

- [ ] Add quote expiry job/handler to mark overdue quotations expired.
- [ ] Add RFQ/quote PDF export placeholder or integrate with existing export/document patterns.
- [ ] Add email/send action placeholders for RFQ and quotation workflows.
- [ ] Add receipt-driven vendor bill eligibility computation.
- [ ] Add scheduled or command-triggered recomputation for bill-control and match statuses.
- [ ] Keep email/PDF/accounting posting behind existing infrastructure; do not invent a new document engine if one already exists.

### Acceptance Criteria

- [ ] Expired quotations transition predictably.
- [ ] RFQ and quotation screens expose print/send affordances.
- [ ] Purchase documents recompute bill eligibility consistently.
- [ ] Automation can be tested without running the full app visually.
- [ ] Relevant backend and web builds succeed.

## Issue 8: Phase 7 - Reports And Dashboards

**Status:** Planned  
**Suggested branch:** `codex/odoo-phase-7-reports-dashboards`

### Goal

Surface the new Odoo-style controls in dashboards and operational reports.

### Implementation Changes

- [ ] Add procurement dashboard widgets for RFQs sent, POs awaiting receipt, billable receipts, 3-way match exceptions, and tender/blanket summaries.
- [ ] Add sales dashboard widgets for quotations expiring soon, optional product adoption, down payments, pro-forma quotes, and conversion status.
- [ ] Add customer/supplier summary panels for commercial/accounting defaults and recent document activity.
- [ ] Use existing dashboard/report page patterns and avoid new APIs when existing service data is sufficient.

### Acceptance Criteria

- [ ] Dashboards show actionable counts, not just decorative cards.
- [ ] Reports respect company/branch scope and permissions.
- [ ] Empty/loading/error states are present.
- [ ] API and web builds succeed.

## Global Test Plan

- [ ] Run focused backend builds for touched modules after each issue.
- [ ] Always run `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj` for UI phases.
- [ ] Run `dotnet build src/Bootstraper/Api/Api.csproj` before marking each phase complete.
- [ ] For EF changes, generate migrations using `dotnet ef migrations add ... --startup-project src/Bootstraper/Api/Api.csproj`.
- [ ] Use manual smoke tests for create/edit/detail/list flows in Arabic and English.
- [ ] Use `git diff --check` and inspect edited UI files for hardcoded visual debt.

## Assumptions

- Phase 0 remains the base branch work.
- Phase 1 should be implemented next because it makes the already-added backend fields usable.
- Full accounting, marketplace connectors, eCommerce, CRM lead scoring, subscriptions, rentals, and full Odoo accounting parity are out of scope.
- Existing routes, permissions, localization, company/branch scope rules, DTOs, and business behavior stay intact unless a roadmap issue explicitly changes them.
