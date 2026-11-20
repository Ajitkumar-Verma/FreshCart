CREATE TABLE dbo.AuditLogs
(
    Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_AuditLogs_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId  BIGINT IDENTITY(1,1) NOT NULL,
    ActorUserId UNIQUEIDENTIFIER NOT NULL,
    Action      nvarchar(120) NOT NULL,
    EntityName  nvarchar(120) NOT NULL,
    EntityId    nvarchar(100) NOT NULL,
    Details     nvarchar(max) NULL,
    CreatedBy   UNIQUEIDENTIFIER NULL,
    CreatedOn   DATETIME2(7) NOT NULL CONSTRAINT DF_AuditLogs_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate DATE NOT NULL CONSTRAINT DF_AuditLogs_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy   UNIQUEIDENTIFIER NULL,
    UpdatedOn   DATETIME2(7) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_AuditLogs_IsDeleted DEFAULT (0),
    DeletedBy   UNIQUEIDENTIFIER NULL,
    DeletedAt   DATETIME2(7) NULL,
    CONSTRAINT PK_AuditLogs PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_AuditLogs_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_AuditLogs_AspNetUsers_ActorUserId FOREIGN KEY (ActorUserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AuditLogs_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AuditLogs_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AuditLogs_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

