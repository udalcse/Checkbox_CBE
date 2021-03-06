IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_UserImportConfig]') AND type in (N'U'))
BEGIN
CREATE TABLE ckbx_UserImportConfig (
    ID int primary key IDENTITY(1,1) NOT NULL,
	FieldName [nvarchar](510)
)
END
GO


IF EXISTS (SELECT * FROM   sys.objects WHERE  object_id = OBJECT_ID(N'[Split]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [Split]
GO 


IF NOT EXISTS (SELECT * FROM   sys.objects WHERE  object_id = OBJECT_ID(N'[Split]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE FUNCTION Split(@input AS NVARCHAR(MAX) )
RETURNS
      @Result TABLE(CsvConfig NVARCHAR(255))
AS
BEGIN
      DECLARE @str VARCHAR(20)
      DECLARE @ind Int
      IF(@input is not null)
      BEGIN
            SET @ind = CharIndex('','', @input)
            WHILE @ind > 0
            BEGIN
                  SET @str = SUBSTRING(@input,1,@ind-1)
                  SET @input = SUBSTRING(@input,@ind+1,LEN(@input)-@ind)
                  INSERT INTO @Result values (@str)
                  SET @ind = CharIndex('','',@input)
            END
            SET @str = @input
            INSERT INTO @Result values (@str)
      END
      RETURN
END
'
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_GetUserImportConfig]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_GetUserImportConfig]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_GetUserImportConfig]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_GetUserImportConfig]
As
    SET NOCOUNT ON;  
    SELECT FieldName
    FROM ckbx_UserImportConfig
'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SaveUserImportConfig]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_SaveUserImportConfig]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SaveUserImportConfig]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_SaveUserImportConfig]

	@ImportConfigsStr nvarchar(max)

As

	DECLARE @ImportConfigsTable TABLE (CsvConfig NVARCHAR(255))
	INSERT INTO @ImportConfigsTable
    SELECT * FROM Split(@ImportConfigsStr)
	
	DELETE FROM ckbx_UserImportConfig

	INSERT INTO ckbx_UserImportConfig
	(FieldName)
	SELECT CsvConfig FROM @ImportConfigsTable

'
END
GO






