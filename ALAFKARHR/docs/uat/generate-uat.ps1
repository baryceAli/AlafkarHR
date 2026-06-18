param(
    [string]$Root = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
)

$ErrorActionPreference = "Stop"

function New-Dir([string]$Path) {
    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path | Out-Null
    }
}

function Normalize-Expression([string]$Value) {
    if ([string]::IsNullOrWhiteSpace($Value)) { return "" }
    return (($Value -replace "\s+", " ") -replace "`r|`n", " ").Trim()
}

function Get-ModuleFromPath([string]$Path) {
    $relative = $Path.Substring($Root.Length).TrimStart("\", "/")
    $parts = $relative -split "[\\/]"
    if ($parts.Length -ge 3 -and $parts[0] -eq "src" -and $parts[1] -eq "Modules") {
        return $parts[2]
    }
    if ($parts.Length -ge 5 -and $parts[0] -eq "UI") {
        $featuresIndex = [Array]::IndexOf($parts, "Features")
        if ($featuresIndex -ge 0 -and $parts.Length -gt ($featuresIndex + 1)) {
            return $parts[$featuresIndex + 1]
        }
    }
    return ""
}

function Get-FeatureParts([string]$Path) {
    $relative = $Path.Substring($Root.Length).TrimStart("\", "/")
    $parts = $relative -split "[\\/]"
    $featuresIndex = [Array]::IndexOf($parts, "Features")
    if ($featuresIndex -lt 0) { return @("", "") }
    $after = $parts[($featuresIndex + 1)..($parts.Length - 1)]
    if ($after.Length -ge 2) {
        return @($after[0], $after[$after.Length - 2])
    }
    if ($after.Length -eq 1) {
        return @($after[0], "")
    }
    return @("", "")
}

function Get-ActionKind([string]$Text, [string]$HttpMethod) {
    $haystack = "$Text $HttpMethod".ToLowerInvariant()
    if ($haystack -match "dashboard|report") { return "Report/Dashboard" }
    if ($haystack -match "approve|accept|reject|submit|cancel|close|receive|post|send|match|checkout|confirm|deliver|invoice|complete|return|calculate|commit|assign|reassign|progress|comment|attachment|completion|normalization") { return "Workflow" }
    if ($haystack -match "create|add|post") { return "Create" }
    if ($haystack -match "update|edit|put|patch") { return "Edit" }
    if ($haystack -match "delete|remove") { return "Delete" }
    if ($haystack -match "get|view|list|search") { return "View/List" }
    return "General"
}

function Get-ModuleFamily([string]$Name) {
    switch -Regex ($Name) {
        "Auth|Users|Roles|Authentication" { return "Auth and security" }
        "Organization|Company|Branch|Administration|Department" { return "Organization" }
        "Employee|Employees|Academic|Specialization|Position" { return "HR employees" }
        "Attendance|Leave|Leaves" { return "Attendance and leave" }
        "Payroll|Salary|Loan|Contract|Component" { return "Payroll" }
        "Customer" { return "Customers" }
        "Supplier" { return "Suppliers" }
        "Catalog|Product|Category|Brand|Unit|Variant|Package|Pricing|Price" { return "Catalog and pricing" }
        "Inventory|Inventories|Warehouse|Batch|Stock" { return "Inventory" }
        "^/Dashboard$|Control Panel|Dashboard" { return "Control Panel" }
        "Sales|SalesOrder|Orders|Cart|Payment|POS|Intake" { return "Sales, POS, and orders" }
        "Procurement|Purchase|Quotation|Receipt|Invoice|Return|RFQ" { return "Procurement" }
        "Task" { return "Task management" }
        "GeneralSettings|Settings|Currency|SystemSettings" { return "General settings" }
        default { return $Name }
    }
}

function Get-ManualSteps([string]$ActionKind, [string]$UiRoute, [string]$Entity, [string]$Permission) {
    $target = if ($UiRoute) { $UiRoute } else { "the relevant UI page for $Entity" }
    switch ($ActionKind) {
        "Create" { return "Open $target; click Add/New; enter valid required data; save; confirm success toast and new row/detail is visible; repeat with missing required data to verify validation." }
        "Edit" { return "Open $target; select an existing record; edit allowed fields; save; verify success toast and persisted changes after refresh." }
        "Delete" { return "Open $target; select a disposable test record; choose Delete/Remove; confirm dialog; verify record is removed or marked inactive and cannot be selected where inappropriate." }
        "Workflow" { return "Open $target; create or select a record in the required status; execute the workflow action linked to $Permission; verify status, audit trail, downstream data, and unavailable invalid next actions." }
        "Report/Dashboard" { return "Open $target; verify widgets/table load; apply available filters/date range; compare totals with source records; verify empty-state behavior with filters that return no data." }
        default { return "Open $target; verify page loads; search/filter if available; inspect details; check pagination/sorting where present; verify empty-state and refresh behavior." }
    }
}

function Get-ExpectedResult([string]$ActionKind, [string]$Entity) {
    switch ($ActionKind) {
        "Create" { return "$Entity is created only with valid data, visible in the UI, and persisted after reload." }
        "Edit" { return "$Entity changes are saved, reloaded correctly, and validation prevents invalid updates." }
        "Delete" { return "$Entity deletion/removal follows the product rule and no deleted item remains actionable." }
        "Workflow" { return "Workflow transition succeeds only from valid state, updates visible status, and preserves permission and audit behavior." }
        "Report/Dashboard" { return "Dashboard/report data loads, filters correctly, and matches underlying transactional records." }
        default { return "$Entity page/function is reachable, usable, permission-protected, and data is consistent with backend behavior." }
    }
}

$outputDir = Join-Path $Root "docs\uat"
New-Dir $outputDir

$permissionFile = Join-Path $Root "src\Shared\SharedWithUI\SharedWithUI\Permissions\PermissionList.cs"
$permissionText = Get-Content $permissionFile -Raw
$permissionClasses = @()
$permissionRegex = [regex]'(?s)public static class (?<class>[A-Za-z0-9_]+Permissions).*?GroupName\s*\{\s*get;\s*set;\s*\}\s*=\s*"(?<group>[^"]+)";(?<body>.*?)(?=public static class|\z)'
foreach ($match in $permissionRegex.Matches($permissionText)) {
    $className = $match.Groups["class"].Value
    $groupName = $match.Groups["group"].Value
    $body = $match.Groups["body"].Value
    $actionMatches = [regex]::Matches($body, 'public static string (?<action>[A-Za-z0-9_]+)\s*\{\s*get;\s*set;\s*\}\s*=')
    foreach ($actionMatch in $actionMatches) {
        $action = $actionMatch.Groups["action"].Value
        if ($action -eq "GroupName") { continue }
        $permissionClasses += [pscustomobject]@{
            PermissionClass = $className
            PermissionGroup = $groupName
            Entity = ($groupName -split "\.")[-1]
            ModuleFamily = Get-ModuleFamily $groupName
            Action = $action
            Permission = "$groupName.$action"
        }
    }
}

$menuFile = Join-Path $Root "UI\AlAfkarERP\AlAfkarERP.Shared\Layout\MuenuItem.cs"
$menuLines = Get-Content $menuFile
$menuRows = @()
for ($i = 0; $i -lt $menuLines.Length; $i++) {
    $line = $menuLines[$i]
    if ($line.TrimStart().StartsWith("//")) { continue }
    if ($line -match 'Url\s*=\s*"(?<url>[^"]+)"') {
        $url = $matches["url"]
        $start = [Math]::Max(0, $i - 18)
        $context = $menuLines[$start..$i]
        $textEn = ""
        $permission = ""
        foreach ($contextLine in $context) {
            if ($contextLine.TrimStart().StartsWith("//")) { continue }
            if ($contextLine -match 'TextEn\s*=\s*"(?<text>[^"]+)"') { $textEn = $matches["text"] }
            if ($contextLine -match 'PermissionPolicy\s*=\s*(?<perm>[^,]+)') { $permission = Normalize-Expression $matches["perm"] }
        }
        $menuRows += [pscustomobject]@{
            TextEn = $textEn
            Route = $url
            PermissionExpression = $permission
            PermissionReference = if ($permission -match 'PermissionList\.(?<class>[A-Za-z0-9_]+Permissions)\.(?<action>[A-Za-z0-9_]+)') { "$($matches["class"]).$($matches["action"])" } else { $permission.Trim('"') }
            ModuleFamily = Get-ModuleFamily $url
            SourceFile = "UI/AlAfkarERP/AlAfkarERP.Shared/Layout/MuenuItem.cs"
            Line = $i + 1
        }
    }
}

$pageRows = @()
$pageFiles = Get-ChildItem (Join-Path $Root "UI\AlAfkarERP\AlAfkarERP.Shared\Pages\Features") -Recurse -Filter "*.razor"
foreach ($file in $pageFiles) {
    $lines = Get-Content $file.FullName
    for ($i = 0; $i -lt $lines.Length; $i++) {
        if ($lines[$i] -match '@page\s+"(?<route>[^"]+)"') {
            $route = $matches["route"].TrimEnd(";")
            $pageRows += [pscustomobject]@{
                Module = Get-ModuleFromPath $file.FullName
                ModuleFamily = Get-ModuleFamily (Get-ModuleFromPath $file.FullName)
                Route = $route
                PageFile = $file.FullName.Substring($Root.Length).TrimStart("\", "/").Replace("\", "/")
                MenuReachable = [bool]($menuRows | Where-Object { $_.Route -eq $route })
            }
        }
    }
}

$uiPermissionRows = @()
foreach ($file in $pageFiles) {
    $text = Get-Content $file.FullName -Raw
    $matches = [regex]::Matches($text, 'PermissionList\.(?<class>[A-Za-z0-9_]+Permissions)\.(?<action>[A-Za-z0-9_]+)')
    foreach ($match in $matches) {
        $class = $match.Groups["class"].Value
        $action = $match.Groups["action"].Value
        $permission = $permissionClasses | Where-Object { $_.PermissionClass -eq $class -and $_.Action -eq $action } | Select-Object -First 1
        $uiPermissionRows += [pscustomobject]@{
            PermissionReference = "$class.$action"
            Permission = if ($permission) { $permission.Permission } else { "$class.$action" }
            ModuleFamily = if ($permission) { $permission.ModuleFamily } else { Get-ModuleFamily $class }
            PageFile = $file.FullName.Substring($Root.Length).TrimStart("\", "/").Replace("\", "/")
        }
    }
}
$uiPermissionRows = $uiPermissionRows | Sort-Object PermissionReference, PageFile -Unique

$endpointRows = @()
$endpointFiles = Get-ChildItem (Join-Path $Root "src\Modules") -Recurse -Filter "*.cs" | Where-Object {
    (Select-String -Path $_.FullName -Pattern "ICarterModule" -Quiet)
}
foreach ($file in $endpointFiles) {
    $lines = Get-Content $file.FullName
    $featureParts = Get-FeatureParts $file.FullName
    $moduleName = Get-ModuleFromPath $file.FullName
    for ($i = 0; $i -lt $lines.Length; $i++) {
        if ($lines[$i] -match 'app\.Map(?<method>Get|Post|Put|Delete|Patch)\((?<route>.+)') {
            $method = $matches["method"].ToUpperInvariant()
            $routeExpression = Normalize-Expression $matches["route"]
            $permissionExpression = ""
            for ($j = $i; $j -lt [Math]::Min($lines.Length, $i + 20); $j++) {
                if ($j -gt $i -and $lines[$j] -match 'app\.Map(Get|Post|Put|Delete|Patch)\(') { break }
                if ($lines[$j] -match 'RequireAuthorization\((?<perm>[^)]+)\)') {
                    $permissionExpression = Normalize-Expression $matches["perm"]
                    break
                }
            }
            $permissionReference = ""
            if ($permissionExpression -match 'PermissionList\.(?<class>[A-Za-z0-9_]+Permissions)\.(?<action>[A-Za-z0-9_]+)') {
                $permissionReference = "$($matches["class"]).$($matches["action"])"
            }
            $entity = if ($featureParts[0]) { $featureParts[0] } else { $moduleName }
            $action = if ($featureParts[1]) { $featureParts[1] } else { $file.BaseName }
            $endpointRows += [pscustomobject]@{
                Module = $moduleName
                ModuleFamily = Get-ModuleFamily $moduleName
                Entity = $entity
                FeatureAction = $action
                ActionKind = Get-ActionKind "$entity $action $routeExpression" $method
                HttpMethod = $method
                RouteExpression = $routeExpression
                PermissionExpression = $permissionExpression
                PermissionReference = $permissionReference
                EndpointFile = $file.FullName.Substring($Root.Length).TrimStart("\", "/").Replace("\", "/")
                Line = $i + 1
            }
        }
    }
}

$coverageRows = @()
foreach ($endpoint in $endpointRows) {
    $menuMatch = $null
    $pageMatch = $null
    $permissionMatch = $null
    if ($endpoint.PermissionReference) {
        $menuMatch = $menuRows | Where-Object { $_.PermissionReference -eq $endpoint.PermissionReference } | Select-Object -First 1
        $permissionMatch = $uiPermissionRows | Where-Object { $_.PermissionReference -eq $endpoint.PermissionReference } | Select-Object -First 1
    }
    $moduleFamily = $endpoint.ModuleFamily
    $pageMatch = $pageRows | Where-Object { $_.ModuleFamily -eq $moduleFamily } | Select-Object -First 1
    $status = "Not represented in UI"
    $uiRoute = ""
    if ($menuMatch) {
        $status = "Menu reachable"
        $uiRoute = $menuMatch.Route
    } elseif ($permissionMatch) {
        $status = "In-page action reachable"
        $matchingPage = $pageRows | Where-Object { $_.PageFile -eq $permissionMatch.PageFile } | Select-Object -First 1
        $uiRoute = if ($matchingPage) { $matchingPage.Route } else { $permissionMatch.PageFile }
    } elseif ($pageMatch) {
        $status = "Page route reachable"
        $uiRoute = $pageMatch.Route
    }
    $severity = if ($status -eq "Not represented in UI") { "Blocker" } elseif ($status -eq "Page route reachable") { "Needs manual confirmation" } else { "Covered" }
    $coverageRows += [pscustomobject]@{
        Module = $endpoint.ModuleFamily
        BackendModule = $endpoint.Module
        Entity = $endpoint.Entity
        FeatureAction = $endpoint.FeatureAction
        ActionKind = $endpoint.ActionKind
        HttpMethod = $endpoint.HttpMethod
        BackendRoute = $endpoint.RouteExpression
        RequiredPermission = $endpoint.PermissionExpression
        PermissionReference = $endpoint.PermissionReference
        UICoverageStatus = $status
        UIRouteOrPath = $uiRoute
        GapSeverity = $severity
        BackendSource = $endpoint.EndpointFile
    }
}

$masterRows = @()
foreach ($permission in $permissionClasses) {
    $reference = "$($permission.PermissionClass).$($permission.Action)"
    $menuMatch = $menuRows | Where-Object { $_.PermissionReference -eq $reference } | Select-Object -First 1
    $uiPermissionMatch = $uiPermissionRows | Where-Object { $_.PermissionReference -eq $reference } | Select-Object -First 1
    $endpointMatches = $endpointRows | Where-Object { $_.PermissionReference -eq $reference }
    $modulePage = $pageRows | Where-Object { $_.ModuleFamily -eq $permission.ModuleFamily } | Select-Object -First 1
    $actionKind = Get-ActionKind "$($permission.Entity) $($permission.Action)" ""
    if ($endpointMatches) {
        $actionKind = ($endpointMatches | Select-Object -First 1).ActionKind
    }
    $uiRoute = ""
    if ($menuMatch) { $uiRoute = $menuMatch.Route }
    elseif ($uiPermissionMatch) {
        $page = $pageRows | Where-Object { $_.PageFile -eq $uiPermissionMatch.PageFile } | Select-Object -First 1
        $uiRoute = if ($page) { $page.Route } else { $uiPermissionMatch.PageFile }
    } elseif ($modulePage) { $uiRoute = $modulePage.Route }
    $apiCoverage = if ($endpointMatches) { (($endpointMatches | Select-Object -ExpandProperty HttpMethod -Unique) -join "/") } else { "No direct endpoint found or covered through parent UI/service" }
    $masterRows += [pscustomobject]@{
        TestId = "UAT-{0:D4}" -f ($masterRows.Count + 1)
        Module = $permission.ModuleFamily
        Entity = $permission.Entity
        Function = $permission.Action
        ActionKind = $actionKind
        UINavPathOrRoute = $uiRoute
        RequiredPermission = $permission.Permission
        BackendAPIFunctionCovered = $apiCoverage
        PreconditionsTestData = "Seeded company context, authorized test user, and valid $($permission.Entity) data where applicable."
        ManualSteps = Get-ManualSteps $actionKind $uiRoute $permission.Entity $permission.Permission
        ExpectedResult = Get-ExpectedResult $actionKind $permission.Entity
        NegativePermissionCase = "Sign in as a user without $($permission.Permission); menu/action must be hidden or disabled, and direct navigation/action must be denied."
        LocalizationRtlCheck = "Switch English/Arabic and verify labels, direction, validation messages, and action alignment remain usable."
        Result = ""
        Evidence = ""
        Notes = if (-not $uiRoute) { "Potential UI coverage gap; confirm reachable in-page or add UI coverage." } else { "" }
    }
}

foreach ($menu in $menuRows) {
    $masterRows += [pscustomobject]@{
        TestId = "UAT-{0:D4}" -f ($masterRows.Count + 1)
        Module = $menu.ModuleFamily
        Entity = $menu.TextEn
        Function = "Navigation/access"
        ActionKind = "View/List"
        UINavPathOrRoute = $menu.Route
        RequiredPermission = $menu.PermissionExpression
        BackendAPIFunctionCovered = "Menu route and page load"
        PreconditionsTestData = "Authorized user has the menu permission and company/session context is selected."
        ManualSteps = "Open sidebar/workspace menu; click $($menu.TextEn); verify route $($menu.Route) loads without authorization or data errors."
        ExpectedResult = "$($menu.TextEn) is visible for authorized users and the target page loads successfully."
        NegativePermissionCase = "Remove the listed permission from the test role; menu item should be hidden and direct route should not expose data."
        LocalizationRtlCheck = "Switch English/Arabic and verify menu label, route page title, and layout direction."
        Result = ""
        Evidence = ""
        Notes = "Menu coverage row"
    }
}

$roleRows = @(
    [pscustomobject]@{ Persona = "UAT Admin"; PermissionSet = "All permissions in PermissionList.GetAll"; CoverageFocus = "Full navigation, all CRUD, all workflows, dashboards, reports, setup data"; ExpectedAccess = "All menu items and all in-page actions available"; NegativeCase = "None for access; still validate bad input and invalid workflow state." },
    [pscustomobject]@{ Persona = "Manager/Approver"; PermissionSet = "View plus approval/review workflow permissions for HR, attendance, payroll, procurement, sales, task management"; CoverageFocus = "Approve/reject/submit/close/cancel/receive/review flows and scoped dashboards"; ExpectedAccess = "Approval actions visible only where status allows them"; NegativeCase = "Create/delete/admin-only settings remain hidden or denied unless explicitly granted." },
    [pscustomobject]@{ Persona = "Employee/Self-service"; PermissionSet = "My attendance, leave/permission requests, my tasks, view own profile where available"; CoverageFocus = "Self-service request creation and own-data visibility"; ExpectedAccess = "Only own records and request actions are available"; NegativeCase = "Cannot view all employees, payroll administration, company settings, or other users' records." },
    [pscustomobject]@{ Persona = "Cashier/POS"; PermissionSet = "POS, product lookup, cart create/edit/checkout, sales order view as needed"; CoverageFocus = "POS cart, checkout, customer/product selection, sales order creation"; ExpectedAccess = "POS workflow works without exposing admin modules"; NegativeCase = "Cannot manage catalog, pricing, inventory adjustments, or security settings unless explicitly granted." },
    [pscustomobject]@{ Persona = "No-permission user"; PermissionSet = "Authenticated but no ERP module permissions"; CoverageFocus = "Authorization hardening"; ExpectedAccess = "No protected menus/actions visible"; NegativeCase = "Direct routes/actions return no data or authorization failure." }
)

$testDataRows = @(
    [pscustomobject]@{ Area = "Company context"; RequiredData = "Parent company, child company, branch, administration, department"; UsedBy = "All scoped modules, organization, employees, reports"; Notes = "Use unique UAT names and avoid production-like records." },
    [pscustomobject]@{ Area = "Security"; RequiredData = "Admin, manager/approver, employee, cashier, no-permission roles and users"; UsedBy = "Role_Permission_UAT and every negative permission scenario"; Notes = "Record usernames and assigned permissions before execution." },
    [pscustomobject]@{ Area = "HR"; RequiredData = "Employees with positions, academic institution, specialization, department/branch assignments"; UsedBy = "Attendance, leave, payroll, tasks, reports"; Notes = "Include active and inactive employees if supported." },
    [pscustomobject]@{ Area = "Attendance/leave"; RequiredData = "Shift, shift assignment, holiday, attendance session, late request, permission request, emergency leave, leave balance"; UsedBy = "Attendance and leave workflows/reports"; Notes = "Use dates covering normal workday, weekend/holiday, and empty report period." },
    [pscustomobject]@{ Area = "Payroll"; RequiredData = "Payroll components, contract, assigned employee contract, loan, salary run period"; UsedBy = "Payroll calculations, approvals, loans"; Notes = "Keep at least one disposable salary run for cancel/approval tests." },
    [pscustomobject]@{ Area = "Catalog/pricing"; RequiredData = "Category, brand, unit, variant, package, product, SKU, price list"; UsedBy = "Catalog, POS, sales, inventory"; Notes = "Include one priced and one unpriced SKU." },
    [pscustomobject]@{ Area = "Inventory"; RequiredData = "Warehouse, batch, current stock, stock operation records"; UsedBy = "Stock in/out/adjustment/reserve/release and POS availability"; Notes = "Use enough stock for reservation, sale, and release tests." },
    [pscustomobject]@{ Area = "Customers/suppliers"; RequiredData = "Customer group, customer, pricing profile, supplier group, supplier"; UsedBy = "Sales, POS, procurement, customer/supplier modules"; Notes = "Include one customer/supplier with complete contact and tax/commercial fields where available." },
    [pscustomobject]@{ Area = "Sales/POS/orders"; RequiredData = "Cart-ready products, default/selected customer, order intake, sales order in multiple statuses"; UsedBy = "POS checkout, order intake accept/reject, sales order workflows"; Notes = "Keep test orders separate by prefix UAT." },
    [pscustomobject]@{ Area = "Procurement"; RequiredData = "Purchase request, RFQ, supplier quotation, purchase order, goods receipt, purchase return, supplier invoice"; UsedBy = "Procurement lifecycle actions"; Notes = "Create linked documents where the UI supports conversion or references." },
    [pscustomobject]@{ Area = "Task management"; RequiredData = "Tasks assigned to different users, comments/actions/checklist, attachment-ready file"; UsedBy = "Task list, view/edit, kanban, notifications, reports"; Notes = "Use one open, one in-progress, and one closed task." },
    [pscustomobject]@{ Area = "General settings"; RequiredData = "Company setting record, currencies including active/default and inactive/non-default"; UsedBy = "System settings, currencies, POS defaults"; Notes = "Capture current defaults before changing values." }
)

$summary = [pscustomobject]@{
    GeneratedAt = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    BackendEndpointRows = $endpointRows.Count
    PermissionActionRows = $permissionClasses.Count
    MenuRouteRows = $menuRows.Count
    BlazorPageRouteRows = $pageRows.Count
    UIPermissionReferences = $uiPermissionRows.Count
    CoverageGaps = ($coverageRows | Where-Object { $_.GapSeverity -eq "Blocker" }).Count
}

$endpointRows | Sort-Object Module, Entity, FeatureAction, HttpMethod | Export-Csv (Join-Path $outputDir "Backend_Functionality_Inventory.csv") -NoTypeInformation -Encoding UTF8
$menuRows | Sort-Object ModuleFamily, Route | Export-Csv (Join-Path $outputDir "Frontend_Menu_Routes.csv") -NoTypeInformation -Encoding UTF8
$pageRows | Sort-Object ModuleFamily, Route | Export-Csv (Join-Path $outputDir "Frontend_Page_Routes.csv") -NoTypeInformation -Encoding UTF8
$permissionClasses | Sort-Object ModuleFamily, Entity, Action | Export-Csv (Join-Path $outputDir "Permission_Inventory.csv") -NoTypeInformation -Encoding UTF8
$coverageRows | Sort-Object GapSeverity, Module, Entity, FeatureAction | Export-Csv (Join-Path $outputDir "UI_Coverage_Gaps.csv") -NoTypeInformation -Encoding UTF8
$masterRows | Sort-Object Module, Entity, Function, TestId | Export-Csv (Join-Path $outputDir "UAT_Master_Matrix.csv") -NoTypeInformation -Encoding UTF8
$roleRows | Export-Csv (Join-Path $outputDir "Role_Permission_UAT.csv") -NoTypeInformation -Encoding UTF8
$testDataRows | Export-Csv (Join-Path $outputDir "Test_Data_Setup.csv") -NoTypeInformation -Encoding UTF8

$readme = @"
# AlAfkar ERP UAT Pack

Generated at: $($summary.GeneratedAt)

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
| Backend endpoint rows | $($summary.BackendEndpointRows) |
| Permission action rows | $($summary.PermissionActionRows) |
| Menu route rows | $($summary.MenuRouteRows) |
| Blazor page route rows | $($summary.BlazorPageRouteRows) |
| UI permission references | $($summary.UIPermissionReferences) |
| Potential UI blocker gaps | $($summary.CoverageGaps) |

## Manual Execution Rules

1. Start with Test_Data_Setup.csv; create or verify all prerequisite data.
2. Execute Role_Permission_UAT.csv first to confirm the test users are configured correctly.
3. Execute UAT_Master_Matrix.csv by module. Fill Result, Evidence, and Notes.
4. Review UI_Coverage_Gaps.csv; anything marked Not represented in UI is a UAT blocker until a visible UI route/action is confirmed or implemented.
5. For every row, run the positive case, invalid input/status case where relevant, permission-denied case, and English/Arabic RTL smoke check.

## Regeneration

From the repository root:

    powershell -NoProfile -ExecutionPolicy Bypass -File .\docs\uat\generate-uat.ps1
"@

Set-Content -Path (Join-Path $outputDir "README.md") -Value $readme -Encoding UTF8

$summary
