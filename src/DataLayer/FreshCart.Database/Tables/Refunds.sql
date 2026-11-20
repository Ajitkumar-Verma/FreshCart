CREATE TABLE dbo.Refunds
(
    Id               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Refunds_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId       BIGINT IDENTITY(1,1) NOT NULL,
    PaymentId        UNIQUEIDENTIFIER NOT NULL,
    Amount           decimal(12,2) NOT NULL,
    Reason           nvarchar(500) NOT NULL,
    Status           tinyint NOT NULL DEFAULT 0,
    GatewayReference nvarchar(200) NULL,
    CreatedBy        UNIQUEIDENTIFIER NULL,
    CreatedOn        DATETIME2(7) NOT NULL CONSTRAINT DF_Refunds_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate      DATE NOT NULL CONSTRAINT DF_Refunds_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy        UNIQUEIDENTIFIER NULL,
    UpdatedOn        DATETIME2(7) NULL,
    IsDeleted        BIT NOT NULL CONSTRAINT DF_Refunds_IsDeleted DEFAULT (0),
    DeletedBy        UNIQUEIDENTIFIER NULL,
    DeletedAt        DATETIME2(7) NULL,
    CONSTRAINT PK_Refunds PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Refunds_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Refunds_Payments_PaymentId FOREIGN KEY (PaymentId) REFERENCES dbo.Payments(Id),
    CONSTRAINT FK_Refunds_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Refunds_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Refunds_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

