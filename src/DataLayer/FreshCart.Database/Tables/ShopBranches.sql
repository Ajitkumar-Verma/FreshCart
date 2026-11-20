CREATE TABLE dbo.ShopBranches
(
    Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_ShopBranches_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId  BIGINT IDENTITY(1,1) NOT NULL,
    ShopId      UNIQUEIDENTIFIER NOT NULL,
    Name        nvarchar(120) NOT NULL,
    AddressLine nvarchar(250) NOT NULL,
    City        nvarchar(100) NOT NULL,
    PostalCode  nvarchar(20) NOT NULL,
    Latitude    decimal(9,6) NULL,
    Longitude   decimal(9,6) NULL,
    IsActive    bit NOT NULL DEFAULT 1,
    CreatedBy   UNIQUEIDENTIFIER NULL,
    CreatedOn   DATETIME2(7) NOT NULL CONSTRAINT DF_ShopBranches_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate DATE NOT NULL CONSTRAINT DF_ShopBranches_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy   UNIQUEIDENTIFIER NULL,
    UpdatedOn   DATETIME2(7) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_ShopBranches_IsDeleted DEFAULT (0),
    DeletedBy   UNIQUEIDENTIFIER NULL,
    DeletedAt   DATETIME2(7) NULL,
    CONSTRAINT PK_ShopBranches PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_ShopBranches_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_ShopBranches_Shops_ShopId FOREIGN KEY (ShopId) REFERENCES dbo.Shops(Id),
    CONSTRAINT FK_ShopBranches_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_ShopBranches_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_ShopBranches_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

