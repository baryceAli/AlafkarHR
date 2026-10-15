# ERP text encoding audit and repair

Implemented on `codex/fix-erp-mojibake` on 2026-10-15.

## Results

- Audited 2,013 application-owned source files across ERP UI, shared UI utilities,
  first-party export/notification JavaScript, backend modules, and shared contracts.
- Recovered 994 corrupted non-ASCII spans on 435 lines in 13 Razor pages.
  Four spans were comment icons; the remaining spans were Arabic text.
- Final audit: **0 errors, 0 review candidates**. No intentional exceptions needed.
- The garbled text was already present in source literals; the document declares
  UTF-8 and `SharedDataService.SelectViewLang` returns the selected literal unchanged.
- Every repair was a strict, reversible Windows-1252/UTF-8 byte recovery. No guessed
  translations, lossy decoding, or file-wide encoding conversion was needed.

The complete [repair inventory](erp-mojibake-repairs.json) records each file, route,
line number, original line and corrected line. Line numbers refer to this repair's
source snapshot. Its original mojibake is intentionally retained as audit evidence;
documentation is outside the production-source scanner's scope.

## Affected pages

All files below are in `UI/AlAfkarERP/AlAfkarERP.Shared/Pages/Features/`.

| Module / file | Existing routes | Repaired spans |
| --- | --- | ---: |
| Attendance / Shifts.razor | `/Attendance/Shifts` | 14 |
| Catalog / ProductList.razor | `/Catalog/Product/List` | 186 |
| SalesOrder / OrderIntakeDetail.razor | `/Orders/Intakes/{Id:guid}` | 75 |
| SalesOrder / OrderIntakeList.razor | `/Orders/Intakes` | 65 |
| SalesOrder / SalesDashboard.razor | `/Sales/Dashboard` | 38 |
| SalesOrder / SalesDeliveryNoteList.razor | `/Sales/DeliveryNotes` | 60 |
| SalesOrder / SalesOrderCreate.razor | `/Sales/Orders/Create` | 95 |
| SalesOrder / SalesOrderDetail.razor | `/Sales/Orders/{Id:guid}` | 107 |
| SalesOrder / SalesOrderList.razor | `/Sales/Orders` | 70 |
| SalesOrder / SalesQuotationEdit.razor | `/Sales/Quotations/Create`, `/Sales/Quotations/{Id:guid}` | 132 |
| SalesOrder / SalesQuotationList.razor | `/Sales/Quotations` | 107 |
| SalesOrder / SalesReturnList.razor | `/Sales/Returns` | 44 |
| TaskManagement / TaskList.razor | `/TaskManagement/List` | 1 |

Corrections are exposed through existing pages and actions; no new menus or routes
were required. Existing language subscriptions and disposal were present in all
13 pages. Company/branch scope, normal permissions, branch access enforcement,
services, APIs and DTOs were untouched.

## Repeatable audit

From the repository workspace directory, run:

```powershell
python tools/audit-erp-text.py
python tools/audit-erp-text.py --json
```

The checker is read-only and uses only Python's standard library. Invalid UTF-8,
replacement characters, and recoverable high-confidence corruption return exit
code 1. Ambiguous signatures print `REVIEW` with file/line locations and need human
assessment, even when exit code is 0. The JSON mode includes proposed replacements.
All decoding uses strict error handling. Undefined Windows-1252 C1 bytes are
mapped explicitly to their original byte values without discarding bytes.

It covers application-owned `.razor`, `.cs`, `.resx`, `.json`, `.js`, `.css` and
`.html` files under the shared/web projects, backend modules and shared contracts.
It includes ERP user/role and storefront administration text. It excludes the
public website, login/registration/password recovery, `/store` and its templates,
dedicated POS pages, cart implementation, migrations, generated build directories,
vendor/minified assets, and documentation. First-party `lib/MyJs` is included.

This is a heuristic source audit, not proof that runtime database/API values or
all possible historical encoding combinations are correct. Source files outside
the declared roots/extensions are not audited.

## Verification performed

- Static scan passed with no remaining findings.
- `git diff --check -- UI/AlAfkarERP/AlAfkarERP.Shared` passed.
- Verified repaired files retained their original BOM and newline sequences.
- Verified repairs preserved ASCII code, including routes, markup, placeholders,
  interpolation and permission identifiers. Original valid Unicode spans were retained.
- Exercised checker cases for Arabic, punctuation and emoji recovery, valid Arabic
  preservation, invalid UTF-8, replacement characters, ambiguous review and CRLF
  line numbers using temporary verification code.
- Web build passed: **194 warnings, 0 errors**, using:

```powershell
dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj --no-restore --nologo -m:1 /nodeReuse:false -v:normal
```

The initial default build failed without diagnostics; the single-worker build
completed successfully. No backend source changed, so no additional backend build
was required. Existing login/public website working-tree changes were preserved.

## Manual checklist (not executed)

- Open each listed route with an authorized user and representative records;
  switch Arabic → English → Arabic without reloading. Check titles, subtitles,
  buttons, tooltips, RTL/LTR direction and status labels.
- On Sales lists/dashboard, inspect filters, pagination, tables, empty/loading
  states and summary cards. On detail pages, inspect line items and totals.
- On quotation/order create and edit forms, inspect selectors, options, validation,
  confirmation dialogs and success/failure notifications using suitable test data.
- On Product List, inspect the edit form, delete confirmation, active/inactive
  labels, paging, product types and costing-policy options.
- On Shifts, inspect break mode, local break start/end and paid-break options.
  On Task List, confirm the card title reads `المهام` in Arabic.
- Spot-check navigation and any available reports/exports in both languages;
  compare static labels with user-entered names to distinguish source from data issues.
- If a displayed value remains garbled, capture its route and field, trace the
  API/database origin, and prepare a separate reviewed data-recovery change.

The app/browser was not launched. No stored data was read or rewritten, so stored
data corruption remains unassessed. No source wording issues remain unresolved.
