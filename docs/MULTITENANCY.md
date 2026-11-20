# FreshCart Multi-Tenancy Direction

Yes, one FreshCart API deployment can serve many vendors. A shared application and database are a reasonable starting point for a marketplace, as long as every tenant-owned query enforces tenant isolation. A multi-shop database is not automatically multi-tenant: the current schema has shops but no tenant registry or tenant context, so it does not yet isolate one vendor organization from another.

## Recommended model

1. Add `Tenants` with a GUID `Id`, display name, unique slug, status, and audit/soft-delete columns.
2. Add `TenantDomains` mapping each tenant to one or more hostnames (for example, `vendor-a.freshcart.example` or a verified custom domain). This lets one deployment serve a shared API URL or tenant-specific host names.
3. Add `TenantId` foreign keys to tenant-owned data and ensure every repository stored procedure filters by the trusted tenant context. A shared database/table model is simpler initially; separate databases can be adopted later if isolation or compliance needs require it.
4. Resolve the tenant from a verified request host/domain before application logic runs. Do not trust a `TenantId` supplied in a JSON body or query parameter.
5. Scope registration/login to the resolved tenant and include the tenant ID in the signed JWT. For protected requests, require the token tenant to match the resolved host tenant; global PlatformAdmin operations are explicit exceptions.
6. Keep authorization checks in services and data constraints in stored procedures. Test cross-tenant access attempts for every tenant-owned resource before onboarding vendors.

A path-based scheme such as `/t/{tenantSlug}/...` can also share a single API hostname, though a host/domain mapping usually gives vendors stronger branding. The current `Shops` table can continue representing branches or storefronts inside each tenant; tenant and shop are separate concepts.

Microsoft describes multitenancy as a choice of isolation levels across the application and data tiers; a shared app/database is one valid model when its isolation guarantees match the requirements. See the [Azure multitenancy tenancy models guide](https://learn.microsoft.com/en-us/azure/architecture/guide/multitenant/considerations/tenancy-models).
