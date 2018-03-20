--Drop
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetFieldsByTypeId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_GetFieldsByTypeId]
GO

--Procs
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetFieldsByTypeId]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_GetFieldsByTypeId]
(
	@CustomFieldTypeId int
)
AS
	SELECT CustomUserFieldName, Position FROM ckbx_CustomUserField
	WHERE CustomUserFieldTypeID = @CustomFieldTypeId
	ORDER BY Position
' 
END
GO

