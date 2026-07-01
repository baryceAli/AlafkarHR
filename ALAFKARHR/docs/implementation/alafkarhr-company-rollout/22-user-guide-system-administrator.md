# User Guide: System Administrator

## Role Purpose

The System Administrator manages organization setup, users, roles, permissions, branch access, and general system settings.

## Main Pages

- Security dashboard: `/Auth/Dashboard`
- Roles: `/Auth/Role/List`, `/Auth/Role/Form`
- Users: `/Auth/User/List`, `/Auth/User/Form`
- Assign user roles: `/Auth/User/AssignRole`
- Organization: `/Organization/Dashboard`, `/Organization/Company/List`, `/Organization/Branch/List`, `/Organization/Administration/List`, `/Organization/Department/List`
- Settings: `/GeneralSettings/SystemSettings`, `/GeneralSettings/HomePageTemplates`, `/GeneralSettings/Currencies`

## Required Permissions

- `Authentication.Users.*`
- `Authentication.Roles.*`
- `Organization.Company.*`
- `Organization.Branch.*`
- `Organization.Branch.AssignUsers`
- `Organization.BranchAccess.ViewAll` where company policy allows all-branch visibility
- `Organization.Administration.*`
- `Organization.Department.*`
- `GeneralSettings.SystemSettings.*`

## Daily Tasks

### Create or Update a Role

1. Open `/Auth/Role/List`.
2. Select create or edit.
3. Enter role name and description.
4. Select permission groups needed by the role.
5. Save.
6. Test with a user assigned to that role.

### Assign Roles to a User

1. Open `/Auth/User/AssignRole`.
2. Select the user.
3. Select the approved role or roles.
4. Save assignment.
5. Ask the user to sign out and sign in if access does not refresh.

### Validate Branch Access

1. Confirm the user has the correct normal permissions.
2. Assign branch access where the user should be limited by branch.
3. Confirm default branch is included in selected branches.
4. Test filtered pages such as employee, attendance, payroll, and reports.

### Maintain Organization Setup

1. Confirm company and main branch exist.
2. Maintain branches, administrations, departments, and business lines.
3. Avoid deleting active structures that are referenced by employees or payroll.

## Common Mistakes

| Mistake | Avoidance |
| --- | --- |
| Granting branch view-all without business approval. | Use least privilege and document approval. |
| Assuming branch access replaces permission claims. | Always assign both normal permissions and branch access. |
| Giving payroll edit access to HR users by default. | Keep payroll permissions restricted to payroll roles. |
| Editing roles during payroll processing without notice. | Use change windows and communicate. |

## Troubleshooting

| Symptom | Check |
| --- | --- |
| User cannot see a menu item. | Role permission group, branch access, sign-in refresh. |
| User sees too much data. | Branch access, view-all permissions, role overlap. |
| User can view but not edit. | Edit/create/delete action permissions. |
| Role changes do not apply. | User token/session refresh may be required. |

## Escalation

Escalate unresolved access defects to IT Lead and Implementation Lead. Escalate permission policy decisions to HR Sponsor and Executive Sponsor.
