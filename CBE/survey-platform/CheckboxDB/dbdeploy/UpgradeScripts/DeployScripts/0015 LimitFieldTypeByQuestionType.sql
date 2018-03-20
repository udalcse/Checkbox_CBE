--Drop
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetKeysAndIds]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_CustomUserField_GetKeysAndIds]             
GO

--Procs
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetKeysAndIds]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_CustomUserField_GetKeysAndIds]
(
	@CustomFieldTypeId int
)
AS
	SELECT CustomUserFieldID, CustomUserFieldName 
    FROM [dbo].[ckbx_CustomUserField]
	WHERE CustomUserFieldTypeID = @CustomFieldTypeId
' 
END
GO

