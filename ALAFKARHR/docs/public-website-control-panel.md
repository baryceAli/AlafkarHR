# Alafkar Public Website control panel

The control panel is at `/PublicWebsite/ControlPanel`, under **Admin → General Settings → Alafkar Public Website**. It uses existing ERP accounts. Arabic public URLs are unchanged; their English equivalents are under `/en`.

## Deployment

1. Set **API** configuration `PublicWebsite:OwnerCompanyId` to the actual Alafkar owning company's ID. The checked-in empty GUID deliberately disables management until an owner is configured. Other companies cannot read drafts, upload, save, restore or publish, even if their roles contain website permissions.
2. Set `PublicWebsite:StorageRoot` to an absolute durable directory outside `wwwroot`, or mount a persistent volume at the default `App_Data/PublicWebsite`. Give the API process read/write access. Back up this directory together with the database; revision restoration depends on retained files.
3. Apply the generated `AddPublicWebsiteControlPanel` migration for `GeneralSettingsDbContext` using the API startup project. Follow the existing production migration process; do not depend on the development startup migration in production.
4. Grant `PublicWebsite.Content.View` plus the required `Edit`, `Upload`, and `Publish` permissions through the existing company role screen. Refresh the user's login/token after role changes. Grant View to every panel user. Upload controls also require Edit. Publishers may publish a saved draft without editing it.
5. Keep the Web application's `ApiConfig:BaseURL` pointing to this API. The Web server streams published files through same-origin `/website/media/{id}` URLs, so the backend address does not need to be exposed to visitors. Preserve `PublicWebsite:Origin` as the public HTTP(S) origin. Set reverse-proxy upload limits to at least 105 MiB and an appropriate upload timeout.

Environment-variable equivalents include `PublicWebsite__OwnerCompanyId` and `PublicWebsite__StorageRoot`. Upload defaults are 10 MiB for images, 25 MiB for PDFs and 100 MiB for video/audio/ZIP files. The API validates both file signatures and MIME types.

## Editing and publishing

- Choose a page, Arabic/English and desktop/mobile preview. Click text, an image or a file slot in the preview; the inspector shows the selected fields. Link labels and destinations are separate fields.
- Preview switches to a clean, interactive draft view; Edit in place restores editing overlays. Public page links within the draft keep the editor session, and draft downloads open separately.
- The section list controls visibility and ordering. Repeated item lists support adding, removing and moving items. Added items use an existing approved template. Layout code, form submission behavior and filter values remain in code.
- Header, footer and SEO controls expose global and page metadata. English and Arabic content can be edited side by side. Images have separately editable alternative text.
- Upload replacement media, then save the draft. Drafts and unpublished files are private. Clicking Publish saves pending edits first and atomically publishes the entire website in both languages. Required translations and media ownership are checked by the API.
- Version History restores an immutable published snapshot into the shared draft. Review it and publish separately. Save or discard unsaved edits before restoration.
- First-time initialization records the original website as a baseline revision, so the first publication can also be rolled back.
- A conflicting save leaves local edits intact and returns HTTP 409. Copy any changes to retain, discard the local changes and reload the latest draft before reapplying them. The system never silently overwrites another editor's draft.

The preview receives snapshots from its authorized parent editor through same-origin, session-validated messages. It never receives ERP tokens. Draft-media URLs are short-lived server capabilities restricted to IDs in that draft; preview sessions expire after 30 minutes without editor activity and are revoked when the editor is disposed.

Public pages request the current published snapshot on each load. If the API is temporarily unavailable, the Web process uses its last successful published snapshot; before the first successful load it uses the original seeded content. Private drafts never enter this fallback cache.

The original company profile, press release, video and media-kit slots remain unavailable until real files are uploaded and published. The original demonstration form behavior and content claims remain editable; review the seeded English copy and official company details before publishing.

## Content maintenance

The allowlisted catalogue is embedded from `PublicWebsite.manifest.json` in SharedWithUI. Keep identifiers stable when updating components. Only add approved fields/templates through development; an editor cannot add arbitrary HTML, scripts, CSS or Razor. Snapshot schema version 1 contains fields, page sections and typed repeated items.

Storage is intentionally separate from ERP Document Management and Media Center records. Only website-specific files explicitly referenced by a published revision are anonymously readable. Published files remain available for historical revisions and rollback; no automatic file deletion is performed.

## Acceptance checklist

Run runtime/visual checks only when explicitly requested with **“Run and verify visually”**.

- Sign in as an owner-company editor; verify navigation, direct-route access, Arabic/English panel labels and both public language routes.
- Edit a heading, button label/destination, form placeholder, image/alternative text, logo and SEO description. Check the unsaved preview, save/reload persistence, and leave-page warning.
- Hide/reorder sections. Add, edit, remove and reorder project, partner, gallery and news items. Check cloned news cards expand independently and package/project filters still work.
- Replace image/PDF/video/ZIP slots. Verify upload progress and errors preserve local edits. Confirm draft files are inaccessible without their editor capability and public URLs return 404 before publication.
- Publish and load fresh Arabic/English public pages; verify RTL/LTR, metadata, downloads and video range playback. Restore a previous revision into the draft, then publish it.
- Try another company with the same permission names, an unprivileged account, stale draft tokens, unknown schema fields, foreign media IDs, oversized files and mismatched/executable uploads. Expect denial or validation errors with no partial publication.
- Confirm ERP pages, login and `/store` behavior and styling remain unchanged.

Build validation uses the affected backend and Web projects. If an existing process locks normal output files, build with `--configuration PublicWebsiteVerification -m:1` without stopping or starting the application.
