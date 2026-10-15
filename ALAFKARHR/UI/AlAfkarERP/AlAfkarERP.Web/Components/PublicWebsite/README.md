# Alafkar Public Website / PublicWebsite

The selected institutional design (formerly design 2) is now the sole public presentation. Eight Arabic pages use static document-approved content, local SVG icons/fonts, responsive layouts, motion and explicit WhatsApp handoffs. This code change does not deploy the website.

The activities page displays the five business lines previously shown on About, preserving their content, icons and order. About retains the company profile download. Activities appears immediately after About in the shared header/mobile/footer navigation.

## Canonical routes and migration

| Page | Canonical | Former preview (301) | Former English (301) |
|---|---|---|---|
| Home | `/` | `/design-2` | `/en` |
| About | `/about` | `/design-2/about` | `/en/about` |
| Activities (أنشطتنا) | `/activities` | `/design-2/activities` | `/en/activities` |
| Services | `/services` | `/design-2/services` | `/en/services` |
| Projects | `/projects` | `/design-2/projects` | `/en/projects` |
| Partners | `/partners` | `/design-2/partners` | `/en/partners` |
| Contact | `/contact` | `/design-2/contact` | `/en/contact` |
| Request proposal | `/request-proposal` | `/design-2/request-proposal` | `/en/request-proposal` |

| Former Arabic path (also `/en` + path) | Result |
|---|---|
| `/services/iftar-haram` | 301 `/services#iftar` |
| `/services/suqya` | 301 `/services#suqya` |
| `/services/meals` | 301 `/services#meals` |
| `/services/food-baskets` | 301 `/services#baskets` |
| `/services/hajj-season` | 301 `/services#hajj` |
| `/infrastructure` | 301 `/services#infrastructure` |
| `/packages`, `/privacy-policy`, `/terms-and-conditions` | 410, styled Arabic retirement message |
| `/services/pilgrim-housing` | 404, remains unpublished |
| Unknown preview/public URL | 404 via existing `/website-not-found` status-page re-execution |

Redirects retain query values (including quotation `service` and campaign parameters), inserting them before destination anchors. Obsolete `websiteEditor`, `websitePreview`, `previewSession`, and `editorSession` parameters are removed. Recognized canonical URLs with those parameters redirect to their cleaned URL. Trailing slashes/case on recognized routes normalize to canonical spelling. Unrecognized English paths are not broadly redirected. No global catch-all was added. ERP, login and `/store` retain their existing routing and resources.

## Static content workflow and boundaries

- Edit approved copy/collections in `WebsiteContent.cs`; update page composition in `Website*.razor` only when required. Review against the Word source, build Shared/Web, and use the checklist below. Pending editorial items never become published copy.
- Source: `D:/Development/AlAfkar/HR/Website Content/AlAfkar_Website_Content.docx`, QAEED, 10 October 2026. Layout source: `D:/Development/AlAfkar/HR/second_template/stitch_alafkar_corporate_website_design/_1`–`_6` and its institutional `DESIGN.md`. The quotation page follows that design's conventions. Source directories/documents are retained.
- `/PublicWebsite/ControlPanel` is a permission-gated inactive-editor notice. It exposes website and authorized configuration links only. Drafts, publication history, media, backend CMS APIs, permissions and configuration/storage remain preserved. Publishing a CMS snapshot does **not** change this static website.
- Removed old rendering components, styles, image-effects/editor scripts, preview-session capabilities and frontend registrations. Retained `/website/media/{id}` for published stored media, original hashed website assets for saved CMS references, branding, fonts, `WebsiteRuntimeSettings` and `WebsiteWhatsApp` utilities. No schema/API changes or migrations.
- Selected assets live in `wwwroot/website/images/`; `wwwroot/website/asset-sources.json` records original local files, original URLs, dimensions and SHA-256 hashes. All six images are illustrative, not evidence of actual projects/team. The Qur'an project uses a local inline SVG. No invented logos, testimonials or downloads.
- `website.css` scopes tokens/classes to `pw-` / `.pw-site`. `motion.js` connects/disposes through `WebsiteLayout`; its dataset contract is `pwMotion` / `pwMotionState` (HTML `data-pw-motion` / `data-pw-motion-state`). The stylesheet and module URL use the existing asset resolver. Footer motion choice is session-only; OS reduced-motion changes always take priority. No external motion/icon dependencies.
- Canonical public links use Blazor navigation within the existing circuit. `PublicRoutes` keys `RouteView` by pathname to remount page/layout, metadata, active navigation and motion on page changes. Query/fragment changes preserve form state. Mobile links close their menu locally, including same-page anchors. Error-page exits and non-public destinations retain full document navigation; public reconnect uses a dedicated handler with a three-second grace period; persistent failures retain Arabic retry/resume actions, and the shared ERP handler remains unchanged.
- `WebsiteSeo` supplies unique Arabic title/description, canonical URLs, Arabic OG/Twitter metadata, and homepage Organization data from approved contact facts. No English alternates. Canonical pages are indexable. 404/410 responses carry noindex metadata and headers.
- `WebsiteRoutes.Paths` drives robots allow-rules and sitemap independently of CMS publications. The runtime-origin helper still uses configured public settings with configuration fallback. Redirects, errors, retired pages, ERP and `/store` are absent from the sitemap.

