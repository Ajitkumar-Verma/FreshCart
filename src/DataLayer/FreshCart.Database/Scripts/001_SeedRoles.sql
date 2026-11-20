IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'CUSTOMER')
    INSERT dbo.AspNetRoles(Id, Name, NormalizedName) VALUES ('11111111-1111-4111-8111-111111111111', N'Customer', N'CUSTOMER');
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'SHOPOWNER')
    INSERT dbo.AspNetRoles(Id, Name, NormalizedName) VALUES ('22222222-2222-4222-8222-222222222222', N'ShopOwner', N'SHOPOWNER');
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'SHOPSTAFF')
    INSERT dbo.AspNetRoles(Id, Name, NormalizedName) VALUES ('33333333-3333-4333-8333-333333333333', N'ShopStaff', N'SHOPSTAFF');
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'PLATFORMADMIN')
    INSERT dbo.AspNetRoles(Id, Name, NormalizedName) VALUES ('44444444-4444-4444-8444-444444444444', N'PlatformAdmin', N'PLATFORMADMIN');

