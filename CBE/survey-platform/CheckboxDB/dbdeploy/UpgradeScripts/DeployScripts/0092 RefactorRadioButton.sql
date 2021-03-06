IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldRadio]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldRadio]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldRadioOption]') AND type in (N'U'))
DROP TABLE ckbx_CustomUserFieldRadioOption
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldRadioOption_User]') AND type in (N'U'))
DROP TABLE ckbx_CustomUserFieldRadioOption_User
GO

-- Create needed tables
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldRadio]') AND type in (N'U'))
BEGIN
Create table ckbx_CustomUserFieldRadio
(
	Id int not null identity(1,1) primary key,
	FieldName nvarchar(255)
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldRadioOption]') AND type in (N'U'))
BEGIN
Create table ckbx_CustomUserFieldRadioOption
(
	Id int not null identity(1,1) primary key,
	OptionText nvarchar(MAX),
	RadioButtonId int
)
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldRadioOption_User]') AND type in (N'U'))
BEGIN
Create table ckbx_CustomUserFieldRadioOption_User
(
	Id int not null identity(1,1) primary key,
	SelectedOptionId int,
	RadioButtonId int,
	UserID uniqueidentifier
)
END
GO

-- Created proc that deleted the old radio button data for gived field name

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpRadioButtonField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_CleanUpRadioButtonField]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpRadioButtonField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_Profile_CleanUpRadioButtonField
 @FieldName [nvarchar] (450),
 @UserID uniqueidentifier
As

SET NOCOUNT ON

  BEGIN TRANSACTION; 

  DECLARE @RadioButtonId int 

   DELETE ru from [ckbx_CustomUserFieldRadioOption_User] ru JOIN ckbx_CustomUserFieldRadio r ON ru.RadioButtonId = r.Id
   WHERE r.FieldName = @FieldName AND UserID = @UserID
  
  COMMIT;
'
END
GO

-- Created sp to add a new radio button to ckbx_CustomUserFieldRadio only. There is another ap for adding options

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddRadioField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_AddRadioField]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddRadioField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[ckbx_sp_Profile_AddRadioField]
	@PropertyName nvarchar(510)
AS

SET NOCOUNT ON
	IF EXISTS (SELECT 1 FROM ckbx_CustomUserFieldRadio WHERE FieldName = @PropertyName)
		BEGIN		
			UPDATE	ckbx_CustomUserFieldRadio
			SET		FieldName = @PropertyName
			WHERE	FieldName = @PropertyName
		END
	ELSE
		BEGIN
			INSERT INTO ckbx_CustomUserFieldRadio(FieldName)
			VALUES (@PropertyName)

			select @@IDENTITY
		END
'
END
GO

-- Create sp to add radio button field options

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddRadioFieldOption]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_AddRadioFieldOption]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddRadioFieldOption]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[ckbx_sp_Profile_AddRadioFieldOption] 
	@FieldName nvarchar(450),
	@UserID uniqueidentifier,
	@Value nvarchar(max)
As

SET NOCOUNT ON
	DECLARE @RadioID int 
	
	SELECT @RadioID = Id from ckbx_CustomUserFieldRadio where FieldName = @FieldName

	IF NOT EXISTS (Select 1 from ckbx_CustomUserFieldRadioOption where OptionText = @Value and RadioButtonId = @RadioID)
	BEGIN
		INSERT INTO ckbx_CustomUserFieldRadioOption (OptionText, RadioButtonId)
		Values (@Value, @RadioID)
	END
'
END
GO

-- sp that gets radio button field

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

SELECT Id, OptionText, CASE WHEN (@SelectedOptionId = Id) THEN ''true'' ELSE ''false'' END AS IsSelected
from ckbx_CustomUserFieldRadioOption 
where RadioButtonId = @FieldId

'
END
GO

-- sp that adds or updates selected option for user field and user

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

Insert into ckbx_CustomUserFieldRadioOption_User
(SelectedOptionId, RadioButtonId, UserId)
values
(@SelectedOptionId, @RadioButtonId, @UserID)
'
END
GO



-- sp that updates selected option for user

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_UpdateSelectedRadioOption]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_CustomUserField_UpdateSelectedRadioOption
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_UpdateSelectedRadioOption]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_CustomUserField_UpdateSelectedRadioOption
	@OptionText nvarchar(MAX),
	@FieldName nvarchar(450),
	@UserID uniqueidentifier
AS

SET NOCOUNT ON

DECLARE @RadioButtonId int
SELECT @RadioButtonId = Id from ckbx_CustomUserFieldRadio
WHERE FieldName = @FieldName

IF EXISTS (SELECT 1 FROM ckbx_CustomUserFieldRadioOption_User WHERE RadioButtonId = @RadioButtonId AND UserID = @UserID)
BEGIN
	UPDATE ckbx_CustomUserFieldRadioOption_User
	SET SelectedOptionId = (SELECT Id from ckbx_CustomUserFieldRadioOption WHERE OptionText = @OptionText AND RadioButtonId = @RadioButtonId)
	WHERE RadioButtonId = @RadioButtonId AND UserID = @UserID
END
ELSE
BEGIN
	INSERT INTO ckbx_CustomUserFieldRadioOption_User (SelectedOptionId, RadioButtonId, UserID)
	VALUES
	((SELECT Id from ckbx_CustomUserFieldRadioOption WHERE OptionText = @OptionText AND RadioButtonId = @RadioButtonId), @RadioButtonId, @UserID)
END
'
END
GO