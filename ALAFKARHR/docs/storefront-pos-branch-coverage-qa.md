# StoreFront POS Branch Coverage QA

Use this focused checklist when validating StoreFront branch-scoped POS, cashier, inventory, and procurement coverage.

## Access And Discovery

- A scoped StoreFront cashier without company-wide StoreFront permissions can open `/StoreFront/POS`.
- `/StoreFront/POS` shows the Stores action when the user has StoreFront branch-role store or item permissions.
- The normal menu authorization for unrelated modules remains unchanged.

## POS Readiness

- A selected store without a StoreFront branch shows a readiness warning.
- A selected store without a default warehouse shows a stock-out readiness warning.
- A selected store without active sellable SKUs shows an allowed SKU readiness warning.
- A selected store without active branch cash accounts shows a cashier ledger readiness warning.
- Cash payment with no open cashier session shows a visible blocker and disables checkout.

## Cashier Sessions And Checkout

- A cashier can open a session with an active StoreFront branch cash account.
- Cash checkout without an open session fails before submit.
- Cash checkout with an open session succeeds and keeps the active session/cash account visible.
- Recorded card checkout can select a StoreFront branch bank account when one exists.
- Recorded card checkout with no selected bank account uses the backend default branch bank account.
- Store manager can close without handover, hand over to another open session, and hand over to a branch cash account.
- Session history shows handover target labels, payment totals, expected cash, counted cash, and variance.

## Branch Scope

- Store A user cannot see Store B sessions, cash accounts, POS state, or handover targets.
- Inventory transfer source/destination selectors do not offer warehouses outside the user branch scope.
- Procurement warehouse selectors do not offer warehouses outside the user branch scope.
- Procurement source-document branch alignment clears an incompatible selected warehouse.
- Company admin or finance user with view-all branch access can see cross-branch choices where backend access allows it.

## Backend Outcomes

- StoreFront POS cash checkout creates payment, receipt, sales order, accounting document, journal entry, and ZATCA invoice with the same StoreFront branch.
- Cash receipt journal debits the selected cashier cash account.
- Recorded card receipt journal debits the selected branch bank account, or backend default branch bank account when none is selected.
- POS stock-out consumes the StoreFront default warehouse and fails on insufficient stock.
