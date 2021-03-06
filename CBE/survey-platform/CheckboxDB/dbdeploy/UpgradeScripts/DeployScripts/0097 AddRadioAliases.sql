-- Create table that will hold RadioFieldOption / Alias mapping
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldRadioOption_Alias]') AND type in (N'U'))
DROP TABLE ckbx_CustomUserFieldRadioOption_Alias
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldRadioOption_Alias]') AND type in (N'U'))
BEGIN
CREATE TABLE ckbx_CustomUserFieldRadioOption_Alias
(
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ItemId INT,
	RadioButtonOptionId INT NOT NULL,
	Alias nvarchar(max)
)
END
GO

-- sp that add/updates radio button field option alias
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddRadioFieldOptionAlias]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_Profile_AddRadioFieldOptionAlias
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddRadioFieldOptionAlias]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_Profile_AddRadioFieldOptionAlias] 
	@RadioButtonOptionId int,
	@ItemId int,
	@Alias nvarchar(max)
As

SET NOCOUNT ON


	IF NOT EXISTS (SELECT 1 FROM ckbx_CustomUserFieldRadioOption_Alias WHERE RadioButtonOptionId = @RadioButtonOptionId AND ItemId = @ItemId)
	BEGIN
		INSERT INTO ckbx_CustomUserFieldRadioOption_Alias (ItemId, RadioButtonOptionId, Alias)
		VALUES (@ItemId, @RadioButtonOptionId, @Alias)
	END
	ELSE
	BEGIN
		UPDATE ckbx_CustomUserFieldRadioOption_Alias
		SET Alias = @Alias
		WHERE RadioButtonOptionId = @RadioButtonOptionId AND ItemId = @ItemId
	END
'
END
GO

-- need to alter sp tha returns radio field to returns aliases for options

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

SELECT o.Id, OptionText, CASE WHEN (@SelectedOptionId = o.Id) THEN ''true'' ELSE ''false'' END AS IsSelected, a.Alias as Alias
from ckbx_CustomUserFieldRadioOption o
Left join ckbx_CustomUserFieldRadioOption_Alias a
on o.Id = a.RadioButtonOptionId
where RadioButtonId = @FieldId

'
END
GO

-- DELETE ALL OPTION ALIASES FOR ITEM
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpRadioFieldOptionAlias]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_Profile_CleanUpRadioFieldOptionAlias
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpRadioFieldOptionAlias]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_Profile_CleanUpRadioFieldOptionAlias] 
	@ItemId INT
As

SET NOCOUNT ON

	DELETE FROM ckbx_CustomUserFieldRadioOption_Alias 
	WHERE ItemId = @ItemId
'
END
GO