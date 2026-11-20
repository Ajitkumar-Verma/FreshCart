# Authentication setup

Authentication endpoints:

- `POST /api/auth/register` creates a Customer account and returns a bearer token.
- `POST /api/auth/login` verifies the password hash and returns a bearer token with role claims.
- `POST /api/superusers` creates another PlatformAdmin; it requires an authenticated PlatformAdmin token.
- `POST /api/superusers/bootstrap` creates the first PlatformAdmin one time, when `X-Bootstrap-Key` matches the configured bootstrap secret.

New API endpoints require authentication by default. Public endpoints must explicitly use `[AllowAnonymous]`. The product catalog is public. The API validates token signature, issuer, audience, lifetime, and role claims. Passwords are hashed with the ASP.NET Core Identity password hasher and are never returned in responses.

## Local secrets

The API project already has a User Secrets ID. From the repository root, set development-only secrets:

```powershell
dotnet user-secrets set "Jwt:SigningKey" "<at-least-32-random-bytes>" --project src/FreshCart.Api/FreshCart.Api.csproj
dotnet user-secrets set "Bootstrap:SuperUserKey" "<long-random-one-time-bootstrap-secret>" --project src/FreshCart.Api/FreshCart.Api.csproj
```

Use secret management or environment variables in production. After the first PlatformAdmin is created through bootstrap, remove/rotate the bootstrap secret. The bootstrap endpoint becomes unavailable for creation once an active PlatformAdmin account exists. Apply request-rate limiting at the API or gateway before production use.

Run the database project first so the auth tables, roles, indexes, and stored procedures exist. The post-deployment script seeds the role names used by the API.

## Multi-tenant status

The current schema supports multiple shops in one deployment, but it is **not yet a tenant-isolated SaaS API**: there are no tenant/domain tables, no tenant context resolver, and auth tokens do not yet contain a tenant claim. See [MULTITENANCY.md](MULTITENANCY.md) for the recommended next step before onboarding independent vendors.


