# FreshCart API and Swagger Guide

## Open the API reference

Run `FreshCart.Api` with the Development environment, then open `/swagger` on the API's local base address. Swagger reads the XML comments generated from the controller source and displays endpoint summaries, parameters, request schemas, and response codes. The application currently enables Swagger only in Development.

For protected operations, select **Authorize** in Swagger and enter the access token returned by login or registration. Swagger formats it as an HTTP Bearer token. Anonymous registration, login, bootstrap, and product catalog operations do not require a token.

## Endpoints

| Method | Route | Access | Parameters | Success | Other documented responses |
| --- | --- | --- | --- | --- | --- |
| POST | `/api/auth/register` | Anonymous | JSON `RegisterRequest` | 200 `AuthResponse` | 400 validation, 409 email conflict, 500 server error |
| POST | `/api/auth/login` | Anonymous | JSON `LoginRequest` | 200 `AuthResponse` | 400 validation, 401 invalid credentials, 500 server error |
| POST | `/api/superusers` | Platform administrator JWT | JSON `CreateSuperUserRequest` | 200 `AuthResponse` | 400 validation, 401 unauthenticated, 403 forbidden, 409 conflict, 500 server error |
| POST | `/api/superusers/bootstrap` | Anonymous with deployment key | JSON `CreateSuperUserRequest`; `X-Bootstrap-Key` header | 200 `AuthResponse` | 400 validation, 403 invalid key, 409 bootstrap/email conflict, 500 server error |
| GET | `/api/products` | Anonymous | Optional `shopId`, `categoryId`, `search` query parameters | 200 product list | 400 invalid filter, 500 server error |
| GET | `/api/products/{productId}` | Anonymous | GUID `productId` route parameter | 200 `Product` | 400 invalid ID, 404 missing product, 500 server error |

`ApiErrorResponse` contains `code`, localized `message`, `traceId`, and `path`. Data-annotation validation failures and application exceptions use this response shape. Authorization middleware may return its standard empty 401/403 response.

## Keeping the reference accurate

- Add an XML `<summary>` for every controller action and `<param>` descriptions for every request parameter.
- Add XML `<response>` entries and `ProducesResponseType` attributes for each expected status. Specify a response type when the status includes a response body.
- Bind request values explicitly with `[FromBody]`, `[FromQuery]`, `[FromRoute]`, and `[FromHeader]` so Swagger shows where each value comes from.
- Keep endpoint behavior in the service layer. Controllers handle HTTP binding, status mapping, and localized error responses.
- Update this guide whenever a route, authorization requirement, parameter, or status response changes.
