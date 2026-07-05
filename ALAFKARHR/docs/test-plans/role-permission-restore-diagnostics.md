# Role Permission Diagnostics After Database Restore

Use this checklist when a user has a company role such as `All Roles`, but protected pages still show a missing-permission message.

## Confirm Role Assignment Shape

Run read-only checks against the restored database:

```sql
DECLARE @UserName nvarchar(256) = N'<user-name>';

SELECT u.Id, u.UserName, u.CompanyId
FROM Auth.AspNetUsers u
WHERE u.UserName = @UserName OR u.NormalizedUserName = UPPER(@UserName);

SELECT r.Id, r.Name, r.DisplayName, r.CompanyId, r.TemplateKey
FROM Auth.AspNetUsers u
JOIN Auth.AspNetUserRoles ur ON ur.UserId = u.Id
JOIN Auth.AspNetRoles r ON r.Id = ur.RoleId
WHERE u.UserName = @UserName OR u.NormalizedUserName = UPPER(@UserName);
```

The assigned role `CompanyId` must match the user's `CompanyId`.

## Confirm Template Role Claims

```sql
DECLARE @CompanyId uniqueidentifier = '<company-id>';

SELECT r.Id, r.Name, r.DisplayName, r.TemplateKey, COUNT(rc.Id) AS PermissionClaimCount
FROM Auth.AspNetRoles r
LEFT JOIN Auth.AspNetRoleClaims rc ON rc.RoleId = r.Id AND rc.ClaimType = 'Permission'
WHERE r.CompanyId = @CompanyId AND r.TemplateKey = 'all-roles'
GROUP BY r.Id, r.Name, r.DisplayName, r.TemplateKey;
```

The permission claim count should be close to `PermissionList.GetTenantPermissions().Count`. A very small or zero count means the restored role claims were not synchronized.

## Confirm Active JWT

After role repair or assignment, the affected user must get a new access token. Sign out and sign in again, or use the role assignment page's current-session refresh when assigning roles to yourself.

Decode the browser access token and confirm it contains normal `Permission` claims, for example:

```text
Permission: Authentication.Users.View
Permission: Authentication.Roles.View
```

`ScopedPermission` claims are branch-role hints and do not satisfy normal company-wide authorization policies.
