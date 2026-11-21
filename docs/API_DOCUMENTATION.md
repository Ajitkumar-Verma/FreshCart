# FreshCart API and Swagger Guide

## Open the API reference

Run `FreshCart.Api` with the Development environment, then open `/swagger` on the API's local base address. Swagger reads the XML comments generated from the controller source and displays endpoint summaries, parameters, request schemas, and response codes. The application currently enables Swagger only in Development.

For protected operations, select **Authorize** in Swagger and enter the access token returned by login or registration. Swagger formats it as an HTTP Bearer token. Anonymous registration, login, bootstrap, and product catalog operations do not require a token.

## Endpoints

| Method | Route | Access | Parameters | Success | Other documented responses |
| --- | --- | --- | --- | --- | --- |
| POST | `/api/auth/register` | Anonymous | JSON `RegisterRequest` | 200 `ApiResponse<AuthResponse>` | 400 validation, 409 email conflict, 500 server error |
| POST | `/api/auth/login` | Anonymous | JSON `LoginRequest` | 200 `ApiResponse<AuthResponse>` | 400 validation, 401 invalid credentials, 500 server error |
| POST | `/api/superusers` | Platform administrator JWT | JSON `CreateSuperUserRequest` | 200 `ApiResponse<AuthResponse>` | 400 validation, 401 unauthenticated, 403 forbidden, 409 conflict, 500 server error |
| POST | `/api/superusers/bootstrap` | Anonymous with deployment key | JSON `CreateSuperUserRequest`; `X-Bootstrap-Key` header | 200 `ApiResponse<AuthResponse>` | 400 validation, 403 invalid key, 409 bootstrap/email conflict, 500 server error |
| GET | `/api/products` | Anonymous | Optional `shopId`, `categoryId`, `search` query parameters | 200 `ApiResponse<Product[]>` | 400 invalid filter, 500 server error |
| GET | `/api/products/{productId}` | Anonymous | GUID `productId` route parameter | 200 `ApiResponse<Product>` | 400 invalid ID, 404 missing product, 500 server error |
| POST | `/api/products` | Shop owner, shop staff, or platform administrator JWT | JSON `ProductCreateRequest` including an initial variant | 201 product and its initial variant | 400 validation, 401 unauthenticated, 403 shop access denied, 500 server error |
| PUT | `/api/products/{productId}` | Shop owner, shop staff, or platform administrator JWT | GUID `productId`; JSON `ProductUpdateRequest` | 200 updated product | 400 validation, 401 unauthenticated, 403 shop access denied, 404 product/category not found, 500 server error |
| DELETE | `/api/products/{productId}` | Shop owner, shop staff, or platform administrator JWT | GUID `productId` | 200 soft-deleted result | 401 unauthenticated, 403 shop access denied, 404 product not found, 500 server error |
| GET | `/api/products/{productId}/variants` | Anonymous | GUID `productId` | 200 active sale variants | 404 product not found, 500 server error |
| POST | `/api/products/{productId}/variants` | Shop owner, shop staff, or platform administrator JWT | GUID `productId`; JSON `ProductVariantRequest` | 201 created variant and location | 400 validation, 401 unauthenticated, 403 shop access denied, 404 product not found, 500 server error |
| GET | `/api/products/{productId}/variants/{variantId}` | Shop owner, shop staff, or platform administrator JWT | GUID `productId` and `variantId` | 200 managed variant | 401 unauthenticated, 403 shop access denied, 404 variant not found, 500 server error |
| PUT | `/api/products/{productId}/variants/{variantId}` | Shop owner, shop staff, or platform administrator JWT | GUID `productId` and `variantId`; JSON `ProductVariantRequest` | 200 updated variant | 400 validation, 401 unauthenticated, 403 shop access denied, 404 variant not found, 409 default variant conflict, 500 server error |
| DELETE | `/api/products/{productId}/variants/{variantId}` | Shop owner, shop staff, or platform administrator JWT | GUID `productId` and `variantId` | 200 soft-deleted result | 401 unauthenticated, 403 shop access denied, 404 variant not found, 409 default variant conflict, 500 server error |
| GET | `/api/categories` | Anonymous | None | 200 active category list | 500 server error |
| GET | `/api/categories/{categoryId}` | Anonymous | GUID `categoryId` route parameter | 200 active category | 404 category not found, 500 server error |
| POST | `/api/categories` | Platform administrator JWT | JSON `CategoryRequest` | 201 created category and location | 400 invalid category/parent, 401 unauthenticated, 403 forbidden, 409 slug conflict, 500 server error |
| PUT | `/api/categories/{categoryId}` | Platform administrator JWT | GUID `categoryId`; JSON `CategoryRequest` | 200 updated category | 400 invalid category/parent, 401 unauthenticated, 403 forbidden, 404 category not found, 409 slug conflict, 500 server error |
| DELETE | `/api/categories/{categoryId}` | Platform administrator JWT | GUID `categoryId` | 200 soft-deleted result | 401 unauthenticated, 403 forbidden, 404 category not found, 500 server error |
| GET | `/api/shops` | Anonymous | None | 200 approved shops | 500 server error |
| GET | `/api/shops/mine` | Shop owner, shop staff, or platform administrator JWT | None | 200 shops the caller manages | 401 unauthenticated, 403 forbidden, 500 server error |
| GET | `/api/shops/{shopId}` | Shop owner, shop staff, or platform administrator JWT | GUID `shopId` | 200 managed shop | 401 unauthenticated, 403 forbidden, 404 shop not found, 500 server error |
| POST | `/api/shops` | Shop owner or platform administrator JWT | JSON `ShopRequest` | 201 created shop in pending approval | 400 validation, 401 unauthenticated, 403 forbidden, 409 slug conflict, 500 server error |
| PUT | `/api/shops/{shopId}` | Shop owner, shop staff, or platform administrator JWT | GUID `shopId`; JSON `ShopRequest` | 200 updated shop | 400 validation, 401 unauthenticated, 403 forbidden, 404 shop not found, 409 slug conflict, 500 server error |
| PUT | `/api/shops/{shopId}/status` | Platform administrator JWT | GUID `shopId`; JSON `ShopStatusRequest` | 200 updated approval state | 400 invalid status, 401 unauthenticated, 403 forbidden, 404 shop not found, 500 server error |
| DELETE | `/api/shops/{shopId}` | Shop owner, shop staff, or platform administrator JWT | GUID `shopId` | 200 soft-deleted result | 401 unauthenticated, 403 forbidden, 404 shop not found, 500 server error |
| GET | `/api/addresses` | Authenticated user | None | 200 user's addresses | 401 unauthenticated, 500 server error |
| GET | `/api/addresses/{addressId}` | Authenticated user | GUID `addressId` | 200 address owned by caller | 401 unauthenticated, 404 address not found, 500 server error |
| POST | `/api/addresses` | Authenticated user | JSON `AddressRequest` | 201 created address and location | 400 validation, 401 unauthenticated, 500 server error |
| PUT | `/api/addresses/{addressId}` | Authenticated user | GUID `addressId`; JSON `AddressRequest` | 200 updated address | 400 validation, 401 unauthenticated, 404 address not found, 500 server error |
| PUT | `/api/addresses/{addressId}/default` | Authenticated user | GUID `addressId` | 200 selected default address | 401 unauthenticated, 404 address not found, 500 server error |
| DELETE | `/api/addresses/{addressId}` | Authenticated user | GUID `addressId` | 200 soft-deleted result | 401 unauthenticated, 404 address not found, 500 server error |
| GET | `/api/cart` | Authenticated user | None | 200 cart, items, current line totals, subtotal, and availability | 401 unauthenticated, 500 server error |
| POST | `/api/cart/items/{productVariantId}` | Authenticated user | GUID `productVariantId`; JSON `CartItemRequest` | 200 updated cart | 400 validation, 401 unauthenticated, 404 unavailable product variant, 500 server error |
| PUT | `/api/cart/items/{productVariantId}` | Authenticated user | GUID `productVariantId`; JSON `CartItemRequest` | 200 updated cart | 400 validation, 401 unauthenticated, 404 cart item not found, 500 server error |
| DELETE | `/api/cart/items/{productVariantId}` | Authenticated user | GUID `productVariantId` | 200 updated cart | 401 unauthenticated, 404 cart item not found, 500 server error |
| GET | `/api/wishlists` | Authenticated user | None | 200 owned wishlists | 401 unauthenticated, 500 server error |
| POST | `/api/wishlists` | Authenticated user | JSON `WishlistRequest` | 201 created wishlist and location | 400 validation, 401 unauthenticated, 409 name conflict, 500 server error |
| GET | `/api/wishlists/{wishlistId}` | Authenticated user | GUID `wishlistId` | 200 owned wishlist | 401 unauthenticated, 404 wishlist not found, 500 server error |
| PUT | `/api/wishlists/{wishlistId}` | Authenticated user | GUID `wishlistId`; JSON `WishlistRequest` | 200 renamed wishlist | 400 validation, 401 unauthenticated, 404 wishlist not found, 409 name conflict, 500 server error |
| DELETE | `/api/wishlists/{wishlistId}` | Authenticated user | GUID `wishlistId` | 200 soft-deleted result | 401 unauthenticated, 404 wishlist not found, 500 server error |
| GET | `/api/wishlists/{wishlistId}/items` | Authenticated user | GUID `wishlistId` | 200 currently visible saved products | 401 unauthenticated, 404 wishlist not found, 500 server error |
| POST | `/api/wishlists/{wishlistId}/items/{productId}` | Authenticated user | GUID `wishlistId`, `productId` | 200 saved result | 401 unauthenticated, 404 wishlist/product not found, 500 server error |
| DELETE | `/api/wishlists/{wishlistId}/items/{productId}` | Authenticated user | GUID `wishlistId`, `productId` | 200 soft-deleted result | 401 unauthenticated, 404 wishlist item not found, 500 server error |
| GET | `/api/shops/{shopId}/branches` | Anonymous | GUID `shopId` | 200 active approved shop branches | 500 server error |
| GET | `/api/shops/{shopId}/branches/{branchId}` | Shop owner, shop manager, or platform administrator JWT | GUID `shopId`, `branchId` | 200 managed branch | 401 unauthenticated, 403 forbidden, 404 branch not found, 500 server error |
| POST | `/api/shops/{shopId}/branches` | Shop owner, shop manager, or platform administrator JWT | JSON `ShopBranchRequest` | 201 created branch | 400 validation, 401 unauthenticated, 403 forbidden, 500 server error |
| PUT | `/api/shops/{shopId}/branches/{branchId}` | Shop owner, shop manager, or platform administrator JWT | JSON `ShopBranchRequest` | 200 updated branch | 400 validation, 401 unauthenticated, 403 forbidden, 404 branch not found, 500 server error |
| DELETE | `/api/shops/{shopId}/branches/{branchId}` | Shop owner, shop manager, or platform administrator JWT | GUID `shopId`, `branchId` | 200 soft-deleted result | 401 unauthenticated, 403 forbidden, 404 branch not found, 500 server error |
| GET | `/api/delivery-zones?postalCode={postalCode}` | Anonymous | Postal code | 200 approved shops serving the postal code | 400 invalid postal code, 500 server error |
| GET | `/api/shops/{shopId}/branches/{branchId}/delivery-zones` | Shop owner, manager, or platform administrator JWT | Branch GUID | 200 branch delivery zones | 401 unauthenticated, 403 forbidden, 404 branch not found |
| POST | `/api/shops/{shopId}/branches/{branchId}/delivery-zones` | Shop owner, manager, or platform administrator JWT | JSON `DeliveryZoneRequest` | 201 delivery zone created | 400 validation, 401 unauthenticated, 403 forbidden, 404 branch not found |
| PUT | `/api/shops/{shopId}/branches/{branchId}/delivery-zones/{zoneId}` | Shop owner, manager, or platform administrator JWT | JSON `DeliveryZoneRequest` | 200 updated delivery zone | 400 validation, 401 unauthenticated, 403 forbidden, 404 zone not found |
| DELETE | `/api/shops/{shopId}/branches/{branchId}/delivery-zones/{zoneId}` | Shop owner, manager, or platform administrator JWT | Zone GUID | 200 soft-deleted result | 401 unauthenticated, 403 forbidden, 404 zone not found |
| GET | `/api/shops/{shopId}/branches/{branchId}/delivery-slots` | Shop owner, manager, or platform administrator JWT | Branch GUID | 200 recurring slot templates | 401 unauthenticated, 403 forbidden, 404 branch not found |
| POST | `/api/shops/{shopId}/branches/{branchId}/delivery-slots` | Shop owner, manager, or platform administrator JWT | JSON `DeliverySlotRequest` | 201 slot template created | 400 validation, 401 unauthenticated, 403 forbidden, 404 branch not found |
| PUT | `/api/shops/{shopId}/branches/{branchId}/delivery-slots/{slotId}` | Shop owner, manager, or platform administrator JWT | JSON `DeliverySlotRequest` | 200 updated slot template | 400 validation, 401 unauthenticated, 403 forbidden, 404 slot not found |
| DELETE | `/api/shops/{shopId}/branches/{branchId}/delivery-slots/{slotId}` | Shop owner, manager, or platform administrator JWT | Slot GUID | 200 soft-deleted result | 401 unauthenticated, 403 forbidden, 404 slot not found |
| GET | `/api/shops/{shopId}/memberships` | Shop owner or platform administrator JWT | GUID `shopId` | 200 active memberships | 401 unauthenticated, 403 forbidden, 500 server error |
| POST | `/api/shops/{shopId}/memberships` | Shop owner, shop staff, or platform administrator JWT; service authorizes owner/manager | JSON `ShopMembershipRequest` (`Staff` or `Manager`) | 201 membership created | 400 validation, 401 unauthenticated, 403 forbidden, 409 membership exists, 500 server error |
| PUT | `/api/shops/{shopId}/memberships/{membershipId}` | Shop owner, shop staff, or platform administrator JWT; service authorizes owner/manager | JSON `ShopMembershipRequest` | 200 membership updated | 400 validation, 401 unauthenticated, 403 forbidden, 404 membership not found, 500 server error |
| DELETE | `/api/shops/{shopId}/memberships/{membershipId}` | Shop owner, shop staff, or platform administrator JWT; service authorizes owner/manager | GUID membership ID | 200 deactivated/soft-deleted result | 401 unauthenticated, 403 forbidden, 404 membership not found, 500 server error |
| GET | `/api/shops/{shopId}/inventory` | Shop owner, shop staff, shop manager, or platform administrator JWT | GUID `shopId` | 200 branch inventory rows | 401 unauthenticated, 403 forbidden, 500 server error |
| POST | `/api/shops/{shopId}/inventory/branches/{branchId}/variants/{productVariantId}` | Shop owner, shop staff, shop manager, or platform administrator JWT | Branch and variant GUIDs | 201 empty inventory row | 401 unauthenticated, 403 forbidden, 404 branch/variant not found, 409 inventory exists, 500 server error |
| POST | `/api/shops/{shopId}/inventory/{inventoryId}/adjustments` | Shop owner, shop staff, shop manager, or platform administrator JWT | GUID `inventoryId`; JSON `InventoryAdjustmentRequest` | 200 updated inventory and movement | 400 validation, 401 unauthenticated, 403 forbidden, 404 inventory not found, 409 insufficient stock, 500 server error |
| GET | `/api/orders` | Authenticated customer | None | 200 customer's orders | 401 unauthenticated, 500 server error |
| POST | `/api/orders/checkout` | Authenticated customer | JSON `CheckoutRequest` with owned address, one branch/slot per cart shop, optional global coupon, COD method | 201 server-priced order; inventory adjusted and cart cleared in one transaction | 400 invalid request/payment method, 401 unauthenticated, 404 address not found, 409 stock/delivery/coupon/cart conflict, 500 server error |
| GET | `/api/orders/{orderId}` | Order owner or platform administrator JWT | GUID `orderId` | 200 order summary | 401 unauthenticated, 403 forbidden, 404 order not found, 500 server error |
| GET | `/api/orders/{orderId}/payment` | Order owner or platform administrator JWT | GUID `orderId` | 200 payment record and status | 401 unauthenticated, 403 forbidden, 404 order/payment not found, 500 server error |
| POST | `/api/orders/{orderId}/payment/cash-received` | Platform administrator JWT | JSON `CashPaymentConfirmationRequest` | 200 COD payment reconciled as paid | 400 invalid receipt reference, 401 unauthenticated, 403 forbidden, 404 order/payment not found, 409 not delivered or payment not pending COD, 500 server error |
| POST | `/api/orders/{orderId}/cancel` | Order owner | GUID `orderId` | 200 cancelled order summary | 401 unauthenticated, 404 order not found, 409 order cannot be cancelled, 500 server error |
| GET | `/api/shops/{shopId}/orders` | Shop owner, shop manager, or platform administrator JWT | GUID `shopId` | 200 shop order summaries | 401 unauthenticated, 403 forbidden, 500 server error |
| PUT | `/api/shops/{shopId}/orders/{shopOrderId}/status` | Shop owner, shop manager, or platform administrator JWT | JSON `OrderStatusRequest` | 200 updated shop order status | 400 validation, 401 unauthenticated, 403 forbidden, 404 order not found, 409 invalid transition, 500 server error |
| GET | `/api/products/{productId}/reviews` | Anonymous | GUID `productId` | 200 published reviews | 500 server error |
| GET | `/api/products/{productId}/images` | Anonymous | GUID `productId` | 200 product image URL metadata | 404 product not found, 500 server error |
| POST | `/api/products/{productId}/images` | Shop owner, staff, manager, or platform administrator JWT | JSON `ProductImageRequest` | 201 image URL metadata created | 400 validation, 401 unauthenticated, 403 forbidden, 404 product not found, 500 server error |
| PUT | `/api/products/{productId}/images/{imageId}` | Shop owner, staff, manager, or platform administrator JWT | JSON `ProductImageRequest` | 200 updated image metadata | 400 validation, 401 unauthenticated, 403 forbidden, 404 image not found, 500 server error |
| DELETE | `/api/products/{productId}/images/{imageId}` | Shop owner, staff, manager, or platform administrator JWT | Product and image GUIDs | 200 soft-deleted result | 401 unauthenticated, 403 forbidden, 404 image not found, 500 server error |
| PUT | `/api/products/{productId}/reviews/order-items/{orderItemId}` | Authenticated customer who received delivery | JSON `ProductReviewRequest` | 200 saved review | 400 validation, 401 unauthenticated, 403 purchase/delivery required, 500 server error |
| PATCH | `/api/products/{productId}/reviews/moderation/{reviewId}?isPublished={bool}` | Platform administrator JWT | GUID `reviewId`, visibility query | 200 updated review | 401 unauthenticated, 403 forbidden, 404 review not found, 500 server error |
| DELETE | `/api/products/{productId}/reviews/mine/{reviewId}` | Review owner | GUID `productId`, `reviewId` | 200 soft-deleted result | 401 unauthenticated, 404 review not found, 500 server error |
| GET | `/api/notifications` | Authenticated user | None | 200 owned notifications | 401 unauthenticated, 500 server error |
| PUT | `/api/notifications/{notificationId}/read` | Notification owner | GUID `notificationId` | 200 updated notification | 401 unauthenticated, 404 notification not found, 500 server error |
| DELETE | `/api/notifications/{notificationId}` | Notification owner | GUID `notificationId` | 200 soft-deleted result | 401 unauthenticated, 404 notification not found, 500 server error |
| GET | `/api/coupons/validate?code={code}&subtotal={amount}&shopId={guid}` | Anonymous | Code, subtotal, optional shop GUID | 200 valid coupon | 400 invalid request, 404 coupon not found, 409 coupon cannot apply, 500 server error |
| GET | `/api/coupons` | Platform administrator JWT | None | 200 active coupons | 401 unauthenticated, 403 forbidden, 500 server error |
| POST | `/api/coupons` | Shop owner or platform administrator JWT | JSON `CouponRequest` | 201 coupon created | 400 validation, 401 unauthenticated, 403 forbidden, 409 duplicate code, 500 server error |
| PUT | `/api/coupons/{couponId}` | Shop owner or platform administrator JWT | JSON `CouponRequest` | 200 coupon updated | 400 validation, 401 unauthenticated, 403 forbidden, 404 coupon not found, 409 duplicate code, 500 server error |
| DELETE | `/api/coupons/{couponId}` | Shop owner or platform administrator JWT | GUID `couponId` | 200 soft-deleted result | 401 unauthenticated, 403 forbidden, 404 coupon not found, 500 server error |
| GET | `/api/users` | Platform administrator JWT | None | 200 `ApiResponse<ManagedUserResponse[]>` | 401 unauthenticated, 403 forbidden, 500 server error |
| GET | `/api/users/{userId}` | Platform administrator JWT | GUID `userId` route parameter | 200 `ApiResponse<ManagedUserResponse>` | 401 unauthenticated, 403 forbidden, 404 user not found, 500 server error |
| POST | `/api/users` | Platform administrator JWT | JSON `CreateManagedUserRequest` | 201 `ApiResponse<ManagedUserResponse>` and location | 400 validation/role, 401 unauthenticated, 403 forbidden, 409 email conflict, 500 server error |
| PUT | `/api/users/{userId}` | Platform administrator JWT | GUID `userId`; JSON `UpdateManagedUserRequest` | 200 `ApiResponse<ManagedUserResponse>` | 400 validation, 401 unauthenticated, 403 forbidden, 404 user not found, 409 email conflict, 500 server error |
| DELETE | `/api/users/{userId}` | Platform administrator JWT | GUID `userId` route parameter | 200 `ApiResponse<UserDeletionResponse>`; account is soft-deleted | 401 unauthenticated, 403 forbidden, 404 user not found, 409 self-delete/last admin conflict, 500 server error |

