CREATE TABLE dbo.Categories
(
    Id                               UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Categories_Id DEFAULT (NEWSEQUENTIALID()),
    InternalId                       BIGINT IDENTITY(1,1) NOT NULL,
    ParentCategoryIdUNIQUEIDENTIFIER NULL,
    Name                             nvarchar(100) NOT NULL,
    Slug                             nvarchar(120) NOT NULL UNIQUE,
    IsActive                         bit NOT NULL DEFAULT 1,
    SortOrder                        int NOT NULL DEFAULT 0,
    CreatedBy                        UNIQUEIDENTIFIER NULL,
    CreatedOn                        DATETIME2(7) NOT NULL CONSTRAINT DF_Categories_CreatedOn DEFAULT (SYSUTCDATETIME()),
    CreatedDate                      DATE NOT NULL CONSTRAINT DF_Categories_CreatedDate DEFAULT (CONVERT(date, SYSUTCDATETIME())),
    UpdatedBy                        UNIQUEIDENTIFIER NULL,
    UpdatedOn                        DATETIME2(7) NULL,
    IsDeleted                        BIT NOT NULL CONSTRAINT DF_Categories_IsDeleted DEFAULT (0),
    DeletedBy                        UNIQUEIDENTIFIER NULL,
    DeletedAt                        DATETIME2(7) NULL,
    CONSTRAINT PK_Categories PRIMARY KEY NONCLUSTERED (Id),
    CONSTRAINT UQ_Categories_InternalId UNIQUE CLUSTERED (InternalId),
    CONSTRAINT FK_Categories_Categories_ParentCategoryId FOREIGN KEY (ParentCategoryId) REFERENCES dbo.Categories(Id),
    CONSTRAINT FK_Categories_AspNetUsers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Categories_AspNetUsers_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES dbo.AspNetUsers(Id),
    CONSTRAINT FK_Categories_AspNetUsers_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES dbo.AspNetUsers(Id)
);

