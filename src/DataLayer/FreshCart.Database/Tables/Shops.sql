CREATE TABLE dbo.Shops
(
    Id                 UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Shops_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId         BIGINT IDENTITY(1,1) NOT NULL,
    Name               nvarchar(160) NOT NULL,
    Slug               nvarchar(180) NOT NULL UNIQUE,
    Description        nvarchar(1000) NULL,
    OwnerUserId        UNIQUEIDENTIFIER NOT NULL,
    Status             tinyint NOT NULL CONSTRAINT DF_Shops_Status DEFAULT 0,
    PlatformFeePercent decimal(5,2) NOT NULL CONSTRAINT DF_Shops_Fee DEFAULT 0,
    CONSTRAINT CK_Shops_Fee CHECK (PlatformFeePercent BETWEEN 0 AND 100),
    CreatedBy          UNIQUEIDENTIFIER NULL,
    CreatedOn          DATETIME2(7) NOT NULL CONSTRAINT DF_Shops_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate        DATE NOT NULL CONSTRAINT DF_Shops_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy          UNIQUEIDENTIFIER NULL,
    UpdatedOn          DATETIME2(7) NULL,
    IsDeleted          BIT NOT NULL CONSTRAINT DF_Shops_IsDeleted DEFAULT (0),
    DeletedBy          UNIQUEIDENTIFIER NULL,
    DeletedAt          DATETIME2(7) NULL,
    CONSTRAINT PK_Shops PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Shops_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Shops_AspNetUsers_OwnerUserId FOREIGN KEY (OwnerUserId) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Shops_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Shops_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Shops_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

