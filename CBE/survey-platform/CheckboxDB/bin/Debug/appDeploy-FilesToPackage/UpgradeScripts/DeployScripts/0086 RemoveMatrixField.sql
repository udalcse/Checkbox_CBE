
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_RemoveMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_RemoveMatrixField]
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_RemoveMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_RemoveMatrixField] 

 @FieldName [nvarchar] (450)

As

SET NOCOUNT ON

  DECLARE @MatrixID int 
 
   DELETE mc from [ckbx_CustomUserFieldMatrixCell] mc JOIN ckbx_CustomUserFieldMatrix m ON mc. MatrixID = m .MatrixID
   WHERE m.FieldName = @FieldName
   
   SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName

   DELETE from [ckbx_CustomUserFieldMatrixCell_User] WHERE MatrixID = @MatrixID

   DELETE FROM [dbo].[ckbx_CustomUserFieldMatrix] WHERE FieldName = @FieldName

   DELETE FROM ckbx_CustomUserFieldItemMap WHERE CustomUserFieldId = (SELECT CustomUserFieldID FROM ckbx_CustomUserField WHERE CustomUserFieldName = @FieldName)
   
 
 '
END

GO


