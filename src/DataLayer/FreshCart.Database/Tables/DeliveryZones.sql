CREATE TABLE dbo.DeliveryZones
(
    Id           UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_DeliveryZones_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId   BIGINT IDENTITY(1,1) NOT NULL,
    ShopBranchId UNIQUEIDENTIFIER NOT NULL,
    Name         nvarchar(120) NOT NULL,
    PostalCode   nvarchar(20) NOT NULL,
    DeliveryFee  decimal(10,2) NOT NULL DEFAULT 0,
    MinimumOrder decimal(10,2) NOT NULL DEFAULT 0,
    IsActive     bit NOT NULL DEFAULT 1,
    CreatedBy    UNIQUEIDENTIFIER NULL,
    CreatedOn    DATETIME2(7) NOT NULL CONSTRAINT DF_DeliveryZones_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate  DATE NOT NULL CONSTRAINT DF_DeliveryZones_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy    UNIQUEIDENTIFIER NULL,
    UpdatedOn    DATETIME2(7) NULL,
    IsDeleted    BIT NOT NULL CONSTRAINT DF_DeliveryZones_IsDeleted DEFAULT (0),
    DeletedBy    UNIQUEIDENTIFIER NULL,
    DeletedAt    DATETIME2(7) NULL,
    CONSTRAINT PK_DeliveryZones PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_DeliveryZones_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_DeliveryZones_ShopBranches_ShopBranchId FOREIGN KEY (ShopBranchId) REFERENCES dbo.ShopBranches(Id),
    CONSTRAINT FK_DeliveryZones_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_DeliveryZones_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_DeliveryZones_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

