# Sales Dashboard query translation regression

Branch: `codex/fix-sales-dashboard-query`.

## Change and scope

`GetSalesDashboardHandler` previously summed the unmapped C# getter
`SalesOrderLine.TotalAmount` inside `TopProducts`, causing EF Core SQL translation
to fail. The query now projects mapped fields and the equivalent net amount,
then aggregates net plus tax. Aggregation, sorting and the five-group limit remain
in the database; no client-side materialization was introduced.

Grouping by SKU and both product names, the English-name null fallback, quantities,
cancellation token and other dashboard metrics remain unchanged. Remaining query
expressions were inspected: order/quotation/return totals are stored properties,
and other expressions use mapped fields/navigation properties.

The existing company filter and `SalesOrderPermissions.ViewReports` endpoint
permission are unchanged. Branch access behavior is unchanged; this fix adds no
authorization or scope semantics. No API, DTO, mapping, migration or UI changes
were required. The result is consumed by the existing `/Sales/Dashboard` page.

## Verification status

- Completed: focused source and diff inspection; no unmapped line amount getters
  remain in the product aggregation. `git diff --check` passed.
- Sales module build passed: **7 warnings, 0 errors**.
- ERP Web build passed: **194 warnings, 0 errors**.
- Build commands (single worker, node reuse disabled):

```powershell
dotnet build src/Modules/Sales/Sales/Sales.csproj --nologo -m:1 /nodeReuse:false -v:minimal
dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj --nologo -m:1 /nodeReuse:false -v:minimal
```

- Runtime scenarios below: **not executed**. Repository defaults permit builds
  without launching the application or browser. A build alone does not prove
  runtime SQL translation or database results.

## Runtime checklist

Use controlled non-production fixtures and an authorized reporting user. Request
`GET /api/v1/sales/dashboard/company/{companyId}` and inspect the `Dashboard`
response, then check the existing dashboard UI when visual validation is authorized.

| Scenario | Expected result |
| --- | --- |
| Company with orders, quotations and returns | HTTP 200; no LINQ translation exception; existing metrics and product breakdown are returned. |
| Empty company | HTTP 200; empty top-products array and no aggregate exception. |
| Same SKU and same names across two orders | One group; value and quantity equal the sums of the contributing lines. |
| Quantity 2, price 100, discount 10%, tax 15% | Product value contribution 207; quantity contribution 2. |
| Quantity 1.5, price 20, discount 10%, tax 15% | Product value contribution 31.05; quantity contribution 1.5. |
| Discount 0%, tax 0% | Value equals quantity multiplied by unit price. |
| Price 0 or discount 100% | Value contribution 0; ordered quantity still contributes. |
| Six or more groups with distinct totals | Exactly five groups in descending total-value order. |
| Same SKU but different saved names | Separate groups, preserving the existing grouping behavior. |
| English name is null | Arabic name is returned; empty English strings keep their existing behavior. |
| Another company has a larger order for the same SKU | Its value and quantity are excluded from the requested company. |
| Other dashboard totals | Match their pre-change calculations; no status inclusion rules changed. |

For fractional values, compare with the existing entity formula using the values
actually persisted in SQL Server. No new explicit rounding is introduced; database
decimal precision rules still apply. If inspecting generated SQL, verify `SUM`,
`GROUP BY`, descending value ordering and the five-row limit execute server-side.