## Document content alignment

Approved Arabic copy and page-specific collections are centralized in `WebsiteContent.cs`, based on `AlAfkar_Website_Content.docx` (QAEED, 10 October 2026). Components retain operational labels and accessible descriptions; illustrative-image badges and notices are no longer displayed. Source mapping:

| Page | Word document sections | Implementation |
|---|---|---|
| Home | الصفحة الرئيسية، الأقسام الأول–التاسع | Original sequence: hero/metrics, introduction/short values, six services, workflow, five reasons, three explicitly selected featured projects, partners, five home FAQs, closing CTA. |
| About | صفحة من نحن | Introduction, three story stages, complete founder statement/attribution, vision/mission, full values, team/departments, audiences. No extra workflow. |
| Services | صفحة الخدمات + صفحة البنية التشغيلية | Five main service descriptions/inclusions, applicable figures and iftar audience, four supporting services, infrastructure/safety/reports, two answered service FAQs. Includes 500 first-year coolers and 150,000 first-year Qur’ans. |
| Projects | صفحة المشاريع والمركز الإعلامي، جدول المشاريع | All seven projects, in source order. Summer water has no pending quantity; Hajj has the documented store presence but no pending client count. Qur’an uses a decorative local SVG instead of a misleading photograph. |
| Partners | الرئيسية: الشركاء/لماذا الأفكار الناجحة/كيف نعمل؛ من نحن: من نخدم/قيمنا؛ البنية التشغيلية: التقارير | Document-derived assembly because the source has no separate Partners page. No invented endorsements or logos. |
| Request proposal | صفحة طلب عرض سعر | Document quotation copy/fields, multi-service selection, explicit Arabic validation and WhatsApp handoff. Housing and the unconfirmed delivery/response-time message are omitted. |
| Contact/footer | صفحة تواصل معنا؛ للجهات؛ تذييل الموقع | Exact contact facts and introduction, documented footer summary/slogan/copyright, canonical navigation and infrastructure link. |

Intentional exceptions: pending housing, prices, certifications, hours, cart/summer quantities, minimum quantities, branding capability, and the 19-entity Hajj figure remain unpublished. The meals branding inclusion conflicts with its unanswered FAQ and is withheld. News requires dates/photos; media downloads require real files, so neither is fabricated. Source image captions describing actual field/team photos are not published as claims about the bundled images. Visible illustrative labels/notices have been removed; accurate alternative text, image metadata and internal asset provenance remain. Projects metadata says “بالأرقام” rather than promising actual project photographs. The contact form retains Arabic validation, its numeric phone example, and explicit WhatsApp handoff rather than the source's unverified delivery-success message. Only the eight approved pages are published. Unsupported standalone pages are redirected, retired or withheld as listed below.

## Forms and interactions

