# Parent Company ERP Onboarding Reference

## Purpose

Use this checklist when onboarding a new parent company into Alafkar ERP. It starts with the parent company record and finishes when employees are created, attached to the correct organization structure, and ready for operational use.

This is an operator reference, not a developer implementation guide. It does not change application behavior, routes, permissions, DTOs, or business rules.

## Before You Start

- Confirm the implementation owner, HR owner, IT/security owner, and finance or licensing owner.
- Collect legal company details, contact details, VAT number, headquarters location, currency, timezone, logo, and licensing information.
- Confirm the target business lines, expected branch count, expected user count, and whether employees need self-service login access.
- Confirm that the operator performing setup has parent company, organization, role/user, branch access, and employee setup permissions.

## Step-By-Step Checklist

| Step | Area | Route | What to do | Done |
| --- | --- | --- | --- | --- |
| 1 | Parent company | `/Organization/ParentCompanies` | Create the parent company record. Capture Arabic and English names, company code, VAT number, headquarters location, phone, email, currency, timezone, and logo if available. | |
| 2 | License | `/Organization/ParentCompanies` | Configure the license or plan on the parent company record. Set plan, start date, end date, max users, max child companies, max branches, monthly or yearly price, currency, and licensed business lines. | |
| 3 | Company admin | `/Organization/ParentCompanies` | Create or confirm the initial company admin. Record the admin username, email, phone number, and temporary password. Verify the admin can log in and is tied to the correct company context. | |
| 4 | Main branch | `/Organization/Branch/List` | Confirm the company has one main branch. Check name, code, location, phone, email, specialization, company association, and main branch flag. | |
| 5 | Additional branches | `/Organization/Branch/List`, `/organization/Branch/Form/{Id?}` | Add every operating branch that is in scope. Capture Arabic and English names, code, location, phone, email, specialization, and company association. | |
| 6 | Administrations | `/Organization/Administration/List` | Add administrations or management units. Link each one to the correct company and branch when applicable. Mark higher management units where the business requires it. | |
| 7 | Departments | `/Organization/Department/List` | Add departments. Link each department to the correct administration and branch where applicable. Confirm names, codes, active status, and attendance location settings if used. | |
| 8 | Security roles | `/Auth/Role/List` | Review seeded company roles first. Create custom roles only when needed, using tenant/company permissions and least-privilege access. | |
| 9 | System users | `/Auth/User/List`, `/Auth/User/Form` | Add ERP users who need system access. Capture username, email, phone, password policy requirements, and company membership. | |
| 10 | User role assignment | `/Auth/User/AssignRole` | Assign each user to the correct company role. Confirm setup users, HR users, managers, payroll users, attendance users, and employee self-service users have the intended access. | |
| 11 | Branch access | User access workflow | For branch-restricted users, assign allowed branches and a default branch. Confirm the default branch is included in the selected branch list. Keep company-wide role permissions separate from branch visibility. | |
| 12 | Positions | `/Employee/Position/List` | Create required positions before employee entry. Capture Arabic and English title, code, base salary if used, and company. | |
| 13 | Specializations | `/Employee/Specialization/List` | Create specializations needed for employee profiles. Capture Arabic and English names and company. | |
| 14 | Academic institutions | `/Employee/AcademicInistitution/List` | Create academic institutions if employee education data will be tracked. Capture Arabic and English names and company. | |
| 15 | Teams | `/Employee/Teams` | Create employee teams if the implementation needs project, operational, or grouping structures. | |
| 16 | Employees | `/Employee/Employee/List`, `/Employee/Employee/Form/{Id:guid?}` | Add employee records. Capture employee number, Arabic and English name fields, email, phone, birth date, national ID, hire date, company, branch, administration, department, position, manager, grade, work location, attendance type, nationality, address, marital status, employment type, qualification, specialization, academic institution, and graduation year as applicable. | |
| 17 | Employee user link | `/Auth/User/List`, `/Auth/User/AssignRole`, employee form/profile | For employees who need login access, create or confirm the corresponding user, assign roles, assign branch access, and link the employee profile to the user account where supported. | |
| 18 | Final readiness check | `/Employee/Employee/List`, employee profile pages | Confirm employees appear in the list and profile pages. Validate company, branch, administration, department, position, manager, active status, and user access. | |

## Acceptance Checklist

- Parent company exists and is active.
- License limits, dates, pricing, currency, and business lines are configured.
- Company admin can log in and sees the correct company context.
- Main branch exists and is marked correctly.
- Required additional branches exist.
- Administrations and departments match the approved organization structure.
- Seeded roles were reviewed before custom roles were created.
- Users are assigned to the correct company roles.
- Branch-restricted users have selected branches and a valid default branch.
- Positions, specializations, academic institutions, and teams exist before employee creation when needed.
- Employees are visible in `/Employee/Employee/List`.
- Employee profiles show the correct company, branch, administration, department, position, manager, and active status.
- Login-enabled employees have matching user accounts, roles, and branch access.

## Scope And Permission Notes

- This flow is parent-company onboarding. Child-company onboarding should be handled as a separate checklist.
- Parent company and license setup are company/platform administration activities.
- Branches, administrations, departments, positions, lookup data, users, roles, and employees are company-scoped, with branch visibility layered on top where applicable.
- Normal permissions control access to pages and actions. Branch access controls which branches a user can see or manage; it does not replace normal role permissions.
- Users with view-all branch access still need the relevant company role permissions for the operation they perform.
- Any user-selected company, branch, department, position, employee, or linked user should be verified against the active company context by the application.

## Common Data To Prepare

| Data group | Examples |
| --- | --- |
| Company identity | Arabic name, English name, code, VAT number, logo, phone, email, HQ location. |
| Localization | Currency, timezone, Arabic/English naming standards. |
| License | Plan, dates, limits, price, licensed business lines. |
| Branches | Main branch, operating branches, location, specialization, contact details. |
| Organization | Administrations, departments, hierarchy, managers. |
| Security | Roles, users, role assignments, branch access, default branches. |
| HR lookups | Positions, specializations, academic institutions, teams. |
| Employees | Employee number, names, contact details, identity, job assignment, manager, work location, attendance profile, education details. |

## Handover Notes

- Give the company admin the initial login details through the approved secure channel.
- Ask HR to validate employee records before attendance, leave, payroll, or self-service workflows begin.
- Ask IT/security to validate role and branch access with at least one restricted user.
- Keep the signed checklist with the implementation evidence for UAT and go-live readiness.
