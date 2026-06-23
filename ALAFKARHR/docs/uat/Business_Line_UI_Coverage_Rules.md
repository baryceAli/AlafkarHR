# Business Line UI Coverage Rules

Generated for the Business Line entitlement implementation.

## Purpose

Every Business Line feature must be reachable from the UI or explicitly documented as intentionally backend-only. Backend endpoints, DTOs, services, and license gates are not considered complete unless a user can reach the feature through a page, route, visible action, or documented exception.

## Coverage Matrix

Use `Business_Line_UI_Coverage_Matrix.csv` as the source of truth for Catering, Real Estate, and Business Line licensing/catalog coverage.

Required columns:

- BusinessLineKey
- Workspace
- MenuRoot
- Feature
- ServiceOrApiFeature
- UIExposureType
- UIRouteOrPath
- UIEntryPoint
- PermissionReference
- LicenseGate
- CoverageStatus
- Notes

## Maintenance Rule

When adding a new Business Line endpoint, service method, page, or menu root:

1. Add or update a row in `Business_Line_UI_Coverage_Matrix.csv`.
2. Mark the feature as `Page`, `Inline Action`, `Detail/Form Route`, or `Intentionally Backend-Only`.
3. Confirm the UI entry point is protected by the existing permission plus the Business Line license gate.
4. Add a menu item only for standalone pages; workflow endpoints can be covered by visible buttons, modals, or detail actions.
5. Do not add future Business Lines such as Car Washing or Flower Shop to navigation until their pages and matrix rows exist.

## Gap Definition

A gap exists when a user-facing Business Line API or service method has no reachable UI route, button, modal, table action, dashboard link, or backend-only justification.

Detail and form routes do not need direct menu entries when they are reachable from list or detail pages.
