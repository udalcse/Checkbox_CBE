IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetRadioField]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_Profile_GetRadioField]
END
GO

CREATE PROCEDURE [ckbx_sp_Profile_GetRadioField]
(
	@FieldName nvarchar(450),
	@UserID uniqueidentifier
)
AS
BEGIN
	DECLARE @FieldId int
	SELECT @FieldId = Id FROM ckbx_CustomUserFieldRadio WHERE FieldName = @FieldName

	DECLARE @SelectedOptionId int
	SELECT @SelectedOptionId = SelectedOptionId FROM ckbx_CustomUserFieldRadioOption_User 
	WHERE UserID = @UserID AND RadioButtonId = @FieldId

	SELECT o.Id, OptionText, CASE WHEN (@SelectedOptionId = o.Id) THEN 'true' ELSE 'false' END AS IsSelected
	FROM ckbx_CustomUserFieldRadioOption o
	WHERE RadioButtonId = @FieldId
END
GO