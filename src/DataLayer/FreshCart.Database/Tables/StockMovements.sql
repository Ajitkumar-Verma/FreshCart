CREATE TABLE dbo.StockMovements
(
    Id              UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_StockMovements_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId      BIGINT IDENTITY(1,1) NOT NULL,
    InventoryId     UNIQUEIDENTIFIER NOT NULL,
    QuantityChange  decimal(12,3) NOT NULL,
    Reason          nvarchar(40) NOT NULL,
    Notes           nvarchar(500) NULL,
    ChangedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedBy       UNIQUEIDENTIFIER NULL,
    CreatedOn       DATETIME2(7) NOT NULL CONSTRAINT DF_StockMovements_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate     DATE NOT NULL CONSTRAINT DF_StockMovements_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy       UNIQUEIDENTIFIER NULL,
    UpdatedOn       DATETIME2(7) NULL,
    IsDeleted       BIT NOT NULL CONSTRAINT DF_StockMovements_IsDeleted DEFAULT (0),
    DeletedBy       UNIQUEIDENTIFIER NULL,
    DeletedAt       DATETIME2(7) NULL,
    CONSTRAINT PK_StockMovements PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_StockMovements_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_StockMovements_Inventory_InventoryId FOREIGN KEY (InventoryId) REFERENCES dbo.Inventory(Id),
    CONSTRAINT FK_StockMovements_AspNetUsers_ChangedByUserId FOREIGN KEY (ChangedByUserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_StockMovements_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_StockMovements_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_StockMovements_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

