using GeneralSettings.GeneralSettings.Models;
using SharedWithUI.GeneralSettings.Dtos;

namespace GeneralSettings.GeneralSettings.Features.HomePageTemplates;

public static class HomePageTemplateDefaults
{
    public static IReadOnlyList<HomePageTemplateSummaryDto> Templates { get; } =
    [
        new()
        {
            Key = HomePageTemplateKeys.CurrentStorefront,
            NameEn = "Current Storefront",
            NameAr = "واجهة المتجر الحالية",
            DescriptionEn = "The current Alafkar store design with hero, service highlights, and product catalog.",
            DescriptionAr = "تصميم متجر الأفكار الحالي مع الواجهة الرئيسية ومزايا الخدمة وكتالوج المنتجات."
        },
        new()
        {
            Key = HomePageTemplateKeys.MinimalistLanding,
            NameEn = "Minimalist Landing",
            NameAr = "صفحة مبسطة",
            DescriptionEn = "A high-contrast, structured landing page based on the uploaded minimalist design.",
            DescriptionAr = "صفحة عالية التباين ومنظمة مستندة إلى التصميم المبسط المرفوع."
        },
        new()
        {
            Key = HomePageTemplateKeys.SoftSaasLanding,
            NameEn = "Soft SaaS Landing",
            NameAr = "صفحة ساس ناعمة",
            DescriptionEn = "A friendly rounded landing page based on the uploaded soft SaaS design.",
            DescriptionAr = "صفحة ودودة بحواف ناعمة مستندة إلى تصميم الساس الناعم المرفوع."
        },
        new()
        {
            Key = HomePageTemplateKeys.BoldEnergeticLanding,
            NameEn = "Bold Energetic Landing",
            NameAr = "صفحة جريئة وحيوية",
            DescriptionEn = "A vibrant, high-motion landing page based on the uploaded bold energetic design.",
            DescriptionAr = "صفحة حيوية وغنية بالحركة مستندة إلى التصميم الجريء المرفوع."
        },
        new()
        {
            Key = HomePageTemplateKeys.CorporateTrustLanding,
            NameEn = "Corporate Trust Landing",
            NameAr = "صفحة الثقة المؤسسية",
            DescriptionEn = "A precise enterprise landing page based on the uploaded corporate trust design.",
            DescriptionAr = "صفحة مؤسسية دقيقة مستندة إلى تصميم الثقة المؤسسية المرفوع."
        },
        new()
        {
            Key = HomePageTemplateKeys.ModernDarkModeLanding,
            NameEn = "Modern Dark Mode Landing",
            NameAr = "صفحة داكنة حديثة",
            DescriptionEn = "A neon glass dark landing page based on the uploaded modern dark mode design.",
            DescriptionAr = "صفحة داكنة بزجاجية مضيئة مستندة إلى التصميم الداكن الحديث المرفوع."
        }
    ];

    public static List<HomePageContentSeed> GetDefaultContent(Guid companyId, string templateKey)
    {
        var content = templateKey switch
        {
            HomePageTemplateKeys.MinimalistLanding => MinimalistLanding(companyId),
            HomePageTemplateKeys.SoftSaasLanding => SoftSaasLanding(companyId),
            HomePageTemplateKeys.BoldEnergeticLanding => BoldEnergeticLanding(companyId),
            HomePageTemplateKeys.CorporateTrustLanding => CorporateTrustLanding(companyId),
            HomePageTemplateKeys.ModernDarkModeLanding => ModernDarkModeLanding(companyId),
            _ => CurrentStorefront(companyId)
        };

        AddTemplateChrome(companyId, templateKey, content);
        return content;
    }

