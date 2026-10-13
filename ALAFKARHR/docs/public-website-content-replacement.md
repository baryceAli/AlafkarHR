> Historical implementation record. The former presentation and editor workflow below have been superseded by the selected static Arabic website. CMS data remains preserved, but publishing does not update the website. See [current workflow](../UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/README.md).

# Alafkar Public Website content replacement

The replacement uses AlAfkar_Website_Content.docx and the corrections approved in the implementation conversation. Arabic and English are maintained together. ERP, login and /store are outside this change.

## Review and publish

1. Deploy the updated website and GeneralSettings assemblies together. Existing snapshots are adapted on read; added sections are hidden until explicitly selected. Historical revision records are not rewritten.
2. Sign in with the website owner's authorised account and open /PublicWebsite/ControlPanel.
3. Select **Load approved website content**. The current local draft, including unsaved edits, is downloaded as JSON before replacement. Retain that backup. Uploaded branding, resource files and mapped illustrations are retained.
4. Review Arabic and English pages, illustrations, metadata and resource uploads. The new service pages are available in the page selector.
5. **Save draft** preserves the existing published version. **Discard unsaved edits** returns to the last saved draft.
6. Use **Publish both languages** only when review is complete. This creates a normal published revision. To roll back, restore an earlier revision into the draft, review it, and publish.

Company-level management uses the existing normal PublicWebsite View/Edit/Publish/Upload permissions. Backend owner-company and media-ownership validation, optimistic concurrency and auditing remain enforced. There are no branch selectors, new inquiry APIs, database tables or migrations.

## Content and assets still needed

- Actual company, field-project, fleet and warehouse photographs, with approved bilingual descriptions. Existing images are labelled as illustrative.
- Approved company profile PDF, press releases, video/audio reports and media kit. Unavailable downloads stay hidden.
- Approved partner names/logos and permission to display them. No partner logo endorsements are included by default.
- Exact headquarters map pin; warehouse locations if they should be public.
- Licence/certificate documents and numbers before adding accreditation claims.
- Package contents, quantities, current prices and applicable commercial terms. Packages currently invite WhatsApp inquiries.
- Housing districts, room counts, capacity, distance and availability before adding these claims.
- Confirmed operating hours and response times.
- Confirmed news dates, text and supporting photographs.
- Confirmation of the 19-entity Hajj figure and the calendar year behind the first-year cooler/Quran figures before adding them.
- Legal review and completion of privacy, payment, cancellation and refund wording. Legal pages remain marked as drafts and noindexed.

## Manual acceptance checklist

- At desktop and mobile sizes, review all ten original pages and six service detail pages in Arabic and English. Check heading wrapping, card layouts, menu reachability and RTL/LTR direction.
- Switch language on service detail pages; verify the same service opens in the other language. The Services navigation item should remain active.
- Check homepage and service overview anchors, footer links, email, telephone and package WhatsApp actions.
- In quotations, select multiple services; supply a quantity, date and optional email. Check the WhatsApp destination and decoded message. Confirm blank optional values are omitted and no receipt/saved-request claim appears.
- Verify service query preselection and legacy package query prefill. Test Arabic, ampersands, plus signs and multiline details.
- If the browser blocks automatic opening, use the generated **Open the message in WhatsApp** link.
- In the editor, export/load/discard/save a draft; edit card text, reorder and duplicate items, upload an image and review both languages. Confirm public content does not change on draft save.
- With an old published snapshot, verify added service pages return 404 and are absent from the sitemap. After publishing the replacement, verify the six services appear in both languages.
- Confirm incomplete media/news sections are hidden, legal pages remain noindexed, and each published page has one H1.
- Check that another editor's stale save is rejected and unauthorised/cross-company management and media references are denied using the existing backend checks.

## Automated verification

Build:

    dotnet build UI/AlAfkarERP/AlAfkarERP.Web/AlAfkarERP.Web.csproj -m:1
    dotnet build src/Modules/GeneralSettings/GeneralSettings/GeneralSettings.csproj -m:1
    dotnet build tools/PublicWebsiteContentChecks/PublicWebsiteContentChecks.csproj -m:1

Run the standalone regression checks (no website server or browser):

    dotnet tools/PublicWebsiteContentChecks/bin/Debug/net10.0/PublicWebsiteContentChecks.dll

The checks cover catalog completeness, snapshot read/restore compatibility, preserved IDs/order/media, approved replacement, bilingual route mappings, hidden services, WhatsApp encoding, and in-memory rendering of public pages.
