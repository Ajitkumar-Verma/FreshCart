CREATE PROCEDURE dbo.Notification_SoftDelete
    @NotificationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Notifications
    SET IsDeleted = 1,
        DeletedBy = @UserId,
        DeletedAt = SYSUTCDATETIME(),
        UpdatedBy = @UserId,
        UpdatedOn = SYSUTCDATETIME()
    WHERE Id = @NotificationId
      AND UserId = @UserId
      AND IsDeleted = 0;

    SELECT CAST(CASE WHEN @@ROWCOUNT > 0 THEN 1 ELSE 0 END AS BIT);
END;
