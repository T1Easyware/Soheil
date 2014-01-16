@REM EXEC sp_configure 'show advanced options', 1
@REM GO
@REM RECONFIGURE
@REM GO
@REM EXEC sp_configure 'xp_cmdshell', 1
@REM GO
@REM RECONFIGURE
@REM GO
@REM exec master..xp_cmdshell 'osql -E -i[PATH]'


sqlcmd -iC:\Users\Bizhan\Documents\TFS\Soheil\Soheil.Dal\SoheilEdm.edmx.sql
sqlcmd -iC:\Users\Bizhan\Documents\TFS\Soheil\Soheil.Dal\SqlScripts\InitDatabaseforTest.sql
sqlcmd -iC:\Users\Bizhan\Documents\TFS\Soheil\Soheil.Dal\SqlScripts\MultiInsertAccessRules.sql
