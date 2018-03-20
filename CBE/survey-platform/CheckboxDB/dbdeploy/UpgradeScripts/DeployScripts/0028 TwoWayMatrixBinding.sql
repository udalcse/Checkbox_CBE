IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfilePropertiesList_Get') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ProfilePropertiesList_Get]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_CreateFieldItemMap') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_CustomUserField_CreateFieldItemMap]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpMatrixField') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_CleanUpMatrixField]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Template_DeleteItemFieldMapping') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Template_DeleteItemFieldMapping]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfilePropertiesList_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_ProfilePropertiesList_Get]

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

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_CreateFieldItemMap]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_CustomUserField_CreateFieldItemMap]
(
	@CustomUserFieldId int,
	@ItemId int
)
AS
	DECLARE @CheckFieldId int,
			@CheckItemId int

	SELECT @CheckItemId = ItemId,
		   @CheckFieldId = CustomUserFieldId
	FROM ckbx_CustomUserFieldItemMap WHERE ItemId = @ItemId

	IF (@CheckFieldId = @CustomUserFieldId AND @CheckItemId = @ItemId)
		RETURN
	ELSE
		INSERT INTO ckbx_CustomUserFieldItemMap
			(CustomUserFieldId, ItemId)
			VALUES
			(@CustomUserFieldId, @ItemId)
' 
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_CleanUpMatrixField] 

 @FieldName [nvarchar] (450),
 @WithHeaders bit

As

SET NOCOUNT ON

  BEGIN TRANSACTION; 

  DECLARE @MatrixID int 

  IF @WithHeaders = 1
  BEGIN
   DELETE mc from [ckbx_CustomUserFieldMatrixCell] mc JOIN ckbx_CustomUserFieldMatrix m ON mc. MatrixID = m .MatrixID
   WHERE m.FieldName = @FieldName
   
   SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName

   DELETE from [ckbx_CustomUserFieldMatrixCell_User] WHERE MatrixID = @MatrixID

   DELETE FROM [dbo].[ckbx_CustomUserFieldMatrix] WHERE FieldName = @FieldName

   DELETE FROM ckbx_CustomUserFieldItemMap WHERE CustomUserFieldId = (SELECT CustomUserFieldID FROM ckbx_CustomUserField WHERE CustomUserFieldName = @FieldName)
   
  END
  IF @WithHeaders = 0
  BEGIN
   DELETE mc from [ckbx_CustomUserFieldMatrixCell] mc JOIN ckbx_CustomUserFieldMatrix m ON mc. MatrixID = m .MatrixID
   WHERE m.FieldName = @FieldName AND mc.IsHeader = 0 AND mc.IsRowHeader = 0
  END
  
  COMMIT;
' 
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Template_DeleteItemFieldMapping]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Template_DeleteItemFieldMapping] 
@ItemID int
AS
DELETE FROM ckbx_CustomUserFieldItemMap WHERE ItemId = @ItemID 

'

END
GO


