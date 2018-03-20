IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Matrix_DeleteRowsAndColumns]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Matrix_DeleteRowsAndColumns]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Matrix_DeleteRowsAndColumns]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Matrix_DeleteRowsAndColumns] 
(
  @ItemId int
)
AS
begin
  DELETE FROM ckbx_ItemData_MatrixColumns WHERE MatrixID = @ItemId
  DELETE FROM ckbx_ItemData_MatrixRows WHERE MatrixID = @ItemId
 
end
'
END

GO


