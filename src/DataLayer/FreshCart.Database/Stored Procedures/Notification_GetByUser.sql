CREATE PROCEDURE dbo.Notification_GetByUser
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, Title, Message, ReadAt, CreatedOn
    FROM dbo.Notifications
    WHERE UserId = @UserId
      AND IsDeleted = 0
    ORDER BY CreatedOn DESC;
END;
