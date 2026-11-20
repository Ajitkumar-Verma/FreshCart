# FreshCart Architecture Rules

Apply these rules whenever you add or change a feature. The goal is to keep contracts visible and implementation details in the correct layer.

## Method and interface rules

1. **Add a public operation to its interface and implementation together.** Every new service operation belongs in `FreshCart.IService` and its implementation in `FreshCart.Service`. Every new persistence operation belongs in `FreshCart.IRepository` and its implementation in `FreshCart.Repository`.
2. **Keep signatures aligned.** Method name, parameters, return type, nullability, and cancellation-token behavior must match between the interface and implementation.
3. **Document each new interface method with a one-line XML summary.** The summary states what the method guarantees, including relevant conditions or outcomes. Keep implementation-only details in the concrete class.
4. **Put business conditions in service logic, not controllers or repositories.** If a condition changes the public behavior of an operation, describe that behavior in the interface method summary and implement it in the service.
5. **Controllers call service interfaces only.** They handle HTTP binding/status codes and delegate feature behavior; they do not contain database access or business rules.
6. **Services call repository interfaces only.** They validate inputs, apply business rules, coordinate multiple repositories, and map results to shared contracts.
7. **Repositories call stored procedures only.** Do not add inline SQL. Define procedure names in `src/DataLayer/FreshCart.Repository/StoredProcedure.cs`, define procedures in the SSDT database project, and pass input values as parameters.
8. **Use the scoped unit of work for multi-repository writes.** Start one transaction, ensure all repositories use that scoped unit of work and pass its transaction to Dapper, then commit with `SaveChangesAsync`/`CommitAsync` or roll back on failure.
9. **Use shared exception types.** Throw application exceptions from `FreshCart.Common.Exceptions`; do not return raw database/system exception messages to clients.
10. **Keep one exception type per declaration, but group these related application exception declarations in `ApplicationExceptions.cs`.** Add a new app exception in that file and add its error code to both English and French catalogs.
11. **Use shared guards and identifiers.** Prefer `FreshCart.Common.Validation.Guard` for common input checks. Generate GUIDs once before insert when the entity/schema uses GUID keys; do not change an existing integer-key schema without a coordinated migration.

## Example: adding a service operation

For a new `GetOrderAsync` operation:

- Add an XML-documented `GetOrderAsync(...)` signature to `IOrderService`.
- Implement that exact signature in `OrderService`; add validation and business-condition handling there.
- If persistence is needed, add the corresponding XML-documented method to `IOrderRepository` and implement it with Dapper calling a named stored procedure.
- Add the procedure name to `StoredProcedure.cs` and the SQL definition to the database project.
- Keep the controller action thin: bind the HTTP input, call `IOrderService`, and translate the result to an HTTP response.

## SQL file organization rules

- **One table per SQL file.** Name every file after its table (for example, `Tables/Products.sql` contains only `dbo.Products`). Do not combine unrelated table definitions in a file such as `Marketplace.sql`.
- **One stored procedure per SQL file.** Name each file after the procedure (for example, `Stored Procedures/Product_GetById.sql`). Keep the procedure definition and its required body together in that file only.
- **Keep each standalone deployment script focused on one purpose.** Name scripts clearly and split unrelated seed, migration, or maintenance operations into separate files. The SSDT post-deployment entry point may include the focused seed scripts in order.
- **Keep indexes and other database objects in clearly named object files** when they are managed as independent SSDT objects; use a descriptive object name for each file.
- **Update the SQL project manifest whenever files are added, renamed, or removed.** Ensure every object file is included exactly once and obsolete combined files are removed.

## Table schema and formatting rules

- **Format table DDL for scanning.** Use `CREATE TABLE dbo.TableName`, then one indented column or constraint per line, with aligned names/types where practical. Avoid single-line table definitions.
- **Use `Id UNIQUEIDENTIFIER` as the primary key.** Use `NEWSEQUENTIALID()` as its database default. Add `InternalId BIGINT IDENTITY(1,1)` as a unique clustered alternate key; SQL Server allows only one primary-key constraint, so `InternalId` is not a second primary key.
- **Use GUID foreign keys that reference the target table’s `Id`.** Declare the foreign key in the same table creation file as the column/table, not in a separate combined relationship file. Keep FK names and columns clear.
- **Add audit fields to persisted tables:** `CreatedBy`, `CreatedOn`, `CreatedDate`, `UpdatedBy`, and `UpdatedOn`. Keep soft-delete fields (`IsDeleted`, `DeletedBy`, `DeletedAt`) and use updates to mark deletion; do not hard-delete application records.
- **Keep indexes with their table.** Put indexes after the `CREATE TABLE` statement in that table’s named file, with `GO` separators as needed. Do not create a separate index file for an index that belongs to one table.
- **Use one SQL file per table and one SQL file per stored procedure.** Table files match the table name; procedure files match the procedure name. Format stored procedures with readable line breaks and indentation.
- **Keep scripts focused.** One seed, migration, or deployment action per script file. The SSDT post-deployment script can include those focused files in dependency order.
- **Update the SQL project manifest** when adding/removing objects and check that each object file is included exactly once.


## Foreign-key declaration style

In every table file, declare foreign keys as named table-level constraints inside the `CREATE TABLE` statement:

```sql
UserId UNIQUEIDENTIFIER NOT NULL,
CONSTRAINT FK_Addresses_AspNetUsers FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id)
```

Do not put inline `REFERENCES` clauses on column definitions. Keep the column and its named foreign-key constraint in the same table file. Apply this style to all relationships, including audit-user fields.
