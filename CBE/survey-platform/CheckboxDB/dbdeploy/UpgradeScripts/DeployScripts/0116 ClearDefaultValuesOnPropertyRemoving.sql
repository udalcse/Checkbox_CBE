
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_DeleteProperty]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_DeleteProperty]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_DeleteProperty]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_DeleteProperty] 
(
	@CustomUserFieldName nvarchar(255)
)	
AS
	DECLARE @CustomUserFieldID int
	DECLARE @Position int

	SELECT @CustomUserFieldID = CustomUserFieldID FROM ckbx_CustomUserField WHERE CustomUserFieldName = @CustomUserFieldName
		
	IF @CustomUserFieldID is not null
	BEGIN		

	    DECLARE @PropertyBindingValue nvarchar(512) = ''@@'' + @CustomUserFieldName

		SELECT @Position = Position FROM ckbx_CustomUserField WHERE CustomUserFieldID = @CustomUserFieldID 
	
		DELETE FROM ckbx_CustomUserFieldMap where CustomUserFieldID = @CustomUserFieldID 

		DELETE FROM ckbx_CustomUserField where CustomUserFieldName = @CustomUserFieldName	
		
		UPDATE ckbx_CustomUserField SET position = position - 1 WHERE position > @position

		DELETE FROM ckbx_Text WHERE TextID like ''%/textItemData/%%/defaultText%'' and CONVERT(NVARCHAR(MAX), TextValue) = @PropertyBindingValue

	END

'

END
GO


