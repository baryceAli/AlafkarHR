using AlAfkarERP.Shared.Utilities;
using Microsoft.AspNetCore.Components;
using SharedWithUI.Permissions;

namespace AlAfkarERP.Shared.Pages.Features.Employees.Pages.Enhancements;

public abstract class HrEnhancementPageBase : ComponentBase, IDisposable
{
    [Inject] protected SharedDataService SharedDataService { get; set; } = default!;

    protected string CommonOverline => Text("HR enhancement program", "برنامج تطوير الموارد البشرية");
    protected string PlannedStatus => Text("Planned", "مخطط");
    protected string PlannedStatusSubtitle => Text("Workflow shell ready for backend implementation.", "واجهة مسار العمل جاهزة لتنفيذ الخلفية.");
    protected string BackendStatus => Text("Pending API", "بانتظار الواجهات");
    protected string BackendSubtitle => Text("No fake CRUD is exposed until endpoints are available.", "لا يتم عرض عمليات وهمية قبل توفر واجهات الخلفية.");
    protected string PrimaryActionText => Text("Create workflow", "إنشاء مسار عمل");
    protected string DashboardText => Text("HR Dashboard", "لوحة الموارد البشرية");
    protected string EmployeeListText => Text("Employees", "الموظفون");
    protected string StatusLabel => Text("Release status", "حالة الإصدار");
    protected string PermissionLabel => Text("Menu permission", "صلاحية القائمة");
    protected string BackendLabel => Text("Backend state", "حالة الخلفية");
    protected string CoverageTitle => Text("Current coverage", "التغطية الحالية");
    protected string CoverageSubtitle => Text("What already exists or is being preserved.", "ما هو موجود أو يتم الحفاظ عليه.");
    protected string WorkflowTitle => Text("Planned workflow", "مسار العمل المخطط");
    protected string WorkflowSubtitle => Text("The page boundary for the future implementation.", "حدود الصفحة للتنفيذ القادم.");
    protected string IntegrationTitle => Text("Integration points", "نقاط التكامل");
    protected string IntegrationSubtitle => Text("Modules this page must connect with.", "الوحدات التي يجب ربطها بهذه الصفحة.");
    protected string NextWorkTitle => Text("Next backend work", "عمل الخلفية القادم");
    protected string NextWorkSubtitle => Text("Implementation steps before active CRUD is enabled.", "خطوات التنفيذ قبل تفعيل العمليات.");
    protected string EmptyTitle => Text("Workflow actions are not enabled yet", "إجراءات مسار العمل غير مفعلة بعد");
    protected string EmptyMessage => Text("This page intentionally exposes the feature boundary without fake forms until authorized APIs and handlers are implemented.", "تعرض هذه الصفحة حدود الميزة دون نماذج وهمية إلى أن يتم تنفيذ الواجهات والمعالجات المصرح بها.");

    protected override void OnInitialized()
    {
        SharedDataService.OnChange1 += HandleChangeAsync;
    }

    protected string Text(string en, string ar) => SharedDataService.SelectViewLang(en, ar);

    protected static HrPlannedFeaturePage.FeaturePoint Point(string title, string description, string icon)
        => new(title, description, icon);

    protected HrPlannedFeaturePage.FeatureLink Link(string title, string url, string icon)
        => new(title, url, icon);

    protected IReadOnlyList<HrPlannedFeaturePage.FeatureLink> DefaultEmployeeLinks =>
    [
        Link(DashboardText, "/Employee/Dashboard", "bi-speedometer2"),
        Link(EmployeeListText, "/Employee/Employee/List", "bi-people")
    ];

    protected IReadOnlyList<HrPlannedFeaturePage.FeatureLink> HrProgramLinks =>
    [
        Link(Text("Command Center", "مركز القيادة"), "/HR/CommandCenter", "bi-grid-1x2"),
        Link(Text("Reports", "التقارير"), "/HR/Reports", "bi-bar-chart-line")
    ];

    private async Task HandleChangeAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        SharedDataService.OnChange1 -= HandleChangeAsync;
        GC.SuppressFinalize(this);
    }
}
