# FreshCart Architecture Guide

This guide explains the current FreshCart solution structure and how an API request reaches SQL Server. It is written for developers who are new to the repository.

## 1. Solution at a glance

FreshCart is organized as a React frontend, an ASP.NET Core Web API, shared contracts/models, business-layer projects, data-layer projects, and a SQL Server database project.

```text
FreshCart/
├── FreshCart.sln
├── frontend/                         React + Vite application
├── database/                         Database source-of-truth pointer
└── src/
    ├── FreshCart.Api/                HTTP entry point and controllers
    ├── FreshCart.Common/             Shared model types
    ├── BusinessLayer/
    │   ├── FreshCart.IService/       Business service interfaces
    │   └── FreshCart.Service/        Service implementations and DI setup
    └── DataLayer/
        ├── FreshCart.IRepository/    Persistence interfaces
        ├── FreshCart.Repository/     SQL Server repository implementations
        └── FreshCart.Database/       SSDT database project and SQL assets
```

The active C# projects are under `src/`. The root-level `BusinessLayer` and `DataLayer` directories are not where the current `.csproj` files live; use the paths shown above when navigating or adding projects.

## 2. What belongs in each project

### `FreshCart.Api`

The API is the HTTP boundary. Controllers translate HTTP requests into service calls and translate service results into HTTP responses. `Program.cs` configures dependency injection, controllers, Swagger, and middleware.

Keep database queries and business rules out of controllers. Add one controller per area as the API grows, for example `UsersController`, `OrdersController`, and `ShopsController`.

### `FreshCart.Common`

Contains types shared between layers, currently the catalog `Product` record. Place shared domain models and request/response DTOs here when more features are added. Keep database-specific connection code out of this project.

### `FreshCart.IService`

Contains service contracts such as `IProductService`. Controllers depend on these abstractions, which keeps HTTP code independent from business implementation details.

### `FreshCart.Service`

Contains business logic, validation, and coordination. `ProductService` validates filter IDs and delegates reads to `IProductRepository`. `DependencyInjection.cs` registers service implementations through `AddFreshCartServices()`.

### `FreshCart.IRepository`

Contains repository contracts such as `IProductRepository`. The service layer depends on these contracts instead of SQL Server details.

### `FreshCart.Repository`

Contains SQL Server data access. `ProductRepository` uses Dapper to call stored procedures with named parameters and map result rows to shared models. It does not contain inline SQL. Keep SQL execution and persistence mapping here.

### `FreshCart.Database`

The SQL Server Database Project contains schema, stored procedures, roles, user-defined types, and post-deployment scripts. Stored procedure names used by repositories are centralized in `src/DataLayer/FreshCart.Repository/StoredProcedure.cs`. It uses SSDT (`.sqlproj`) and normally builds/deploys from Visual Studio with the SQL Server Data Tools workload installed.

### `frontend`

The current React/Vite entry point is `frontend/src/main.jsx`; styles are in `frontend/src/style.css`. It is currently a storefront UI shell and is not yet wired to the API.

## 3. Example request flow: list available products

The existing endpoint is `GET /api/products` with optional `shopId`, `categoryId`, and `search` query parameters.

```text
HTTP client
  -> ProductsController.GetAvailable
  -> IProductService
  -> ProductService
  -> IProductRepository
  -> ProductRepository
  -> dbo.Product_GetAvailable stored procedure
  -> SQL rows mapped to Common.Catalog.Product
  -> HTTP 200 JSON response
```

The single-product endpoint follows the same path: `GET /api/products/{productId}` calls `Product_GetById` and returns 404 when no active product is found.

## 4. Dependency injection and configuration

At startup, `Program.cs` calls `AddFreshCartServices()` to register services. It registers `IProductRepository` with a `ProductRepository` created from the `ConnectionStrings:FreshCart` setting in `src/FreshCart.Api/appsettings.json`.

For local development, update the connection string for the SQL Server instance you use. Do not commit production credentials or secrets to `appsettings.json`; use environment variables or a managed secret store for deployment.

## 5. Database files

`src/DataLayer/FreshCart.Database` currently contains:

- `Tables/` — one table definition per table-named SQL file, with its indexes in the same file.
- `Stored Procedures/` — one stored procedure per procedure-named SQL file.
- `Security/Roles.sql` — database role/security setup.
- `UserDefinedTypes/OrderItemInput.sql` — table-valued type for order-item input.
- `Scripts/Script.PostDeployment.sql` — post-deployment seed script.

`database/README.md` points to the SSDT project as the schema source of truth.

## 6. How to add another API feature

For a new feature such as user registration or orders, keep the same separation:

