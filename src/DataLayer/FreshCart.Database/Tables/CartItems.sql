CREATE TABLE dbo.CartItems
(
    Id                               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_CartItems_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId                       BIGINT IDENTITY(1,1) NOT NULL,
    CartId                           UNIQUEIDENTIFIER NOT NULL,
    ProductVariantIdUNIQUEIDENTIFIER NOT NULL,
    Quantity                         decimal(10,3) NOT NULL,
    UNIQUE(CartId, ProductVariantId),
    CHECK(Quantity > 0),
    CreatedBy                        UNIQUEIDENTIFIER NULL,
    CreatedOn                        DATETIME2(7) NOT NULL CONSTRAINT DF_CartItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate                      DATE NOT NULL CONSTRAINT DF_CartItems_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy                        UNIQUEIDENTIFIER NULL,
    UpdatedOn                        DATETIME2(7) NULL,
    IsDeleted                        BIT NOT NULL CONSTRAINT DF_CartItems_IsDeleted DEFAULT (0),
    DeletedBy                        UNIQUEIDENTIFIER NULL,
    DeletedAt                        DATETIME2(7) NULL,
    CONSTRAINT PK_CartItems PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_CartItems_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_CartItems_Carts_CartId FOREIGN KEY (CartId) REFERENCES dbo.Carts(Id),
    CONSTRAINT FK_CartItems_ProductVariants_ProductVariantId FOREIGN KEY (ProductVariantId) REFERENCES dbo.ProductVariants(Id),
    CONSTRAINT FK_CartItems_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_CartItems_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_CartItems_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

