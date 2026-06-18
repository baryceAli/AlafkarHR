# Alafkar ERP Design Contract

Use this reference when changing tokens or deciding whether a visual treatment matches the official design system.

## Sources Of Truth

- Official contract: `D:/Development/AlAfkar/UI Design/stitch_alafkar_erp_ui_redesign/DESIGN.md`.
- Current implementation: `UI/AlAfkarERP/AlAfkarERP.Shared/wwwroot/theme.css`.
- `DESIGN.md` YAML tokens override conflicting prose. The canonical primary blue is `#0051d5`.
- `theme.css` contains an older token block near the top and a later design-system override block. Prefer `DESIGN.md` and the later override block around the `--erp-color-*` / official `--erp-*` variables.
- Do not introduce negative letter spacing. The implemented display tracking default is `0`.

## Visual Intent

- Build a quiet, professional, high-density ERP workspace.
- Prioritize clarity, scanability, precise alignment, accessible contrast, and low cognitive load.
- Avoid decorative pages, marketing-style heroes inside admin surfaces, one-note palettes, large soft blobs, and visual noise.
- Use tokens and shared classes so theme/color changes happen by changing variables, not every page.

## Core Tokens

- Font families: Inter for UI, JetBrains Mono for codes, SKU values, IDs, and compact technical values.
- Type scale: display large `30px/38px/700`, mobile display `24px/32px/700`, headline `20px/28px/600`, title `16px/24px/600`, body `14px/20px/400`, compact body `13px/18px/400`, labels `12px/16px/600`, code `12px/16px/400`.
- Spacing scale: 4px base unit with `4`, `8`, `12`, `16`, `24`, and `32` pixel steps. Container margin is 24px and gutter is 16px.
- Radius scale: small `.125rem`, default `.25rem`, medium `.375rem`, large `.5rem`, extra large `.75rem`, full `9999px`.
- Shell sizing: sidebar expanded `240px`, collapsed `64px`, desktop rail `64px`, topbar `64px`, default control height `36px`, small control height `32px`, dense row height about `32-40px`.

## Color And Surface System

- Use `--erp-color-*` tokens for official colors and compatibility aliases such as `--erp-primary`, `--erp-surface`, `--erp-bg`, `--erp-text`, `--erp-border`, `--erp-success`, `--erp-warning`, `--erp-danger`, and `--erp-info`.
- Use surface levels instead of ad hoc backgrounds:
  - Level 0: page/app canvas, `--erp-bg` / `--erp-surface-level-0`.
  - Level 1: cards, tables, whiteboards, and main panels, `--erp-surface` / `--erp-surface-level-1`.
  - Level 2: modals, popovers, dropdowns, and elevated overlays, `--erp-surface-level-2` plus tokenized shadow.
- Use `--erp-border` for normal outlines and `--erp-border-strong` for stronger separation.
- Use subtle tonal hover states with `color-mix()` or existing token classes. Avoid increasing shadow on hover unless the existing component pattern already does it.
- Support `data-color-scheme` variants for blue/default, emerald, purple, orange, and slate, and `data-theme-mode="dark"`.

## Component Rules

- Buttons: default height `36px`, radius default, semibold text, Bootstrap Icons inside actions when an icon is needed. Primary is solid `--erp-primary` with `--erp-primary-contrast`; secondary is ghost/border with `--erp-secondary`.
- Icon buttons: square or near-square, tokenized border/background, centered Bootstrap icon, no text where a familiar icon is clearer.
- Inputs/selects: persistent 12px semibold labels, 1px `--erp-border`, default radius, `36px` min height, primary focus border, and 2px tokenized focus ring.
- Data grids: responsive overflow wrapper, dense rows, 12px uppercase headers, subtle borders, compact cell padding, tonal row hover, and no hardcoded table colors.
- Status chips: 20px minimum height, full radius, 11-12px bold text, semantic tinted background and border, high-contrast semantic text.
- Sidebar: `DesktopWorkspaceNav` is the active desktop sidebar. Use 240px expanded and 64px collapsed width, company switcher at the top, subtle blue active background, and a 2px leading active bar mirrored with logical properties.
- Multi-company switcher: show logo/initials, company label/name, and chevron at the top of the sidebar. It is display-first and claim-based unless a real auth/session switch API exists.
- RTL/LTR: use logical properties for spacing, borders, inset, active bars, sidebar placement, and content alignment. Mirror chevrons/icons when direction changes.
