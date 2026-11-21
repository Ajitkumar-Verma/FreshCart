CREATE PROCEDURE dbo.Notification_MarkRead
    @NotificationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Notifications
    SET ReadAt = COALESCE(ReadAt, SYSUTCDATETIME()),
        UpdatedBy = @UserId,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @NotificationId
      AND UserId = @UserId
      AND IsDeleted = 0;

    SELECT Id, Title, Message, ReadAt, CreatedOn
    FROM dbo.Notifications
    WHERE Id = @NotificationId
      AND UserId = @UserId
      AND IsDeleted = 0;
END;
