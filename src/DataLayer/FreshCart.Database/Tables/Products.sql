CREATE TABLE dbo.Products
(
    Id          UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Products_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId  BIGINT IDENTITY(1,1) NOT NULL,
    ShopId      UNIQUEIDENTIFIER NOT NULL,
    CategoryId  UNIQUEIDENTIFIER NOT NULL,
    Name        nvarchar(180) NOT NULL,
    Description nvarchar(2000) NULL,
    IsActive    bit NOT NULL DEFAULT 1,
    CreatedBy   UNIQUEIDENTIFIER NULL,
    CreatedOn   DATETIME2(7) NOT NULL CONSTRAINT DF_Products_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate DATE NOT NULL CONSTRAINT DF_Products_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy   UNIQUEIDENTIFIER NULL,
    UpdatedOn   DATETIME2(7) NULL,
    IsDeleted   BIT NOT NULL CONSTRAINT DF_Products_IsDeleted DEFAULT (0),
    DeletedBy   UNIQUEIDENTIFIER NULL,
    DeletedAt   DATETIME2(7) NULL,
    CONSTRAINT PK_Products PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Products_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Products_Shops_ShopId FOREIGN KEY (ShopId) REFERENCES dbo.Shops(Id),
    CONSTRAINT FK_Products_Categories_CategoryId FOREIGN KEY (CategoryId) REFERENCES dbo.Categories(Id),
    CONSTRAINT FK_Products_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Products_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Products_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);
GO

CREATE INDEX IX_Products_Shop_Category ON dbo.Products(ShopId, CategoryId, IsActive);

