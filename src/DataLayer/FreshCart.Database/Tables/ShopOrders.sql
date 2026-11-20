CREATE TABLE dbo.ShopOrders
(
    Id             UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_ShopOrders_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId     BIGINT IDENTITY(1,1) NOT NULL,
    OrderId        UNIQUEIDENTIFIER NOT NULL,
    ShopId         UNIQUEIDENTIFIER NOT NULL,
    ShopBranchId   UNIQUEIDENTIFIER NOT NULL,
    DeliverySlotId UNIQUEIDENTIFIER NULL,
    Status         tinyint NOT NULL DEFAULT 0,
    DeliveryFee    decimal(10,2) NOT NULL DEFAULT 0,
    UNIQUE(OrderId, ShopId),
    CreatedBy      UNIQUEIDENTIFIER NULL,
    CreatedOn      DATETIME2(7) NOT NULL CONSTRAINT DF_ShopOrders_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate    DATE NOT NULL CONSTRAINT DF_ShopOrders_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy      UNIQUEIDENTIFIER NULL,
    UpdatedOn      DATETIME2(7) NULL,
    IsDeleted      BIT NOT NULL CONSTRAINT DF_ShopOrders_IsDeleted DEFAULT (0),
    DeletedBy      UNIQUEIDENTIFIER NULL,
    DeletedAt      DATETIME2(7) NULL,
    CONSTRAINT PK_ShopOrders PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_ShopOrders_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_ShopOrders_Orders_OrderId FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
    CONSTRAINT FK_ShopOrders_Shops_ShopId FOREIGN KEY (ShopId) REFERENCES dbo.Shops(Id),
    CONSTRAINT FK_ShopOrders_ShopBranches_ShopBranchId FOREIGN KEY (ShopBranchId) REFERENCES dbo.ShopBranches(Id),
    CONSTRAINT FK_ShopOrders_DeliverySlots_DeliverySlotId FOREIGN KEY (DeliverySlotId) REFERENCES dbo.DeliverySlots(Id),
    CONSTRAINT FK_ShopOrders_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_ShopOrders_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_ShopOrders_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);
GO

CREATE INDEX IX_ShopOrders_Shop_Status ON dbo.ShopOrders(ShopId, Status);