The user creation API assigns `Customer` when no role is supplied. It also accepts `ShopOwner` and `ShopStaff`; `PlatformAdmin` accounts must be created through `/api/superusers`. User profile updates do not change passwords or roles. Deactivation sets `IsDeleted` and audit fields; it never removes the database row. The API prevents the current administrator from deactivating their own account and preserves at least one active platform administrator.

All endpoints use a consistent `ApiResponse<T>` envelope. It contains the HTTP `status`, a `success` flag, an optional error `code`, a localized `message`, `data`, `traceId`, and `path`. Success responses put the operation result in `data`; error responses set `success` to `false`, provide an error code, and set `data` to `null`. Validation, application, authentication, and authorization errors use this same shape. For example, a missing user returns HTTP 404 and a body containing `"status": 404`.

## Current schema-backed scope and gaps

- Checkout creates customer, shop, item, payment, coupon-use, and status-history rows in a single unit-of-work transaction. It computes totals from current cart prices, validates the customer's saved address, per-shop delivery zone and active slot template, deducts inventory at the selected branch, and clears cart rows only after all writes succeed. The current slot table has no delivery date or capacity reservation counter, so date-specific availability and overbooking prevention remain unavailable; payment checkout currently supports cash on delivery only.
- Payment APIs currently read payment status and let a platform administrator reconcile a delivered COD order with an external receipt reference. The solution has no online payment gateway or webhook verification, and refund request/settlement APIs remain unimplemented.
- Delivery branch, zone, and recurring slot-template management endpoints are implemented. The current slot table does not describe a service date or booked quantity, so date-specific slot availability and booking remain unavailable.
- Product image URL metadata endpoints are implemented. Binary image upload/storage needs a configured storage provider; the table only stores URLs. Audit log queries, dashboard/report endpoints, and coupon-use/order application workflows remain to be implemented.
- Multiple shops are supported. The database has no tenant registry or tenant isolation key, so hosting multiple independent FreshCart tenants is not yet implemented.
- ASP.NET Identity claims, login, role assignment, and token tables are intentionally accessed through authentication and administration workflows rather than generic CRUD endpoints.

## Keeping the reference accurate

- Add an XML `<summary>` for every controller action and `<param>` descriptions for every request parameter.
- Add XML `<response>` entries and `ProducesResponseType` attributes for each expected status. Specify a response type when the status includes a response body.
- Bind request values explicitly with `[FromBody]`, `[FromQuery]`, `[FromRoute]`, and `[FromHeader]` so Swagger shows where each value comes from.
- Keep endpoint behavior in the service layer. Controllers handle HTTP binding, status mapping, and localized error responses.
- Update this guide whenever a route, authorization requirement, parameter, or status response changes.
