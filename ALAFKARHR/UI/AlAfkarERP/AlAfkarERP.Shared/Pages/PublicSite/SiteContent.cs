namespace AlAfkarERP.Shared.Pages.PublicSite;

public record SiteText(string En, string Ar);
public record SiteService(string Key, string Icon, SiteText Title, SiteText Description, SiteText Outcome);
public record SiteMedia(string Url, SiteText Alt, bool Video = false, string? Poster = null);
public record SiteProject(string Slug, string ServiceKey, SiteText Title, SiteText Summary, SiteText Challenge, SiteText Solution, SiteText Impact, IReadOnlyList<SiteMedia> Media, bool Approved = false);
public record SiteDownload(string Url, SiteText Label);
public record SiteNews(string Slug, SiteText Title, SiteText Summary, SiteText Body, DateOnly Date, IReadOnlyList<SiteMedia> Media, IReadOnlyList<SiteDownload> Downloads, bool Approved = false);
public record SitePartner(string Name, string LogoUrl, bool Approved = false);
public record SiteStatistic(int Value, SiteText Label, bool Approved = false);
public record SitePackage(string Key, SiteText Title, SiteText Description, decimal? Price = null);

// Publish only approved company assets and facts. Empty collections are intentional.
public static class SiteContent
{
    public static SiteText Brand { get; } = new("Successful Ideas", "الأفكار الناجحة");
    public static SiteText Headline { get; } = new("Ideas that become solutions. Solutions that make an impact.", "نحوّل الأفكار إلى حلول تصنع الأثر");
    public static SiteText Intro { get; } = new("We develop institutional solutions, initiatives and digital platforms, and build capabilities to support growth across sectors and nonprofit organizations.", "نطوّر الحلول المؤسسية والمبادرات والمنصات الرقمية، ونبني القدرات لدعم نمو القطاعات المختلفة والقطاع غير الربحي.");
    public static string? HeroImage { get; } = null;
    public static string? HeroVideo { get; } = null;
    public static string? Email { get; } = null;
    public static string? Phone { get; } = null;
    public static SiteText? Address { get; } = null;
    public static string? MapUrl { get; } = null;
    public static IReadOnlyList<SiteDownload> SocialLinks { get; } = [];
    public static IReadOnlyList<SiteStatistic> Statistics { get; } = [];
    public static IReadOnlyList<SiteProject> Projects { get; } = [];
    public static IReadOnlyList<SitePartner> Partners { get; } = [];
    public static IReadOnlyList<SiteNews> News { get; } = [];
    public static IReadOnlyList<SiteService> Services { get; } = [
        new("innovation", "bi-lightbulb", new("Innovation & institutional solutions", "الابتكار والحلول المؤسسية"), new("Operating models and growth strategies shaped around your goals.", "تطوير النماذج التشغيلية واستراتيجيات النمو بما يتناسب مع أهداف الجهات."), new("Operating models, growth roadmaps and actionable solutions.", "نماذج تشغيلية، وخطط نمو، وحلول قابلة للتطبيق.")),
        new("initiatives", "bi-diagram-3", new("Initiative & project development", "إدارة وتطوير المبادرات والمشاريع"), new("Planning and oversight for distinctive programs and development projects.", "تخطيط وإدارة البرامج والمشاريع النوعية والتنموية والإشراف عليها."), new("Project plans, delivery coordination and impact measurement.", "خطط مشاريع، وتنسيق التنفيذ، وقياس الأثر.")),
        new("digital", "bi-code-slash", new("Digital transformation", "التحول والتطوير الرقمي"), new("Smart platforms, enterprise systems and technical solutions.", "بناء المنصات والأنظمة الذكية والحلول التقنية لدعم تطوير الأعمال."), new("Digital platforms and integrated business workflows.", "منصات رقمية وسير عمل مؤسسي متكامل.")),
        new("capabilities", "bi-people", new("Capability building & consulting", "بناء القدرات والاستشارات"), new("Consulting and development programs for institutions and nonprofit organizations.", "برامج استشارية وتطويرية للمؤسسات والقطاع غير الربحي."), new("Development programs and practical advisory guidance.", "برامج تطويرية، وإرشاد استشاري عملي."))
    ];
    // Service request options; prices remain unset until commercially approved.
    public static IReadOnlyList<SitePackage> Packages { get; } = Services.Select(x => new SitePackage(x.Key, x.Title, x.Description)).ToList();
    public static IReadOnlyList<SiteText> Values { get; } = [new("Innovation", "الابتكار"), new("Quality", "الجودة"), new("Transparency", "الشفافية"), new("Sustainability", "الاستدامة")];
}
