CREATE TABLE dbo.Addresses
(
    Id            UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Addresses_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId    BIGINT IDENTITY(1,1) NOT NULL,
    UserId        UNIQUEIDENTIFIER NOT NULL,
    [Label]       nvarchar(60) NOT NULL,
    RecipientName nvarchar(160) NOT NULL,
    Phone         nvarchar(30) NOT NULL,
    AddressLine   nvarchar(300) NOT NULL,
    City          nvarchar(100) NOT NULL,
    PostalCode    nvarchar(20) NOT NULL,
    IsDefault     bit NOT NULL DEFAULT 0,
    CreatedBy     UNIQUEIDENTIFIER NULL,
    CreatedOn     DATETIME2(7) NOT NULL CONSTRAINT DF_Addresses_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate   DATE NOT NULL CONSTRAINT DF_Addresses_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy     UNIQUEIDENTIFIER NULL,
    UpdatedOn     DATETIME2(7) NULL,
    IsDeleted     BIT NOT NULL CONSTRAINT DF_Addresses_IsDeleted DEFAULT (0),
    DeletedBy     UNIQUEIDENTIFIER NULL,
    DeletedAt     DATETIME2(7) NULL,
    CONSTRAINT PK_Addresses PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Addresses_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Addresses_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Addresses_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Addresses_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Addresses_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

