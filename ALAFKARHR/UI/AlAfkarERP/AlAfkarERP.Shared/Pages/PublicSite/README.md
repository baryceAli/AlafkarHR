# Corporate public website

The public site uses `CorporateLayout`. `/` is the corporate homepage; the existing product catalog now lives at `/store`. Routes for about, services, projects, project details, partners, media, article details and contact are defined in `CorporatePage` and `Contact`.

## Content approval

`SiteContent.cs` is the structured content source. Add approved images/video, statistics, projects, news, partner logos, contact details, map URL and social links there. Projects, news, partners and statistics require `Approved = true` before rendering. Empty sections are hidden on the homepage and show a localized empty state on the listing pages. No sample customers or impact metrics are published.

Vision, mission and management copy on `/about` is visibly marked as editorial draft. Obtain company approval before removing that label. Service options on `/store` have no invented prices. Event coverage uses an approved `SiteNews` record and date; no date is inferred from “Thursday”.

## Inquiry persistence and deployment

The anonymous `POST /api/v1/Settings/public/inquiries` endpoint stores inquiries in `GeneralSettings.WebsiteInquiries`. Apply the generated `AddWebsiteInquiries` migration using the API as the startup project through the project's normal migration/deployment workflow. Development already runs GeneralSettings migrations at startup; production requires its normal migration step. The implementation does not send email or add a content-management interface.

Set the API's `PublicWebsite:CompanyId` to the website's company, matching the web app's `ApiConfig:CompanyId`. The supplied default matches the existing web configuration. The API resolves company scope from configuration and verifies the company is available. A visitor cannot submit a company or branch ID. Product inquiries additionally verify company ownership, public visibility, sellability and active catalog relations through a Catalog contract. Public catalog results and filter metadata use the configured company while retaining existing API shapes.

Inquiries are company-level and have no branch assignment. Creation is intentionally anonymous and does not grant access to protected ERP actions. Existing ERP permissions, branch access and scoped branch-role workflows are unchanged. No new read/list/update inquiry API is exposed.

## Verification checklist

- Build the web app and API; do not launch the app or browser unless explicitly authorized with `Run and verify visually`.
- When runtime verification is authorized: visit every route in both languages, switch languages after initial rendering, navigate from details back to listings, and test missing slugs.
- Check desktop, tablet and mobile navigation, keyboard focus, skip link and reduced-motion behavior.
- Confirm unapproved/empty collections stay hidden on the homepage; listings have empty states; available media has alternative text and user-controlled video playback.
- Test required fields, malformed email/mobile, too-short messages, submission progress, saved success, API failure/retry and a second request.
- Verify service/package query context, product reference and source page survive submission; unknown query values fall back safely. The backend must reject a missing product, inactive/unlisted product, foreign-company product and malformed direct API request.
- Inspect stored inquiry data after migration, and confirm the catalog, prices, filtering and pagination still use the existing services. A catalog failure must show retry instead of an empty-success state.
- Confirm ERP screens have no visual changes from the scoped `.af-site` styles.

Runtime and visual checks are not claimed by compile-time verification.
