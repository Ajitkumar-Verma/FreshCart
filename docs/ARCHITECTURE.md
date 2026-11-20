# FreshCart Architecture Guide

This guide explains the current FreshCart solution structure and how an API request reaches SQL Server. It is written for developers who are new to the repository.

## 1. Solution at a glance

FreshCart is organized as a React frontend, an ASP.NET Core Web API, shared contracts/models, business-layer projects, data-layer projects, and a SQL Server database project.

```text
FreshCart/
├── FreshCart.sln
├── frontend/                         React + Vite application
├── database/                         Standalone SQL schema/seed draft
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

Contains SQL Server data access. `ProductRepository` calls stored procedures with parameterized inputs and maps result rows to shared models. Keep SQL execution and row mapping here.

### `FreshCart.Database`

The SQL Server Database Project contains schema, stored procedures, roles, user-defined types, and post-deployment scripts. It uses SSDT (`.sqlproj`) and normally builds/deploys from Visual Studio with the SQL Server Data Tools workload installed.

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

- `Tables/Marketplace.sql` — marketplace tables and indexes.
- `Stored Procedures/Catalog.sql` — catalog read procedures and inventory adjustment procedure.
- `Security/Roles.sql` — database role/security setup.
- `UserDefinedTypes/OrderItemInput.sql` — table-valued type for order-item input.
- `Scripts/Script.PostDeployment.sql` — post-deployment seed script.

There is also a standalone `database/001-schema-and-seed.sql` draft at the repository root. Before deploying, decide which source is authoritative and keep the schema in one canonical location to avoid drift.

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