Contact and quotation forms use `novalidate`, explicit Arabic errors, accessible associations, Arabic/Persian digit normalization, `WebsiteWhatsApp.Message/Link` and `/website/whatsapp.js`. Submission stays disabled until interactive; a direct WhatsApp link remains usable independently. A valid handoff retains fields and offers a fallback link; the user must send in WhatsApp. No delivery confirmation or response-time promise is shown.

Quotation accepts multiple approved services; `?service=<key>` preselects once on initialization (unknown/housing keys ignored). Name/entity/services/quantity/phone are required; optional timing/email/details retain their original validation and length limits. Quantity descriptions are preserved when digits normalize. Forms stay stationary while adjacent cards may animate.

## Verification and manual checklist

Run focused checks with `python UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/verify-static.py`, then `git diff --check` and the web build. The script checks camel-case dataset names against the HTML/CSS contract, asset resolver use, canonical navigation, pathname keys and listener/observer cleanup. Shared/Web builds are required when those projects change. Use `AlAfkarERP.Web/artifacts/design-2-build/` as isolated output when normal DLLs are locked. Browser execution remains pending until explicitly requested with **Run and verify visually**.

- Load all eight canonical pages on desktop/mobile: Arabic title, one H1, RTL, correct content length, no preview/comparison wording, no horizontal overflow, clipped captions or layout shifts. Check header active state, seven desktop links, proposal CTA, footer links and skip link.
- Follow every navigation/quotation/section link. Confirm service tabs and legacy detail redirects land at all six matching service anchors; query-preserving quotation links preselect the expected service. Unknown/housing service queries stay unselected.
- Check HTTP 301 mappings in both tables, trailing slash/case normalization, campaign/repeated query preservation, obsolete editor-parameter removal and no loops. Check all six retired Arabic/English URLs return 410 with retirement wording after hydration. Housing and unknown Arabic/English/preview URLs return 404. ERP/login/store routes keep their shells.
- Check rendered canonical/OG/Twitter URLs match configured runtime origin; homepage Organization facts match the document. Sitemap has exactly eight Arabic URLs even without CMS publication availability; robots excludes noncanonical content; errors remain noindex. No English alternates.
- Check seven project cards, each category/count including Qur'an and summer water, filtering, SVG/image fallbacks, category labels, five homepage FAQs and service FAQs. Unconfirmed values and unavailable profile downloads remain absent.
- Tab through links, filters, FAQ summaries and fields: visible focus stays on interactive controls; page-load/navigation H1 focus has no outline; skip link works. Check errors announced and associated, native checkbox fieldset/legend, no English browser-validation popups.
- Contact: missing/whitespace name, invalid/Arabic/Persian phone and empty/overlong message. Quotation: empty/whitespace required fields, no entity/services, malformed phone/email, all length limits, optional omissions, multiple services/quantities and digit normalization.
- Valid forms: all supplied Arabic field names/values appear in encoded WhatsApp text; popup blocking has a usable fallback; entered values persist; a later invalid submission clears the previous fallback. Requests reach the team only after sending in WhatsApp.
- Desktop fine pointer: hero tilt, image-contained zoom, card lift/tilt/highlight, staggered reveals and floating accents. Mobile: scrolling reveals without sticky hover. Keyboard focus reveals pending content immediately. Filters register new cards; navigation removes observers/listeners/frames.
- Footer motion toggle persists for the session across pages; live OS reduced-motion changes stop all tilt/zoom/floating and show content. Offscreen/hidden-tab accents pause. Failed JavaScript/IntersectionObserver and unavailable icon fonts leave content/icons usable and visible. Contact/proposal forms never tilt.
- Authorized control panel shows only the inactive-editor notice and website/configuration links; unauthorized users get no management surface. Check ERP language-toggle rerendering, permission-gated configuration link and retained storage settings. No editor/publish/restore/upload/preview actions or claim that CMS publishing updates the static website.

## Recorded implementation checks

- Focused static checks passed: eight explicit routes and matching page definitions; canonical internal links/anchors; unchanged approved Arabic content literals and form logic; 29 SVG mappings; retained focus rules; no obsolete presentation/preview dependencies; static sitemap and permission-gated notice.
- All six selected-image SHA-256 hashes match provenance records. All 50 original non-preview media/font files remain byte-for-byte identical. ERP resource branches in App remain unchanged.
- `git diff --check` passed. Shared build succeeded with zero errors (304 warnings in existing unrelated shared UI code); final Web build succeeded with zero warnings/errors. Both used the existing isolated output folder.
- No application or browser was run. HTTP status/redirect behavior, rendered SEO and visual/interactive checklist items await an explicitly requested runtime verification. No deployment was performed.

