CREATE PROCEDURE dbo.Payment_Create
    @Id UNIQUEIDENTIFIER,
    @OrderId UNIQUEIDENTIFIER,
    @Method TINYINT,
    @Status TINYINT,
    @Amount DECIMAL(12,2),
    @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Payments (Id, OrderId, Method, Status, Amount, CreatedBy)
    VALUES (@Id, @OrderId, @Method, @Status, @Amount, @CreatedBy);
END;
