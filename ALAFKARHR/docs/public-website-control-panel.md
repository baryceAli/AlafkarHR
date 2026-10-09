# Alafkar Public Website control panel

The control panel is at `/PublicWebsite/ControlPanel`, under **Admin → General Settings → Alafkar Public Website**. It uses existing ERP accounts. Arabic public URLs are unchanged; their English equivalents are under `/en`.

## Deployment and interface configuration

1. Deploy the API and Web changes and apply `AddPublicWebsiteControlPanel`, `AddPublicWebsiteConfiguration`, and `AddPublicWebsiteManagedStorage` migrations for `GeneralSettingsDbContext`, using the API startup project and the existing production migration process. The page does not apply migrations. If tables are missing, it reports that deployment action is required.
2. Provision durable media storage outside `wwwroot`, with API-process read/write/delete access and backups. The existing `PublicWebsite:StorageRoot` remains the reserved `legacy` location, preserving existing files. Administrators can now add folders from the configuration page within managed storage roots; editors never type filesystem paths into the interface. Server-defined locations remain supported. Do not change a location's path after uploads exist.
3. Preserve Web `ApiConfig:BaseURL`, API version, and the fallback public Web origin. Configure proxy upload limits to at least 105 MiB and suitable timeouts. The UI cannot modify connection settings, mount volumes, grant operating-system permissions, or confirm proxy limits.
4. Sign in as the parent-company administrator and open **Admin → General Settings → Public Website Configuration** (`/PublicWebsite/Configuration`). Configuration access uses normal `PublicWebsite.Configuration.View` and `PublicWebsite.Configuration.Edit` permissions. Initial setup additionally requires `Organization.Company.EditChild` and a parent-company identity, checked server-side. Refresh login after deployment/role changes; the existing parent-admin permission template includes the new configuration permissions.
5. Select an active owning company from that parent company's hierarchy, choose a storage location (or use **Add Storage Folder** to create one inside a managed root), enter the public HTTP(S) origin, and set upload limits. Click **Save Settings**, **Check Readiness**, then **Activate Control Panel**. Activation does not publish or change website content. After setup, authorized owner-company administrators and its authorized parent administrator can manage configuration; unrelated companies cannot.
6. Grant content users `PublicWebsite.Content.View` plus required `Edit`, `Upload`, and `Publish` permissions through existing role management. Configuration access does not grant content access or modify roles. Refresh login after permission changes, then use **Open Control Panel**.

Database settings take precedence once saved. Before the first save, existing owner/storage/limit configuration remains the compatibility fallback. A previously configured website stays active and retains its records. The public origin is loaded from saved settings for metadata, language alternatives, robots and sitemap without restarting. When the API is unavailable, the Web process uses its last successful public origin, or its deployment fallback before the first successful read.

Ownership is locked after activation or once website records exist. Storage selection is locked once any media exists, including unpublished files. Transfers and file relocation require a separate process. Stale configuration saves return HTTP 409 and retain local edits. Registered folders persist in the database and require no restart. Adding or moving a managed root remains a deployment task; do not change a root path once registered folders contain media.

Approved locations use this **API** configuration shape (the reserved `legacy` key is provided automatically):

```json
"PublicWebsite": {
  "StorageRoot": "App_Data/PublicWebsite",
  "StorageLocations": [
    {
      "Key": "durable-media",
      "Name": "Durable website media",
      "Description": "Persistent volume; backed up by deployment operations.",
      "Path": "D:/AlafkarData/PublicWebsite"
    }
  ]
}
```

The example path must be provisioned on the actual API host. Storage readiness checks create, read and delete only a generated temporary file in the chosen approved location. Successful access checks do not establish durability or backups. Upload defaults are 10 MiB for images, 25 MiB for PDFs, and 100 MiB for video/audio/ZIP; each can be configured from 1 to 100 MiB. Server-side signature, extension and MIME validation remain in force.

### Adding a media storage folder

