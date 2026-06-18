# AlAfkar ERP UAT Pack

Generated at: 2026-06-18 14:55:02

This pack is generated from the repository source of truth:

- Backend Carter endpoint modules under src/Modules
- Permission definitions in src/Shared/SharedWithUI/SharedWithUI/Permissions/PermissionList.cs
- Blazor menu routes in UI/AlAfkarERP/AlAfkarERP.Shared/Layout/MuenuItem.cs
- Blazor feature page routes under UI/AlAfkarERP/AlAfkarERP.Shared/Pages/Features

## Files

- UAT_Master_Matrix.csv: Manual execution matrix grouped by module, permission, UI route, expected behavior, negative permission case, and evidence columns.
- UI_Coverage_Gaps.csv: Backend endpoint coverage status: Menu reachable, In-page action reachable, Page route reachable, or Not represented in UI.
- Role_Permission_UAT.csv: Persona-based access tests for admin, manager/approver, employee, cashier, and no-permission users.
- Test_Data_Setup.csv: Required reusable test data before running the UAT.
- Backend_Functionality_Inventory.csv: Extracted backend endpoint inventory.
- Frontend_Menu_Routes.csv: Extracted sidebar/workspace menu route inventory.
- Frontend_Page_Routes.csv: Extracted Blazor @page route inventory.
- Permission_Inventory.csv: Extracted permission action inventory.

## Current Counts

| Metric | Count |
| --- | ---: |
| Backend endpoint rows | 263 |
| Permission action rows | 278 |
| Menu route rows | 87 |
| Blazor page route rows | 103 |
| UI permission references | 184 |
| Potential UI blocker gaps | 0 |

## Manual Execution Rules

1. Start with Test_Data_Setup.csv; create or verify all prerequisite data.
2. Execute Role_Permission_UAT.csv first to confirm the test users are configured correctly.
3. Execute UAT_Master_Matrix.csv by module. Fill Result, Evidence, and Notes.
4. Review UI_Coverage_Gaps.csv; anything marked Not represented in UI is a UAT blocker until a visible UI route/action is confirmed or implemented.
5. For every row, run the positive case, invalid input/status case where relevant, permission-denied case, and English/Arabic RTL smoke check.

## Regeneration

From the repository root:

    powershell -NoProfile -ExecutionPolicy Bypass -File .\docs\uat\generate-uat.ps1
