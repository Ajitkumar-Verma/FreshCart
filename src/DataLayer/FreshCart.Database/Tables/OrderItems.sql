CREATE TABLE dbo.OrderItems
(
    Id                               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_OrderItems_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId                       BIGINT IDENTITY(1,1) NOT NULL,
    ShopOrderId                      UNIQUEIDENTIFIER NOT NULL,
    ProductVariantId                 UNIQUEIDENTIFIER NULL,
    ProductName                      nvarchar(180) NOT NULL,
    UnitName                         nvarchar(40) NOT NULL,
    Quantity                         decimal(10,3) NOT NULL,
    UnitPrice                        decimal(10,2) NOT NULL,
    LineTotal                        decimal(12,2) NOT NULL,
    CHECK(Quantity > 0),
    CreatedBy                        UNIQUEIDENTIFIER NULL,
    CreatedOn                        DATETIME2(7) NOT NULL CONSTRAINT DF_OrderItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate                      DATE NOT NULL CONSTRAINT DF_OrderItems_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy                        UNIQUEIDENTIFIER NULL,
    UpdatedOn                        DATETIME2(7) NULL,
    IsDeleted                        BIT NOT NULL CONSTRAINT DF_OrderItems_IsDeleted DEFAULT (0),
    DeletedBy                        UNIQUEIDENTIFIER NULL,
    DeletedAt                        DATETIME2(7) NULL,
    CONSTRAINT PK_OrderItems PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_OrderItems_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_OrderItems_ShopOrders_ShopOrderId FOREIGN KEY (ShopOrderId) REFERENCES dbo.ShopOrders(Id),
    CONSTRAINT FK_OrderItems_ProductVariants_ProductVariantId FOREIGN KEY (ProductVariantId) REFERENCES dbo.ProductVariants(Id),
    CONSTRAINT FK_OrderItems_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_OrderItems_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_OrderItems_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

