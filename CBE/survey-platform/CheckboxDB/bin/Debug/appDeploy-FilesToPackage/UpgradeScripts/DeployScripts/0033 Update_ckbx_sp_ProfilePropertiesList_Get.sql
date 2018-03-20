IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfilePropertiesList_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
alter PROCEDURE [ckbx_sp_ProfilePropertiesList_Get]

AS
   	SELECT STUFF((
		SELECT '','' + Convert(nvarchar(50), cuftm.ItemId)
			from ckbx_CustomUserFieldItemMap as cuftm
			where cuftm.CustomUserFieldId = cuf.CustomUserFieldID
			FOR XML PATH(''''), TYPE).value(''.'', ''NVARCHAR(MAX)''), 1, 1, '''') As ItemIds,
	cuf.CustomUserFieldID As FieldId,
    cuf.CustomUserFieldName AS PropertyName,
	cuf.Hidden as IsHidden,
    cuft.CustomFieldType AS FieldType
	FROM ckbx_CustomUserField as cuf
	JOIN ckbx_CustomUserFieldType as cuft ON cuf.CustomUserFieldTypeID = cuft.CustomUserFieldTypeID
' 
END
GO