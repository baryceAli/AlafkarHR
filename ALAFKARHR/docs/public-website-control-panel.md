# Alafkar Public Website control panel

The website now uses the selected Arabic static design. `/PublicWebsite/ControlPanel` remains under General Settings and requires `PublicWebsite.Content.View`, but displays an inactive-editor notice instead of editing tools. Approved content is maintained in code; CMS publishing does not update the public website.

The notice provides a website link and a configuration link for users with `PublicWebsite.Configuration.View`. It exposes no editing, publication, restoration, upload or preview actions. Existing drafts, publication history, media, API contracts and permission definitions remain preserved.

## Configuration and stored media

`/PublicWebsite/Configuration` retains existing configuration/storage functionality and permission checks. Ownership, storage locations, readiness and activation continue to govern preserved CMS data; activation does not publish the static website. Public runtime-origin settings remain used for canonical metadata, robots and sitemap, with the existing configured fallback.

Retain durable media storage and backups outside wwwroot, existing storage keys/paths, API connection settings and migration history. This promotion adds no migration and does not delete stored data or change backend authorization. `/website/media/{id}` remains available for published media references; frontend editor-preview capabilities have been removed.

See [the website implementation guide](../UI/AlAfkarERP/AlAfkarERP.Web/Components/PublicWebsite/README.md) for canonical routes, redirects/retirement responses, content sources, asset provenance and manual verification.
