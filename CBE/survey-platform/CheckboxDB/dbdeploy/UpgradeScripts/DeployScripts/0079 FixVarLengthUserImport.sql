-- changing the size of nvarchar field to hold any kind of field name

IF EXISTS (SELECT * FROM   sys.objects WHERE  object_id = OBJECT_ID(N'[Split]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [Split]
GO 


IF NOT EXISTS (SELECT * FROM   sys.objects WHERE  object_id = OBJECT_ID(N'[Split]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE FUNCTION Split(@input AS NVARCHAR(MAX) )
RETURNS
      @Result TABLE(CsvConfig NVARCHAR(510))
AS
BEGIN
      DECLARE @str VARCHAR(510)
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


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SaveUserImportConfig]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_SaveUserImportConfig]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SaveUserImportConfig]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_SaveUserImportConfig]

	@ImportConfigsStr nvarchar(max)

As

	DECLARE @ImportConfigsTable TABLE (CsvConfig NVARCHAR(510))
	INSERT INTO @ImportConfigsTable
    SELECT * FROM Split(@ImportConfigsStr)
	
	DELETE FROM ckbx_UserImportConfig

	INSERT INTO ckbx_UserImportConfig
	(FieldName)
	SELECT CsvConfig FROM @ImportConfigsTable

'
END
GO