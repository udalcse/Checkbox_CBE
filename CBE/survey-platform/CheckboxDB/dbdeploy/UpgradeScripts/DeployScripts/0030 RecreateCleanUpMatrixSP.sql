
EXEC dbo.sp_executesql @statement = N'alter PROCEDURE [ckbx_sp_Profile_CleanUpMatrixField] 

 @FieldName [nvarchar] (450),
 @WithHeaders bit,
 @UserID uniqueidentifier
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
   DELETE mc from [ckbx_CustomUserFieldMatrixCell_User] mc JOIN ckbx_CustomUserFieldMatrix m ON mc. MatrixID = m .MatrixID
   WHERE m.FieldName = @FieldName AND mc.IsHeader = 0 AND mc.IsRowHeader = 0 AND UserID = @UserID
  END
  
  COMMIT;
' 

