CREATE PROCEDURE dbo.ProductVariant_Create
    @Id UNIQUEIDENTIFIER,
    @ProductId UNIQUEIDENTIFIER,
    @SKU NVARCHAR(80),
    @UnitName NVARCHAR(40),
    @UnitQuantity DECIMAL(10,3),
    @UnitPrice DECIMAL(10,2),
    @CurrencyCode CHAR(3),
    @IsDefault BIT,
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.ProductVariants
    (
        Id,
        ProductId,
        SKU,
        UnitName,
        UnitQuantity,
        UnitPrice,
        CurrencyCode,
        IsDefault,
        CreatedBy
    )
    VALUES
    (
        @Id,
        @ProductId,
        @SKU,
        @UnitName,
        @UnitQuantity,
        @UnitPrice,
        @CurrencyCode,
        @IsDefault,
        @CreatedBy
    );
END;
