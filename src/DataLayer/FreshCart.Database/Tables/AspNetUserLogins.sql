CREATE TABLE dbo.AspNetUserLogins
(
    Id                  UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_AspNetUserLogins_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId          BIGINT IDENTITY(1,1) NOT NULL,
    LoginProvider       nvarchar(450) NOT NULL,
    ProviderKey         nvarchar(450) NOT NULL,
    ProviderDisplayName nvarchar(max) NULL,
    UserId              UNIQUEIDENTIFIER NOT NULL,
    CreatedBy           UNIQUEIDENTIFIER NULL,
    CreatedOn           DATETIME2(7) NOT NULL CONSTRAINT DF_AspNetUserLogins_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate         DATE NOT NULL CONSTRAINT DF_AspNetUserLogins_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy           UNIQUEIDENTIFIER NULL,
    UpdatedOn           DATETIME2(7) NULL,
    IsDeleted           BIT NOT NULL CONSTRAINT DF_AspNetUserLogins_IsDeleted DEFAULT (0),
    DeletedBy           UNIQUEIDENTIFIER NULL,
    DeletedAt           DATETIME2(7) NULL,
    CONSTRAINT PK_AspNetUserLogins PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_AspNetUserLogins_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT UQ_AspNetUserLogins_Natural UNIQUE (LoginProvider,ProviderKey),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserLogins_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

