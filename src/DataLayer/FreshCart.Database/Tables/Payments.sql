CREATE TABLE dbo.Payments
(
    Id               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Payments_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId       BIGINT IDENTITY(1,1) NOT NULL,
    OrderId          UNIQUEIDENTIFIER NOT NULL,
    Method           tinyint NOT NULL,
    Status           tinyint NOT NULL DEFAULT 0,
    Amount           decimal(12,2) NOT NULL,
    GatewayReference nvarchar(200) NULL,
    PaidAt           datetime2 NULL,
    CreatedBy        UNIQUEIDENTIFIER NULL,
    CreatedOn        DATETIME2(7) NOT NULL CONSTRAINT DF_Payments_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate      DATE NOT NULL CONSTRAINT DF_Payments_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy        UNIQUEIDENTIFIER NULL,
    UpdatedOn        DATETIME2(7) NULL,
    IsDeleted        BIT NOT NULL CONSTRAINT DF_Payments_IsDeleted DEFAULT (0),
    DeletedBy        UNIQUEIDENTIFIER NULL,
    DeletedAt        DATETIME2(7) NULL,
    CONSTRAINT PK_Payments PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Payments_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Payments_Orders_OrderId FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
    CONSTRAINT FK_Payments_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Payments_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Payments_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

