IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetKeysAndIds]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'ALTER PROCEDURE [ckbx_sp_CustomUserField_GetKeysAndIds]
(
	@CustomFieldType nvarchar(100)
)
AS
	SELECT CustomUserFieldID, CustomUserFieldName 
    FROM [ckbx_CustomUserField]
    WHERE CustomUserFieldTypeID = (SELECT CustomUserFieldTypeID FROM ckbx_CustomUserFieldType WHERE CustomFieldType = @CustomFieldType )
' 
END