
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetConnectedProfileFieldNameById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_CustomUserField_GetConnectedProfileFieldNameById]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetConnectedProfileFieldNameById]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_CustomUserField_GetConnectedProfileFieldNameById] (@ItemId int)
AS
	SELECT userField.CustomUserFieldName FROM ckbx_CustomUserFieldItemMap fieldMap
		JOIN ckbx_CustomUserField userField ON fieldMap.CustomUserFieldId = userField.CustomUserFieldID
			WHERE fieldMap.ItemId = @ItemId
'
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetCustomFieldByItemId]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'ALTER PROCEDURE [ckbx_sp_CustomUserField_GetCustomFieldByItemId]
(
	@ItemId int,
	@UniqueIdentifier nvarchar(256)
)
AS
	SET NOCOUNT ON
	SELECT uf.CustomUserFieldName, fm.Value FROM ckbx_CustomUserFieldItemMap im
	JOIN ckbx_CustomUserField uf
	ON im.CustomUserFieldId = uf.CustomUserFieldID
	JOIN ckbx_CustomUserFieldMap fm
	ON im.CustomUserFieldId = fm.CustomUserFieldID
	WHERE ItemId = @ItemId AND UniqueIdentifier = @UniqueIdentifier
' 
END
