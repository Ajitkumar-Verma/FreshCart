IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'CUSTOMER')
    INSERT dbo.AspNetRoles(Id, Name, NormalizedName) VALUES (N'freshcart-role-customer', N'Customer', N'CUSTOMER');
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'SHOPOWNER')
    INSERT dbo.AspNetRoles(Id, Name, NormalizedName) VALUES (N'freshcart-role-shop-owner', N'ShopOwner', N'SHOPOWNER');
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'SHOPSTAFF')
    INSERT dbo.AspNetRoles(Id, Name, NormalizedName) VALUES (N'freshcart-role-shop-staff', N'ShopStaff', N'SHOPSTAFF');
IF NOT EXISTS (SELECT 1 FROM dbo.AspNetRoles WHERE NormalizedName = N'PLATFORMADMIN')
    INSERT dbo.AspNetRoles(Id, Name, NormalizedName) VALUES (N'freshcart-role-platform-admin', N'PlatformAdmin', N'PLATFORMADMIN');
