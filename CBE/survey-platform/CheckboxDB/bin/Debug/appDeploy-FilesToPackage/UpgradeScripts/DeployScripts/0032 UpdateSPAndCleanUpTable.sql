
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_CustomUserField_CreateFieldItemMap]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'ALTER PROCEDURE [ckbx_sp_CustomUserField_CreateFieldItemMap]
(
	@CustomUserFieldId int,
	@ItemId int
)
AS
	IF EXISTS ( SELECT 1 FROM ckbx_CustomUserFieldItemMap WHERE ItemId = @ItemId)
		BEGIN
			UPDATE ckbx_CustomUserFieldItemMap SET CustomUserFieldId = @CustomUserFieldId WHERE ItemId = @ItemId 
		END
	ELSE
		BEGIN
			INSERT INTO ckbx_CustomUserFieldItemMap
				(CustomUserFieldId, ItemId)
				VALUES
				(@CustomUserFieldId, @ItemId)
		END

'

END

truncate table ckbx_CustomUserFieldItemMap

DELETE  from [ckbx_CustomUserFieldMatrixCell]

DELETE from [ckbx_CustomUserFieldMatrixCell_User]

DELETE FROM [dbo].[ckbx_CustomUserFieldMatrix]

DELETE FROM ckbx_CustomUserFieldItemMap
