CREATE TABLE dbo.AspNetUserClaims
(
    Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_AspNetUserClaims_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId  BIGINT IDENTITY(1,1) NOT NULL,
    UserId      UNIQUEIDENTIFIER NOT NULL,
    ClaimType   nvarchar(max) NULL,
    ClaimValue  nvarchar(max) NULL,
    CreatedBy   UNIQUEIDENTIFIER NULL,
    CreatedOn   DATETIME2(7) NOT NULL CONSTRAINT DF_AspNetUserClaims_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate DATE NOT NULL CONSTRAINT DF_AspNetUserClaims_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy   UNIQUEIDENTIFIER NULL,
    UpdatedOn   DATETIME2(7) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_AspNetUserClaims_IsDeleted DEFAULT (0),
    DeletedBy   UNIQUEIDENTIFIER NULL,
    DeletedAt   DATETIME2(7) NULL,
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_AspNetUserClaims_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