## Motion and navigation regression checklist

- Directly load each of the eight pages, then use desktop/mobile navigation through every page and browser Back/Forward. Confirm normal navigation completes without a page reload or a connecting/reconnecting flash; active navigation, Arabic title/description/canonical metadata and content update correctly.
- Check service anchors and the skip link with pointer/keyboard. Select a same-page link on mobile and confirm the menu closes immediately. H1 focus has no decorative outline, and interactive controls retain visible keyboard focus.
- Scroll each page and hover hero/images/cards on a fine-pointer device. Confirm reveals, tilt, floating accents and highlights resume on newly visited pages and filtered project cards. Touch devices have no sticky hover; reduced motion and the session toggle work across navigation.
- Navigate rapidly while motion loads, then filter projects repeatedly. Confirm old observers/listeners/frames are removed, there are no duplicate motion effects, and content stays visible if JavaScript/observers fail.
- Open quotation links with known/unknown/housing service queries: approved services preselect on page entry. Enter form data and change only the fragment/query without leaving the page; values persist. Leave and revisit the page: the new page initializes normally. Check validation and WhatsApp handoff/fallback remain unchanged.
- Check error-page exit links make a fresh HTTP request. Check any ERP/login/store destination loads its own shell; retired/redirect/404 statuses remain server-controlled.
- When runtime verification is explicitly authorized, simulate a real connection loss: Arabic reconnect/retry remains visible and usable. Normal navigation must not simply hide genuine failures.
- After publishing the fix, check the HTML stylesheet reference and imported motion module resolve to the current asset URLs and return successfully; check deployed motion uses `pwMotion` / `pwMotionState` rather than stale dataset names. No deployment was performed by this task.

Implementation verification: focused static checks, JavaScript module syntax checking (`node --input-type=module --check`) and `git diff --check` passed. The generated static-asset manifest contains fingerprinted routes for both motion.js and website.css. The isolated web build succeeded with zero errors (existing unrelated shared UI warnings remain). Application/browser and published-site verification remain pending until **Run and verify visually** is requested.

## Public reconnect grace period

`WebsiteReconnect` loads fingerprinted `/website/reconnect.js` independently of the ERP handler. The dialog starts closed and `.pw-reconnect:not([open])` explicitly hides it. Transient show/retrying states share one three-second timer; repeated events do not restart the deadline. Recovery cancels that timer and closes an open dialog. Failed, paused and resume-failed states show immediately. Rejected circuits retain the existing reload behavior; retry/resume still use Blazor's recovery APIs. This changes presentation timing, not server timeouts or production connectivity.

Timers and document/button/state listeners are removed on pagehide; restoring a cached document reattaches one set of listeners. The module also handles failure states reported before it finishes loading. The shared ERP reconnect JavaScript and its configuration are unchanged.

Checks:

- `python UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/verify-static.py`
- `node UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/verify-reconnect.cjs` (simulated dialog/clock; no application, browser or network)
- JavaScript module syntax check, `git diff --check`, and isolated web build.

Manual checks, pending **Run and verify visually**:

- Direct load, refresh, navigate and use Back/Forward across all eight pages on desktop/mobile. Healthy loading must show no reconnect dialog; motion, forms and keyboard focus remain usable.
- Recover before three seconds: no dialog or backdrop appears. Persist beyond three seconds: one Arabic dialog appears. Repeated retry events must not reset the deadline. Recovery after display closes it immediately.
- Force failure, pause and resume failure: their matching Arabic text/actions appear immediately; retry/resume recover or reload as before. Test automatic retry on visibility restoration and unload during a pending timer/recovery.
- Verify the published page loads only the fingerprinted public reconnect module; ERP still loads its original handler. Check no duplicate event handling or stale-resource failures.
- Inspect Blazor connection events and network requests if healthy navigation still produces persistent reconnects. Record the observed connection/hosting fault before proposing configuration changes; the grace period does not claim to repair an unverified network problem.

