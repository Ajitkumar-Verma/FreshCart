CREATE TABLE dbo.AspNetUserTokens
(
    Id            UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_AspNetUserTokens_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId    BIGINT IDENTITY(1,1) NOT NULL,
    UserId        UNIQUEIDENTIFIER NOT NULL,
    LoginProvider nvarchar(450) NOT NULL,
    Name          nvarchar(450) NOT NULL,
    Value         nvarchar(max) NULL,
    CreatedBy     UNIQUEIDENTIFIER NULL,
    CreatedOn     DATETIME2(7) NOT NULL CONSTRAINT DF_AspNetUserTokens_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate   DATE NOT NULL CONSTRAINT DF_AspNetUserTokens_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy     UNIQUEIDENTIFIER NULL,
    UpdatedOn     DATETIME2(7) NULL,
    IsDeleted     BIT NOT NULL CONSTRAINT DF_AspNetUserTokens_IsDeleted DEFAULT (0),
    DeletedBy     UNIQUEIDENTIFIER NULL,
    DeletedAt     DATETIME2(7) NULL,
    CONSTRAINT PK_AspNetUserTokens PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_AspNetUserTokens_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT UQ_AspNetUserTokens_Natural UNIQUE (UserId,LoginProvider,Name),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserTokens_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

