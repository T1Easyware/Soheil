use SoheilDb
GO

CREATE LOGIN soheiladmin WITH PASSWORD = 'fromdust'
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = N'soheiladmin')
BEGIN
    CREATE USER [soheiladmin] FOR LOGIN [soheiladmin]
    EXEC sp_addrolemember N'db_owner', N'soheiladmin'
END;
GO
use SoheilDb