    private static List<HomePageContentSeed> CurrentStorefront(Guid companyId) =>
    [
        Text(companyId, "Hero", "Eyebrow", "Alafkar Catering Store", "متجر الأفكار للإعاشة", 10),
        Text(companyId, "Hero", "Title", "Ready meals for organized field operations.", "وجبات جاهزة لعمليات ميدانية منظمة.", 20),
        Text(companyId, "Hero", "Subtitle", "Browse hot and dry meal options from the product catalog, compare prices, and start with the items that fit seasonal and charitable projects.", "استعرض خيارات الوجبات الساخنة والجافة من كتالوج المنتجات، وقارن الأسعار، وابدأ بالأصناف المناسبة للمشاريع الموسمية والخيرية.", 30),
        Text(companyId, "Hero", "PrimaryButtonText", "Browse products", "استعراض المنتجات", 40),
        Text(companyId, "Hero", "PrimaryButtonUrl", "#products", "#products", 50),
        Text(companyId, "Hero", "SecondaryButtonText", "Login to ERP", "دخول النظام", 60),
        Text(companyId, "Hero", "SecondaryButtonUrl", "/login", "/login", 70),
        Image(companyId, "Hero", "HeroImage", "_content/AlAfkarERP.Shared/images/Meals/alkaramSingleMeal.png", "Alafkar meal", "وجبة الأفكار", 80),
        Text(companyId, "Hero", "Badge", "Ready to order", "جاهزة للطلب", 90),
        Text(companyId, "Hero", "CardTitle", "Fast preparation from request to delivery.", "تجهيز سريع من الطلب إلى التسليم.", 100),
        Text(companyId, "Hero", "CardSubtitle", "Clear product data, visible pricing, and catalog filters help teams choose meals without back-and-forth.", "بيانات منتج واضحة وأسعار ظاهرة وفلاتر كتالوج تساعد الفرق على اختيار الوجبات بسهولة.", 110),
        Text(companyId, "Hero", "DeliveryNote", "Kitchen pickup with delivery availability.", "الاستلام من المطبخ مع إمكانية التوصيل.", 120),
        Text(companyId, "Feature1", "Title", "Season-ready", "مناسبة للمواسم", 130),
        Text(companyId, "Feature1", "Text", "Prepared for seasonal, campaign, and charitable meal programs.", "تجهيز مناسب للمواسم والحملات وبرامج الوجبات الخيرية.", 140),
        Text(companyId, "Feature2", "Title", "Catalog filters", "فلاتر الكتالوج", 150),
        Text(companyId, "Feature2", "Text", "Search by product, category, brand, package, price, and newest items.", "ابحث حسب المنتج أو الصنف أو العلامة أو التغليف أو السعر أو الأحدث.", 160),
        Text(companyId, "Feature3", "Title", "Clear product data", "بيانات منتج واضحة", 170),
        Text(companyId, "Feature3", "Text", "Names, images, package details, and prices come from catalog records.", "الأسماء والصور والتغليف والأسعار تظهر من سجلات الكتالوج.", 180),
        Text(companyId, "Products", "Kicker", "Product menu", "قائمة المنتجات", 190),
        Text(companyId, "Products", "Title", "Choose the right meal package.", "اختر باقة الوجبات المناسبة.", 200),
        Text(companyId, "Products", "Subtitle", "Filter available catalog products and review price, brand, package, and category details before moving into the ERP workflow.", "صف المنتجات المتاحة في الكتالوج وراجع السعر والعلامة والتغليف والصنف قبل الانتقال إلى سير عمل النظام.", 210)
    ];

    private static List<HomePageContentSeed> MinimalistLanding(Guid companyId) =>
    [
        Text(companyId, "Hero", "Eyebrow", "Operational catering partner", "شريك تشغيل الإعاشة", 10),
        Text(companyId, "Hero", "Title", "Reliable meal operations for teams, campaigns, and field sites.", "تشغيل موثوق للوجبات للفرق والحملات والمواقع الميدانية.", 20),
        Text(companyId, "Hero", "Subtitle", "Present your service promise, delivery quality, and product catalog from a professional company-first landing page.", "اعرض وعد الخدمة وجودة التوصيل وكتالوج المنتجات من صفحة احترافية تركز على الشركة.", 30),
        Text(companyId, "Hero", "PrimaryButtonText", "View catalog", "عرض الكتالوج", 40),
        Text(companyId, "Hero", "PrimaryButtonUrl", "#products", "#products", 50),
        Text(companyId, "Hero", "SecondaryButtonText", "Login", "دخول", 60),
        Text(companyId, "Hero", "SecondaryButtonUrl", "/login", "/login", 70),
        Image(companyId, "Hero", "HeroImage", "_content/AlAfkarERP.Shared/images/Meals/alkaramSingleMeal.png", "Prepared meal operations", "تشغيل وجبات جاهزة", 80),
        Text(companyId, "Hero", "Badge", "Company-ready", "جاهزة للشركات", 90),
        Text(companyId, "Hero", "CardTitle", "From planning to fulfillment in one workflow.", "من التخطيط إلى التنفيذ في سير عمل واحد.", 100),
        Text(companyId, "Hero", "CardSubtitle", "Use the storefront to communicate quality while the ERP handles catalog and sales operations.", "استخدم الواجهة لعرض الجودة بينما يدير النظام الكتالوج وعمليات البيع.", 110),
        Text(companyId, "Hero", "DeliveryNote", "Built for repeat orders and organized delivery.", "مصمم للطلبات المتكررة والتوصيل المنظم.", 120),
        Text(companyId, "Feature1", "Title", "Service clarity", "وضوح الخدمة", 130),
        Text(companyId, "Feature1", "Text", "Show what your tenant offers before users browse products.", "اعرض ما تقدمه الشركة قبل استعراض المنتجات.", 140),
        Text(companyId, "Feature2", "Title", "Trusted operations", "تشغيل موثوق", 150),
        Text(companyId, "Feature2", "Text", "Highlight readiness, delivery, and quality commitments.", "أبرز الجاهزية والتوصيل والتزامات الجودة.", 160),
        Text(companyId, "Feature3", "Title", "ERP connected", "مرتبط بالنظام", 170),
        Text(companyId, "Feature3", "Text", "Keep public content editable while product data stays connected to catalog records.", "اجعل المحتوى العام قابلا للتعديل مع بقاء بيانات المنتجات مرتبطة بالكتالوج.", 180),
        Text(companyId, "Products", "Kicker", "Available catalog", "الكتالوج المتاح", 190),
        Text(companyId, "Products", "Title", "Browse company products.", "استعرض منتجات الشركة.", 200),
        Text(companyId, "Products", "Subtitle", "Search, filter, and compare product details from the tenant catalog.", "ابحث وصف وقارن تفاصيل المنتجات من كتالوج الشركة.", 210)
    ];

