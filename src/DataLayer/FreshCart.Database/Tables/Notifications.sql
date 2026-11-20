CREATE TABLE dbo.Notifications
(
    Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Notifications_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId  BIGINT IDENTITY(1,1) NOT NULL,
    UserId      UNIQUEIDENTIFIER NOT NULL,
    Title       nvarchar(160) NOT NULL,
    Message     nvarchar(1000) NOT NULL,
    ReadAt      datetime2 NULL,
    CreatedBy   UNIQUEIDENTIFIER NULL,
    CreatedOn   DATETIME2(7) NOT NULL CONSTRAINT DF_Notifications_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate DATE NOT NULL CONSTRAINT DF_Notifications_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy   UNIQUEIDENTIFIER NULL,
    UpdatedOn   DATETIME2(7) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_Notifications_IsDeleted DEFAULT (0),
    DeletedBy   UNIQUEIDENTIFIER NULL,
    DeletedAt   DATETIME2(7) NULL,
    CONSTRAINT PK_Notifications PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Notifications_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Notifications_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Notifications_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Notifications_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Notifications_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

