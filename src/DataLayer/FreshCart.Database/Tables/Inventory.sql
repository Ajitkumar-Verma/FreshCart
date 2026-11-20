CREATE TABLE dbo.Inventory
(
    Id                               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Inventory_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId                       BIGINT IDENTITY(1,1) NOT NULL,
    ShopBranchId                     UNIQUEIDENTIFIER NOT NULL,
    ProductVariantId                 UNIQUEIDENTIFIER NOT NULL,
    QuantityOnHand                   decimal(12,3) NOT NULL DEFAULT 0,
    ReorderLevel                     decimal(12,3) NOT NULL DEFAULT 0,
    UNIQUE(ShopBranchId, ProductVariantId),
    CHECK (QuantityOnHand >= 0),
    CreatedBy                        UNIQUEIDENTIFIER NULL,
    CreatedOn                        DATETIME2(7) NOT NULL CONSTRAINT DF_Inventory_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate                      DATE NOT NULL CONSTRAINT DF_Inventory_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy                        UNIQUEIDENTIFIER NULL,
    UpdatedOn                        DATETIME2(7) NULL,
    IsDeleted                        BIT NOT NULL CONSTRAINT DF_Inventory_IsDeleted DEFAULT (0),
    DeletedBy                        UNIQUEIDENTIFIER NULL,
    DeletedAt                        DATETIME2(7) NULL,
    CONSTRAINT PK_Inventory PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Inventory_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Inventory_ShopBranches_ShopBranchId FOREIGN KEY (ShopBranchId) REFERENCES dbo.ShopBranches(Id),
    CONSTRAINT FK_Inventory_ProductVariants_ProductVariantId FOREIGN KEY (ProductVariantId) REFERENCES dbo.ProductVariants(Id),
    CONSTRAINT FK_Inventory_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Inventory_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Inventory_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);
GO

CREATE INDEX IX_Inventory_Variant_Availability ON dbo.Inventory(ProductVariantId, QuantityOnHand);

