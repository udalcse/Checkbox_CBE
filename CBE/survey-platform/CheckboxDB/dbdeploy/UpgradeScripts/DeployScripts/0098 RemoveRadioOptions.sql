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

   print @RadioFieldId

   DELETE FROM ckbx_CustomUserFieldRadioOption_User WHERE RadioButtonId = @RadioFieldId

   DELETE FROM ckbx_CustomUserFieldRadioOption WHERE RadioButtonId = @RadioFieldId
   
   DELETE FROM ckbx_CustomUserFieldRadio WHERE Id = @RadioFieldId
  
  COMMIT;
'
END
GO



-- need to change sp because it should not return  alias. Alias is tied up o an item, not to a user
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

SELECT o.Id, OptionText, CASE WHEN (@SelectedOptionId = o.Id) THEN ''true'' ELSE ''false'' END AS IsSelected
from ckbx_CustomUserFieldRadioOption o
where RadioButtonId = @FieldId

'
END
GO


-- create sp that will return options along with its aliases
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetRadioOptionAliases]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_Profile_GetRadioOptionAliases
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetRadioOptionAliases]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_Profile_GetRadioOptionAliases
	@FieldName nvarchar(450),
	@ItemID int
AS

SET NOCOUNT ON

DECLARE @FieldId int
SELECT @FieldId = Id from ckbx_CustomUserFieldRadio where FieldName = @FieldName

SELECT OptionText, a.Alias from ckbx_CustomUserFieldRadioOption o
LEFT JOIN ckbx_CustomUserFieldRadioOption_Alias a
ON o.Id = a.RadioButtonOptionId
WHERE RadioButtonId = @FieldId AND a.ItemId = @ItemId

'
END
GO


