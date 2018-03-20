--Drop
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetCustomFieldByItemId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_CustomUserField_GetCustomFieldByItemId]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldItemMap]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldItemMap]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_CreateFieldItemMap]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_CustomUserField_CreateFieldItemMap]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetKeysAndIds]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_CustomUserField_GetKeysAndIds]
GO


--Tables
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldItemMap]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_CustomUserFieldItemMap](
    [Id] [int] NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[CustomUserFieldId] [int] NOT NULL,
	[ItemId] [int] NOT NULL	
)
END
GO


--Procs
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_GetCustomFieldByItemId]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_CustomUserField_GetCustomFieldByItemId]
(
	@ItemId int
)
AS
	SET NOCOUNT ON
	SELECT uf.CustomUserFieldName, fm.Value FROM ckbx_CustomUserFieldItemMap im
	JOIN ckbx_CustomUserField uf
	ON im.CustomUserFieldId = uf.CustomUserFieldID
	JOIN ckbx_CustomUserFieldMap fm
	ON im.CustomUserFieldId = fm.CustomUserFieldID
	WHERE ItemId = @ItemId
' 
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_CreateFieldItemMap]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_CustomUserField_CreateFieldItemMap]
(
	@CustomUserFieldId int,
	@ItemId int
)
AS
	SET NOCOUNT ON
	INSERT INTO ckbx_CustomUserFieldItemMap
	(CustomUserFieldId, ItemId)
	VALUES
	(@CustomUserFieldId, @ItemId)
' 
END
GO

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

--insert data
EXEC ckbx_sp_Install_CustomUserFieldType 'RadioButton'