1. Add shared model/DTO types in `FreshCart.Common` when multiple layers need them.
2. Add the service interface in `FreshCart.IService/<Feature>`.
3. Add the repository interface in `FreshCart.IRepository/<Feature>`.
4. Implement stored procedures and schema changes in `FreshCart.Database`.
5. Implement database calls in `FreshCart.Repository/<Feature>` using the repository interface and stored procedures.
6. Implement validation and business rules in `FreshCart.Service/<Feature>` using the service and repository interfaces.
7. Register the implementations in dependency injection.
8. Add a focused controller in `FreshCart.Api/Controllers` that calls the service interface.
9. Add frontend API integration and screens in `frontend` as needed.

Keep one primary class or interface per file, named after the type (for example, `IUserService.cs`, `UserService.cs`, `IUserRepository.cs`, `UserRepository.cs`, and `UsersController.cs`).

## 7. Current implementation boundaries

The current API implements read-only product listing and product detail retrieval. The database includes a broader marketplace schema draft, but most of its features do not yet have API/service/repository implementations. User registration/login, JWT authentication and authorization, order placement, payments, owner management, and React-to-API integration are not currently implemented. Do not assume a database table or stored procedure automatically means its feature is available through the API.

The API currently calls `UseAuthorization()` but does not configure authentication or JWT validation. Protected endpoints should not be added until authentication and authorization middleware, token validation, and role/policy configuration are implemented and verified.

## 8. Build and run

From the repository root:

```powershell
dotnet build FreshCart.sln
```

The SQL project requires Visual Studio SSDT targets and may not build with a plain `dotnet` CLI installation. To run the API, configure the SQL Server connection string first, then run:

```powershell
dotnet run --project src/FreshCart.Api/FreshCart.Api.csproj
```

Swagger is enabled in the Development environment. The React app uses Vite; install Node.js and npm, then run `npm install` and `npm run dev` from `frontend/`.

## 9. Repository SQL conventions

Repositories use Dapper with CommandType.StoredProcedure. Keep procedure names in StoredProcedure.cs, add or update procedure definitions in the SSDT database project, and pass inputs as Dapper parameter objects. Do not place inline SQL statements in repository classes. Shared API/domain models remain in FreshCart.Common; don't create a second set of database-only DTOs unless a concrete mapping need arises.


## 10. Shared infrastructure in `FreshCart.Common`

The Common project now contains reusable, provider-neutral building blocks:

- `Configuration/DatabaseOptions.cs` — typed holder for the database connection string.
- `Persistence/IUnitOfWork.cs` and `IDbConnectionFactory.cs` — connection/transaction contracts shared by repository implementations.
- `Persistence/IIdGenerator.cs` and `GuidIdGenerator.cs` — creates GUIDs before inserts so a parent and related child records can share the same identifier.
- `Exceptions/ApplicationExceptions.cs` — application exception types for validation, unauthorized access, forbidden access, missing resources, and conflicts.
- `Contracts/ApiResponse.cs` — shared response envelope with HTTP status, success flag, data, localized message, trace ID, and request path.
- `Localization/` and `Resources/Errors.en.json`, `Errors.fr.json` — English/French error messages.
- `Validation/Guard.cs` — reusable null, blank, positive, non-negative, non-empty-GUID, true, and false checks.

The API exception middleware translates known application exceptions into a safe JSON error response. Unexpected exceptions are logged with a trace ID and return a generic localized 500 response; system exception details are not exposed to clients.

The concrete SQL Server connection factory and `UnitOfWork` are in `FreshCart.Repository`. Repositories receive the scoped `IUnitOfWork`, so multiple repositories resolved in one request can share its connection and active transaction. Use `BeginTransactionAsync`, pass `Transaction` into Dapper commands, then call `SaveChangesAsync`/`CommitAsync`; call `RollbackAsync` on failure. Disposing an uncommitted unit of work rolls back its active transaction.

A GUID and a unit of work solve different problems: the GUID identifies a new row, while the unit of work makes several writes atomic. The current marketplace SQL tables mostly use integer identity keys. `GuidIdGenerator` is available for new GUID-keyed entities, but changing existing tables to GUID keys requires a deliberate schema migration and matching stored procedure/repository changes.

For method/interface conventions and feature-addition rules, see [ARCHITECTURE_RULES.md](ARCHITECTURE_RULES.md).




All persisted tables now use GUID `Id` primary keys and `InternalId` identity alternate keys. Business tables include create/update audit metadata and soft-delete metadata. Foreign keys reference target GUID `Id` columns. The ASP.NET Identity schema therefore expects GUID-based Identity types in the eventual authentication implementation.

Authentication setup and endpoint behavior are documented in [AUTHENTICATION.md](AUTHENTICATION.md). Multi-tenant isolation is not implemented yet; see [MULTITENANCY.md](MULTITENANCY.md) for the recommended design before onboarding independent vendors.