    private static List<HomePageContentSeed> SoftSaasLanding(Guid companyId) =>
    [
        Text(companyId, "Hero", "Eyebrow", "Featured products", "منتجات مميزة", 10),
        Text(companyId, "Hero", "Title", "Put your best meal packages at the front of the store.", "ضع أفضل باقات الوجبات في واجهة المتجر.", 20),
        Text(companyId, "Hero", "Subtitle", "A visual layout that gives product images, categories, and quick browsing more space.", "تصميم بصري يمنح صور المنتجات والأصناف والتصفح السريع مساحة أكبر.", 30),
        Text(companyId, "Hero", "PrimaryButtonText", "Explore products", "استكشف المنتجات", 40),
        Text(companyId, "Hero", "PrimaryButtonUrl", "#products", "#products", 50),
        Text(companyId, "Hero", "SecondaryButtonText", "ERP login", "دخول النظام", 60),
        Text(companyId, "Hero", "SecondaryButtonUrl", "/login", "/login", 70),
        Image(companyId, "Hero", "HeroImage", "_content/AlAfkarERP.Shared/images/Meals/alkaramSingleMeal.png", "Featured product", "منتج مميز", 80),
        Text(companyId, "Hero", "Badge", "Featured", "مميز", 90),
        Text(companyId, "Hero", "CardTitle", "Product-led public storefront.", "واجهة عامة تقودها المنتجات.", 100),
        Text(companyId, "Hero", "CardSubtitle", "Use stronger product imagery while preserving the ERP catalog filters.", "استخدم صور منتجات أقوى مع الحفاظ على فلاتر كتالوج النظام.", 110),
        Text(companyId, "Hero", "DeliveryNote", "Perfect for visual catalogs and meal bundles.", "مناسب للكتالوجات المرئية وباقات الوجبات.", 120),
        Text(companyId, "Feature1", "Title", "Visual browsing", "تصفح بصري", 130),
        Text(companyId, "Feature1", "Text", "Lead with product photography and clear details.", "ابدأ بصور المنتجات والتفاصيل الواضحة.", 140),
        Text(companyId, "Feature2", "Title", "Fast comparison", "مقارنة سريعة", 150),
        Text(companyId, "Feature2", "Text", "Help buyers compare category, brand, package, and price.", "ساعد المشترين على مقارنة الصنف والعلامة والتغليف والسعر.", 160),
        Text(companyId, "Feature3", "Title", "Catalog powered", "مدعوم بالكتالوج", 170),
        Text(companyId, "Feature3", "Text", "Products still come directly from catalog records.", "تظل المنتجات قادمة مباشرة من سجلات الكتالوج.", 180),
        Text(companyId, "Products", "Kicker", "Shop by product", "تسوق حسب المنتج", 190),
        Text(companyId, "Products", "Title", "Find the package that fits.", "اعثر على الباقة المناسبة.", 200),
        Text(companyId, "Products", "Subtitle", "Use filters to narrow the catalog to the products customers need.", "استخدم الفلاتر لتضييق الكتالوج إلى المنتجات التي يحتاجها العملاء.", 210)
    ];

