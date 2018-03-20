
/****** Data******/
EXEC ckbx_sp_Install_CustomUserFieldType 'Matrix'

exec ckbx_sp_Text_Set '/pageText/settings/modal/addMatrixType', 'en-US', 'Add matrix field type'
exec ckbx_sp_Text_Set '/pageText/settings/modal/addMatrixType/rowsCountTitle', 'en-US', 'Number of Rows'
exec ckbx_sp_Text_Set '/pageText/settings/modal/addMatrixType/columnCountTitle', 'en-US', 'Number of Columns'
exec ckbx_sp_Text_Set '/pageText/settings/modal/addMatrixType/addHeaderTitle', 'en-US', 'Add Headers'
exec ckbx_sp_Text_Set '/pageText/settings/modal/addMatrixType/matrixPreview', 'en-US', 'Preview'

/****** Stored procedures ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_CleanUpMatrixField]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_AddMatrixField]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixFieldCell]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_AddMatrixFieldCell]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_GetMatrixField]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'Create Procedure [ckbx_sp_Profile_CleanUpMatrixField]
	@FieldName [nvarchar] (450)

As

SET NOCOUNT ON

	 BEGIN TRANSACTION; 

	 DELETE mc from [ckbx_CustomUserFieldMatrixCell] mc JOIN ckbx_CustomUserFieldMatrix m ON mc. MatrixID = m .MatrixID
		WHERE m. FieldName = @FieldName
	 
	 DELETE FROM [dbo].[ckbx_CustomUserFieldMatrix] WHERE FieldName = @FieldName

	 COMMIT;
' 
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'Create Procedure [ckbx_sp_Profile_AddMatrixField]
@PropertyName nvarchar(510)

As

SET NOCOUNT ON

	IF EXISTS (SELECT 1 FROM ckbx_CustomUserFieldMatrix WHERE FieldName = @PropertyName)
	BEGIN		
		UPDATE	ckbx_CustomUserFieldMatrix
		SET		FieldName = @PropertyName 
		WHERE	FieldName = @PropertyName
	END
	ELSE
	BEGIN
		INSERT INTO ckbx_CustomUserFieldMatrix(FieldName)
		VALUES (@PropertyName)

		select @@IDENTITY

	END
' 
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixFieldCell]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'Create Procedure [ckbx_sp_Profile_AddMatrixFieldCell]
	@FieldName nvarchar(450),
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
	
	BEGIN
		INSERT INTO ckbx_CustomUserFieldMatrixCell(RowNumber,ColumnNumber,Data,MatrixID,IsHeader,IsRowHeader)
		VALUES (@RowNumber, @ColumnNumber, @Value, @MatrixID, @IsHeader, @IsRowHeader)

	END
' 
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_GetMatrixField] AS' 
END
GO

ALTER Procedure [ckbx_sp_Profile_GetMatrixField]
	@FieldName nvarchar(450)
As

SET NOCOUNT ON

	SET NOCOUNT ON

	DECLARE @MatrixID int 
	
	SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName
	
	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader FROM ckbx_CustomUserFieldMatrixCell cell JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID 

GO



/****** Table structure ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrix]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldMatrix]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrix]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_CustomUserFieldMatrix](
	[MatrixID] [int] IDENTITY(1,1) NOT NULL,
	[FieldName] [nvarchar](450) NULL,
 CONSTRAINT [PK_ckbx_CustomUserFieldMatrix] PRIMARY KEY CLUSTERED 
(
	[MatrixID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_ckbx_CustomUserFieldMatrix] UNIQUE NONCLUSTERED 
(
	[MatrixID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ckbx_CustomUserFieldMatrixCell_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [ckbx_CustomUserFieldMatrixCell] DROP CONSTRAINT [DF_ckbx_CustomUserFieldMatrixCell_IsActive]
END

GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]') AND name = N'IX_ckbx_CustomUserFieldMatrixCell')
DROP INDEX [IX_ckbx_CustomUserFieldMatrixCell] ON [ckbx_CustomUserFieldMatrixCell]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldMatrixCell]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_CustomUserFieldMatrixCell](
	[CellID] [int] IDENTITY(1,1) NOT NULL,
	[RowNumber] [int] NOT NULL,
	[ColumnNumber] [int] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[IsHeader] [bit] NOT NULL,
	[IsRowHeader] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[MatrixID] [int] NOT NULL,
 CONSTRAINT [PK_ckbx_CustomUserFieldMatrixCell] PRIMARY KEY CLUSTERED 
(
	[CellID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]') AND name = N'IX_ckbx_CustomUserFieldMatrixCell')
CREATE UNIQUE NONCLUSTERED INDEX [IX_ckbx_CustomUserFieldMatrixCell] ON [ckbx_CustomUserFieldMatrixCell]
(
	[CellID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ckbx_CustomUserFieldMatrixCell_IsActive]') AND type = 'D')
BEGIN
ALTER TABLE [ckbx_CustomUserFieldMatrixCell] ADD  CONSTRAINT [DF_ckbx_CustomUserFieldMatrixCell_IsActive]  DEFAULT ((1)) FOR [IsActive]
END

GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix] FOREIGN KEY([MatrixID])
REFERENCES [dbo].[ckbx_CustomUserFieldMatrix] ([MatrixID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrix]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell] FOREIGN KEY([CellID])
REFERENCES [dbo].[ckbx_CustomUserFieldMatrixCell] ([CellID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_ckbx_CustomUserFieldMatrixCell]
GO





