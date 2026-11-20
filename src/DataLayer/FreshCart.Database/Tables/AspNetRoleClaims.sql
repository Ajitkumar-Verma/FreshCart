CREATE TABLE dbo.AspNetRoleClaims
(
    Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_AspNetRoleClaims_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId  BIGINT IDENTITY(1,1) NOT NULL,
    RoleId      UNIQUEIDENTIFIER NOT NULL,
    ClaimType   nvarchar(max) NULL,
    ClaimValue  nvarchar(max) NULL,
    CreatedBy   UNIQUEIDENTIFIER NULL,
    CreatedOn   DATETIME2(7) NOT NULL CONSTRAINT DF_AspNetRoleClaims_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate DATE NOT NULL CONSTRAINT DF_AspNetRoleClaims_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy   UNIQUEIDENTIFIER NULL,
    UpdatedOn   DATETIME2(7) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_AspNetRoleClaims_IsDeleted DEFAULT (0),
    DeletedBy   UNIQUEIDENTIFIER NULL,
    DeletedAt   DATETIME2(7) NULL,
    CONSTRAINT PK_AspNetRoleClaims PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_AspNetRoleClaims_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_AspNetRoleClaims_AspNetRoles_RoleId FOREIGN KEY (RoleId) REFERENCES dbo.AspNetRoles(Id),
    CONSTRAINT FK_AspNetRoleClaims_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetRoleClaims_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetRoleClaims_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

