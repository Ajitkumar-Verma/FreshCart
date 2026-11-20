CREATE TABLE dbo.AspNetUsers
(
    Id                   UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_AspNetUsers_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId           BIGINT IDENTITY(1,1) NOT NULL,
    UserName             nvarchar(256) NULL,
    NormalizedUserName   nvarchar(256) NULL,
    Email                nvarchar(256) NULL,
    NormalizedEmail      nvarchar(256) NULL,
    EmailConfirmed       bit NOT NULL DEFAULT 0,
    PasswordHash         nvarchar(max) NULL,
    SecurityStamp        nvarchar(max) NULL,
    ConcurrencyStamp     nvarchar(max) NULL,
    PhoneNumber          nvarchar(max) NULL,
    PhoneNumberConfirmed bit NOT NULL DEFAULT 0,
    TwoFactorEnabled     bit NOT NULL DEFAULT 0,
    LockoutEnd           datetimeoffset NULL,
    LockoutEnabled       bit NOT NULL DEFAULT 0,
    AccessFailedCount    int NOT NULL DEFAULT 0,
    DisplayName          nvarchar(160) NULL,
    CreatedBy            UNIQUEIDENTIFIER NULL,
    CreatedOn            DATETIME2(7) NOT NULL CONSTRAINT DF_AspNetUsers_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate          DATE NOT NULL CONSTRAINT DF_AspNetUsers_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy            UNIQUEIDENTIFIER NULL,
    UpdatedOn            DATETIME2(7) NULL,
    IsDeleted            BIT NOT NULL CONSTRAINT DF_AspNetUsers_IsDeleted DEFAULT (0),
    DeletedBy            UNIQUEIDENTIFIER NULL,
    DeletedAt            DATETIME2(7) NULL,
    CONSTRAINT PK_AspNetUsers PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_AspNetUsers_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_AspNetUsers_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUsers_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUsers_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);
GO

CREATE INDEX IX_AspNetUsers_NormalizedEmail ON dbo.AspNetUsers(NormalizedEmail);
GO

CREATE UNIQUE INDEX IX_AspNetUsers_NormalizedUserName ON dbo.AspNetUsers(NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;

