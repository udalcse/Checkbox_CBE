GO 

ALTER PROCEDURE [dbo].[ckbx_sp_CustomUserField_UpdateSelectedRadioOption]
	@OptionText nvarchar(MAX),
	@FieldName nvarchar(450),
	@UserID uniqueidentifier
AS

SET NOCOUNT ON

DECLARE @RadioButtonId int
SELECT @RadioButtonId = Id from ckbx_CustomUserFieldRadio
WHERE FieldName = @FieldName

DECLARE @SelectedOptionId int 

SELECT @SelectedOptionId = cf_user.Id FROM ckbx_CustomUserFieldRadioOption_User cf_user
	LEFT JOIN ckbx_CustomUserFieldRadioOption cf_option on 
		cf_user.SelectedOptionId = cf_option.Id
	WHERE cf_option.RadioButtonId = @RadioButtonId AND UserID = @UserID

IF NOT @SelectedOptionId IS NULL
BEGIN
	UPDATE ckbx_CustomUserFieldRadioOption_User
	SET SelectedOptionId = (SELECT Id from ckbx_CustomUserFieldRadioOption WHERE OptionText = @OptionText AND RadioButtonId = @RadioButtonId)
	WHERE Id = @SelectedOptionId
END
ELSE
BEGIN
	INSERT INTO ckbx_CustomUserFieldRadioOption_User (SelectedOptionId, UserID)
	VALUES
	((SELECT Id from ckbx_CustomUserFieldRadioOption WHERE OptionText = @OptionText AND RadioButtonId = @RadioButtonId), @UserID)
END

GO