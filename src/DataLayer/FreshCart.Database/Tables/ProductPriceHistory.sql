CREATE TABLE dbo.ProductPriceHistory
(
    Id                               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_ProductPriceHistory_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId                       BIGINT IDENTITY(1,1) NOT NULL,
    ProductVariantId                 UNIQUEIDENTIFIER NOT NULL,
    OldPrice                         decimal(10,2) NULL,
    NewPrice                         decimal(10,2) NOT NULL,
    ChangedByUserId                  UNIQUEIDENTIFIER NOT NULL,
    ChangedAt                        datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedBy                        UNIQUEIDENTIFIER NULL,
    CreatedOn                        DATETIME2(7) NOT NULL CONSTRAINT DF_ProductPriceHistory_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate                      DATE NOT NULL CONSTRAINT DF_ProductPriceHistory_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy                        UNIQUEIDENTIFIER NULL,
    UpdatedOn                        DATETIME2(7) NULL,
    IsDeleted                        BIT NOT NULL CONSTRAINT DF_ProductPriceHistory_IsDeleted DEFAULT (0),
    DeletedBy                        UNIQUEIDENTIFIER NULL,
    DeletedAt                        DATETIME2(7) NULL,
    CONSTRAINT PK_ProductPriceHistory PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_ProductPriceHistory_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_ProductPriceHistory_ProductVariants_ProductVariantId FOREIGN KEY (ProductVariantId) REFERENCES dbo.ProductVariants(Id),
    CONSTRAINT FK_ProductPriceHistory_AspNetUsers_ChangedByUserId FOREIGN KEY (ChangedByUserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_ProductPriceHistory_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_ProductPriceHistory_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_ProductPriceHistory_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