Before uploading any media, use **Add Storage Folder** on `/PublicWebsite/Configuration`. Choose a managed root, enter a display name and a folder name such as `website-media`, and optionally enter a description. Folder names allow 1–64 ASCII letters, digits, hyphens, or underscores; paths, reserved names, and duplicate names within the same root/parent-company scope are rejected. Names are stored in lowercase.

The default root is `App_Data/PublicWebsiteStorage`, relative to the API content root. The API creates folders below a parent-company-specific subdirectory. Additional durable roots can be provisioned through `PublicWebsite:ManagedStorageRoots`, using lowercase keys:

```json
"ManagedStorageRoots": [
  {
    "Key": "durable-volume",
    "Name": "Durable website storage",
    "Description": "Persistent volume maintained by deployment operations.",
    "Path": "D:/AlafkarData/ManagedWebsiteStorage"
  }
]
```

The reserved `default` root is automatic. Production deployment must mount durable storage at the default location or provision/select another managed root. Root paths must remain outside `wwwroot`; managed roots and their ancestors/folders cannot use symbolic links or redirected directories. Existing nonempty directories cannot be registered. The API creates, reads, and deletes only a generated probe to verify access before storing location metadata.

Creation refreshes the dropdown and selects the new folder as an unsaved setting. Other unsaved values remain intact. Click **Save Settings**, then **Check Readiness** before activation. Creation does not save configuration, activate management, or publish content. Failed operations retain the form values. Retrying the same registration after a lost response uses its operation identifier to recover the existing location rather than create a duplicate.

**Refresh storage locations** updates choices and lock state without replacing unsaved settings or silently changing their concurrency token. The page explains missing edit permission, the single-location state, and existing-upload locks. Once media exists, **Add Storage Folder** is unavailable and switching locations is rejected server-side; relocation and deletion remain separate workflows.

Configuration contracts live under `/api/v1/publicwebsite/configuration` (read/save, `/readiness`, `/activate`, and POST `/storage-locations`). Anonymous `/runtime-settings` exposes only the public origin. `/management-status` requires content-view permission and checks the configured owning company.

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

- Add a managed storage folder, verify it becomes the unsaved selected location, save/reload and upload to it. Check duplicate/reserved names, traversal, absolute paths, foreign location keys, redirected paths, nonempty directories, missing permissions, concurrent first uploads, and registration retry after a lost response. Verify existing legacy files remain available.
- Before activation, configure as a parent administrator; check owner choices exclude other hierarchies, empty selectors require a selection, invalid origins/limits and unavailable storage fail safely, and concurrent saves return 409. Verify saved settings survive reload and activation leaves content unchanged.
- Verify owner-company configuration administrators can update unlocked settings, ownership locks after activation, storage locks after any upload, and unrelated companies/content-only editors are denied. Confirm existing installations retain media and history.
- Change public origin and upload limits; verify metadata/sitemap and upload enforcement update without restarting. Missing migrations remain a deployment action.
- Sign in as an owner-company editor; verify navigation, direct-route access, Arabic/English panel labels and both public language routes.
- Edit a heading, button label/destination, form placeholder, image/alternative text, logo and SEO description. Check the unsaved preview, save/reload persistence, and leave-page warning.
- Hide/reorder sections. Add, edit, remove and reorder project, partner, gallery and news items. Check cloned news cards expand independently and package/project filters still work.
- Replace image/PDF/video/ZIP slots. Verify upload progress and errors preserve local edits. Confirm draft files are inaccessible without their editor capability and public URLs return 404 before publication.
- Publish and load fresh Arabic/English public pages; verify RTL/LTR, metadata, downloads and video range playback. Restore a previous revision into the draft, then publish it.
- Try another company with the same permission names, an unprivileged account, stale draft tokens, unknown schema fields, foreign media IDs, oversized files and mismatched/executable uploads. Expect denial or validation errors with no partial publication.
- Confirm ERP pages, login and `/store` behavior and styling remain unchanged.

Build validation uses the affected backend and Web projects. If an existing process locks normal output files, build with `--configuration PublicWebsiteVerification -m:1` without stopping or starting the application.
