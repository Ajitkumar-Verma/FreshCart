CREATE PROCEDURE dbo.Payment_SetStatus
    @OrderId UNIQUEIDENTIFIER,
    @Status TINYINT,
    @UpdatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Payments
    SET Status = @Status,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = SYSUTCDATETIME()
    WHERE OrderId = @OrderId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;
