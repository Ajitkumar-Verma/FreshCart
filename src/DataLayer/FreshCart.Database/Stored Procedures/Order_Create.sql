CREATE PROCEDURE dbo.Order_Create
    @Id UNIQUEIDENTIFIER,
    @OrderNumber NVARCHAR(32),
    @UserId UNIQUEIDENTIFIER,
    @AddressId UNIQUEIDENTIFIER,
    @OrderStatus TINYINT,
    @PaymentStatus TINYINT,
    @Subtotal DECIMAL(12,2),
    @DeliveryFee DECIMAL(12,2),
    @DiscountAmount DECIMAL(12,2),
    @GrandTotal DECIMAL(12,2),
    @CurrencyCode CHAR(3),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Orders
    (
        Id, OrderNumber, UserId, AddressId, OrderStatus, PaymentStatus,
        Subtotal, DeliveryFee, DiscountAmount, GrandTotal, CurrencyCode, CreatedBy
    )
    VALUES
    (
        @Id, @OrderNumber, @UserId, @AddressId, @OrderStatus, @PaymentStatus,
        @Subtotal, @DeliveryFee, @DiscountAmount, @GrandTotal, @CurrencyCode, @CreatedBy
    );

    SELECT Id, OrderNumber, OrderStatus, PaymentStatus, Subtotal, DeliveryFee,
           DiscountAmount, GrandTotal, CurrencyCode, CreatedOn
    FROM dbo.Orders
    WHERE Id = @Id;
END;