    private static List<HomePageContentSeed> BoldEnergeticLanding(Guid companyId) =>
    [
        Text(companyId, "Hero", "Eyebrow", "Seasonal campaign", "حملة موسمية", 10),
        Text(companyId, "Hero", "Title", "Launch focused meal campaigns with editable public content.", "أطلق حملات وجبات مركزة بمحتوى عام قابل للتعديل.", 20),
        Text(companyId, "Hero", "Subtitle", "Use this template for Ramadan, Hajj, charitable programs, corporate drives, and limited-time initiatives.", "استخدم هذا القالب لرمضان والحج والبرامج الخيرية وحملات الشركات والمبادرات المحدودة.", 30),
        Text(companyId, "Hero", "PrimaryButtonText", "Start browsing", "ابدأ التصفح", 40),
        Text(companyId, "Hero", "PrimaryButtonUrl", "#products", "#products", 50),
        Text(companyId, "Hero", "SecondaryButtonText", "Admin login", "دخول الإدارة", 60),
        Text(companyId, "Hero", "SecondaryButtonUrl", "/login", "/login", 70),
        Image(companyId, "Hero", "HeroImage", "_content/AlAfkarERP.Shared/images/Meals/alkaramSingleMeal.png", "Campaign meal package", "باقة وجبات الحملة", 80),
        Text(companyId, "Hero", "Badge", "Limited campaign", "حملة محدودة", 90),
        Text(companyId, "Hero", "CardTitle", "Seasonal message, same catalog engine.", "رسالة موسمية بنفس محرك الكتالوج.", 100),
        Text(companyId, "Hero", "CardSubtitle", "Change copy and imagery for each initiative while keeping product browsing stable.", "غير النصوص والصور لكل مبادرة مع بقاء تصفح المنتجات مستقرا.", 110),
        Text(companyId, "Hero", "DeliveryNote", "Ready for high-volume campaign planning.", "جاهز لتخطيط الحملات عالية الحجم.", 120),
        Text(companyId, "Feature1", "Title", "Campaign message", "رسالة الحملة", 130),
        Text(companyId, "Feature1", "Text", "Put the seasonal offer or initiative clearly above the catalog.", "ضع العرض الموسمي أو المبادرة بوضوح فوق الكتالوج.", 140),
        Text(companyId, "Feature2", "Title", "Quick action", "إجراء سريع", 150),
        Text(companyId, "Feature2", "Text", "Guide users straight to relevant products.", "وجه المستخدمين مباشرة إلى المنتجات المناسبة.", 160),
        Text(companyId, "Feature3", "Title", "Editable media", "وسائط قابلة للتعديل", 170),
        Text(companyId, "Feature3", "Text", "Swap campaign images without changing code.", "بدل صور الحملة دون تغيير الكود.", 180),
        Text(companyId, "Products", "Kicker", "Campaign products", "منتجات الحملة", 190),
        Text(companyId, "Products", "Title", "Select products for this initiative.", "اختر منتجات هذه المبادرة.", 200),
        Text(companyId, "Products", "Subtitle", "Filter the tenant catalog to match campaign needs and budgets.", "صف كتالوج الشركة بما يناسب احتياجات الحملة وميزانيتها.", 210)
    ];

