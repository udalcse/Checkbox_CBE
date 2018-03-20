IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetRadioField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_GetRadioField]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetRadioField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_Profile_GetRadioField
	@FieldName nvarchar(450),
	@UserID uniqueidentifier
AS

SET NOCOUNT ON

DECLARE @FieldId int
SELECT @FieldId = Id from ckbx_CustomUserFieldRadio where FieldName = @FieldName

DECLARE @SelectedOptionId int
SELECT @SelectedOptionId = SelectedOptionId from ckbx_CustomUserFieldRadioOption_User 
WHERE UserID = @UserID AND RadioButtonId = @FieldId

IF @SelectedOptionId IS NULL
BEGIN
	SELECT @SelectedOptionId = DefaultSelectedOptionId FROM ckbx_CustomUserFieldRadio
	WHERE Id = @FieldId
END

SELECT o.Id, OptionText, CASE WHEN (@SelectedOptionId = o.Id) THEN ''true'' ELSE ''false'' END AS IsSelected
FROM ckbx_CustomUserFieldRadioOption o
WHERE RadioButtonId = @FieldId

'
END
GO


