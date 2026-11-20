CREATE TABLE dbo.WishlistItems
(
    Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_WishlistItems_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId  BIGINT IDENTITY(1,1) NOT NULL,
    WishlistId  UNIQUEIDENTIFIER NOT NULL,
    ProductId   UNIQUEIDENTIFIER NOT NULL,
    AddedAt     datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UNIQUE(WishlistId, ProductId),
    CreatedBy   UNIQUEIDENTIFIER NULL,
    CreatedOn   DATETIME2(7) NOT NULL CONSTRAINT DF_WishlistItems_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate DATE NOT NULL CONSTRAINT DF_WishlistItems_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy   UNIQUEIDENTIFIER NULL,
    UpdatedOn   DATETIME2(7) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_WishlistItems_IsDeleted DEFAULT (0),
    DeletedBy   UNIQUEIDENTIFIER NULL,
    DeletedAt   DATETIME2(7) NULL,
    CONSTRAINT PK_WishlistItems PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_WishlistItems_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_WishlistItems_Wishlists_WishlistId FOREIGN KEY (WishlistId) REFERENCES dbo.Wishlists(Id),
    CONSTRAINT FK_WishlistItems_Products_ProductId FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
    CONSTRAINT FK_WishlistItems_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_WishlistItems_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_WishlistItems_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

