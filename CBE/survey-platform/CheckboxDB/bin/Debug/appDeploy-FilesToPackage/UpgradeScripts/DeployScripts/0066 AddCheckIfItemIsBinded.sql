IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Item_IsItemBindedToProfileField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Item_IsItemBindedToProfileField]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Item_IsItemBindedToProfileField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Item_IsItemBindedToProfileField] (@ItemId int, @PropertyId int)
AS
DECLARE @IsAlreadyBinded bit = 0

	IF EXISTS(SELECT 1 FROM ckbx_CustomUserFieldItemMap WHERE CustomUserFieldId = @PropertyId AND ItemId = @ItemId) 
		SET @IsAlreadyBinded = 1 

	SELECT @IsAlreadyBinded;
'
END
GO


