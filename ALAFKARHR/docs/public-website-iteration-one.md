# Public company website — iteration one

Branch: `codex/public-company-website`.

## Implementation

The approved `_4`, `_1`, `_2`, and `_3` designs became native Razor pages at `/`, `/services`, `/packages`, and `/projects`. Shared header, footer, SEO, section components, and locally validated demonstration forms live in `UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/`. Content remains static component parameters/local state; there is no database migration, CMS, business API fetching, payment, email, or request persistence.

Additional public destinations are `/about`, `/infrastructure`, `/contact`, `/request-proposal`, `/privacy-policy`, and `/terms-and-conditions`. About and infrastructure reuse approved sections; contact and proposal reuse the approved inquiry visual language. Legal pages are explicitly drafts, marked `noindex`, and excluded from the sitemap.

Projects/media links lead to `/projects#media-center`; media downloads/video lead to the missing-resource notice at `/projects#media-resources`; news archive leads to `/projects#news-archive`; direct contact leads to `/contact#direct-contact`; quick inquiry leads to `/#quick-inquiry`. Package CTAs preselect package/service through query parameters at `/request-proposal`. No placeholder `href="#"` remains. Existing storefront moved to `/store`; authentication and ERP services/permissions remain intact.

`Components/App.razor` selects the public Arabic RTL document, scoped website assets, and public router on initial requests, while keeping the ERP shell on business routes. `Program.cs` supplies sitemap/robots endpoints and status-code re-execution. Public content and metadata are server prerendered before an interactive connection. Forms stay disabled until local interactive handlers are ready.

## Assets and source decisions

Every supplied `code.html`, `DESIGN.md`, and available `screen.png` was inspected before implementation. HTML values govern concrete colors, radii, spacing, and section geometry where general DESIGN prose differs. The referenced Latin IBM Plex Sans does not contain Arabic glyphs; the explicit DESIGN requirement for IBM Plex Sans Arabic governs Arabic text. Both exact font families and Material Symbols are local, with licence files.

`wwwroot/website/asset-inventory.json` maps all 28 referenced image URLs. Twenty-seven exact images were obtained and stored as lossless WebP, preserving pixel quality and crop behavior. The shared header/profile image returned HTTP 403; a labelled unavailable-image slot is retained rather than substituting a logo. `font-inventory.json` records fonts. Website CSS is isolated from authenticated ERP styles. Regenerate it with `npm ci` then `npm run build` in `tools/public-website`; the generated CSS is checked in and no Node dependency is required at runtime.

The packages `_2/screen.png` is a 28-byte error payload (`<FIFE Image failed to fetch>`), not an image. No visual screenshot reference exists for that page; its approved HTML and DESIGN were used. No referenced PDF, video, or media archive was supplied.

## Verification

- `dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj -v:q` passed: zero errors and zero warnings in the final incremental build. Earlier full compilation reported existing nullable/unused-code warnings.
- Ran the Development web host at `http://localhost:5148`. Production deployment/publishing was not exercised.
- Captured all four approved pages at 1920, 1280, 768, and 390 CSS pixels. Compared landing, services, and projects with their valid 1920-pixel reference captures (landing 6847, services 5902, projects 8499 source heights); the captures appear to use a scaled desktop CSS viewport, so both 1920 and 1280 widths were checked. Inspected mobile captures and corrected wrapping, menu positioning, counters, layout overflow, typography, images, and spacing.
- The responsive browser report covers 35 route/viewport checks: every public page at desktop, tablet, mobile, plus the four at 1280 and a missing route. All have one H1, initial Arabic RTL, no horizontal overflow, no broken loaded images, no unlabelled form fields, and no placeholder links. Screenshots and report are under `output/playwright/`.
- HTTP verification confirms initial content, unique titles/descriptions, canonical/OG/Twitter metadata, valid WebPage JSON-LD, all 33 distinct link/query/fragment destinations, public-only sitemap, missing-page HTTP 404, and preserved `/store` and `/login` ERP shells. See `website-http-verification.json`.
- Checked direct navigation and refresh, mobile navigation by keyboard with a visible focus outline, landing/package/project filters, news expansion, proposal query preselection, empty-form native validation, all three local demonstration forms, and reduced-motion CSS (transition duration zero). Proposal network inspection contained only the normal Blazor connection and no business submission.

## Review before production

Visual conversion is complete within the supplied resources, but pixel equality is not claimed. Arabic font wrapping differs from the screenshot fallback font; explicit static/demo notices and unavailable-resource notices add height. The unavailable header image and invalid packages reference remain the concrete asset/reference limitations.

Approved design copy contains unverified business statistics, certifications (including HACCP/ISO references), partner names, project/date claims, addresses, and contact details. They were preserved as source content, not established as verified facts. Source phone numbers contain zeros; the `info`, `sales`, and `media` mailboxes at `najehah.com.sa` and social/contact links need confirmation. Contacts link to a notice rather than invoking placeholder phone/email endpoints. Capacity claims conflict: services says 120,000+ daily while packages says 500,000+ daily. Some supplied dates/copyright remain 2025 / 1446. Review these before publishing.

New missing-resource/contact/interaction explanations are draft website copy. Privacy and terms contain only draft review notices and a factual description of this static iteration, not invented legal commitments. Obtain approved legal documents before removing `noindex`.

Canonical, social image, and sitemap origin use `PublicWebsite:Origin` when supplied, otherwise existing `ApiConfig:WebSiteURL`. Current production fallback is `https://www.alafkarsa.com`; Development fallback is `https://localhost:7153`. Confirm the real public origin and configure `PublicWebsite__Origin` (environment variable) or the equivalent configuration key before production publication. No domain was invented, no indexing/ranking or performance score is promised.

## Changed paths

- `UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/` — pages, shared sections, local interactions, routing, metadata.
- `UI/AlAfkarERP/AlAfkarERP.Web/wwwroot/website/` — scoped CSS, exact local images/fonts, inventories and licences.
- `UI/AlAfkarERP/AlAfkarERP.Web/Components/App.razor` and `Program.cs` — rendering shell, sitemap, robots, missing-route handling.
- `UI/AlAfkarERP/AlAfkarERP.Shared/Pages/Home.razor` — storefront route `/store`.
- `tools/public-website/` — reproducible scoped CSS compiler and visual QA script.
- `docs/public-website-iteration-one.md` — delivery and review notes.

Work is uncommitted on the dedicated branch. Pre-existing `Templates/` and `approved_WebDesign/` resources were preserved.

To repeat browser checks with a running host on port 5148, open a Playwright CLI browser session and use `run-code --filename=tools/public-website/verify-visual.js`. Use `verify-interactions.js` for local forms, media expansion, query parameters, and reduced motion.
