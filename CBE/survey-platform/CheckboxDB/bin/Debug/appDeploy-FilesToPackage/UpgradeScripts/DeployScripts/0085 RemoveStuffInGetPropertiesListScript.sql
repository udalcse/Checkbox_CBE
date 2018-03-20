IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfilePropertiesList_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
ALTER PROCEDURE [ckbx_sp_ProfilePropertiesList_Get]

AS
   	SELECT 
	  (SELECT cast(cuftm.ItemId AS nvarchar(50)) + '',''
                FROM dbo.ckbx_CustomUserFieldItemMap AS cuftm
                WHERE cuftm.CustomUserFieldId = cuf.CustomUserFieldID
                For XML PATH ('''')) As ItemIds,
	cuf.CustomUserFieldID As FieldId,
    cuf.CustomUserFieldName AS PropertyName,
	cuf.Hidden AS IsHidden,
    cuft.CustomFieldType AS FieldType
	FROM ckbx_CustomUserField AS cuf
	JOIN ckbx_CustomUserFieldType AS cuft ON cuf.CustomUserFieldTypeID = cuft.CustomUserFieldTypeID
	ORDER BY cuf.Position
' 
END
GO