-- Add new column to CustomUserField
IF NOT EXISTS(
    SELECT *
    FROM sys.columns 
    WHERE Name      = N'GridLines'
      AND Object_ID = Object_ID(N'ckbx_CustomUserFieldMatrix'))
BEGIN
    ALTER TABLE [ckbx_CustomUserFieldMatrix] 
    ADD GridLines VARCHAR(100) NULL
END

GO

-- Drop sp for adding new matrix type
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_AddMatrixField]
GO

-- Modified sp for adding new matrix field
CREATE PROCEDURE [dbo].[ckbx_sp_Profile_AddMatrixField]
	@PropertyName nvarchar(510),
	@IsRowsFixed bit,
	@IsColumnsFixed bit,
	@GridLines varchar(100)
AS

SET NOCOUNT ON
	IF EXISTS (SELECT 1 FROM ckbx_CustomUserFieldMatrix WHERE FieldName = @PropertyName)
	BEGIN		
		UPDATE	ckbx_CustomUserFieldMatrix
		SET		FieldName = @PropertyName,
				IsRowsFixed = @IsRowsFixed,
				IsColumnsFixed = @IsColumnsFixed,
				GridLines = @GridLines
			WHERE	FieldName = @PropertyName
	END
	ELSE
	BEGIN
		INSERT INTO ckbx_CustomUserFieldMatrix(FieldName,IsRowsFixed,IsColumnsFixed, GridLines)
		VALUES (@PropertyName,@IsRowsFixed,@IsColumnsFixed,@GridLines)

		select @@IDENTITY
	END

GO

-- Drop sp for retrieving matrix field
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_GetMatrixField]
GO

-- Modified sp for retieving matrix field
CREATE PROCEDURE [ckbx_sp_Profile_GetMatrixField]
	@FieldName nvarchar(450),
	@UserID uniqueidentifier
As BEGIN

SET NOCOUNT ON
	DECLARE @MatrixID int 	
	SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName

	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader, cell.IsRowHeader, matrix.IsRowsFixed, matrix.IsColumnsFixed, 0 as UserCell FROM ckbx_CustomUserFieldMatrixCell cell
		 JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID WHERE cell.MatrixID = @MatrixID
    UNION
	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader, cell.IsRowHeader, matrix.IsRowsFixed, matrix.IsColumnsFixed, 1 as UserCell  FROM ckbx_CustomUserFieldMatrixCell_User cell
		 JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID WHERE cell.
		 MatrixID = @MatrixID AND cell.UserID = @UserID

	SELECT GridLines from ckbx_CustomUserFieldMatrix where FieldName = @FieldName
END
GO

