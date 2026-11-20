CREATE TABLE dbo.DeliverySlots
(
    Id           UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_DeliverySlots_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId   BIGINT IDENTITY(1,1) NOT NULL,
    ShopBranchId UNIQUEIDENTIFIER NOT NULL,
    StartsAt     time NOT NULL,
    EndsAt       time NOT NULL,
    Capacity     int NOT NULL,
    IsActive     bit NOT NULL DEFAULT 1,
    CONSTRAINT CK_DeliverySlots_Time CHECK (EndsAt > StartsAt),
    CONSTRAINT CK_DeliverySlots_Capacity CHECK (Capacity > 0),
    CreatedBy    UNIQUEIDENTIFIER NULL,
    CreatedOn    DATETIME2(7) NOT NULL CONSTRAINT DF_DeliverySlots_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate  DATE NOT NULL CONSTRAINT DF_DeliverySlots_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy    UNIQUEIDENTIFIER NULL,
    UpdatedOn    DATETIME2(7) NULL,
    IsDeleted    BIT NOT NULL CONSTRAINT DF_DeliverySlots_IsDeleted DEFAULT (0),
    DeletedBy    UNIQUEIDENTIFIER NULL,
    DeletedAt    DATETIME2(7) NULL,
    CONSTRAINT PK_DeliverySlots PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_DeliverySlots_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_DeliverySlots_ShopBranches_ShopBranchId FOREIGN KEY (ShopBranchId) REFERENCES dbo.ShopBranches(Id),
    CONSTRAINT FK_DeliverySlots_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_DeliverySlots_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_DeliverySlots_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

