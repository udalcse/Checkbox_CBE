/****** DROP matrix  cell keys  ******/

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

/****** DROP matrix  cell keys  ******/



/****** DROP matrix cell user keys  ******/


IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell_User]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell_User] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ckbx_CustomUserFieldMatrixCell_User_Active]') AND type = 'D')
BEGIN
ALTER TABLE [ckbx_CustomUserFieldMatrixCell_User] DROP CONSTRAINT [DF_ckbx_CustomUserFieldMatrixCell_User_Active]
END

GO

/****** Object:  Index [IX_ckbx_CustomUserFieldMatrixCell_User]    Script Date: 12/8/2016 1:56:41 PM ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell_User]') AND name = N'IX_ckbx_CustomUserFieldMatrixCell_User')
DROP INDEX [IX_ckbx_CustomUserFieldMatrixCell_User] ON [ckbx_CustomUserFieldMatrixCell_User]
GO

/****** DROP matrix user cell keys  ******/


/****** DROP tables ******/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldMatrixCell]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell_User]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldMatrixCell_User]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrix]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldMatrix]
GO


/****** DROP tables ******/


/****** Create tables ******/


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrix]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_CustomUserFieldMatrix](
	[MatrixID] [int] IDENTITY(1,1) NOT NULL,
	[FieldName] [nvarchar](450) NULL,
	[IsRowsFixed] [bit] NULL,
	[IsColumnsFixed] [bit] NULL,
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




IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell_User]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_CustomUserFieldMatrixCell_User](
	[UserCellID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [uniqueidentifier] NOT NULL,
	[RowNumber] [int] NOT NULL,
	[ColumnNumber] [int] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[IsHeader] [bit] NOT NULL,
	[IsRowHeader] [bit] NOT NULL,
	[Active] [bit] NOT NULL,
	[MatrixID] [int] NOT NULL,
 CONSTRAINT [PK_ckbx_CustomUserFieldMatrixCell_User] PRIMARY KEY CLUSTERED 
(
	[UserCellID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


/****** Add keys ******/

/****** Object:  Index [IX_ckbx_CustomUserFieldMatrixCell]    Script Date: 12/8/2016 1:55:57 PM ******/
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




/****** Object:  Index [IX_ckbx_CustomUserFieldMatrixCell_User]    Script Date: 12/8/2016 1:56:41 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell_User]') AND name = N'IX_ckbx_CustomUserFieldMatrixCell_User')
CREATE UNIQUE NONCLUSTERED INDEX [IX_ckbx_CustomUserFieldMatrixCell_User] ON [ckbx_CustomUserFieldMatrixCell_User]
(
	[UserCellID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_ckbx_CustomUserFieldMatrixCell_User_Active]') AND type = 'D')
BEGIN
ALTER TABLE [ckbx_CustomUserFieldMatrixCell_User] ADD  CONSTRAINT [DF_ckbx_CustomUserFieldMatrixCell_User_Active]  DEFAULT ((1)) FOR [Active]
END

GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell_User]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell_User]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix] FOREIGN KEY([MatrixID])
REFERENCES [dbo].[ckbx_CustomUserFieldMatrix] ([MatrixID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMatrixCell_User]'))
ALTER TABLE [ckbx_CustomUserFieldMatrixCell_User] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMatrixCell_User_ckbx_CustomUserFieldMatrix]
GO



/****** Add keys ******/






/****** Object:  StoredProcedure [ckbx_sp_Profile_AddMatrixField]    Script Date: 12/8/2016 1:43:06 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_AddMatrixField]
GO

/****** Object:  StoredProcedure [ckbx_sp_Profile_AddMatrixField]    Script Date: 12/8/2016 1:43:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_AddMatrixField]

@PropertyName nvarchar(510),
@IsRowsFixed bit,
@IsColumnsFixed bit

AS

SET NOCOUNT ON

	IF EXISTS (SELECT 1 FROM ckbx_CustomUserFieldMatrix WHERE FieldName = @PropertyName)
	BEGIN		
		UPDATE	ckbx_CustomUserFieldMatrix
		SET		FieldName = @PropertyName,
				IsRowsFixed = @IsRowsFixed,
				IsColumnsFixed = @IsColumnsFixed
			WHERE	FieldName = @PropertyName
	END
	ELSE
	BEGIN
		INSERT INTO ckbx_CustomUserFieldMatrix(FieldName,IsRowsFixed,IsColumnsFixed)
		VALUES (@PropertyName,@IsRowsFixed,@IsColumnsFixed)

		select @@IDENTITY

	END
'

END




/****** Object:  StoredProcedure [ckbx_sp_Profile_AddMatrixFieldCell]    Script Date: 12/8/2016 1:44:13 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixFieldCell]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_AddMatrixFieldCell]
GO

/****** Object:  StoredProcedure [ckbx_sp_Profile_AddMatrixFieldCell]    Script Date: 12/8/2016 1:44:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_AddMatrixFieldCell]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_AddMatrixFieldCell] 

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
			INSERT INTO ckbx_CustomUserFieldMatrixCell(RowNumber,ColumnNumber,Data,MatrixID,IsHeader,IsRowHeader)
			VALUES (@RowNumber, @ColumnNumber, @Value, @MatrixID, @IsHeader, @IsRowHeader)
		END
	ELSE
		BEGIN
			INSERT INTO ckbx_CustomUserFieldMatrixCell_User(UserID,RowNumber,ColumnNumber,Data,MatrixID,IsHeader,IsRowHeader)
			VALUES (@UserID, @RowNumber, @ColumnNumber, @Value, @MatrixID, @IsHeader, @IsRowHeader)
		END
	

'
END
GO




/****** Object:  StoredProcedure [ckbx_sp_Profile_CleanUpMatrixField]    Script Date: 12/8/2016 1:44:51 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_CleanUpMatrixField]
GO

/****** Object:  StoredProcedure [ckbx_sp_Profile_CleanUpMatrixField]    Script Date: 12/8/2016 1:44:51 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_CleanUpMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_CleanUpMatrixField] 

	@FieldName [nvarchar] (450),
	@WithHeaders bit

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

			
			
		END
	 IF @WithHeaders = 0
		BEGIN
			DELETE mc from [ckbx_CustomUserFieldMatrixCell] mc JOIN ckbx_CustomUserFieldMatrix m ON mc. MatrixID = m .MatrixID
			WHERE m.FieldName = @FieldName AND mc.IsHeader = 0 AND mc.IsRowHeader = 0
		END
	 
	 COMMIT;
'

END
GO



/****** Object:  StoredProcedure [ckbx_sp_Profile_GetMatrixField]    Script Date: 12/8/2016 1:45:43 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_GetMatrixField]
GO

/****** Object:  StoredProcedure [ckbx_sp_Profile_GetMatrixField]    Script Date: 12/8/2016 1:45:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_GetMatrixField]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Profile_GetMatrixField] 
	@FieldName nvarchar(450)
As

SET NOCOUNT ON

	SET NOCOUNT ON

	DECLARE @MatrixID int 
	
	SELECT @MatrixID = MatrixID from ckbx_CustomUserFieldMatrix where FieldName = @FieldName
	

	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader, cell.IsRowHeader, matrix.IsRowsFixed, matrix.IsColumnsFixed, 0 as UserCell FROM ckbx_CustomUserFieldMatrixCell cell
		 JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID WHERE cell.MatrixID = @MatrixID
    UNION
	SELECT cell.RowNumber, cell.ColumnNumber, cell.Data, cell.IsHeader, cell.IsRowHeader, matrix.IsRowsFixed, matrix.IsColumnsFixed, 1 as UserCell  FROM ckbx_CustomUserFieldMatrixCell_User cell
		 JOIN ckbx_CustomUserFieldMatrix matrix ON matrix.MatrixID = cell.MatrixID WHERE cell.MatrixID = @MatrixID

'
END
GO





