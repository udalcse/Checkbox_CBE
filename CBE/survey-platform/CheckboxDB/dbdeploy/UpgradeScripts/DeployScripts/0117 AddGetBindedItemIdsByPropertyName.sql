IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetBindedItemIdsByPropertyName]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_GetBindedItemIdsByPropertyName]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetBindedItemIdsByPropertyName]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_GetBindedItemIdsByPropertyName] 
(
	@PropertyName nvarchar(512)
)
AS
 
DECLARE @PropertyID  int = (SELECT CustomUserFieldID FROM ckbx_CustomUserField WHERE CustomUserFieldName = @PropertyName)

SELECT ItemId FROM ckbx_CustomUserFieldItemMap WHERE CustomUserFieldId = @PropertyID
'
END

GO


