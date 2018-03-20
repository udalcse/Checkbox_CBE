
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetAllRadioButtonOptions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_GetAllRadioButtonOptions]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetAllRadioButtonOptions]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_GetAllRadioButtonOptions] 
	@PropertyName nvarchar(510)
AS

SET NOCOUNT ON

DECLARE @PropertyID int
SELECT @PropertyID = Id FROM ckbx_CustomUserFieldRadio WHERE FieldName = @PropertyName 

SELECT alias.ItemId ,options.OptionText,  alias.Alias,  options.RadioButtonId FROM ckbx_CustomUserFieldRadioOption options 
	JOIN ckbx_CustomUserFieldRadioOption_Alias alias ON options.Id = alias.RadioButtonOptionId
		WHERE options.RadioButtonId = @PropertyID 
'
END
GO


