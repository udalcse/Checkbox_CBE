
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_CleanUpMatrixField]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_RemoveCustomMatrixCells]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_RemoveCustomMatrixCells]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_RemoveCustomMatrixCells]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_RemoveCustomMatrixCells]
 @FieldName [nvarchar] (450),
 @UserID uniqueidentifier
As

SET NOCOUNT ON

  DECLARE @MatrixID int 
 
  DELETE mc from [ckbx_CustomUserFieldMatrixCell_User] mc JOIN ckbx_CustomUserFieldMatrix m ON mc. MatrixID = m .MatrixID
  WHERE m.FieldName = @FieldName AND UserID = @UserID
'
END 


GO


