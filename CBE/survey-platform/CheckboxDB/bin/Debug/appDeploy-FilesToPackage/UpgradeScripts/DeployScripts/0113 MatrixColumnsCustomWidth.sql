exec ckbx_sp_Text_Set '/pageText/settings/modal/addMatrixType/colWidth', 'en-US', 'Column Widths'

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ColumnWidth' AND Object_ID = Object_ID(N'ckbx_CustomUserFieldMatrixCell'))
BEGIN
	ALTER TABLE ckbx_CustomUserFieldMatrixCell
	ADD ColumnWidth INT NULL;
END



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixFieldCell]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_Profile_AddMatrixFieldCell
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixFieldCell]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[ckbx_sp_Profile_AddMatrixFieldCell] 

	@FieldName nvarchar(450),
	@UserID uniqueidentifier,
	@CustomUserCell bit, 
	@RowNumber int,
	@ColumnNumber int, 
	@Value nvarchar(max),
	@IsHeader bit,
	@IsRowHeader bit,
	@ColumnWidth int
As
SET NOCOUNT ON
	SET NOCOUNT ON

	DECLARE @MatrixID int 
	
	SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName
	IF @CustomUserCell = 0 
		BEGIN
			IF @IsHeader = 1 OR @IsRowHeader = 1
			BEGIN
				INSERT INTO ckbx_CustomUserFieldMatrixCell(RowNumber,ColumnNumber,Data,MatrixID,IsHeader,IsRowHeader,ColumnWidth)
				VALUES (@RowNumber, @ColumnNumber, @Value, @MatrixID, @IsHeader, @IsRowHeader, @ColumnWidth)
			END
		END
	ELSE
		BEGIN
			INSERT INTO ckbx_CustomUserFieldMatrixCell_User(UserID,RowNumber,ColumnNumber,Data,MatrixID,IsHeader,IsRowHeader)
			VALUES (@UserID, @RowNumber, @ColumnNumber, @Value, @MatrixID, @IsHeader, @IsRowHeader)
		END
'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_Profile_GetMatrixField
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_Profile_GetMatrixField
	@FieldName nvarchar(450),
	@UserID uniqueidentifier
As

SET NOCOUNT ON
	SET NOCOUNT ON
	DECLARE @MatrixID int 	
	SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName

	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader, cell.IsRowHeader, matrix.IsRowsFixed, matrix.IsColumnsFixed, 0 as UserCell, ColumnWidth FROM ckbx_CustomUserFieldMatrixCell cell
		 JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID WHERE cell.MatrixID = @MatrixID
    UNION
	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader, cell.IsRowHeader, matrix.IsRowsFixed, matrix.IsColumnsFixed, 1 as UserCell, '''' FROM ckbx_CustomUserFieldMatrixCell_User cell
		 JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID WHERE cell.
		 MatrixID = @MatrixID AND cell.UserID = @UserID

	SELECT GridLines from ckbx_CustomUserFieldMatrix where FieldName = @FieldName
'
END
GO



