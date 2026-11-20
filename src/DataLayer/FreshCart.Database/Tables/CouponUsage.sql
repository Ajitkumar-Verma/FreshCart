CREATE TABLE dbo.CouponUsage
(
    Id             UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_CouponUsage_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId     BIGINT IDENTITY(1,1) NOT NULL,
    CouponId       UNIQUEIDENTIFIER NOT NULL,
    OrderId        UNIQUEIDENTIFIER NOT NULL,
    UserId         UNIQUEIDENTIFIER NOT NULL,
    DiscountAmount decimal(10,2) NOT NULL,
    UsedAt         datetime2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UNIQUE(CouponId, OrderId),
    CreatedBy      UNIQUEIDENTIFIER NULL,
    CreatedOn      DATETIME2(7) NOT NULL CONSTRAINT DF_CouponUsage_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate    DATE NOT NULL CONSTRAINT DF_CouponUsage_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy      UNIQUEIDENTIFIER NULL,
    UpdatedOn      DATETIME2(7) NULL,
    IsDeleted      BIT NOT NULL CONSTRAINT DF_CouponUsage_IsDeleted DEFAULT (0),
    DeletedBy      UNIQUEIDENTIFIER NULL,
    DeletedAt      DATETIME2(7) NULL,
    CONSTRAINT PK_CouponUsage PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_CouponUsage_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_CouponUsage_Coupons_CouponId FOREIGN KEY (CouponId) REFERENCES dbo.Coupons(Id),
    CONSTRAINT FK_CouponUsage_Orders_OrderId FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
    CONSTRAINT FK_CouponUsage_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_CouponUsage_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_CouponUsage_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_CouponUsage_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

