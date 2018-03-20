
/****** Object:  StoredProcedure [ckbx_sp_Profile_GetMatrixField]    Script Date: 12/9/2016 12:21:24 AM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_GetMatrixField]
GO

/****** Object:  StoredProcedure [ckbx_sp_Profile_GetMatrixField]    Script Date: 12/9/2016 12:21:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_GetMatrixField]

	@FieldName nvarchar(450),
	@UserID uniqueidentifier

As

SET NOCOUNT ON

	SET NOCOUNT ON

	DECLARE @MatrixID int 
	
	SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName
	

	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader, cell.IsRowHeader, matrix.IsRowsFixed, matrix.IsColumnsFixed, 0 as UserCell FROM ckbx_CustomUserFieldMatrixCell cell
		 JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID WHERE cell.MatrixID = @MatrixID
    UNION
	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader, cell.IsRowHeader, matrix.IsRowsFixed, matrix.IsColumnsFixed, 1 as UserCell  FROM ckbx_CustomUserFieldMatrixCell_User cell
		 JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID WHERE cell.MatrixID = @MatrixID AND cell.UserID = @UserID

'
END
GO


