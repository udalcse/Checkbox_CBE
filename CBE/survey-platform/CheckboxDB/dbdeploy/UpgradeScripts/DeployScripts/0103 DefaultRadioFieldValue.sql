
IF NOT EXISTS(
    SELECT *
    FROM sys.columns 
    WHERE Name      = N'DefaultSelectedOptionId'
      AND Object_ID = Object_ID(N'ckbx_CustomUserFieldRadio'))
BEGIN
	ALTER TABLE [ckbx_CustomUserFieldRadio]
	ADD DefaultSelectedOptionId INT NULL;
END


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddSelectedRadioFieldOption]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_Profile_AddSelectedRadioFieldOption
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddSelectedRadioFieldOption]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_Profile_AddSelectedRadioFieldOption
	@FieldName nvarchar(450),
	@UserID uniqueidentifier,
	@SelectedOptionText nvarchar(450)
AS

SET NOCOUNT ON

DECLARE @RadioButtonId int
SELECT @RadioButtonId = Id from ckbx_CustomUserFieldRadio
WHERE FieldName = @FieldName

DECLARE @SelectedOptionId int
SELECT @SelectedOptionId = Id from ckbx_CustomUserFieldRadioOption
WHERE OptionText = @SelectedOptionText AND RadioButtonId = @RadioButtonId

IF @UserID IS NULL
BEGIN
	UPDATE ckbx_CustomUserFieldRadio
	SET DefaultSelectedOptionId = @SelectedOptionId
	WHERE ID = @RadioButtonId
END

ELSE
BEGIN
	INSERT INTO ckbx_CustomUserFieldRadioOption_User
	(SelectedOptionId, RadioButtonId, UserId)
	VALUES
	(@SelectedOptionId, @RadioButtonId, @UserID)
END
'
END
GO



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

DECLARE @DefaultSelectedOptionId int
SELECT @DefaultSelectedOptionId = DefaultSelectedOptionId FROM ckbx_CustomUserFieldRadio
WHERE Id = @FieldId

IF @SelectedOptionId IS NOT NULL
BEGIN
	SELECT o.Id, OptionText, CASE WHEN (@SelectedOptionId = o.Id) THEN ''true'' ELSE ''false'' END AS IsSelected, a.Alias as Alias
	from ckbx_CustomUserFieldRadioOption o
	Left join ckbx_CustomUserFieldRadioOption_Alias a
	on o.Id = a.RadioButtonOptionId
	where RadioButtonId = @FieldId
END

ELSE
BEGIN
	SELECT o.Id, OptionText, CASE WHEN (@DefaultSelectedOptionId = o.Id) THEN ''true'' ELSE ''false'' END AS IsSelected, a.Alias as Alias
	from ckbx_CustomUserFieldRadioOption o
	Left join ckbx_CustomUserFieldRadioOption_Alias a
	on o.Id = a.RadioButtonOptionId
	where RadioButtonId = @FieldId
END

'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpRadioButtonField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_CleanUpRadioButtonField]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpRadioButtonField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_Profile_CleanUpRadioButtonField
 @FieldName [nvarchar] (450)
As

SET NOCOUNT ON

  BEGIN TRANSACTION; 

   DECLARE @RadioFieldId int
   SELECT @RadioFieldId = Id FROM ckbx_CustomUserFieldRadio
   WHERE FieldName = @FieldName

   DELETE FROM ckbx_CustomUserFieldRadioOption_User WHERE RadioButtonId = @RadioFieldId

   DELETE FROM ckbx_CustomUserFieldRadioOption WHERE RadioButtonId = @RadioFieldId
  
  COMMIT;
'
END
GO



