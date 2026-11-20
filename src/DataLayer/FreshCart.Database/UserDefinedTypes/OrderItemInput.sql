CREATE TYPE dbo.OrderItemInput AS TABLE
(
    ProductVariantId uniqueidentifier NOT NULL,
    Quantity decimal(10,3) NOT NULL CHECK (Quantity > 0),
    PRIMARY KEY(ProductVariantId)
);