No application/browser was run and no deployment was performed.

Reconnect implementation checks passed: static contracts, isolated JavaScript recovery/timer scenarios, module syntax, unchanged shared ERP handler, fingerprinted asset manifest and `git diff --check`. The isolated web build succeeded with zero warnings and zero errors. Published-site and visual checks remain pending.

## Mobile image entrance motion

Touch devices matching `(hover: none)` or `(pointer: coarse)` now receive an automatic image entrance: scale 1.04 to 1 over 1,200ms with the existing easing curve. The homepage hero, About/service images and project photographs animate once when the actual image is both loaded and intersecting the viewport. Card scroll reveals remain independent. Logos, SVG/icon fallbacks and forms are excluded.

The hero image now has an inner rounded clipping wrapper; its caption/statistic badge remain outside that wrapper. Existing desktop tilt and fine-pointer hover effects retain their limits. Other image frames retain their existing clipping. Images are never hidden to prepare the new effect, and missing JavaScript/observers leave them static and visible.

Lazy images wait for successful loading and viewport entry. Newly rendered project photos register after filtering. The footer pause control and live reduced-motion changes cancel image animation without replaying already displayed images; newly encountered images can animate after motion resumes. Hidden tabs pause active animation and defer starting new image entrances. Disposal/filter removal clears image load/animation listeners, observer registrations and temporary classes.

Verification commands:

- `python UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/verify-static.py`
- `node UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/verify-mobile-motion.cjs`
- `node UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/verify-reconnect.cjs`
- Motion module syntax check, `git diff --check` and the isolated web-project build.

The mobile harness simulates viewport observations, media queries, loaded/lazy images, project filtering, hidden tabs and cleanup without running an app/browser. It checks that visible card text does not trigger an offscreen photograph, image entrances do not replay, pointer behavior stays intact and observer failure keeps content visible.

Manual mobile checks, pending **Run and verify visually**:

- Scroll the homepage, About, Services and Projects on mobile: each eligible image gently settles from 1.04 to 1 once. Initially visible photos may animate without disappearing; slow/lazy images animate after loading while visible. Scroll away/back: no replay. Check all eight pages for unaffected interactions.
- Filter projects and revisit pages: new photos receive the effect; Qur'an SVG/icon cards do not. No touch drag, sticky hover or continuous image zoom is introduced.
- Check rounded cropping, readable captions/category labels, hero floating accents and the statistic badge; no horizontal overflow, layout shifts or clipping of overlays. Rotate the device and check portrait/landscape image sizing.
- Toggle motion and change OS reduced-motion settings during playback: animation stops at the normal image size and seen photos never restart. Background/restore the browser tab: current playback pauses/resumes; newly visible images wait until the tab is visible.
- Check desktop hero tilt, card lift/highlight, hover zoom, keyboard focus, forms and WhatsApp handoffs remain unchanged. With JavaScript/IntersectionObserver unavailable or images failing to load, content and alternative text stay usable.

No application/browser was run and no deployment was performed.

Mobile image implementation checks passed: focused static contracts, isolated mobile-image/reconnect JavaScript scenarios, motion module syntax and `git diff --check`. The isolated web-project build succeeded with zero warnings and zero errors. Visual and published-site checks remain pending.

## Visible image label cleanup

Visible illustrative badges, generic service captions, About/Partners caption qualifiers and homepage/Projects image notices have been removed. Meaningful captions and project category badges remain. Images, alternative text, social image metadata, asset provenance, clipping wrappers and motion hooks are preserved. No deployment is included.

Manual checks, pending **Run and verify visually**:

- Review all eight pages on desktop/mobile: no visible “صورة توضيحية” labels or explanatory image notices; remaining captions have balanced spacing and no empty caption strips.
- Filter projects through every category: all photograph cards retain category badges; the Qur’an icon fallback remains intact.
- Check About/Partners captions, rounded image clipping, hero accents, desktop hover and mobile image entrances. Keyboard navigation, forms and reduced-motion controls remain usable.
