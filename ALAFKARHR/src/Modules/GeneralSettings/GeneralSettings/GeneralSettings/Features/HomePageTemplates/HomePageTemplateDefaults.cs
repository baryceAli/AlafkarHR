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
            Key = HomePageTemplateKeys.CorporateShowcase,
            NameEn = "Corporate Showcase",
            NameAr = "واجهة الشركة",
            DescriptionEn = "A company-first homepage for services, trust points, and operations.",
            DescriptionAr = "صفحة تركز على الشركة والخدمات ونقاط الثقة والتشغيل."
        },
        new()
        {
            Key = HomePageTemplateKeys.ProductHighlight,
            NameEn = "Product Highlight",
            NameAr = "إبراز المنتجات",
            DescriptionEn = "A visual product-led layout with strong catalog entry points.",
            DescriptionAr = "تصميم بصري يبرز المنتجات مع مداخل واضحة للكتالوج."
        },
        new()
        {
            Key = HomePageTemplateKeys.CampaignLanding,
            NameEn = "Campaign Landing",
            NameAr = "صفحة الحملات",
            DescriptionEn = "A seasonal campaign layout for offers, initiatives, and fast calls to action.",
            DescriptionAr = "تصميم للحملات الموسمية والعروض والمبادرات ودعوات الإجراء السريعة."
        },
        new()
        {
            Key = HomePageTemplateKeys.MinimalCatalog,
            NameEn = "Minimal Catalog",
            NameAr = "كتالوج مختصر",
            DescriptionEn = "A compact homepage focused on search, filtering, and browsing products.",
            DescriptionAr = "صفحة مختصرة تركز على البحث والتصفية واستعراض المنتجات."
        }
    ];

    public static List<HomePageContentSeed> GetDefaultContent(Guid companyId, string templateKey)
        => templateKey switch
        {
            HomePageTemplateKeys.CorporateShowcase => CorporateShowcase(companyId),
            HomePageTemplateKeys.ProductHighlight => ProductHighlight(companyId),
            HomePageTemplateKeys.CampaignLanding => CampaignLanding(companyId),
            HomePageTemplateKeys.MinimalCatalog => MinimalCatalog(companyId),
            _ => CurrentStorefront(companyId)
        };

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

    private static List<HomePageContentSeed> CorporateShowcase(Guid companyId) =>
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

    private static List<HomePageContentSeed> ProductHighlight(Guid companyId) =>
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

    private static List<HomePageContentSeed> CampaignLanding(Guid companyId) =>
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

    private static List<HomePageContentSeed> MinimalCatalog(Guid companyId) =>
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

    private static HomePageContentSeed Text(Guid companyId, string section, string field, string en, string ar, int sortOrder)
        => new(companyId, section, field, "Text", en, ar, string.Empty, string.Empty, string.Empty, sortOrder, true);

    private static HomePageContentSeed Image(Guid companyId, string section, string field, string imagePath, string altEn, string altAr, int sortOrder)
        => new(companyId, section, field, "Image", string.Empty, string.Empty, imagePath, altEn, altAr, sortOrder, true);
}
