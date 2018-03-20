IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpRadioButtonField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_CleanUpRadioButtonField]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpRadioButtonField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_Profile_CleanUpRadioButtonField
	@FieldName [nvarchar] (450)
AS

SET NOCOUNT ON

   BEGIN TRANSACTION; 

   DECLARE @RadioFieldId int
   SELECT @RadioFieldId = Id FROM ckbx_CustomUserFieldRadio
   WHERE FieldName = @FieldName

   DELETE FROM ckbx_CustomUserFieldRadioOption_User WHERE RadioButtonId = @RadioFieldId

   DELETE FROM ckbx_CustomUserFieldRadioOption_Alias WHERE RadioButtonOptionId IN (SELECT Id FROM ckbx_CustomUserFieldRadioOption WHERE RadioButtonId = @RadioFieldId)

   DELETE FROM ckbx_CustomUserFieldRadioOption WHERE RadioButtonId = @RadioFieldId

   DELETE FROM ckbx_CustomUserFieldRadio WHERE Id = @RadioFieldId
  
   COMMIT;

'
END
GO


