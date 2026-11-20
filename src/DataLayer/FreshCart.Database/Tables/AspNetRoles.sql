CREATE TABLE dbo.AspNetRoles
(
    Id               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_AspNetRoles_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId       BIGINT IDENTITY(1,1) NOT NULL,
    Name             nvarchar(256) NULL,
    NormalizedName   nvarchar(256) NULL,
    ConcurrencyStamp nvarchar(max) NULL,
    CreatedBy        UNIQUEIDENTIFIER NULL,
    CreatedOn        DATETIME2(7) NOT NULL CONSTRAINT DF_AspNetRoles_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate      DATE NOT NULL CONSTRAINT DF_AspNetRoles_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy        UNIQUEIDENTIFIER NULL,
    UpdatedOn        DATETIME2(7) NULL,
    IsDeleted        BIT NOT NULL CONSTRAINT DF_AspNetRoles_IsDeleted DEFAULT (0),
    DeletedBy        UNIQUEIDENTIFIER NULL,
    DeletedAt        DATETIME2(7) NULL,
    CONSTRAINT PK_AspNetRoles PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_AspNetRoles_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_AspNetRoles_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetRoles_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetRoles_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);
GO

CREATE UNIQUE INDEX IX_AspNetRoles_NormalizedName ON dbo.AspNetRoles(NormalizedName) WHERE NormalizedName IS NOT NULL;

