CREATE TABLE dbo.Orders
(
    Id             UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Orders_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId     BIGINT IDENTITY(1,1) NOT NULL,
    OrderNumber    nvarchar(32) NOT NULL UNIQUE,
    UserId         UNIQUEIDENTIFIER NOT NULL,
    AddressId      UNIQUEIDENTIFIER NULL,
    OrderStatus    tinyint NOT NULL DEFAULT 0,
    PaymentStatus  tinyint NOT NULL DEFAULT 0,
    Subtotal       decimal(12,2) NOT NULL,
    DeliveryFee    decimal(12,2) NOT NULL DEFAULT 0,
    DiscountAmount decimal(12,2) NOT NULL DEFAULT 0,
    GrandTotal     decimal(12,2) NOT NULL,
    CurrencyCode   char(3) NOT NULL DEFAULT 'INR',
    CHECK(GrandTotal >= 0),
    CreatedBy      UNIQUEIDENTIFIER NULL,
    CreatedOn      DATETIME2(7) NOT NULL CONSTRAINT DF_Orders_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate    DATE NOT NULL CONSTRAINT DF_Orders_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy      UNIQUEIDENTIFIER NULL,
    UpdatedOn      DATETIME2(7) NULL,
    IsDeleted      BIT NOT NULL CONSTRAINT DF_Orders_IsDeleted DEFAULT (0),
    DeletedBy      UNIQUEIDENTIFIER NULL,
    DeletedAt      DATETIME2(7) NULL,
    CONSTRAINT PK_Orders PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Orders_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Orders_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Orders_Addresses_AddressId FOREIGN KEY (AddressId) REFERENCES dbo.Addresses(Id),
    CONSTRAINT FK_Orders_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Orders_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Orders_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);
GO

CREATE INDEX IX_Orders_User_Created ON dbo.Orders(UserId, CreatedOn DESC);