    private static List<HomePageContentSeed> CorporateTrustLanding(Guid companyId) =>
    [
        Text(companyId, "Hero", "Eyebrow", "Fast catalog", "كتالوج سريع", 10),
        Text(companyId, "Hero", "Title", "Search, filter, and choose products faster.", "ابحث وصف واختر المنتجات بسرعة أكبر.", 20),
        Text(companyId, "Hero", "Subtitle", "A compact homepage for tenants that want the product catalog to be the main experience.", "صفحة مختصرة للشركات التي تريد أن يكون كتالوج المنتجات هو التجربة الأساسية.", 30),
        Text(companyId, "Hero", "PrimaryButtonText", "Go to catalog", "الانتقال للكتالوج", 40),
        Text(companyId, "Hero", "PrimaryButtonUrl", "#products", "#products", 50),
        Text(companyId, "Hero", "SecondaryButtonText", "Login", "دخول", 60),
        Text(companyId, "Hero", "SecondaryButtonUrl", "/login", "/login", 70),
        Image(companyId, "Hero", "HeroImage", "_content/AlAfkarERP.Shared/images/Meals/alkaramSingleMeal.png", "Catalog product", "منتج الكتالوج", 80),
        Text(companyId, "Hero", "Badge", "Compact", "مختصر", 90),
        Text(companyId, "Hero", "CardTitle", "Less copy, more catalog.", "نص أقل وكتالوج أكثر.", 100),
        Text(companyId, "Hero", "CardSubtitle", "Keep the top of the page simple and push users into the product grid.", "اجعل أعلى الصفحة بسيطا وادفع المستخدمين إلى شبكة المنتجات.", 110),
        Text(companyId, "Hero", "DeliveryNote", "Optimized for repeat buyers.", "محسن للمشترين المتكررين.", 120),
        Text(companyId, "Feature1", "Title", "Quick search", "بحث سريع", 130),
        Text(companyId, "Feature1", "Text", "Users reach product filters quickly.", "يصل المستخدمون إلى فلاتر المنتجات بسرعة.", 140),
        Text(companyId, "Feature2", "Title", "Compact intro", "مقدمة مختصرة", 150),
        Text(companyId, "Feature2", "Text", "Use only the text needed to orient shoppers.", "استخدم النص اللازم فقط لتوجيه المتسوقين.", 160),
        Text(companyId, "Feature3", "Title", "Stable catalog", "كتالوج ثابت", 170),
        Text(companyId, "Feature3", "Text", "The same public product flow remains available.", "يبقى نفس مسار المنتجات العام متاحا.", 180),
        Text(companyId, "Products", "Kicker", "Catalog", "الكتالوج", 190),
        Text(companyId, "Products", "Title", "Browse products.", "استعرض المنتجات.", 200),
        Text(companyId, "Products", "Subtitle", "Use search and filters to find the right product quickly.", "استخدم البحث والفلاتر للعثور على المنتج المناسب بسرعة.", 210)
    ];

    private static List<HomePageContentSeed> ModernDarkModeLanding(Guid companyId) =>
    [
        Text(companyId, "Hero", "Eyebrow", "Version 5.0 is Live", "الإصدار 5.0 متاح الآن", 10),
        Text(companyId, "Hero", "Title", "Next-gen workflow orchestration.", "تنسيق سير عمل من الجيل التالي.", 20),
        Text(companyId, "Hero", "Subtitle", "A dark, glassy template for technology-forward brands with luminous calls to action.", "قالب داكن وزجاجي للعلامات التقنية مع دعوات إجراء مضيئة.", 30),
        Text(companyId, "Hero", "PrimaryButtonText", "Get Started", "ابدأ الآن", 40),
        Text(companyId, "Hero", "PrimaryButtonUrl", "#pricing", "#pricing", 50),
        Text(companyId, "Hero", "SecondaryButtonText", "Explore Platform", "استكشف المنصة", 60),
        Text(companyId, "Hero", "SecondaryButtonUrl", "#features", "#features", 70),
        Image(companyId, "Hero", "HeroImage", "https://lh3.googleusercontent.com/aida-public/AB6AXuAjeYOzP3890pCOKWLFb2A65_3QDzsc5ynJdh_zw4YUEdcrpugDJJZ2pjMzZwYVo_NUrL-GQKBLJhqfXhK9t8S2lgyfH3ooo-6Khknb3s_WG3RPPMQVrrptfouTlKj_ozcGUsAoKfgOM6M_zAfurFmEe1_39Tdr5tCv0loBZy3zAyMumxe_0r4FgLuHeLL2VGIBcW4Y3Z5ZNsc1VcyvNHBPFbWc0grxNfYwLOB9PinFduR1XH0DmCe8qBAZXH-4_GV9WgbMNPxcj6-T", "Dark futuristic dashboard", "لوحة داكنة مستقبلية", 80),
        Text(companyId, "Hero", "Badge", "Neon glass", "زجاج مضيء", 90),
        Text(companyId, "Hero", "CardTitle", "Autonomous workflows that adapt to teams.", "سير عمل ذاتي يتكيف مع الفرق.", 100),
        Text(companyId, "Hero", "CardSubtitle", "Visualize complex operations in a precise dark-mode experience.", "اعرض العمليات المعقدة في تجربة داكنة دقيقة.", 110),
        Text(companyId, "Hero", "DeliveryNote", "Built for high-stress operational environments.", "مصمم للبيئات التشغيلية عالية الضغط.", 120),
        Text(companyId, "Feature1", "Title", "Autonomous Workflows", "سير عمل ذاتي", 130),
        Text(companyId, "Feature1", "Text", "AI-driven task orchestration that learns from team behavior.", "تنسيق مهام مدعوم بالذكاء يتعلم من سلوك الفريق.", 140),
        Text(companyId, "Feature2", "Title", "Instant Insights", "رؤى فورية", 150),
        Text(companyId, "Feature2", "Text", "Visualize complex datasets in milliseconds.", "اعرض البيانات المعقدة خلال لحظات.", 160),
        Text(companyId, "Feature3", "Title", "Quantum Shield", "درع متقدم", 170),
        Text(companyId, "Feature3", "Text", "End-to-end encryption for every data byte.", "تشفير شامل لكل جزء من البيانات.", 180),
        Text(companyId, "Products", "Kicker", "Platform", "المنصة", 190),
        Text(companyId, "Products", "Title", "Deploy a next-generation public experience.", "انشر تجربة عامة من الجيل التالي.", 200),
        Text(companyId, "Products", "Subtitle", "Use this template when the brand needs a premium, dark, technology-forward presence.", "استخدم هذا القالب عندما تحتاج العلامة إلى حضور تقني داكن وفاخر.", 210)
    ];

