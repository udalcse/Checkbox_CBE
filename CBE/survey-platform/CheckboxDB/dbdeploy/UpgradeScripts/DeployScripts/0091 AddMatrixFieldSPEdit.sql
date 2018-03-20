GO

ALTER PROCEDURE [dbo].[ckbx_sp_Profile_AddMatrixFieldCell] 

	@FieldName nvarchar(450),
	@UserID uniqueidentifier,
	@CustomUserCell bit, 
	@RowNumber int,
	@ColumnNumber int, 
	@Value nvarchar(max),
	@IsHeader bit,
	@IsRowHeader bit


As

SET NOCOUNT ON

	
	SET NOCOUNT ON

	DECLARE @MatrixID int 
	
	SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName
	IF @CustomUserCell = 0 
		BEGIN
			IF @IsHeader = 1 OR @IsRowHeader = 1
			BEGIN
				INSERT INTO ckbx_CustomUserFieldMatrixCell(RowNumber,ColumnNumber,Data,MatrixID,IsHeader,IsRowHeader)
				VALUES (@RowNumber, @ColumnNumber, @Value, @MatrixID, @IsHeader, @IsRowHeader)
			END
		END
	ELSE
		BEGIN
			INSERT INTO ckbx_CustomUserFieldMatrixCell_User(UserID,RowNumber,ColumnNumber,Data,MatrixID,IsHeader,IsRowHeader)
			VALUES (@UserID, @RowNumber, @ColumnNumber, @Value, @MatrixID, @IsHeader, @IsRowHeader)
		END
GO