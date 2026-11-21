CREATE PROCEDURE dbo.ProductVariant_Update
    @Id UNIQUEIDENTIFIER,
    @ProductId UNIQUEIDENTIFIER,
    @SKU NVARCHAR(80),
    @UnitName NVARCHAR(40),
    @UnitQuantity DECIMAL(10,3),
    @UnitPrice DECIMAL(10,2),
    @CurrencyCode CHAR(3),
    @IsDefault BIT,
    @IsActive BIT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.ProductVariants
    SET SKU = @SKU,
        UnitName = @UnitName,
        UnitQuantity = @UnitQuantity,
        UnitPrice = @UnitPrice,
        CurrencyCode = @CurrencyCode,
        IsDefault = @IsDefault,
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @Id
      AND ProductId = @ProductId
      AND IsDeleted = 0;
END;