    private static HomePageContentSeed Text(Guid companyId, string section, string field, string en, string ar, int sortOrder)
        => new(companyId, section, field, "Text", en, ar, string.Empty, string.Empty, string.Empty, sortOrder, true);

    private static HomePageContentSeed Image(Guid companyId, string section, string field, string imagePath, string altEn, string altAr, int sortOrder)
        => new(companyId, section, field, "Image", string.Empty, string.Empty, imagePath, altEn, altAr, sortOrder, true);

    private static void AddTemplateChrome(Guid companyId, string templateKey, List<HomePageContentSeed> content)
    {
        var brand = templateKey switch
        {
            HomePageTemplateKeys.MinimalistLanding => "Minimal",
            HomePageTemplateKeys.SoftSaasLanding => "FlowSpace",
            HomePageTemplateKeys.BoldEnergeticLanding => "Pulse",
            HomePageTemplateKeys.CorporateTrustLanding => "TrustCo",
            HomePageTemplateKeys.ModernDarkModeLanding => "Nexus",
            _ => "ALAFKAR"
        };

        content.AddRange(
        [
            Text(companyId, "Nav", "Brand", brand, brand, 300),
            Text(companyId, "Nav", "LoginText", "Login", "Login", 310)
        ]);

        if (templateKey == HomePageTemplateKeys.CurrentStorefront)
            return;

        content.AddRange(
        [
            Text(companyId, "Nav", "Link1", "Features", "Features", 320),
            Text(companyId, "Nav", "Link1Url", "#features", "#features", 330),
            Text(companyId, "Nav", "Link2", "Solutions", "Solutions", 340),
            Text(companyId, "Nav", "Link2Url", "#solutions", "#solutions", 350),
            Text(companyId, "Nav", "Link3", "Pricing", "Pricing", 360),
            Text(companyId, "Nav", "Link3Url", "#pricing", "#pricing", 370),
            Text(companyId, "Nav", "CtaText", "Get Started", "Get Started", 380),
            Text(companyId, "Nav", "CtaUrl", "#cta", "#cta", 390),
            Text(companyId, "Stat1", "Value", "99.9%", "99.9%", 400),
            Text(companyId, "Stat1", "Label", "Uptime", "Uptime", 410),
            Text(companyId, "Stat2", "Value", "24/7", "24/7", 420),
            Text(companyId, "Stat2", "Label", "Support", "Support", 430),
            Text(companyId, "Stat3", "Value", "5x", "5x", 440),
            Text(companyId, "Stat3", "Label", "Faster launch", "Faster launch", 450),
            Text(companyId, "Cta", "Title", "Ready to launch your homepage?", "Ready to launch your homepage?", 460),
            Text(companyId, "Cta", "Text", "Tune every visible section from tenant settings and publish the active template instantly.", "Tune every visible section from tenant settings and publish the active template instantly.", 470),
            Text(companyId, "Cta", "ButtonText", "Get Started", "Get Started", 480),
            Text(companyId, "Cta", "ButtonUrl", "#", "#", 490),
            Text(companyId, "Footer", "Text", "A configurable tenant homepage.", "A configurable tenant homepage.", 500),
            Text(companyId, "Footer", "Copyright", "© 2026 Alafkar. All rights reserved.", "© 2026 Alafkar. All rights reserved.", 510)
        ]);
    }
}
